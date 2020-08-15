using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssetBundleLoaderAsync : MonoBehaviour 
{
    private string m_FullPath;
    private string mName;

    private AssetBundleCreateRequest request;
    private AssetBundle bundle;

    public Action<UnityEngine.Object> OnLoadComplete;

    public void Init(string path, string name)
    {
        m_FullPath = LocalFileMgr.Instance.LocalFilePath + path;
        mName = name;
    }

    void Start()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        request = AssetBundle.LoadFromMemoryAsync(LocalFileMgr.Instance.GetBuffer(m_FullPath));
        yield return request;
        bundle = request.assetBundle;
        if (OnLoadComplete != null)
        {
            if (bundle.isStreamedSceneAssetBundle)//如果这个bundle是打包的场景 直接在回调中返回bundle
            {
                OnLoadComplete(bundle);
            }
            else
            {
                OnLoadComplete(bundle.LoadAsset(mName));//非场景打成的bundle 从bundle中根据资源名加载出资源后返回
            }
            
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if(bundle != null)
        {
            bundle.Unload(false);
        }
        m_FullPath = null;
        mName = null;
    }

}
