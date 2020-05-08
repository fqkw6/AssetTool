using System.Collections;
using System.Collections.Generic;
using Leyoutech.Core.Loader;
using UnityEngine;
public class GameConst
{
    public static AssetLoaderMode m_AssetLoaderMode = AssetLoaderMode.AssetBundle; //编辑器模式
    public static string m_AssetBundlePath = Application.dataPath + "/../AssetBuild/StandaloneWindows64/assetbundles"; //地址
    public static int m_MaxLoadCount = 30; //同时加载数
}