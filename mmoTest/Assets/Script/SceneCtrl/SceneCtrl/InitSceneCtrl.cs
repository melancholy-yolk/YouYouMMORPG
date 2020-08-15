using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSceneCtrl : MonoBehaviour 
{

	void Start () 
    {
        AppDebug.Log(Application.persistentDataPath);
        //流程：
        //判断persistentDataPath下有没有VersionFile.txt 有的话执行更新流程 没有的话执行资源初始化流程
        //判断StreamingAssets下有没有VersionFile.txt 没有的话执行更新流程 有的话执行资源初始化流程

        //初始化流程
        //将StreamingAssets下的初始资源 解压缩到persistentDataPath下

        //更新流程
        //1.下载服务器版本文件
        //2.对比服务器与本地版本文件 得到下载列表
        //3.下载需要下载的文件
#if DISABLE_ASSETBUNDLE
        SceneMgr.Instance.LoadToLogOn();
#else
        DownloadMgr.Instance.InitStreamingAssets(OnInitComplete);
#endif
    }

    private IEnumerator ToLogOnScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneMgr.Instance.LoadToLogOn();//跳转到登录界面
    }

    public void OnInitComplete()
    {
        StartCoroutine(ToLogOnScene());
    }
}
