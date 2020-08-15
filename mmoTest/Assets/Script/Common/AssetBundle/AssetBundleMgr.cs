using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleMgr : Singleton<AssetBundleMgr> 
{
    public GameObject Load(string path, string name)
    {
#if DISABLE_ASSETBUNDLE
        return AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/{0}", path.Replace("assetbundle", "prefab")));
#else
        using (AssetBundleLoader loader = new AssetBundleLoader(path))
        {
            return loader.LoadAsset<GameObject>(name);
        }
#endif

    }


    public GameObject LoadClone(string path, string name)
    {
#if DISABLE_ASSETBUNDLE
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/{0}", path.Replace("assetbundle", "prefab")));
        return Object.Instantiate<GameObject>(obj);
#else
        using (AssetBundleLoader loader = new AssetBundleLoader(path))
        {
            GameObject obj = loader.LoadAsset<GameObject>(name);
            return Object.Instantiate(obj);
        }
#endif
    }

    public AssetBundleLoaderAsync LoadAsync(string path, string name)
    {
        GameObject obj = new GameObject("AssetBundleLoaderAsync");
        AssetBundleLoaderAsync async = null;
        if(obj.GetComponent<AssetBundleLoaderAsync>() == null)
        {
            async = obj.AddComponent<AssetBundleLoaderAsync>();
        }
        else
        {
            async = obj.GetComponent<AssetBundleLoaderAsync>();
        }
        async.Init(path, name);
        return async;
    }

}
