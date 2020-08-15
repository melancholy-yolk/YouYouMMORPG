using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 采集角色攻击攻击信息
/// </summary>
[System.Serializable]
public class RoleAttackInfo 
{
    public int Index;
    public int SkillId;

    public string EffectName;//特效名称 真正的从ab包中加载特效
#if DEBUG_ROLE_STATE
    public GameObject EffectObject;//测试时 直接在编辑器中拖拽
#endif
    public float EffectLifeTime;//特效存在时间

    public float AttackRange;//攻击范围
    public float HurtDelay;//受击者受伤延迟

    public bool IsCameraShake = false;//是否震屏
    public float CameraShakeDelay;
    public float CameraShakeDuration;
    public float CameraShakeStrength;
    public int CameraShakeVibrto;
}
