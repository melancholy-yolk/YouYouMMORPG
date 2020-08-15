using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ������Ϣ
/// </summary>
public class RoleInfoSkill 
{
    public int SkillId; //���ܱ��
    public int SkillLevel; //���ܵȼ�
    public byte SlotsNo; //���ܲ۱��

    private float m_SkillCDTime = 0f;
    public float SkillCDTime//��ȴʱ��
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
    public int SpendMP//����
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
    public float SkillCDEndTime;//��ȴ����ʱ��

}
