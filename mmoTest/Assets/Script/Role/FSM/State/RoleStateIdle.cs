using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStateIdle : RoleStateAbstract 
{
    private float timer = 0f;//��ʱ��
    private float m_ChangeStep = 5f;//��ʱ���仯��� ÿ�������� ����һ�����д�������
    private bool m_IsXiuXian = false;//��ǰ�Ƿ�������״̬

    private float m_RuningTime = 0;//��״̬������ʱ��

    public RoleStateIdle(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr) { }

    public override void OnEnter()
    {
        base.OnEnter();

        if (CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer || CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.OtherPlayer)//����
        {
            if (CurrRoleFSMMgr.CurrIdleState == RoleIdleState.IdleNormal)//��ͨ����
            {
                timer = 0;//��ͨ�����л������д����ļ�ʱ��
                m_IsXiuXian = false;//��ǰ�������д���

                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleNormal.ToString(), true);//������ͨ����
            }
            else
            {
                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleFight.ToString(), true);//����ս������
            }
            m_RuningTime = 0;
        }
        else//����
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleFight.ToString(), true);
        }
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer || CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.OtherPlayer)
        {
            //======== ����Animator��CurrState���� ========
            if (IsChangedState == false)//�����û�����ù�CurrState����
            {
                if (CurrRoleFSMMgr.CurrIdleState == RoleIdleState.IdleNormal)//�����ǰ����״̬Ϊ normal
                {
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
                    if (m_IsXiuXian == false)//�����ǰ���ŵĲ������д�������
                    {
                        #region ��ͨ����
                        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Normal.ToString()))//�����ǰ����״̬������Idle_Normal״̬
                        {
                            //��ֹƵ���Ľ�����ͬ����״̬
                            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Idle_Normal);

                            //ֻ�е��˶���״̬���г���ת̬ʱ��֮�� �ŰѶ�������CurrState�Ƿ����ñ�־λ��Ϊtrue
                            //m_RuningTime += Time.deltaTime;
                            //if (m_RuningTime > 0.1f)
                            //{
                            //    IsChangedState = true;
                            //}
                        }
                        else
                        {
                            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
                        }
                        #endregion
                    }
                    else//�����ǰ���ŵ������д�������
                    {
                        #region ���д���
                        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.XiuXian.ToString()))
                        {
                            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.XiuXian);
                            //IsChangedState = true;
                        }
                        else
                        {
                            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
                        }
                        #endregion
                    }
                }
                else////�����ǰ����״̬Ϊ fight
                {
                    #region ս������
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
                    if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Fight.ToString()))//�����ǰ����״̬������Idle_Normal״̬
                    {
                        //��ֹƵ���Ľ�����ͬ����״̬
                        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Idle_Fight);

                        //ֻ�е��˶���״̬���г���0.01��֮�� �ŰѶ�������CurrState�Ƿ����ñ�־λ��Ϊtrue
                        //m_RuningTime += Time.deltaTime;
                        //if (m_RuningTime > 0.1f)
                        //{
                        //    IsChangedState = true;
                        //}
                    }
                    else
                    {
                        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
                    }
                    #endregion
                }
            }

            #region ��ͨ���������д����л�
            //======== ���������������������״̬���л� ========
            if (CurrRoleFSMMgr.CurrIdleState == RoleIdleState.IdleNormal)
            {
                timer += Time.deltaTime;//ÿ֡��ʱ���ۼ�
                if (timer >= m_ChangeStep)
                {
                    timer = 0;//��ʱ������ ���¿�ʼ��ʱ
                    m_IsXiuXian = true;
                    IsChangedState = false;//�Ƿ��Ѿ�����������CurrState���ù�ֵ

                    CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleNormal.ToString(), false);
                    CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToXiuXian.ToString(), true);
                }

                if (m_IsXiuXian)
                {
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
                    if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.XiuXian.ToString()) && CurrRoleAnimatorStateInfo.normalizedTime > 1)
                    {
                        m_IsXiuXian = false;
                        IsChangedState = false;

                        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleNormal.ToString(), true);
                        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToXiuXian.ToString(), false);
                    }
                }
            }
            #endregion
        }
        else//����==========================================================================================================
        {
            if (IsChangedState == false)
            {
                CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
                if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Fight.ToString()))
                {
                    //��ֹƵ���Ľ�����ͬ����״̬ ��ɹ���
                    CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Idle_Fight);

                    //��ҵĳ���״̬�Ѿ��ı� ���Ƕ���״̬���ܻ�����ת̬�׶� ���ֱ������CurrState��ֵ �ͻ���ת̬���� ��ҽ��벻����һ��״̬
                    //�������ó���״̬���һ��ʱ���ڣ�����״̬����ת̬�У� 
                    //ת̬��� ����������һ��״̬�ɹ�0.1s֮��
                    //m_RuningTime += Time.deltaTime;
                    //if (m_RuningTime > 0.1f)
                    //{
                    //    IsChangedState = true;
                    //}

                }
                else
                {
                    //��ֹ��ԭ����
                    CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
                }
            }
        }
        

        
    }

    public override void OnLeave()
    {
        base.OnLeave();

        if (CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer || CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.OtherPlayer)//����
        {
            if (CurrRoleFSMMgr.CurrIdleState == RoleIdleState.IdleNormal)
            {
                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleNormal.ToString(), false);
                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToXiuXian.ToString(), false);
            }
            else
            {
                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleFight.ToString(), false);
            }
        }
        else//����
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleNormal.ToString(), false);
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleFight.ToString(), false);
        }
        
    }

}
