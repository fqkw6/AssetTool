using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Leyoutech.Core.Loader;
using Leyoutech.Core.Util;
using System.IO;

/// <summary>
/// 
/// 功能：Assetbundle更新器
/// </summary>

public class GameLoader : MonoSingleton<GameLoader>
{
    static int MAX_DOWNLOAD_NUM = 5;
    static int UPDATE_SIZE_LIMIT = 5 * 1024 * 1024;
    static string APK_FILE_PATH = "/xluaframework_{0}_{1}.apk";

    string resVersionPath = null;
    string noticeVersionPath = null;
    string clientAppVersion = null;
    string serverAppVersion = null;
    string clientResVersion = null;
    string serverResVersion = null;

    bool needDownloadGame = false;
    bool needUpdateGame = false;

    double timeStamp = 0;
    bool isDownloading = false;
    bool hasError = false;

    List<string> needDownloadList = new List<string>();
    Queue<ResUnityWebRequest> downloadingRequest = new Queue<ResUnityWebRequest>();

    int downloadSize = 0;
    int totalDownloadCount = 0;
    int finishedDownloadCount = 0;

    Text statusText;
    Slider slider;


    // Hotfix测试---用于测试热更模块的热修复
    public void TestHotfix()
    {
        Debug.Log("********** AssetbundleUpdater : Call TestHotfix in cs...");
    }
    public void Awake()
    {
        Debug.LogError(Application.persistentDataPath);

        bool isExists = Directory.Exists(Application.streamingAssetsPath) && File.Exists(Application.streamingAssetsPath + "/files.txt");
        if (isExists)
        {
            string dataPath = Application.streamingAssetsPath + "/files.txt";  //数据目录
            string[] files = VersionUtil.GetVersionMap(dataPath);
            int count = files.Length;
            string lastLine = files[count - 1];
            clientAppVersion = Util.GetVersion(lastLine, 0);//获得v1
            Debug.LogError(lastLine);
        }

        

    }
}
