using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

public class GameLoader : MonoSingleton<GameLoader>
{
    public static bool initialize = false;
    private List<string> downloadFiles = new List<string>();
    public List<string> moduleABPaths = new List<string>();
    // 对skgame lua的modules下的lua ab文件进行收集用在luamanager中进行载入包检查

    public string gameVersion = "0.0.0.0";//游戏使用的版本(用于与后端通知更新使用)

    private string cacheAppVersion = "";//v1 本地缓存app版号
    private string lastAppVersion = null;//v1 远程app版号
    private const string appVesionKey = "__app_Version";
    private Dictionary<string, string> localVersionInfo = new Dictionary<string, string>();
    bool DebugMode = false;
    /// 初始化游戏管理器
    public override void Init()
    {
        //PlayerPrefs.DeleteAll();
        cacheAppVersion = PlayerPrefs.GetString(appVesionKey, "");
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;

        CheckExtractResource(); //释放资源
    }

    /// 释放资源
    public void CheckExtractResource()
    {
        if (DebugMode) // 本地调试
            OnResourceInited();
        else
        {
            // 文件已经解压过了
            bool isExists = Directory.Exists(Util.DataPath) && File.Exists(Util.DataPath + "files.txt");
            if (isExists)
            {
                string dataPath = Util.DataPath;  //数据目录
                if (!Directory.Exists(dataPath))
                    Directory.CreateDirectory(dataPath);
                string[] files = VersionUtil.GetVersionMap(dataPath + "files.txt");
                int count = files.Length;
                string lastLine = files[count - 1];
                lastAppVersion = Util.GetVersion(lastLine, 0);//获得v1
                if (lastAppVersion != cacheAppVersion)//本地记录v1与应用文件v1是否相同，不相同表示 1.没解压完成,2.被黑了
                    StartCoroutine(OnExtractResource());
                else
                    StartCoroutine(OnUpdateResource());
            }
            else //启动释放
            {
                PlayerPrefs.DeleteAll();//清除并重置所有记录
                StartCoroutine(OnExtractResource());
            }
        }
    }

