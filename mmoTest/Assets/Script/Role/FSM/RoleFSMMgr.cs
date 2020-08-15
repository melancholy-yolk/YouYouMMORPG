using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoleFSMMgr 
{
    public RoleCtrl CurrRoleCtrl { get; private set; }
    public RoleState CurrRoleStateEnum { get; private set; }
    private RoleStateAbstract m_CurrRoleState = null;
    private Dictionary<RoleState, RoleStateAbstract> m_RoleStateDic;

    public RoleIdleState ToIdleState { get; set; }//Ҫ�л����Ĵ���״̬
    public RoleIdleState CurrIdleState { get; set; }//��ǰ����״̬

    public RoleFSMMgr(RoleCtrl currRoleCtrl, Action onDie, Action onDestroy)
    {
        CurrRoleCtrl = currRoleCtrl;
        m_RoleStateDic = new Dictionary<RoleState, RoleStateAbstract>();
        m_RoleStateDic[RoleState.Idle] = new RoleStateIdle(this);
        m_RoleStateDic[RoleState.Run] = new RoleStateRun(this);
        m_RoleStateDic[RoleState.Attack] = new RoleStateAttack(this);
        m_RoleStateDic[RoleState.Hurt] = new RoleStateHurt(this);
        m_RoleStateDic[RoleState.Die] = new RoleStateDie(this);

        RoleStateDie dieState = (RoleStateDie)m_RoleStateDic[RoleState.Die];
        dieState.OnRoleDie = onDie;
        dieState.OnRoleDestroy = onDestroy;

        m_RoleStateDic[RoleState.Select] = new RoleStateSelect(this);

        if (m_RoleStateDic.ContainsKey(CurrRoleStateEnum))
        {
            m_CurrRoleState = m_RoleStateDic[CurrRoleStateEnum];
        }
    }

    /// <summary>
    /// ����״̬���е�״̬
    /// </summary>
    public RoleStateAbstract GetRoleState(RoleState state)
    {
        if (!m_RoleStateDic.ContainsKey(state))
        {
            return null;
        }
        return m_RoleStateDic[state];
    }

    public void OnUpdate()
    {
        if (m_CurrRoleState != null)
        {
            m_CurrRoleState.OnUpdate();
        }
    }

    public void ChangeState(RoleState newState)
    {
        //�����״̬�뵱ǰ״̬��ͬ ���Ҳ��Ǵ����͹���״̬ ��ôֱ�ӷ���
        if (CurrRoleStateEnum == newState && newState != RoleState.Idle && CurrRoleStateEnum != RoleState.Attack ) return;

        //������ǰ״̬���뿪����
        if (m_CurrRoleState != null)
            m_CurrRoleState.OnLeave();

        //���ĵ�ǰ״̬ö��
        CurrRoleStateEnum = newState;

        //���ĵ�ǰ״̬
        m_CurrRoleState = m_RoleStateDic[newState];

        //�����ǰ�Ǵ���״̬ 
        if (CurrRoleStateEnum == RoleState.Idle)
        {
            //����ǰ����״̬��ֵ
            CurrIdleState = ToIdleState;
        }

        ///������״̬�Ľ��뷽��
        m_CurrRoleState.OnEnter();
    }
}
