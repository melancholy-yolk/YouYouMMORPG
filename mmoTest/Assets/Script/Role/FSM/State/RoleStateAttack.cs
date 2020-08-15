using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStateAttack : RoleStateAbstract 
{
    /// <summary>
    /// ����������ִ������
    /// </summary>
    public string AnimatorCondition;

    /// <summary>
    /// (��)����������ִ������ Ŀ�ģ���Ϊ���ȵ�����һ��״̬���뿪
    /// </summary>
    private string OldAnimatorCondition;

    /// <summary>
    /// ����������ִ������ ֵ
    /// </summary>
    public int AnimatorConditionValue;

    public RoleAnimatorState CurrAnimatorState;

    public RoleStateAttack(RoleFSMMgr roleFSMMgr)
        : base(roleFSMMgr)
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();

        CurrRoleFSMMgr.CurrRoleCtrl.PreviousFightTime = Time.time;
        CurrRoleFSMMgr.CurrRoleCtrl.IsRigidity = true;
        OldAnimatorCondition = AnimatorCondition;
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorCondition, AnimatorConditionValue);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(CurrAnimatorState.ToString()))
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)CurrAnimatorState);

            //�������ִ����һ�� ���л�����
            if (CurrRoleAnimatorStateInfo.normalizedTime > 1)
            {
                CurrRoleFSMMgr.CurrRoleCtrl.IsRigidity = false;
                CurrRoleFSMMgr.CurrRoleCtrl.ToIdle(RoleIdleState.IdleFight);
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

        CurrRoleFSMMgr.CurrRoleCtrl.IsRigidity = false;
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(OldAnimatorCondition, 0);
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
    }
}