    /// 提取游戏包资源到本地目录中
    IEnumerator OnExtractResource()
    {
        string dataPath = Util.DataPath;  //数据目录
        string pkgPath = Util.AppContentPath(); //游戏包资源目录

        if (Directory.Exists(dataPath))//删除所有本地资源
            Directory.Delete(dataPath, true);
        Directory.CreateDirectory(dataPath);

        string pkgMapFile = pkgPath + "files.txt";
        string localMapFile = dataPath + "files.txt";

        if (File.Exists(localMapFile)) //删除旧的map文件
            File.Delete(localMapFile);

        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(pkgMapFile);
            Debug.Log("大小：" + www.size);
            yield return www;
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.isDone)
                File.WriteAllBytes(localMapFile, www.bytes);
        }
        else
        {
            if (!File.Exists(localMapFile)) //删除旧的map文件
                File.Create(localMapFile);
            Debug.LogError(File.Exists(localMapFile));
            File.Copy(pkgMapFile, localMapFile, true);
        }
        yield return new WaitForEndOfFrame();

        string[] files = File.ReadAllLines(localMapFile);//释放所有文件到数据目录
        int count = files.Length;//总文件
        int step = 0;//第N个文件
        string lastLine = files[count - 1];
        lastAppVersion = Util.GetVersion(lastLine, 0);//包中的版本号

        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            if (fs.Length != 2) break;
            pkgMapFile = pkgPath + fs[0];
            localMapFile = dataPath + fs[0];

            string dir = Path.GetDirectoryName(localMapFile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(pkgMapFile);
                yield return www;
                if (www.isDone)
                {
                    File.WriteAllBytes(localMapFile, www.bytes);
                }
            }
            else
            {
                if (File.Exists(localMapFile))
                    File.Delete(localMapFile);
                File.Copy(pkgMapFile, localMapFile, true);
            }
            yield return null;
        }
        yield return 1;

        PlayerPrefs.SetString(appVesionKey, lastAppVersion);// 本地记录v1
        cacheAppVersion = lastAppVersion;//解压完成当前的版本号

        StartCoroutine(OnUpdateResource());//释放完成，开始启动更新资源
    }

    /// 更新本地文件
    IEnumerator OnUpdateResource()
    {
        string dataPath = Util.DataPath;  //数据目录
        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);

        string[] lastMapList = VersionUtil.GetVersionMap(dataPath + "files.txt");
        int count = lastMapList.Length;//总文件
        int step = 0;//第N个文件
        string lastLine = lastMapList[count - 1];
        string lastVersion = lastLine;//最近版本号
        gameVersion = lastVersion.Trim();

        bool hasUpdate = false;//是否存在必要更新
        #region 本地资源版本
        //收集当前版本文件信息
        for (int i = 0; i < count; i++)
        {
            if (string.IsNullOrEmpty(lastMapList[i])) continue;
            string[] keyValue = lastMapList[i].Split('|');
            if (keyValue.Length != 2)
                break;
            localVersionInfo.Add(keyValue[0].Trim(), keyValue[1].Trim());
        }
        lastAppVersion = Util.GetVersion(lastLine, 0);//最近app v1
        string lv2 = Util.GetVersion(lastVersion, 1);//非UI资源
        string lv3 = Util.GetVersion(lastVersion, 2);//UI资源
        string lv4 = Util.GetVersion(lastVersion, 3);//脚本资源
        #endregion

        #region 服务器资源版本
        string remoteVersion = lastVersion;//cdn版本号 暂定与本地一样
        string url = GameConst.GetInstance().WebUrl;
        string random = DateTime.Now.ToString("yyyymmddhhmmss");
        string webMapUrl = url + "files.txt?v=" + random;
        WWW www = new WWW(webMapUrl);
        Debug.Log("资源位置：" + webMapUrl);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("可能网络问题，也可能服务器资源没提交!  此处可以考虑直接进游戏用本地资源");

            yield break;
        }
        else
        {
            Debug.Log("大小：" + www.size);
            int p = Mathf.FloorToInt(www.progress * 100);
            int size = Mathf.CeilToInt(www.size * 0.001f);
        }
        byte[] webMapData = www.bytes;
        string webMap = www.text.Trim();
        string[] webMapList = webMap.Split('\n');
        count = webMapList.Length;
        lastLine = webMapList[count - 1];
        remoteVersion = lastLine;
        string remoteAppVersion = Util.GetVersion(lastLine, 0);
        string rv2 = Util.GetVersion(remoteVersion, 1);//非UI资源
        string rv3 = Util.GetVersion(remoteVersion, 2);//UI资源
        string rv4 = Util.GetVersion(remoteVersion, 3);//脚本资源
        #endregion
        Debug.Log("服务器版本：" + remoteVersion);
        bool updateV1 = !remoteAppVersion.Equals(lastAppVersion);
        bool updateV2 = (!lv2.Equals(rv2)) || updateV1;
        bool updateV3 = (!lv3.Equals(rv3)) || updateV1;
        bool updateV4 = (!lv4.Equals(rv4)) || updateV1;

        int resCount = 0;
        int resStep = 0;
        int uiCount = 0;
        int uiStep = 0;
        int luaCount = 0;
        int luaStep = 0;
        if (updateV2 || updateV3 || updateV4) //需要更新时，计算各部分文件总量
        {
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(webMapList[i])) continue;
                string[] keyValue = webMapList[i].Split('|');
                if (keyValue.Length != 2)
                    break;
                string n = VersionUtil.GetResNormalName(keyValue[0].Trim());
                if (n == " UI ")
                    uiCount++;
                else if (n == "配置")
                    luaCount++;
                else if (n == "环境")
                    resCount++;
            }
        }

        for (int i = 0; i < count; i++)
        {
            if (string.IsNullOrEmpty(webMapList[i])) continue;
            string[] keyValue = webMapList[i].Split('|');
            if (keyValue.Length != 2)
                break;
            string f = keyValue[0].Trim();

            //收集modules中的功能lua ab文件路径
            if (!string.IsNullOrEmpty(f) && f.IndexOf("lua/lua_skgame_modules_") != -1)
                if (f.IndexOf(".manifest") == -1)
                    moduleABPaths.Add(f);
            if (keyValue[0].Contains("scene/"))
            {//跳过场景资源，进行动态加载
                continue;
            }
            if (lastVersion == remoteVersion)//版本一样，不用更新
            {
                // continue;
            }

            string fileUrl = url + "?v=" + random; //接取服务器资源
            string localfile = (dataPath + f).Trim();
            bool canUpdate = true;// 是否需要更新
            string path = "";
            string message = "";
            string msgRes = "";
            bool checkUpdate = true;
            msgRes = VersionUtil.GetResNormalName(f);// 一般资源的代表名字
            // checkUpdate = (msgRes == "" && updateV2) || (msgRes == "环境" && updateV2)
            // || (msgRes == " UI " && updateV3) || (msgRes == "配置" && updateV4);
            Debug.LogError(localfile + "===" + canUpdate);
            if (checkUpdate)
            {
                canUpdate = !File.Exists(localfile);
                path = Path.GetDirectoryName(localfile);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!canUpdate) //检查是否更新
                {
                    string localKey = "*";
                    if (localVersionInfo.ContainsKey(f))
                        localKey = localVersionInfo[f];
                    string remoteKey = keyValue[1].Trim();
                    canUpdate = !remoteKey.Equals(localKey);
                    if (canUpdate)
                        File.Delete(localfile);
                }
            }

            if (canUpdate) //更新或新增文件
            {
                //方式1 www更新
                hasUpdate = true;
                Debug.Log("更新-->" + fileUrl);
                www = new WWW(fileUrl);
                yield return www;
                if (www.error != null)
                {
                    OnUpdateFailed(path);
                    yield break;
                }
                int size = 0;//Mathf.CeilToInt(www.size * 0.001f);
                             //ui 300kb/g
                             //lua 6kb/g
                if (msgRes == " UI ")
                {
                    size = 311 * uiCount;
                    message = String.Format("正在更新{0}文件, 需要消耗流量约 {1} kb@", msgRes, size);
                }
                else if (msgRes == "配置")
                {
                    size = 6 * luaCount;
                    message = String.Format("正在更新{0}文件, 需要消耗流量约 {1} kb@", msgRes, size);
                }
                else if (msgRes == "环境")
                {
                    size = 151 * resCount;
                    message = String.Format("正在更新{0}文件, 需要消耗流量约 {1} kb@", msgRes, size);

                }

                File.WriteAllBytes(localfile, www.bytes);
                yield return null;
            }
        }
        if (hasUpdate)
        {
            File.WriteAllBytes(dataPath + "files.txt", webMapData);
            PlayerPrefs.SetString(appVesionKey, remoteAppVersion);// 本地记录v1
            cacheAppVersion = remoteAppVersion;//解压完成当前的版本号
            gameVersion = remoteVersion.Trim();
            Debug.Log("写入版本号");
        }


        yield return new WaitForEndOfFrame();

        yield return 0;
    }


    //加载失败
    void OnUpdateFailed(string file)
    {
        string message = "游戏环境初始失败!>" + file;
        Debug.Log("更新失败!>" + file);

    }

    /// 资源初始化结束
    public void OnResourceInited()
    {

    }

    /// 析构函数
    void OnDestroy()
    {

        Debug.Log("~GameManager was destroyed");
    }
}
