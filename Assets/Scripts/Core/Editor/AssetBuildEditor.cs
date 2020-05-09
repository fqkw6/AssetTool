using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LeyoutechEditor.Core.Packer;
using Leyoutech.Core.Loader;
using Leyoutech.Utility;
using System.IO;
public class AssetBuildEditor : Editor
{
    const string kEditorMode = "AssetBuild/BuildMode/Editor";
    const string kSimulateMode = "AssetBuild/BuildMode/Simulate";


    [MenuItem(kEditorMode, false)]
    public static void ToggleEditorMode()
    {
        if (GameConst.GetInstance().m_AssetLoaderMode == AssetLoaderMode.AssetBundle)
        {
            EditorPrefs.SetString(kEditorMode, "true");
        }
    }

    [MenuItem(kEditorMode, true)]
    public static bool ToggleEditorModeValidate()
    {
        Menu.SetChecked(kEditorMode, GameConst.GetInstance().m_AssetLoaderMode == AssetLoaderMode.AssetDatabase);
        return true;
    }

    [MenuItem(kSimulateMode)]
    public static void ToggleSimulateMode()
    {
        if (GameConst.GetInstance().m_AssetLoaderMode == AssetLoaderMode.AssetDatabase)
        {
            EditorPrefs.SetString(kEditorMode, "false");
        }
    }

    [MenuItem(kSimulateMode, true)]
    public static bool ToggleSimulateModeValidate()
    {
        Menu.SetChecked(kSimulateMode, GameConst.GetInstance().m_AssetLoaderMode == AssetLoaderMode.AssetBundle);
        return true;
    }


    [MenuItem("AssetBuild/SetName", false)]
    static public void SetName()
    {
        Debug.Log("SetName");
        if (!Application.isPlaying)

        {
            bool result = BundlePackUtil.GenerateConfigs(true, true, true);
            if (!result)
            {
                if (EditorUtility.DisplayDialog("Warning", "Address Repeat!!!!\nDo you want to fix or view??", "OK", "Cancel"))
                {
                    BundlePackWindow.ShowWin();
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog("Success", "Packed Success", "OK"))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    [MenuItem("AssetBuild/CopyToStreamingAssets", false)]
    static public void CopyTo()
    {
        Debug.Log("CopyTostreamingAssetsPath");
        BundlePackConfig m_PackConfig = LeyoutechEditor.Core.Util.FileUtil.ReadFromBinary<BundlePackConfig>(BundlePackUtil.GetPackConfigPath());
        FileUtility.CloneDirectory(m_PackConfig.OutputDirPath, Application.streamingAssetsPath);
        AssetDatabase.Refresh();
    }
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    static string appVersion = "2";

    [MenuItem("AssetBuild/CreatMD5", false)]
    static public void CreatMD5()
    {
        Debug.Log("CreatMD5");
        BundlePackConfig m_PackConfig = LeyoutechEditor.Core.Util.FileUtil.ReadFromBinary<BundlePackConfig>(BundlePackUtil.GetPackConfigPath());
        string resPath = m_PackConfig.OutputDirPath;
        string newFilePath = resPath + "/files.txt";//创建版本文件列表
        paths.Clear();
        files.Clear();
        if (File.Exists(newFilePath)) File.Delete(newFilePath);
        paths.Clear(); files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        string luaAB = "";//用于处理 lua.ab 与 lua.ab.manifest 序列成同一个md5
        string hashCode = "";
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (ext.Equals(".meta") || ext.Equals(".svn") || ext.Equals(".txt")
                || ext.Contains(".DS_Store") || ext.Contains(".exe") || ext.Contains(".bat")) continue;
            string md5 = "";
            if (file.IndexOf("lua/") == -1)
            {
                md5 = FileUtility.MD5file(file);
            }
            else
            {
                if (luaAB != "" && file.IndexOf(luaAB) != -1)
                {
                    md5 = hashCode;
                }
                else
                {
                    md5 = FileUtility.MD5file(file);
                    luaAB = file;
                    hashCode = md5;
                }
            }
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        //格式： v1.v2.v3.v4 其中 vx代表序号
        //版号表示：v1程序更新(全部)，v2（非UI）资源, v3 UI资源， v4 lua脚本
        sw.WriteLine(appVersion + ".0.0.0");
        sw.Close(); fs.Close();
        AssetDatabase.Refresh();
    }
    // 遍历目录及其子目录
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);// 扩展名
            if (ext.Equals(".meta") || ext.Equals(".svn") || ext.Equals(".txt")
                || ext.Contains(".DS_Store") || ext.Contains(".exe") || ext.Contains(".bat")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

}
