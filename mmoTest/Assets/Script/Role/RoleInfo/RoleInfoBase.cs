using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleInfoBase
{
    public int RoleId;//角色编号
    public string RoleNickName;//昵称

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
    /// 查询角色技能等级
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
    /// 每次使用技能时 设置技能冷却结束时间(可以再次使用的时间)
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
    /// 获取当前可使用的技能Id
    /// </summary>
    /// <returns></returns>
    public int GetCanUseSkillId()
    {
        if (SkillList.Count > 0)
        {
            for (int i = 0; i < SkillList.Count; i++)
            {
                //此刻的时间大于技能冷却结束时间 当前魔法值大于等于技能耗蓝
                if (Time.time > SkillList[i].SkillCDEndTime && CurrMP >= SkillList[i].SpendMP)
                {
                    return SkillList[i].SkillId;
                }
            }
        }
        return 0;
    }

}
