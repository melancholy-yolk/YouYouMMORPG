using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStateIdle : RoleStateAbstract 
{
    private float timer = 0f;//计时器
    private float m_ChangeStep = 5f;//计时器变化间隔 每隔多少秒 播放一次休闲待机动画
    private bool m_IsXiuXian = false;//当前是否处于休闲状态

    private float m_RuningTime = 0;//此状态的运行时间

    public RoleStateIdle(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr) { }

    public override void OnEnter()
    {
        base.OnEnter();

        if (CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer || CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.OtherPlayer)//主角
        {
            if (CurrRoleFSMMgr.CurrIdleState == RoleIdleState.IdleNormal)//普通待机
            {
                timer = 0;//普通待机切换到休闲待机的计时器
                m_IsXiuXian = false;//当前不是休闲待机

                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleNormal.ToString(), true);//播放普通待机
            }
            else
            {
                CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleFight.ToString(), true);//播放战斗待机
            }
            m_RuningTime = 0;
        }
        else//怪物
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleFight.ToString(), true);
        }
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer || CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.OtherPlayer)
        {
            //======== 设置Animator的CurrState参数 ========
            if (IsChangedState == false)//如果还没有设置过CurrState参数
            {
                if (CurrRoleFSMMgr.CurrIdleState == RoleIdleState.IdleNormal)//如果当前待机状态为 normal
                {
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
                    if (m_IsXiuXian == false)//如果当前播放的不是休闲待机动画
                    {
                        #region 普通待机
                        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Normal.ToString()))//如果当前动画状态机处于Idle_Normal状态
                        {
                            //防止频繁的进入相同动画状态
                            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Idle_Normal);

                            //只有当此动画状态运行超过转态时长之后 才把动画参数CurrState是否设置标志位设为true
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
                    else//如果当前播放的是休闲待机动画
                    {
                        #region 休闲待机
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
                else////如果当前待机状态为 fight
                {
                    #region 战斗待机
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
                    if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Fight.ToString()))//如果当前动画状态机处于Idle_Normal状态
                    {
                        //防止频繁的进入相同动画状态
                        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Idle_Fight);

                        //只有当此动画状态运行超过0.01秒之后 才把动画参数CurrState是否设置标志位设为true
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

            #region 普通待机与休闲待机切换
            //======== 处理待机和休闲两个动画状态的切换 ========
            if (CurrRoleFSMMgr.CurrIdleState == RoleIdleState.IdleNormal)
            {
                timer += Time.deltaTime;//每帧计时器累加
                if (timer >= m_ChangeStep)
                {
                    timer = 0;//计时器归零 重新开始计时
                    m_IsXiuXian = true;
                    IsChangedState = false;//是否已经给动画参数CurrState设置过值

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
        else//怪物==========================================================================================================
        {
            if (IsChangedState == false)
            {
                CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
                if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Fight.ToString()))
                {
                    //防止频繁的进入相同动画状态 造成鬼畜
                    CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Idle_Fight);

                    //玩家的抽象状态已经改变 但是动画状态可能还处于转态阶段 如果直接设置CurrState的值 就会打断转态条件 玩家进入不了下一个状态
                    //所以设置抽象状态后的一段时间内（动画状态处于转态中） 
                    //转态完成 真正进入下一个状态成功0.1s之后
                    //m_RuningTime += Time.deltaTime;
                    //if (m_RuningTime > 0.1f)
                    //{
                    //    IsChangedState = true;
                    //}

                }
                else
                {
                    //防止怪原地跑
                    CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
                }
            }
        }
        

        
    }

    public override void OnLeave()
    {
        base.OnLeave();

        if (CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer || CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleType == RoleType.OtherPlayer)//主角
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
        else//怪物
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleNormal.ToString(), false);
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToIdleFight.ToString(), false);
        }
        
    }

}
