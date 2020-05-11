﻿using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Util
{
    private static List<string> luaPaths = new List<string>();
    public static int Int(object o)
    {
        return Convert.ToInt32(o);
    }

    public static float Float(object o)
    {
        return (float)Math.Round(Convert.ToSingle(o), 2);
    }

    public static long Long(object o)
    {
        return Convert.ToInt64(o);
    }

    public static int Random(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float Random(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    // 返回半径为1的圆内的一个随机点
    public static Vector2 RandomInsideUnitCircle()
    {
        return UnityEngine.Random.insideUnitCircle;
    }
    //返回半径为1的球体内的一个随机点。
    public static Vector3 RandomInsideUnitSphere()
    {
        return UnityEngine.Random.insideUnitSphere;
    }
    // 返回半径为1的球体在表面上的一个随机点
    public static Vector3 RandomOnUnitSphere()
    {
        return UnityEngine.Random.onUnitSphere;
    }
    // 返回一个随机旋转角度
    public static Quaternion RandomRotation()
    {
        return UnityEngine.Random.rotation;
    }

    ///  设置是否active...
    public static void SetActive(GameObject obj, bool show)
    {
        if (show ^ obj.activeSelf)
            obj.SetActive(show);
    }

    /// <summary>
    ///  找骨骼(gameobject)节点..
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject GetBone(string name, GameObject parent)
    {
        if (parent == null) return null;
        bool b = false;
        if (!parent.activeSelf)
        {
            b = true;
            SetActive(parent, true);
        }
        Transform[] allTran = parent.GetComponentsInChildren<Transform>();
        if (b) SetActive(parent, false);//激活后再复原
        for (int i = 0; i < allTran.Length; i++)
        {
            Transform t = allTran[i];
            if (t != null && t.name == name)
                return t.gameObject;
        }
        return null;
    }

    /// 绑定骨骼
    public static void BindBone(GameObject node, string boneName, GameObject targetRoot, Vector3 pos)
    {
        GameObject bone = GetBone(boneName, targetRoot);
        if (bone != null && node != null)
        {
            Transform transform = bone.transform;
            node.transform.parent = transform;
            if (pos != null)
                node.transform.localPosition = pos;
            node.transform.localScale = Vector3.one;
            node.transform.rotation = transform.rotation;
        }
    }

    #region 设置位置 朝向 缩放

    public static void SetPosition(GameObject target, float x, float y, float z)
    {
        if (target == null) return;
        target.transform.position = new Vector3(x, y, z);
    }
    public static void SetLocalPosition(GameObject target, float x, float y, float z)
    {
        if (target == null) return;
        target.transform.localPosition = new Vector3(x, y, z);
    }
    public static void SetPosition(Transform target, float x, float y, float z)
    {
        if (target == null) return;
        target.position = new Vector3(x, y, z);
    }
    public static void SetLocalPosition(Transform target, float x, float y, float z)
    {
        if (target == null) return;
        target.localPosition = new Vector3(x, y, z);
    }
    public static void SetRot(GameObject target, float x, float y, float z)
    {
        if (target == null) return;
        target.transform.eulerAngles = new Vector3(x, y, z);
    }
    public static void SetLocalRot(GameObject target, float x, float y, float z)
    {
        if (target == null) return;
        target.transform.localEulerAngles = new Vector3(x, y, z);
    }
    public static void SetRot(Transform target, float x, float y, float z)
    {
        if (target == null) return;
        target.eulerAngles = new Vector3(x, y, z);
    }
    public static void SetLocalRot(Transform target, float x, float y, float z)
    {
        if (target == null) return;
        target.localEulerAngles = new Vector3(x, y, z);
    }

    #endregion

    /// 获取游戏对象的大小
    public static Vector3 GetObjectSize(GameObject go)
    {
        Vector3 realSize = Vector3.zero;
        Renderer render = go.GetComponent<Renderer>();
        if (render == null) return realSize;
        Vector3 meshSize = render.bounds.size; // 模型网格的大小
        Vector3 scale = go.transform.parent.lossyScale;//go.transform.lossyScale;  // 放缩  
        realSize = new Vector3(meshSize.x * scale.x, meshSize.y * scale.y, meshSize.z * scale.z);// 游戏中的实际大小
        return realSize;
    }

    public static string Uid(string uid)
    {
        int position = uid.LastIndexOf('_');
        return uid.Remove(0, position + 1);
    }

    public static long GetTime()
    {
        TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        return (long)ts.TotalMilliseconds;
    }

    /// <summary>
    /// 搜索子物体组件-GameObject版
    /// </summary>
    public static T Get<T>(GameObject go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.transform.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Transform版
    /// </summary>
    public static T Get<T>(Transform go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Component版
    /// </summary>
    public static T Get<T>(Component go, string subnode) where T : Component
    {
        return go.transform.Find(subnode).GetComponent<T>();
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] ts = go.GetComponents<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] != null) GameObject.Destroy(ts[i]);
            }
            return go.gameObject.AddComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(Transform go) where T : Component
    {
        return Add<T>(go.gameObject);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(GameObject go, string subnode)
    {
        return Child(go.transform, subnode);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(Transform go, string subnode)
    {
        Transform tran = go.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(GameObject go, string subnode)
    {
        return Peer(go.transform, subnode);
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(Transform go, string subnode)
    {
        Transform tran = go.parent.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }
    //根据名字获取子节点
    public static Transform GetChild(Transform root, string bone)
    {
        if (root.name == bone)
        {
            return root;
        }
        foreach (Transform t in root)
        {
            Transform f = GetChild(t, bone);
            if (f != null)
            {
                return f;
            }
        }
        return null;
    }

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    /// <summary>
    /// 清除所有子节点
    /// </summary>
    public static void ClearChild(Transform go)
    {
        if (go == null) return;
        for (int i = go.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(go.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 清理内存
    /// </summary>
    public static void ClearMemory()
    {
        GC.Collect(); Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
            string game = GameConst.GetInstance().AppName.ToLower();
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/" + game + "/";
            }
            if (GameConst.GetInstance().DebugMode)
            {
                return Application.streamingAssetsPath + "/";
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                int i = Application.dataPath.LastIndexOf('/');
                return Application.dataPath.Substring(0, i + 1) + game + "/";
            }
            // if (Application.platform == RuntimePlatform.WindowsEditor)
            // {
            //     return Application.streamingAssetsPath;
            // }
            return "c:/" + game + "/";
        }
    }

    public static string GetRelativePath()
    {
        if (Application.isEditor)
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/StreamingAssets" + "/";
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
            return "file:///" + DataPath;
        else // For standalone player.
            return "file://" + Application.streamingAssetsPath + "/";
    }

    /// <summary>
    /// 取得行文本
    /// </summary>
    public static string GetFileText(string path)
    {
        return File.ReadAllText(path);
    }

    /// <summary>
    /// 网络可用
    /// </summary>
    public static bool NetAvailable
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    /// <summary>
    /// 是否是无线
    /// </summary>
    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
                break;
            default:
                path = Application.streamingAssetsPath + "/";
                break;
        }
        return path;
    }

    public static void Log(string str)
    {
        Debug.Log(str);
    }

    public static void LogWarning(string str)
    {
        Debug.LogWarning(str);
    }

    public static void LogError(string str)
    {
        Debug.LogError(str);
    }

    /// <summary>
    /// 防止初学者不按步骤来操作
    /// </summary>
    /// <returns></returns>
    public static int CheckRuntimeFile()
    {
        if (!Application.isEditor) return 0;
        string streamDir = Application.dataPath + "/StreamingAssets/";
        if (!Directory.Exists(streamDir))
        {
            return -1;
        }
        else
        {
            string[] files = Directory.GetFiles(streamDir);
            if (files.Length == 0) return -1;

            if (!File.Exists(streamDir + "files.txt"))
            {
                return -1;
            }
        }
        string sourceDir = Application.dataPath + "/ToLua/Source/Generate/";
        if (!Directory.Exists(sourceDir))
        {
            return -2;
        }
        else
        {
            string[] files = Directory.GetFiles(sourceDir);
            if (files.Length == 0) return -2;
        }
        return 0;
    }

    /// <summary>
    /// 检查运行环境
    /// </summary>
    public static bool CheckEnvironment()
    {
#if UNITY_EDITOR
        int resultId = Util.CheckRuntimeFile();
        if (resultId == -1)
        {
            Debug.LogError("没有找到框架所需要的资源，单击Game菜单下Build xxx Resource生成！！");
            EditorApplication.isPlaying = false;
            return false;
        }
        else if (resultId == -2)
        {
            Debug.LogError("没有找到Wrap脚本缓存，单击Lua菜单下Gen Lua Wrap Files生成脚本！！");
            EditorApplication.isPlaying = false;
            return false;
        }
#endif
        return true;
    }

    /**版本号 t 可选为 0，1，2, 3 分别表示获得 v1, v2, v3, v4 版号*/
    public static string GetVersion(string ver, int t = 0)
    {
        return VersionUtil.GetVersion(ver, t);
    }

    public static void SetVoiceData(byte[] bs)
    {
        Debug.Log("收到起始字节：" + bs[0] + "| 最后个字节：" + bs[bs.Length - 1] + "|长度 " + bs.Length);
    }

}
