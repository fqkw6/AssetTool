using System;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
public class HttpDownLoad
{
    public float progress { get; private set; }

    public bool isDone { get; private set; }

    private bool isStop;

    private UnityWebRequest m_Head;

    private UnityWebRequest m_Request;
    /// <summary>
    /// 完成回调
    /// </summary>
    private Action m_FinishHandle;
    /// <summary>
    /// 地址
    /// </summary>
    private string m_Url;
    /// <summary>
    /// 输出路径
    /// </summary>
    private string m_FilePath;
    public void Init(string url, string filePath, Action callBack)
    {
        m_Url = url;
        m_FilePath = filePath;
        m_FinishHandle = callBack;
        m_Head = UnityWebRequest.Head(url);
        m_Request = UnityWebRequest.Get(url);
        m_Head.SendWebRequest();
        m_FinishHead = false;
        var dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    private bool m_FinishHead;
    /// <summary>
    /// 数据总长度
    /// </summary>
    private long m_TotalLength;
    private FileStream m_FileStream;
    public void OnUpdate()
    {
        if (m_Head != null && m_Head.isDone && !m_FinishHead)
        {
            m_TotalLength = long.Parse(m_Head.GetResponseHeader("Content-Length"));
            m_FinishHead = true;
            m_FileStream = new FileStream(m_FilePath, FileMode.OpenOrCreate, FileAccess.Write);
            var fileLength = m_FileStream.Length;
            if (fileLength < m_TotalLength)
            {
                m_FileStream.Seek(fileLength, SeekOrigin.Begin);
                m_Request.SetRequestHeader("Range", "bytes=" + fileLength + "-" + m_TotalLength);
                m_Request.SendWebRequest();
            }
            else
            {
                progress = 1f;
            }

        }
        if (m_FinishHead && m_Request != null && !m_Request.isDone && !isDone)
        {
            OnStart(m_Url, m_FilePath, m_FinishHandle);

        }
    }
    public void OnStart(string url, string filePath, Action callBack)
    {

        Debug.LogError("sss");
        var fileLength = m_FileStream.Length;

        // if (fileLength < m_TotalLength)
        {


            var index = 0;
            if (!m_Request.isDone)
            {
                if (isStop) return;

                var buff = m_Request.downloadHandler.data;
                if (buff != null)
                {
                    var length = buff.Length - index;
                    m_FileStream.Write(buff, index, length);
                    index += length;
                    fileLength += length;

                    if (fileLength == m_TotalLength)
                    {
                        progress = 1f;
                    }
                    else
                    {
                        progress = fileLength / (float)m_TotalLength;
                        Debug.LogError((((int)(m_Request.downloadProgress * 100)) % 100) +
                        "====" + (((int)(progress * 100)) % 100));
                    }
                }
            }
        }

        if (progress >= 1f)
        {
            isDone = true;
            m_FileStream.Close();
            m_FileStream.Dispose();

            if (m_FinishHandle != null)
            {
                m_FinishHandle();
            }
        }
    }

    public void Stop()
    {
        isStop = true;
    }
}