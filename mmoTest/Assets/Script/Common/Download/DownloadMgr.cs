using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// ���ع�����
/// </summary>
public class DownloadMgr : Singleton<DownloadMgr> 
{
    public const int DownloadTimeOut = 5;//��ʱʱ��
    public const string DownloadBaseUrl = "http://127.0.0.1:8081";//�����ַ�Ժ�Ӧ�øĳɴӷ�������ȡ
    public const int DownloadRoutineNum = 1;//������������

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public const string DownloadUrl = DownloadBaseUrl + "/Windows/";
#elif UNITY_ANDROID
    public const string DownloadUrl = DownloadBaseUrl + "/Android/";
#elif UNITY_IPHONE
    public const string DownloadUrl = DownloadBaseUrl + "/iOS/";
#endif

    public string LocalFilePath = Application.persistentDataPath + "/";//������Դ·��

    private string m_LocalVersionFilePath;//���صİ汾�ļ�·��

    public List<DownloadDataEntity> m_NeedDownloadDataList = new List<DownloadDataEntity>();//��Ҫ���ص������б�

    public List<DownloadDataEntity> m_LocalDataList = new List<DownloadDataEntity>();//���ص������б�

    private const string m_VersionFileName = "VersionFile.txt";

    private string m_StreamingAssetsPath;//��Դ��ʼ����ʱ�� ԭʼ·��

    public Action OnInitComplete;//��ʼ�����


    /// <summary>
    /// ��һ������ʼ����Դ:��StreamingAssets/AssetBundles�ļ����µ���Դ �������־û�Ŀ¼��ȥ ����ӷ���������(��װ��--->�ֻ�����)
    /// </summary>
    public void InitStreamingAssets(Action onInitComplete)
    {
        OnInitComplete = onInitComplete;

        m_LocalVersionFilePath = LocalFilePath + m_VersionFileName;//Application.persistentDataPath + "/VersionFile.txt"

        //�жϱ����Ƿ��Ѿ�����Դ
        if (File.Exists(m_LocalVersionFilePath))
        {
            //�������Դ �������
            InitCheckVersion();
        }
        else
        {
            //���û����Դ ִ�г�ʼ�� Ȼ���ټ�����
            m_StreamingAssetsPath = "file:///" + Application.streamingAssetsPath + "/AssetBundles/";

#if UNITY_ANDROID && !UNITY_EDITOR
            m_StreamingAssetsPath = Application.streamingAssetsPath + "/AssetBundles/";
#endif

            string versionFileUrl = m_StreamingAssetsPath + m_VersionFileName;

            //ʹ��WWW��ȡ�� StreamingAssets/AssetBundles/versionFile.txt ������
            GlobalInit.Instance.StartCoroutine(ReadStreamingAssetsVersionFile(versionFileUrl, onReadStreamingAssetsOverCallBack));
        }
    }

    #region ��ʼ����Դ����
    /// <summary>
    /// ��ȡ��ʼ��ԴĿ¼�İ汾�ļ�
    /// </summary>
    private IEnumerator ReadStreamingAssetsVersionFile(string fileUrl, Action<string> onReadStreamingAssetsOver)
    {
        UISceneInitView.Instance.SetProgress("����׼��������Դ��ʼ��", 0);

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
    /// ��ȡ�汾�ļ����
    /// </summary>
    private void onReadStreamingAssetsOverCallBack(string content)
    {
        GlobalInit.Instance.StartCoroutine(InitStreamingAssetsList(content));
    }

    /// <summary>
    /// ��ʼ����Դ�嵥
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

        //ѭ����ѹ
        string[] arrLine = content.Split('\n');
        for (int i = 0; i < arrLine.Length; i++)
        {
            string[] arrData = arrLine[i].Split(' ');

            string fileUrl = arrData[0];//��·�� ���磺Download\Scene\GameScene_XueDi2.unity

            //��ÿһ���ļ���StreamingAssetsPath��ȡ���� Ȼ��д�뵽PersistentDataPath
            yield return GlobalInit.Instance.StartCoroutine(AssetLoadToLocal(m_StreamingAssetsPath + fileUrl, LocalFilePath + fileUrl));

            float value = (i + 1) / (float)arrLine.Length;
            UISceneInitView.Instance.SetProgress(string.Format("��ʼ����Դ���������� {0}/{1}", i + 1, arrLine.Length), value);
        }

        //��ѹ�汾�ļ�
        yield return GlobalInit.Instance.StartCoroutine(AssetLoadToLocal(m_StreamingAssetsPath + m_VersionFileName, LocalFilePath + m_VersionFileName));

        //������
        InitCheckVersion();
    }

    /// <summary>
    /// ��ѹĳ���ļ�������
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
                    string localPath = toPath.Substring(0, lastIndexof);//��ȥ�ļ��������·��
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

    #region ��Դ��������
    /// <summary>
    /// �ڶ��������汾�ļ�
    /// </summary>
    public void InitCheckVersion()
    {
        UISceneInitView.Instance.SetProgress("���ڽ��а汾����", 0);

        //�������İ汾�ļ�·��
        string strVersionUrl = DownloadUrl + m_VersionFileName;
        //���ط������ϵİ汾�ļ�
        AssetBundleDownload.Instance.InitServerVersion(strVersionUrl, OnInitVersionCallBack);
    }

    /// <summary>
    /// �������汾�ļ����سɹ��ص� �Աȷ������뱾�ذ汾�ļ�
    /// </summary>
    /// <param name="serverDownloadDataList"></param>
    private void OnInitVersionCallBack(List<DownloadDataEntity> serverDownloadDataList)
    {
        //�뱾�صİ汾�ļ����жԱ�
        
        if (File.Exists(m_LocalVersionFilePath))
        {
            //������ش��ڰ汾�ļ� ��ͷ������˵İ汾�ļ����жԱ�
            //����������Դ�ֵ�
            Dictionary<string, string> serverDic = PackDownloadDataDic(serverDownloadDataList);

            //�ͻ��˱�����Դ�б�
            string localVersionFileContent = IOUtil.GetFileText(m_LocalVersionFilePath);
            Dictionary<string, string> localDic = PackDownloadDataDic(localVersionFileContent);
            m_LocalDataList = PackDownloadData(localVersionFileContent);

            //�Ա�
            //1.����û�е� �������¼ӵĳ�ʼ��Դ
            for (int i = 0; i < serverDownloadDataList.Count; i++)
            {
                if (serverDownloadDataList[i].IsFirstData && !localDic.ContainsKey(serverDownloadDataList[i].FullName))
                {
                    m_NeedDownloadDataList.Add(serverDownloadDataList[i]);
                }
            }

            //2.�Ѿ����ع� �����и��µ���Դ
            foreach (var item in localDic)
            {
                //���MD5��һ��
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
        else//��һ������
        {
            //������ز����ڰ汾�ļ� ���ʼ��Դ������Ҫ���ص��ļ�
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
            UISceneInitView.Instance.SetProgress("��Դ�������", 1);
            if (OnInitComplete != null)
            {
                OnInitComplete();
            }
            return;
        }

        //�õ���Ҫ���ص���Դ�б� ��������
        AssetBundleDownload.Instance.DownloadFiles(m_NeedDownloadDataList);
    }
    #endregion

    /// <summary>
    /// ������Դ���� ��ȡ��Դʵ��
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
    /// �����ı����ݷ�װ
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
    /// �޸ı����ļ�
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
    /// ���汾�ذ汾�ļ�
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
