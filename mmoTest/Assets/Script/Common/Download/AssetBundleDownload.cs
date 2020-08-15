using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 主下载器
/// </summary>
public class AssetBundleDownload : SingletonMono<AssetBundleDownload>
{
    #region 变量
    private string m_VersionUrl;
    private Action<List<DownloadDataEntity>> OnInitVersion;

    private AssetBundleDownloadRoutine[] m_RoutineArr = new AssetBundleDownloadRoutine[DownloadMgr.DownloadRoutineNum];//下载器的数量
    private int m_RoutineIndex;//下载器索引

    public int TotalSize//总大小
    {
        get;
        private set;
    }

    public int TotalCount//总数量
    {
        get;
        private set;
    }

    private bool m_IsDownloadOver = false;//是否下载完成
    #endregion


    protected override void OnStart()
    {
        base.OnStart();

        //真正的运行
        StartCoroutine(DownloadVersion(m_VersionUrl));
    }

    private float m_SampleTime = 2;//采样时间
    private float m_AlreadyTime = 0;//已经下载的时间

    private float m_NeedTime = 0f;//剩余的时间
    private float m_Speed = 0f;//下载速度

    protected override void OnUpdate()
    {
        base.OnUpdate();

        //如果需要下载的数量大于0 并且还未下载完成 显示下载进度
        if (TotalCount > 0 && !m_IsDownloadOver)
        {
            int totalCompleteCount = CurrCompleteTotalCount();
            totalCompleteCount = totalCompleteCount == 0 ? 1 : totalCompleteCount;

            int totalCompleteSize = CurrCompleteTotalSize();

            m_AlreadyTime += Time.deltaTime;//累计下载时间
            if (m_AlreadyTime > m_SampleTime && m_Speed == 0)
            {
                m_Speed = totalCompleteSize / m_SampleTime;//计算出下载速度
            }

            if (m_Speed > 0)
            {
                m_NeedTime = (TotalSize - totalCompleteSize) / m_Speed;//计算下载剩余时间
            }

            string str = string.Format("正在下载 {0}/{1}", totalCompleteCount, TotalCount);
            string strProgress = string.Format("下载进度 = {0}", totalCompleteSize / (float)TotalSize);

            UISceneInitView.Instance.SetProgress(str, totalCompleteCount/(float)TotalCount);

            //AppDebug.Log(str);
            //AppDebug.Log(strProgress);

            if (m_NeedTime > 0)
            {
                string strNeedTime = string.Format("remain time = {0}", m_NeedTime);
                AppDebug.Log(strNeedTime);
            }

            

            if (totalCompleteCount == TotalCount)
            {
                m_IsDownloadOver = true;
                //AppDebug.Log("download complete!");
                UISceneInitView.Instance.SetProgress("资源更新完毕", 1);
                if (DownloadMgr.Instance.OnInitComplete != null)
	            {
                    DownloadMgr.Instance.OnInitComplete();
	            }
            }
        }
    }


    #region 初始化服务器的版本信息
    /// <summary>
    /// 初始化服务器的版本信息
    /// </summary>
    /// <param name="url"></param>
    /// <param name="onInitVersion"></param>
    public void InitServerVersion(string url, Action<List<DownloadDataEntity>> onInitVersion)
    {
        m_VersionUrl = url;
        OnInitVersion = onInitVersion;
    }
    #endregion

    #region 下载服务器版本文件
    public IEnumerator DownloadVersion(string url)
    {
        WWW www = new WWW(url);
        float timeOut = Time.time;
        float progress = www.progress;
        while (www != null && !www.isDone)
        {
            if (progress < www.progress)
            {
                timeOut = Time.time;
                progress = www.progress;
            }
            if ((Time.time - timeOut) > DownloadMgr.DownloadTimeOut)
            {
                AppDebug.Log("download timeout!");
                yield break;
            }
        }

        yield return www;

        if (www != null && www.error == null)
        {
            string content = www.text;
            AppDebug.Log("download success, content=" + content);
            if (OnInitVersion != null)
            {
                OnInitVersion(DownloadMgr.Instance.PackDownloadData(content));
            }
        }
        else
        {
            AppDebug.Log("download fail, reason=" + www.error);
        }
    }
    #endregion

    #region 使用下载器下载文件
    public void DownloadFiles(List<DownloadDataEntity> needDownloadList)
    {
        m_RoutineIndex = 0;
        TotalSize = 0;
        TotalCount = 0;

        //初始化下载器
        for (int i = 0; i < m_RoutineArr.Length; i++)
        {
            if (m_RoutineArr[i] == null)
            {
                m_RoutineArr[i] = gameObject.AddComponent<AssetBundleDownloadRoutine>();
            }
        }

        //循环的给下载器分配任务
        for (int i = 0; i < needDownloadList.Count; i++)
        {
            m_RoutineIndex = m_RoutineIndex % m_RoutineArr.Length;

            //其中的一个下载器 给他分配一个文件
            m_RoutineArr[m_RoutineIndex].AddDownload(needDownloadList[i]);

            m_RoutineIndex++;
            TotalSize += needDownloadList[i].Size;
            TotalCount++;
        }

        //让下载器开始下载
        for (int i = 0; i < m_RoutineArr.Length; i++)
        {
            if (m_RoutineArr[i] == null) continue;

            m_RoutineArr[i].StartDownload();
        }
    }
    #endregion

    #region 当前已经下载的文件总大小
    /// <summary>
    /// 当前已经下载的文件总大小
    /// </summary>
    public int CurrCompleteTotalSize()
    {
        int currCompleteTotalSize = 0;

        for (int i = 0; i < m_RoutineArr.Length; i++)
        {
            if (m_RoutineArr[i] == null) continue;

            currCompleteTotalSize += m_RoutineArr[i].DownloadSize;
        }

        return currCompleteTotalSize;
    }
    #endregion

    #region 当前已经下载的文件总数量
    /// <summary>
    /// 当前已经下载的文件总数量
    /// </summary>
    public int CurrCompleteTotalCount()
    {
        int currCompleteTotalCount = 0;

        for (int i = 0; i < m_RoutineArr.Length; i++)
        {
            if (m_RoutineArr[i] == null) continue;

            currCompleteTotalCount += m_RoutineArr[i].CompletedCount;
        }

        return currCompleteTotalCount;
    }
    #endregion

}
