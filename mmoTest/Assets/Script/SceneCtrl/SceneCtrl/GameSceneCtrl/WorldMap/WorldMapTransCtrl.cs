using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ͼ���͵������
/// </summary>
public class WorldMapTransCtrl : MonoBehaviour 
{
    private int m_TransPointId;//���͵���
    private int m_TargetSceneId;//Ҫ���͵ĳ���Id
    private int m_TargetSceneTransPointId;//Ŀ�곡���������͵�id

    public int TargetTransSceneId
    {
        get
        {
            return m_TargetSceneId;
        }
    }
	
	void Start () 
    {
		
	}

    public void SetParam(TransPointWorldMapData data)
    {
        m_TransPointId = data.TransPointId;
        m_TargetSceneId = data.TargetSceneId;
        m_TargetSceneTransPointId = data.TargetSceneTransPointId;
    }

    

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            RoleCtrl ctrl = collider.gameObject.GetComponent<RoleCtrl>();

            if (ctrl.CurrRoleType == RoleType.MainPlayer)
            {
                SceneMgr.Instance.TargetWorldMapTransPointId = m_TargetSceneTransPointId;//����Ŀ�������ͼ�Ĵ��͵�Id
                SceneMgr.Instance.LoadToWorldMap(m_TargetSceneId);
            }
        }
    }
}
