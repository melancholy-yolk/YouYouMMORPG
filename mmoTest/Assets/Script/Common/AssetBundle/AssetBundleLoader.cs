using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ͬ������
/// </summary>
public class AssetBundleLoader : IDisposable
{
    private AssetBundle bundle;

    /// <summary>
    /// ���캯�� 
    /// </summary>
    public AssetBundleLoader(string assetBundlePath)
    {
        string fullPath = LocalFileMgr.Instance.LocalFilePath + assetBundlePath;
        bundle = AssetBundle.LoadFromMemory(LocalFileMgr.Instance.GetBuffer(fullPath));
    }

    public T LoadAsset<T>(string name) where T : UnityEngine.Object
    {
        if(bundle == null)
        {
            return default(T);
        }
        return bundle.LoadAsset(name) as T;
    }

    public void Dispose()
    {
        if(bundle != null)
        {
            bundle.Unload(false);
        }
    }
}
