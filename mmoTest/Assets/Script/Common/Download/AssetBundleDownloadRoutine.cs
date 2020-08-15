using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 下载器
/// </summary>
public class AssetBundleDownloadRoutine : MonoBehaviour
{
    #region 变量
    private List<DownloadDataEntity> m_List = new List<DownloadDataEntity>();//下载器需要下载的文件列表
    private DownloadDataEntity m_CurrDownloadData;//当前正在下载的数据

    public int NeedDownloadCount//需要下载的数量
    {
        get;
        private set;
    }

    public int CompletedCount//已经下载完成的数量
    {
        get;
        private set;
    }

    private int m_DownloadSize;//已经下载好的文件的总大小
    private int m_CurrDownloadSize;//当前下载的文件大小

    public int DownloadSize//这个下载器已经下载的大小
    {
        get { return m_DownloadSize + m_CurrDownloadSize; }
    }

    public bool IsStartDownload//是否开始下载
    {
        private set;
        get;
    }
    #endregion


    /// <summary>
    /// 添加要下载对象
    /// </summary>
    /// <param name="entity"></param>
    public void AddDownload(DownloadDataEntity entity)
    {
        m_List.Add(entity);
    }

    /// <summary>
    /// 开始下载
    /// </summary>
    public void StartDownload()
    {
        IsStartDownload = true;
        NeedDownloadCount = m_List.Count;
    }

    void Update()
    {
        if (IsStartDownload)
        {
            IsStartDownload = false;
            StartCoroutine(DownloadData());
        }
    }

    #region 协程 使用WWW下载一个文件
    /// <summary>
    /// 下载一个文件
    /// </summary>
    private IEnumerator DownloadData()
    {
        if (NeedDownloadCount == 0) yield break;

        m_CurrDownloadData = m_List[0];//当前正在下载的实体

        //服务器上的资源下载路径
        string dataUrl = DownloadMgr.DownloadUrl + m_CurrDownloadData.FullName;

        //短路径 用于创建文件夹
        string path = m_CurrDownloadData.FullName.Substring(0, m_CurrDownloadData.FullName.LastIndexOf('\\'));

        //得到本地路径 即在客户端本地当前下载文件存放的文件夹路径
        string localFilePath = DownloadMgr.Instance.LocalFilePath + path;
        if (!Directory.Exists(localFilePath))
        {
            Directory.CreateDirectory(localFilePath);
        }

        //使用WWW类进行下载
        WWW www = new WWW(dataUrl);
        float timeout = Time.time;
        float progress = www.progress;
        while (www != null && !www.isDone)
        {
            if (progress < www.progress)
            {
                timeout = Time.time;
                progress = www.progress;

                m_CurrDownloadSize = (int)(m_CurrDownloadData.Size * progress);
            }

            if ((Time.time - timeout) > DownloadMgr.DownloadTimeOut)
            {
                AppDebug.LogError("download fail!");
                yield break;
            }

            yield return null;//一定要等一帧 否则会卡死
        }

        yield return www;

        if (www != null && www.error == null)
        {
            using (FileStream fs = new FileStream(DownloadMgr.Instance.LocalFilePath + m_CurrDownloadData.FullName, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(www.bytes, 0, www.bytes.Length);
            }
        }

        //下载成功
        m_CurrDownloadSize = 0;
        m_DownloadSize += m_CurrDownloadData.Size;

        //写入本地文件
        DownloadMgr.Instance.ModifyLocalData(m_CurrDownloadData);

        m_List.RemoveAt(0);//将已经下载完成的对象从需要下载列表移除
        CompletedCount++;//已经下载完成的数量加一

        if (m_List.Count == 0)//需要下载列表为空
        {
            m_List.Clear();
        }
        else//需要下载列表不为空 继续下载
        {
            IsStartDownload = true;
        }
    }
    #endregion

}
