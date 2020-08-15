using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 本地文件管理
/// </summary>
public class LocalFileMgr : Singleton<LocalFileMgr> 
{
#if UNITY_EDITOR
    
    public readonly string LocalFilePath = Application.dataPath + "/../AssetBundles/Windows/";
#elif UNITY_ANDROID
    public readonly string LocalFilePath = Application.dataPath + "/../AssetBundles/Android/";
#elif UNITY_IPHONE
    public readonly string LocalFilePath = Application.dataPath + "/../AssetBundles/iOS/";
#elif UNITY_STANDALONE_WIN
    public readonly string LocalFilePath = Application.streamingAssetsPath + "/AssetBundles/Windows/";
    //public readonly string LocalFilePath = Application.dataPath + "/../AssetBundles/Windows/";
#elif UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_WIN
    public readonly string LocalFilePath = Application.persistentDataPath + "/";
#endif

    /// <summary>
    /// 从文件读取字节流
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public byte[] GetBuffer(string path)
    {
        byte[] buffer = null;

        using(FileStream fs = new FileStream(path, FileMode.Open))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
        }
        return buffer;
    }
}
