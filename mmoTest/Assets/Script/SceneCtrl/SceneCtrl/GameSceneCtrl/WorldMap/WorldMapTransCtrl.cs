using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地图传送点控制器
/// </summary>
public class WorldMapTransCtrl : MonoBehaviour 
{
    private int m_TransPointId;//传送点编号
    private int m_TargetSceneId;//要传送的场景Id
    private int m_TargetSceneTransPointId;//目标场景出生传送点id

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
                SceneMgr.Instance.TargetWorldMapTransPointId = m_TargetSceneTransPointId;//设置目标世界地图的传送点Id
                SceneMgr.Instance.LoadToWorldMap(m_TargetSceneId);
            }
        }
    }
}
