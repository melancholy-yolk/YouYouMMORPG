using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色传递的攻击信息
/// </summary>
public class RoleTransferAttackInfo 
{
    public int AttackRoleId;//发起攻击者编号
    public Vector3 AttackRolePos;//攻击者的位置
    public int BeAttackedRoleId;//被攻击者编号
    public int HurtValue;//伤害值
    public int SkillId;//攻击者使用的技能编号
    public int SkillLevel;//攻击者使用的技能等级
    public bool IsAbnormal;//是否附加异常状态
    public bool IsCri;//是否暴击
}
