using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIMainCitySkillView : MonoBehaviour 
{
    public static UIMainCitySkillView Instance;

    [SerializeField]
    private UIMainCitySkillSlotView BtnSkill1;
    [SerializeField]
    private UIMainCitySkillSlotView BtnSkill2;
    [SerializeField]
    private UIMainCitySkillSlotView BtnSkill3;
    [SerializeField]
    private UIMainCitySkillSlotView BtnSkillAddHp;

    private Dictionary<int, UIMainCitySkillSlotView> m_Dic = new Dictionary<int, UIMainCitySkillSlotView>();

    public void SetUI(List<TransferData> list, Action<int> OnSkillClick)
    {
        m_Dic.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            int skillSlotNo = list[i].GetValue<byte>(ConstDefine.SkillSlotNo);
            int skillId = list[i].GetValue<int>(ConstDefine.SkillId);
            int skillLevel = list[i].GetValue<int>(ConstDefine.SkillLevel);
            string skillPic = list[i].GetValue<string>(ConstDefine.SkillPic);
            float skillCDTime = list[i].GetValue<float>(ConstDefine.SkillCDTime);

            switch (skillSlotNo)
            {
                case 1:
                    BtnSkill1.SetUI(skillId, skillPic, skillCDTime, OnSkillClick);
                    m_Dic[skillId] = BtnSkill1;
                    break;
                case 2:
                    BtnSkill2.SetUI(skillId, skillPic, skillCDTime, OnSkillClick);
                    m_Dic[skillId] = BtnSkill2;
                    break;
                case 3:
                    BtnSkill3.SetUI(skillId, skillPic, skillCDTime, OnSkillClick);
                    m_Dic[skillId] = BtnSkill3;
                    break;
            }
        }
    }

    public void BeginCD(int skillId)
    {
        if (m_Dic.ContainsKey(skillId))
        {
            m_Dic[skillId].BeginCD();
        }
    }

    void Awake()
    {
        Instance = this;
    }



}
