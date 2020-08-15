using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 下载管理器
/// </summary>
public class DownloadMgr : Singleton<DownloadMgr> 
{
    public const int DownloadTimeOut = 5;//超时时间
    public const string DownloadBaseUrl = "http://127.0.0.1:8081";//这个地址以后应该改成从服务器读取
    public const int DownloadRoutineNum = 1;//下载器的数量

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public const string DownloadUrl = DownloadBaseUrl + "/Windows/";
#elif UNITY_ANDROID
    public const string DownloadUrl = DownloadBaseUrl + "/Android/";
#elif UNITY_IPHONE
    public const string DownloadUrl = DownloadBaseUrl + "/iOS/";
#endif

    public string LocalFilePath = Application.persistentDataPath + "/";//本地资源路径

    private string m_LocalVersionFilePath;//本地的版本文件路径

    public List<DownloadDataEntity> m_NeedDownloadDataList = new List<DownloadDataEntity>();//需要下载的数据列表

    public List<DownloadDataEntity> m_LocalDataList = new List<DownloadDataEntity>();//本地的数据列表

    private const string m_VersionFileName = "VersionFile.txt";

    private string m_StreamingAssetsPath;//资源初始化的时候 原始路径

    public Action OnInitComplete;//初始化完毕


    /// <summary>
    /// 第一步：初始化资源:将StreamingAssets/AssetBundles文件夹下的资源 拷贝到持久化目录中去 避免从服务器下载(安装包--->手机本地)
    /// </summary>
    public void InitStreamingAssets(Action onInitComplete)
    {
        OnInitComplete = onInitComplete;

        m_LocalVersionFilePath = LocalFilePath + m_VersionFileName;//Application.persistentDataPath + "/VersionFile.txt"

        //判断本地是否已经有资源
        if (File.Exists(m_LocalVersionFilePath))
        {
            //如果有资源 则检查更新
            InitCheckVersion();
        }
        else
        {
            //如果没有资源 执行初始化 然后再检查更新
            m_StreamingAssetsPath = "file:///" + Application.streamingAssetsPath + "/AssetBundles/";

#if UNITY_ANDROID && !UNITY_EDITOR
            m_StreamingAssetsPath = Application.streamingAssetsPath + "/AssetBundles/";
#endif

            string versionFileUrl = m_StreamingAssetsPath + m_VersionFileName;

            //使用WWW读取出 StreamingAssets/AssetBundles/versionFile.txt 的内容
            GlobalInit.Instance.StartCoroutine(ReadStreamingAssetsVersionFile(versionFileUrl, onReadStreamingAssetsOverCallBack));
        }
    }

    #region 初始化资源流程
    /// <summary>
    /// 读取初始资源目录的版本文件
    /// </summary>
    private IEnumerator ReadStreamingAssetsVersionFile(string fileUrl, Action<string> onReadStreamingAssetsOver)
    {
        UISceneInitView.Instance.SetProgress("正在准备进行资源初始化", 0);

        using (WWW www = new WWW(fileUrl))
        {
            yield return www;
            if (www.error == null)
            {
                if (onReadStreamingAssetsOver != null)
                {
                    onReadStreamingAssetsOver(Encoding.UTF8.GetString(www.bytes));
                }
            }
            else
            {
                onReadStreamingAssetsOver("");
            }
        }

    }

    /// <summary>
    /// 读取版本文件完毕
    /// </summary>
    private void onReadStreamingAssetsOverCallBack(string content)
    {
        GlobalInit.Instance.StartCoroutine(InitStreamingAssetsList(content));
    }

    /// <summary>
    /// 初始化资源清单
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private IEnumerator InitStreamingAssetsList(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            InitCheckVersion();
            yield break;
        }

        //循环解压
        string[] arrLine = content.Split('\n');
        for (int i = 0; i < arrLine.Length; i++)
        {
            string[] arrData = arrLine[i].Split(' ');

            string fileUrl = arrData[0];//短路径 例如：Download\Scene\GameScene_XueDi2.unity

            //将每一个文件从StreamingAssetsPath读取出来 然后写入到PersistentDataPath
            yield return GlobalInit.Instance.StartCoroutine(AssetLoadToLocal(m_StreamingAssetsPath + fileUrl, LocalFilePath + fileUrl));

            float value = (i + 1) / (float)arrLine.Length;
            UISceneInitView.Instance.SetProgress(string.Format("初始化资源不消耗流量 {0}/{1}", i + 1, arrLine.Length), value);
        }

        //解压版本文件
        yield return GlobalInit.Instance.StartCoroutine(AssetLoadToLocal(m_StreamingAssetsPath + m_VersionFileName, LocalFilePath + m_VersionFileName));

