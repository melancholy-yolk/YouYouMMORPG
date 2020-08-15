using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSceneCtrl : MonoBehaviour 
{

	void Start () 
    {
        AppDebug.Log(Application.persistentDataPath);
        //���̣�
        //�ж�persistentDataPath����û��VersionFile.txt �еĻ�ִ�и������� û�еĻ�ִ����Դ��ʼ������
        //�ж�StreamingAssets����û��VersionFile.txt û�еĻ�ִ�и������� �еĻ�ִ����Դ��ʼ������

        //��ʼ������
        //��StreamingAssets�µĳ�ʼ��Դ ��ѹ����persistentDataPath��

        //��������
        //1.���ط������汾�ļ�
        //2.�Աȷ������뱾�ذ汾�ļ� �õ������б�
        //3.������Ҫ���ص��ļ�
#if DISABLE_ASSETBUNDLE
        SceneMgr.Instance.LoadToLogOn();
#else
        DownloadMgr.Instance.InitStreamingAssets(OnInitComplete);
#endif
    }

    private IEnumerator ToLogOnScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneMgr.Instance.LoadToLogOn();//��ת����¼����
    }

    public void OnInitComplete()
    {
        StartCoroutine(ToLogOnScene());
    }
}
