using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoleStateDie : RoleStateAbstract 
{
    public Action OnRoleDie;//��ɫ����ί��
    public Action OnRoleDestroy;//��ɫ����ί��
    private float m_BeginDieTime = 0f;
    private bool m_IsDestroy = false;//�Ƿ��Ѿ����ٽ�ɫ

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="roleFSMMgr">����״̬��������</param>
    public RoleStateDie(RoleFSMMgr roleFSMMgr)
        : base(roleFSMMgr)
    {

    }

    /// <summary>
    /// ʵ�ֻ��� ����״̬
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        m_IsDestroy = false;

        //��ɫ�����Ѿ�����״̬
        if (CurrRoleFSMMgr.CurrRoleCtrl.IsDied)
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToDied.ToString(), true);
        }
        else
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToDie.ToString(), true);

            //������Ѫ��Ч
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
    /// ʵ�ֻ��� ִ��״̬
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

            //ȷ������ִֻ��һ��
            if (!m_IsDestroy)
            {
                if (m_BeginDieTime >= 6f)
                {
                    //��ɫ����
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
    /// ʵ�ֻ��� �뿪״̬
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToDie.ToString(), false);
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToDied.ToString(), false);
    }
}
