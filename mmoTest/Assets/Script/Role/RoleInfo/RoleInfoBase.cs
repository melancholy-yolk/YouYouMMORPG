using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleInfoBase
{
    public int RoleId;//��ɫ���
    public string RoleNickName;//�ǳ�

    public int MaxHP;
    public int MaxMP;

    public int CurrHP;
    public int CurrMP;

    public int Attack;
    public int Defense;
    public int Hit;
    public int Dodge;
    public int Cri;
    public int Res;

    public int Fighting;

    public List<RoleInfoSkill> SkillList;
    public int[] PhySkillIds;

    public RoleInfoBase()
    {
        SkillList = new List<RoleInfoSkill>();
    }

    /// <summary>
    /// ��ѯ��ɫ���ܵȼ�
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public int GetSkillLevelBySkillId(int skillId)
    {
        if (SkillList == null)
        {
            return 1;
        }
        for (int i = 0; i < SkillList.Count; i++)
        {
            if (SkillList[i].SkillId == skillId)
            {
                return SkillList[i].SkillLevel;
            }
        }
        return 1;
    }

    public void SetPhySkillId(string phySkillIds)
    {
        string[] ids = phySkillIds.Split(';');
        PhySkillIds = new int[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            PhySkillIds[i] = ids[i].ToInt();
        }
    }

    /// <summary>
    /// ÿ��ʹ�ü���ʱ ���ü�����ȴ����ʱ��(�����ٴ�ʹ�õ�ʱ��)
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="endTime"></param>
    public void SetSkillCDEndTime(int skillId)
    {
        if (SkillList.Count > 0)
        {
            for (int i = 0; i < SkillList.Count; i++)
            {
                if (SkillList[i].SkillId == skillId)
                {
                    SkillList[i].SkillCDEndTime = SkillList[i].SkillCDTime + Time.time;
                    Debug.Log(SkillList[i].SkillCDEndTime);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ��ȡ��ǰ��ʹ�õļ���Id
    /// </summary>
    /// <returns></returns>
    public int GetCanUseSkillId()
    {
        if (SkillList.Count > 0)
        {
            for (int i = 0; i < SkillList.Count; i++)
            {
                //�˿̵�ʱ����ڼ�����ȴ����ʱ�� ��ǰħ��ֵ���ڵ��ڼ��ܺ���
                if (Time.time > SkillList[i].SkillCDEndTime && CurrMP >= SkillList[i].SpendMP)
                {
                    return SkillList[i].SkillId;
                }
            }
        }
        return 0;
    }

}