        //检查更新
        InitCheckVersion();
    }

    /// <summary>
    /// 解压某个文件到本地
    /// </summary>
    private IEnumerator AssetLoadToLocal(string fileUrl, string toPath)
    {
        using (WWW www = new WWW(fileUrl))
        {
            yield return www;
            if (www.error == null)
            {
                int lastIndexof = toPath.LastIndexOf('\\');
                if (lastIndexof != -1)
                {
                    string localPath = toPath.Substring(0, lastIndexof);//除去文件名以外的路径
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }
                }

                using (FileStream fs = File.Create(toPath, www.bytes.Length))
                {
                    fs.Write(www.bytes, 0, www.bytes.Length);
                    fs.Close();
                }
            }
        }
    }
    #endregion

    #region 资源更新流程
    /// <summary>
    /// 第二部：检查版本文件
    /// </summary>
    public void InitCheckVersion()
    {
        UISceneInitView.Instance.SetProgress("正在进行版本更新", 0);

        //服务器的版本文件路径
        string strVersionUrl = DownloadUrl + m_VersionFileName;
        //下载服务器上的版本文件
        AssetBundleDownload.Instance.InitServerVersion(strVersionUrl, OnInitVersionCallBack);
    }

    /// <summary>
    /// 服务器版本文件下载成功回调 对比服务器与本地版本文件
    /// </summary>
    /// <param name="serverDownloadDataList"></param>
    private void OnInitVersionCallBack(List<DownloadDataEntity> serverDownloadDataList)
    {
        //与本地的版本文件进行对比
        
        if (File.Exists(m_LocalVersionFilePath))
        {
            //如果本地存在版本文件 则和服务器端的版本文件进行对比
            //服务器端资源字典
            Dictionary<string, string> serverDic = PackDownloadDataDic(serverDownloadDataList);

            //客户端本地资源列表
            string localVersionFileContent = IOUtil.GetFileText(m_LocalVersionFilePath);
            Dictionary<string, string> localDic = PackDownloadDataDic(localVersionFileContent);
            m_LocalDataList = PackDownloadData(localVersionFileContent);

            //对比
            //1.本地没有的 服务器新加的初始资源
            for (int i = 0; i < serverDownloadDataList.Count; i++)
            {
                if (serverDownloadDataList[i].IsFirstData && !localDic.ContainsKey(serverDownloadDataList[i].FullName))
                {
                    m_NeedDownloadDataList.Add(serverDownloadDataList[i]);
                }
            }

            //2.已经下载过 但是有更新的资源
            foreach (var item in localDic)
            {
                //如果MD5不一致
                if (serverDic.ContainsKey(item.Key) && serverDic[item.Key] != item.Value)
                {
                    DownloadDataEntity entity = GetDownloadData(item.Key, serverDownloadDataList);
                    if (entity != null)
                    {
                        m_NeedDownloadDataList.Add(entity);
                    }
                }
            }

        }
        else//第一次下载
        {
            //如果本地不存在版本文件 则初始资源就是需要下载的文件
            for (int i = 0; i < serverDownloadDataList.Count; i++)
            {
                if (serverDownloadDataList[i].IsFirstData)
                {
                    m_NeedDownloadDataList.Add(serverDownloadDataList[i]);
                }
            }
        }

        if (m_NeedDownloadDataList.Count == 0)
        {
            UISceneInitView.Instance.SetProgress("资源更新完毕", 1);
            if (OnInitComplete != null)
            {
                OnInitComplete();
            }
            return;
        }

        //拿到需要下载的资源列表 进行下载
        AssetBundleDownload.Instance.DownloadFiles(m_NeedDownloadDataList);
    }
    #endregion

    /// <summary>
    /// 根据资源名称 获取资源实体
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    private DownloadDataEntity GetDownloadData(string fullName, List<DownloadDataEntity> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].FullName.Equals(fullName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return list[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 根据文本内容封装
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public List<DownloadDataEntity> PackDownloadData(string content)
    {
        List<DownloadDataEntity> list = new List<DownloadDataEntity>();

        string[] arrLine = content.Split('\n');
        for (int i = 0; i < arrLine.Length; i++)
        {
            string[] arrData = arrLine[i].Split(' ');
            if (arrData.Length == 4)
            {
                DownloadDataEntity entity = new DownloadDataEntity();
                entity.FullName = arrData[0];
                entity.MD5 = arrData[1];
                entity.Size = arrData[2].ToInt();
                entity.IsFirstData = arrData[3].ToInt() == 1;
                list.Add(entity);
            }
        }

        return list;
    }

    public Dictionary<string, string> PackDownloadDataDic(List<DownloadDataEntity> list)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();

        for (int i = 0; i < list.Count; i++)
        {
            dic[list[i].FullName] = list[i].MD5;
        }

        return dic;
    }

    public Dictionary<string, string> PackDownloadDataDic(string content)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();

        string[] arrLine = content.Split('\n');
        for (int i = 0; i < arrLine.Length; i++)
        {
            string[] arrData = arrLine[i].Split(' ');
            if (arrData.Length == 4)
            {
                dic[arrData[0]] = arrData[1];
            }
        }

        return dic;
    }

    /// <summary>
    /// 修改本地文件
    /// </summary>
    /// <param name="entity"></param>
    public void ModifyLocalData(DownloadDataEntity entity)
    {
        if (m_LocalDataList == null)
        {
            return;
        }
        bool isExist = false;

        for (int i = 0; i < m_LocalDataList.Count; i++)
        {
            if (m_LocalDataList[i].FullName.Equals(entity.FullName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                m_LocalDataList[i].MD5 = entity.MD5;
                m_LocalDataList[i].Size = entity.Size;
                m_LocalDataList[i].IsFirstData = entity.IsFirstData;
                isExist = true;
                break;
            }
        }

        if (!isExist)
        {
            m_LocalDataList.Add(entity);
        }

        SaveLocalVersionFile();
    }

    
    /// <summary>
    /// 保存本地版本文件
    /// </summary>
    private void SaveLocalVersionFile()
    {
        StringBuilder sbContent = new StringBuilder();

        for (int i = 0; i < m_LocalDataList.Count; i++)
        {
            DownloadDataEntity entity = m_LocalDataList[i];
            sbContent.AppendLine(string.Format("{0} {1} {2} {3}", entity.FullName, entity.MD5, entity.Size, entity.IsFirstData ? 1 : 0));
        }

        IOUtil.CreateTextFile(m_LocalVersionFilePath, sbContent.ToString());
    }
}
