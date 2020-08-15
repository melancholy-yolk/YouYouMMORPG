using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoleStateDie : RoleStateAbstract 
{
    public Action OnRoleDie;//角色死亡委托
    public Action OnRoleDestroy;//角色销毁委托
    private float m_BeginDieTime = 0f;
    private bool m_IsDestroy = false;//是否已经销毁角色

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleFSMMgr">有限状态机管理器</param>
    public RoleStateDie(RoleFSMMgr roleFSMMgr)
        : base(roleFSMMgr)
    {

    }

    /// <summary>
    /// 实现基类 进入状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        m_IsDestroy = false;

        //角色处于已经死亡状态
        if (CurrRoleFSMMgr.CurrRoleCtrl.IsDied)
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToDied.ToString(), true);
        }
        else
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToDie.ToString(), true);

            //播放喷血特效
            Transform obj = EffectMgr.Instance.PlayEffect("Download/Prefab/Effect/Common/", "Effect_PenXue");
            obj.position = CurrRoleFSMMgr.CurrRoleCtrl.transform.position;
            obj.rotation = CurrRoleFSMMgr.CurrRoleCtrl.transform.rotation;
            EffectMgr.Instance.DestroyEffect(obj, 5f);

            if (OnRoleDie != null)
            {
                OnRoleDie();
            }

            m_BeginDieTime = 0f;
        }
    }

    /// <summary>
    /// 实现基类 执行状态
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (CurrRoleFSMMgr.CurrRoleCtrl.IsDied)
        {
            CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
            if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Died.ToString()))
            {
                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Died);
            }
        }
        else
        {
            m_BeginDieTime += Time.deltaTime;

            //确保销毁只执行一次
            if (!m_IsDestroy)
            {
                if (m_BeginDieTime >= 6f)
                {
                    //角色回收
                    if (OnRoleDestroy != null)
                    {
                        OnRoleDestroy();
                        m_IsDestroy = true;
                    }
                    return;
                }
            }

            CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
            if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Die.ToString()))
            {
                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Die);
            }
        }

    }

    /// <summary>
    /// 实现基类 离开状态
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToDie.ToString(), false);
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToDied.ToString(), false);
    }
}
