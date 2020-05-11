using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ResUnityWebRequest
{
    UnityWebRequest webRequest;
    private string downloadUrl = "";
    private string savePath = "";//如"E://"
    private string downloadFileName = "";
    private bool write;
    public void Create(string url, string path)
    {
        downloadUrl = url;//"https://abserver.oss-cn-beijing.aliyuncs.com/test10.apk"; ;//下载链接
        savePath = path; //Application.streamingAssetsPath + "/";
        Init();
    }

    public void OnUpdate()
    {
        if (!webRequest.isDone)
        {
            Debug.Log("下载进度：" + GetProcess());
        }
        else
        {
            if (!write)
            {
                write = true;
                if (webRequest.isNetworkError)
                {
                    Debug.Log("Download Error:" + webRequest.error);
                }
                else
                {
                    //获取二进制数据
                    var File = webRequest.downloadHandler.data;
                    //创建文件写入对象
                    FileStream nFile = new FileStream(savePath, FileMode.Create);
                    //写入数据
                    nFile.Write(File, 0, File.Length);
                    nFile.Close();
                }
            }

        }
    }
    /// <summary>
    /// 根据URL下载文件
    /// </summary>
    /// <param name="downloadUrl"></param>
    /// <returns></returns>
    void Init()
    {
        //发送请求
        webRequest = UnityWebRequest.Get(downloadUrl);
        webRequest.timeout = 30;//设置超时，若webRequest.SendWebRequest()连接超时会返回，且isNetworkError为true
        webRequest.SendWebRequest();
    }

    /// <summary>
    /// 获取下载进度
    /// </summary>
    /// <returns></returns>
    public float GetProcess()
    {
        if (webRequest != null)
        {
            if (webRequest.isDone) { return 1; }
            return (((int)(webRequest.downloadProgress * 100)) % 100);
        }
        return 0;
    }
    /// <summary>
    /// 获取当前下载内容长度
    /// </summary>
    /// <returns></returns>
    public long GetCurrentLength()
    {
        if (webRequest != null)
        {
            return (long)webRequest.downloadedBytes;
        }
        return 0;
    }

}
