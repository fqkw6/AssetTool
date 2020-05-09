using System.Collections;
using System.Collections.Generic;
using Leyoutech.Core.Loader;
using UnityEngine;
using Leyoutech.Core.Util;

public class GameConst : Singleton<GameConst>
{
    private AssetLoaderMode assetLoaderMode; //编辑器模式
    public string assetBundlePath; //地址
    public static int m_MaxLoadCount = 30; //同时加载数
    const string kEditorMode = "AssetBuild/BuildMode/Editor";
    private string tPlatformName = "Android";

    public bool DebugMode;
    public string AppName = "AssetTool";
    public string WebUrl
    {
        get
        {
            return "https://abserver.oss-cn-beijing.aliyuncs.com/AssetBuild/";
        }
    }
    public string m_AssetBundlePath
    {
        get
        {
            assetBundlePath = Application.streamingAssetsPath + "/" + tPlatformName + "/assetbundles"; //地址
            return assetBundlePath;
        }
    }

    public AssetLoaderMode m_AssetLoaderMode
    {
        get
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorPrefs.HasKey(kEditorMode)) { UnityEditor.EditorPrefs.SetString(kEditorMode, "true"); }
            assetLoaderMode = UnityEditor.EditorPrefs.GetString(kEditorMode) == "false" ? AssetLoaderMode.AssetBundle : AssetLoaderMode.AssetDatabase;
#endif
            return assetLoaderMode;
        }
        private set { assetLoaderMode = value; }
    }
#if UNITY_EDITOR
    public static string GetPlatformName(UnityEditor.BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case UnityEditor.BuildTarget.Android:
                return "Android";
            case UnityEditor.BuildTarget.iOS:
                return "iOS";
            default:
                return "StandaloneWindows64";
        }
    }
#endif


}