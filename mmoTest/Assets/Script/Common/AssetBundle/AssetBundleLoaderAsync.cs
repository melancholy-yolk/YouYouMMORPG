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
            if (bundle.isStreamedSceneAssetBundle)//������bundle�Ǵ���ĳ��� ֱ���ڻص��з���bundle
            {
                OnLoadComplete(bundle);
            }
            else
            {
                OnLoadComplete(bundle.LoadAsset(mName));//�ǳ�����ɵ�bundle ��bundle�и�����Դ�����س���Դ�󷵻�
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
