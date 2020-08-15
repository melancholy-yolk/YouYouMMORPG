using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ��������
/// </summary>
public class AssetBundleDownload : SingletonMono<AssetBundleDownload>
{
    #region ����
    private string m_VersionUrl;
    private Action<List<DownloadDataEntity>> OnInitVersion;

    private AssetBundleDownloadRoutine[] m_RoutineArr = new AssetBundleDownloadRoutine[DownloadMgr.DownloadRoutineNum];//������������
    private int m_RoutineIndex;//����������

    public int TotalSize//�ܴ�С
    {
        get;
        private set;
    }

    public int TotalCount//������
    {
        get;
        private set;
    }

    private bool m_IsDownloadOver = false;//�Ƿ��������
    #endregion


    protected override void OnStart()
    {
        base.OnStart();

        //����������
        StartCoroutine(DownloadVersion(m_VersionUrl));
    }

    private float m_SampleTime = 2;//����ʱ��
    private float m_AlreadyTime = 0;//�Ѿ����ص�ʱ��

    private float m_NeedTime = 0f;//ʣ���ʱ��
    private float m_Speed = 0f;//�����ٶ�

    protected override void OnUpdate()
    {
        base.OnUpdate();

        //�����Ҫ���ص���������0 ���һ�δ������� ��ʾ���ؽ���
        if (TotalCount > 0 && !m_IsDownloadOver)
        {
            int totalCompleteCount = CurrCompleteTotalCount();
            totalCompleteCount = totalCompleteCount == 0 ? 1 : totalCompleteCount;

            int totalCompleteSize = CurrCompleteTotalSize();

            m_AlreadyTime += Time.deltaTime;//�ۼ�����ʱ��
            if (m_AlreadyTime > m_SampleTime && m_Speed == 0)
            {
                m_Speed = totalCompleteSize / m_SampleTime;//����������ٶ�
            }

            if (m_Speed > 0)
            {
                m_NeedTime = (TotalSize - totalCompleteSize) / m_Speed;//��������ʣ��ʱ��
            }

            string str = string.Format("�������� {0}/{1}", totalCompleteCount, TotalCount);
            string strProgress = string.Format("���ؽ��� = {0}", totalCompleteSize / (float)TotalSize);

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
                UISceneInitView.Instance.SetProgress("��Դ�������", 1);
                if (DownloadMgr.Instance.OnInitComplete != null)
	            {
                    DownloadMgr.Instance.OnInitComplete();
	            }
            }
        }
    }


    #region ��ʼ���������İ汾��Ϣ
    /// <summary>
    /// ��ʼ���������İ汾��Ϣ
    /// </summary>
    /// <param name="url"></param>
    /// <param name="onInitVersion"></param>
    public void InitServerVersion(string url, Action<List<DownloadDataEntity>> onInitVersion)
    {
        m_VersionUrl = url;
        OnInitVersion = onInitVersion;
    }
    #endregion

    #region ���ط������汾�ļ�
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

    #region ʹ�������������ļ�
    public void DownloadFiles(List<DownloadDataEntity> needDownloadList)
    {
        m_RoutineIndex = 0;
        TotalSize = 0;
        TotalCount = 0;

        //��ʼ��������
        for (int i = 0; i < m_RoutineArr.Length; i++)
        {
            if (m_RoutineArr[i] == null)
            {
                m_RoutineArr[i] = gameObject.AddComponent<AssetBundleDownloadRoutine>();
            }
        }

        //ѭ���ĸ���������������
        for (int i = 0; i < needDownloadList.Count; i++)
        {
            m_RoutineIndex = m_RoutineIndex % m_RoutineArr.Length;

            //���е�һ�������� ��������һ���ļ�
            m_RoutineArr[m_RoutineIndex].AddDownload(needDownloadList[i]);

            m_RoutineIndex++;
            TotalSize += needDownloadList[i].Size;
            TotalCount++;
        }

        //����������ʼ����
        for (int i = 0; i < m_RoutineArr.Length; i++)
        {
            if (m_RoutineArr[i] == null) continue;

            m_RoutineArr[i].StartDownload();
        }
    }
    #endregion

    #region ��ǰ�Ѿ����ص��ļ��ܴ�С
    /// <summary>
    /// ��ǰ�Ѿ����ص��ļ��ܴ�С
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

    #region ��ǰ�Ѿ����ص��ļ�������
    /// <summary>
    /// ��ǰ�Ѿ����ص��ļ�������
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
