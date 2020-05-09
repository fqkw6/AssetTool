using System;
using System.IO;
public class VersionUtil
{
    public static string[] GetVersionMap(string localMap)
    {
        bool flag = File.Exists(localMap);
        string[] result;
        if (flag)
        {
            FileInfo fileInfo = new FileInfo(localMap);
            FileStream fileStream = new FileStream(localMap, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();
            result = text.Trim().Split(new char[]
            {
                '\n'
            });
        }
        else
        {
            result = new string[0];
        }
        return result;
    }
    public static string GetVersion(string ver, int t = 0)
    {
        string result;
        if (string.IsNullOrEmpty(ver))
        {
            result = "0";
        }
        else
        {
            string[] array = ver.Split(new char[]
            {
                '.'
            });
            result = array[t];
        }
        return result;
    }
    public static string GetResNormalName(string name)
    {
        string result;
        if (name.IndexOf("StreamingAssets") != -1)
        {
            result = "";
        }
        else
        {
            if (name.IndexOf("audio/") != -1 || name.IndexOf("effect/") != -1 || name.IndexOf("prefabs/") != -1 || name.IndexOf("texture/") != -1)
            {
                result = "环境";
            }
            else
            {
                if (name.IndexOf("ui/") != -1)
                {
                    result = " UI ";
                }
                else
                {
                    if (name.IndexOf("lua/") != -1)
                    {
                        result = "配置";
                    }
                    else
                    {
                        if (name.IndexOf("scene/") != -1)
                        {
                            result = "场景";
                        }
                        else
                        {
                            result = "*";
                        }
                    }
                }
            }
        }
        return result;
    }
    public static long GetTime()
    {
        TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        return (long)timeSpan.TotalMilliseconds;
    }
}
