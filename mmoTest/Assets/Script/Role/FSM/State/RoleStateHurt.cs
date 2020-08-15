using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStateHurt : RoleStateAbstract 
{
    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="roleFSMMgr">����״̬��������</param>
    public RoleStateHurt(RoleFSMMgr roleFSMMgr)
        : base(roleFSMMgr)
    {

    }

    /// <summary>
    /// ʵ�ֻ��� ����״̬
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();

        CurrRoleFSMMgr.CurrRoleCtrl.PreviousFightTime = Time.time;
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToHurt.ToString(), true);
    }

    /// <summary>
    /// ʵ�ֻ��� ִ��״̬
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();

        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Hurt.ToString()))
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Hurt);

            //�������ִ����һ�� ���л�����
            if (CurrRoleAnimatorStateInfo.normalizedTime > 1)
            {
                CurrRoleFSMMgr.CurrRoleCtrl.ToIdle(RoleIdleState.IdleFight);
            }
        }
    }

    /// <summary>
    /// ʵ�ֻ��� �뿪״̬
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToHurt.ToString(), false);
    }
}
