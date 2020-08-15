using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCtrl : MonoBehaviour 
{
    [SerializeField]
    private Transform m_HeadBarPos;
    private GameObject m_HeadBar;
    private NPCEntity m_CurrNPCEntity;
    private NPCHeadBarView m_NPCHeadBarView;
    private float m_NextTalkTime = 0;
    private string[] m_NPCTalk;

    public void Init(NPCWorldMapData npcData)
    {
        m_CurrNPCEntity = NPCDBModel.Instance.GetEntity(npcData.NPCId);
        m_NPCTalk = m_CurrNPCEntity.Talk.Split('|');
    }

    private void InitHeadBar()
    {
        if (RoleHeadBarRoot.Instance == null) return;
        if (m_CurrNPCEntity == null) return;
        if (m_HeadBarPos == null) return;

        //克隆预设
        m_HeadBar = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIItem, "MainCity", "NPCHeadBar");
        m_HeadBar.transform.SetParent(RoleHeadBarRoot.Instance.gameObject.transform);
        m_HeadBar.transform.localScale = Vector3.one;
        m_HeadBar.transform.localPosition = Vector3.zero;

        m_NPCHeadBarView = m_HeadBar.GetComponent<NPCHeadBarView>();

        //给预设赋值
        m_NPCHeadBarView.Init(m_HeadBarPos, m_CurrNPCEntity.Name, isShowHPBar: false);
    }

	void Start () 
    {
        InitHeadBar();
	}
	
	
	void Update () 
    {
        if (Time.time > m_NextTalkTime)
        {
            m_NextTalkTime = Time.time + 10f;

            //10s一次
            if (m_NPCHeadBarView != null && m_NPCTalk.Length > 0)
            {

                m_NPCHeadBarView.Talk(m_NPCTalk[Random.Range(0, m_NPCTalk.Length)], 5f);
            }
        }

        
	}
}
