using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStateSelect : RoleStateAbstract 
{
    public RoleStateSelect(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr) { }

    public override void OnEnter()
    {
        base.OnEnter();
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToSelect.ToString(), true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Select.ToString()))
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Select);
            if (CurrRoleAnimatorStateInfo.normalizedTime > 1.0f)
            {
                CurrRoleFSMMgr.CurrRoleCtrl.ToIdle();
            }
        }
        else
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToSelect.ToString(), false);
    }
}
