using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色技能信息
/// </summary>
public class RoleInfoSkill 
{
    public int SkillId; //技能编号
    public int SkillLevel; //技能等级
    public byte SlotsNo; //技能槽编号

    private float m_SkillCDTime = 0f;
    public float SkillCDTime//冷却时间
    {
        get
        {
            if (m_SkillCDTime == 0)
            {
                m_SkillCDTime = SkillLevelDBModel.Instance.GetEntityBySkillIdAndSkillLevel(SkillId, SkillLevel).SkillCDTime;
            }
            return m_SkillCDTime;
        }
    }

    private int m_SpendMP = 0;
    public int SpendMP//耗蓝
    {
        get
        {
            if (m_SpendMP == 0)
            {
                m_SpendMP = SkillLevelDBModel.Instance.GetEntityBySkillIdAndSkillLevel(SkillId, SkillLevel).SpendMP;
            }
            return m_SpendMP;
        }
    }
    public float SkillCDEndTime;//冷却结束时间

}
