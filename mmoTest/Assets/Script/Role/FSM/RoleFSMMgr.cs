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

    public RoleIdleState ToIdleState { get; set; }//要切换到的待机状态
    public RoleIdleState CurrIdleState { get; set; }//当前待机状态

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
    /// 返回状态机中的状态
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
        //如果新状态与当前状态相同 并且不是待机和攻击状态 那么直接返回
        if (CurrRoleStateEnum == newState && newState != RoleState.Idle && CurrRoleStateEnum != RoleState.Attack ) return;

        //调用以前状态的离开方法
        if (m_CurrRoleState != null)
            m_CurrRoleState.OnLeave();

        //更改当前状态枚举
        CurrRoleStateEnum = newState;

        //更改当前状态
        m_CurrRoleState = m_RoleStateDic[newState];

        //如果当前是待机状态 
        if (CurrRoleStateEnum == RoleState.Idle)
        {
            //给当前待机状态赋值
            CurrIdleState = ToIdleState;
        }

        ///调用新状态的进入方法
        m_CurrRoleState.OnEnter();
    }
}
