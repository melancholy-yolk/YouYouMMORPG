using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneCtrl : MonoBehaviour 
{
    [SerializeField]
    private UISceneLoadingView m_UISceneLoadingView;//场景UI

    private AsyncOperation m_Async;//异步加载场景的信息
    
    private int m_CurrProgress = 0;//当前的进度

	void Start () {
        Resources.UnloadUnusedAssets();

        DelegateDefine.Instance.OnSceneLoadOK += OnSceneLoadOK;
        LayerUIManager.Instance.Reset();
        m_UISceneLoadingView.SetProgress(m_CurrProgress * 0.01f);
        StartCoroutine(LoadingScene());

        UIViewUtil.Instance.CloseAllWindow();
	}

    void OnDestroy()
    {
        DelegateDefine.Instance.OnSceneLoadOK -= OnSceneLoadOK;
    }

    private void OnSceneLoadOK()
    {
        if (m_UISceneLoadingView != null)
        {
            Destroy(m_UISceneLoadingView.gameObject);
            Destroy(gameObject);
        }
    }
	
	private IEnumerator LoadingScene()
    {
        string sceneName = string.Empty;
        switch (SceneMgr.Instance.CurrentSceneType)
        {
            case SceneType.LogOn:
                sceneName = ConstDefine.Scene_LogOn;
                break;
            case SceneType.SelectRole:
                sceneName = ConstDefine.Scene_SelectRole;
                break;
            case SceneType.WorldMap:

                WorldMapEntity worldMapEntity = WorldMapDBModel.Instance.GetEntity(SceneMgr.Instance.CurrWorldMapId);
                if (worldMapEntity != null)
                {
                    sceneName = worldMapEntity.SceneName;
                }

                break;
            case SceneType.GameLevel:

                GameLevelEntity gameLevelEntity = GameLevelDBModel.Instance.GetEntity(SceneMgr.Instance.CurrGameLevelId);
                if (gameLevelEntity != null)
                {
                    sceneName = gameLevelEntity.SceneName;
                }

                break;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            yield break;
        }

        if (SceneMgr.Instance.CurrentSceneType == SceneType.SelectRole || SceneMgr.Instance.CurrentSceneType == SceneType.WorldMap || SceneMgr.Instance.CurrentSceneType == SceneType.GameLevel)
        {
#if DISABLE_ASSETBUNDLE
            m_Async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            m_Async.allowSceneActivation = false;//场景加载完毕时 不会自动激活场景
#else
            //从assetbundle中 异步加载出场景 然后切换场景
            AssetBundleLoaderAsync abLoaderAsync = AssetBundleMgr.Instance.LoadAsync(string.Format("Download/Scene/{0}.unity3d", sceneName), sceneName);
            abLoaderAsync.OnLoadComplete = (Object obj) =>
            {
                m_Async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                m_Async.allowSceneActivation = false;//场景加载完毕时 不会自动激活场景
            };
#endif

        }
        else
        {
            m_Async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            m_Async.allowSceneActivation = false;//场景加载完毕时 不会自动激活场景
        }

        yield return m_Async;
    }

    private IEnumerator Load(string path, string strSceneName)
    {
#if DISABLE_ASSETBUNDLE
        yield return null;
        m_Async = SceneManager.LoadSceneAsync(strSceneName, LoadSceneMode.Additive);
        m_Async.allowSceneActivation = false;//场景加载完毕时 不会自动激活场景
#else
        string fullPath = LocalFileMgr.Instance.LocalFilePath + path;
        byte[] buffer = LocalFileMgr.Instance.GetBuffer(fullPath);
        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(buffer);
        yield return request;
        AssetBundle bundle = request.assetBundle;

        m_Async = SceneManager.LoadSceneAsync(strSceneName, LoadSceneMode.Additive);
        m_Async.allowSceneActivation = false;//场景加载完毕时 不会自动激活场景
#endif

    }

	void Update () {
        if (m_Async == null)
        {
            return;
        }

        int toProgress = 0;
        if (m_Async.progress < 0.9f)
        {
            toProgress = (int)(m_Async.progress * 100);
        }
        else
        {
            toProgress = 100;
        }

        if (m_CurrProgress < toProgress)
        {
            m_CurrProgress++;
        }
        else
        {
            m_Async.allowSceneActivation = true;
        }

        m_UISceneLoadingView.SetProgress(m_CurrProgress * 0.01f);
	}
}
