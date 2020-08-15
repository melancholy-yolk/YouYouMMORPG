using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStateRun : RoleStateAbstract 
{
    /// <summary>
    /// 转身速度
    /// </summary>
    private float m_RotationSpeed = 0.2f;

    /// <summary>
    /// 转身的目标方向
    /// </summary>
    private Quaternion m_TargetQuaternion;
    private float m_MoveSpeed = 0f;
    public RoleStateRun(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr) { }

    /// <summary>
    /// 实现基类 进入状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();

        m_RotationSpeed = 0;
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToRun.ToString(), true);
    }

    /// <summary>
    /// 实现基类 执行状态
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();

        //设置动画状态机参数 防止频繁进入同一个动画状态
        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Run.ToString()))
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Run);
        }
        else
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
        }

        //==== 测试代码 ====
        //if (CurrRoleFSMMgr.CurrRoleCtrl.AStarPath == null)
        //{
        //    return;
        //}
        //==== 测试代码 ====

        //玩家A星路径为空
        if (CurrRoleFSMMgr.CurrRoleCtrl.AStarPath == null)
        {
            CurrRoleFSMMgr.CurrRoleCtrl.ToIdle();
            return;
        }

        //玩家完成A星寻路
        if (CurrRoleFSMMgr.CurrRoleCtrl.AStarCurrentWaypoint >= CurrRoleFSMMgr.CurrRoleCtrl.AStarPath.vectorPath.Count)
        {
            CurrRoleFSMMgr.CurrRoleCtrl.AStarPath = null;

            if (CurrRoleFSMMgr.CurrRoleCtrl.PreviousFightTime != 0)//上次战斗时间不为零
            {
                if (Time.time > CurrRoleFSMMgr.CurrRoleCtrl.PreviousFightTime + 30f)//距离上次战斗时间超过了30s 玩家切换到普通待机状态
                {
                    CurrRoleFSMMgr.CurrRoleCtrl.ToIdle();
                    CurrRoleFSMMgr.CurrRoleCtrl.PreviousFightTime = 0;
                }
                else//距离上次战斗时间没有超过30s 玩家切换到战斗待机状态
                {
                    CurrRoleFSMMgr.CurrRoleCtrl.ToIdle(RoleIdleState.IdleFight);
                }
            }
            else//上次战斗时间为0 说明最近没有进行过战斗
            {
                CurrRoleFSMMgr.CurrRoleCtrl.ToIdle();
            }
            

            return;
        }

        //移动方向
        Vector3 moveDirection = Vector3.zero;
        //要到达的目标点位置
        Vector3 tempvv = new Vector3(CurrRoleFSMMgr.CurrRoleCtrl.AStarPath.vectorPath[CurrRoleFSMMgr.CurrRoleCtrl.AStarCurrentWaypoint].x,
                                     CurrRoleFSMMgr.CurrRoleCtrl.transform.position.y,
                                     CurrRoleFSMMgr.CurrRoleCtrl.AStarPath.vectorPath[CurrRoleFSMMgr.CurrRoleCtrl.AStarCurrentWaypoint].z);

        //计算方向
        moveDirection = (tempvv - CurrRoleFSMMgr.CurrRoleCtrl.transform.position).normalized;//归一化
        m_MoveSpeed = CurrRoleFSMMgr.CurrRoleCtrl.ModifySpeed > 0 ? CurrRoleFSMMgr.CurrRoleCtrl.ModifySpeed : CurrRoleFSMMgr.CurrRoleCtrl.Speed;
        moveDirection = moveDirection * (Time.deltaTime * m_MoveSpeed);//移动速度
        moveDirection.y = 0;

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            //缓慢转身
            if (moveDirection != Vector3.zero)
            {
                CurrRoleFSMMgr.CurrRoleCtrl.roleSlerp += 3f;
                var rotation = Quaternion.LookRotation(moveDirection);
                CurrRoleFSMMgr.CurrRoleCtrl.transform.rotation = Quaternion.Slerp(CurrRoleFSMMgr.CurrRoleCtrl.transform.rotation, rotation, Time.deltaTime * CurrRoleFSMMgr.CurrRoleCtrl.roleSlerp);

                if (Quaternion.Angle(CurrRoleFSMMgr.CurrRoleCtrl.transform.rotation, rotation) < 1f)
                {
                    CurrRoleFSMMgr.CurrRoleCtrl.roleSlerp = 0f;
                }
            }
        }

        float distxx1 = Vector3.Distance(CurrRoleFSMMgr.CurrRoleCtrl.transform.position, tempvv);
        moveDirection.y = 0;
        if (distxx1 <= moveDirection.magnitude + 0.1f)
        {
            CurrRoleFSMMgr.CurrRoleCtrl.AStarCurrentWaypoint++;
        }
        //考虑重力加速度，让角色紧紧贴在地面上
        moveDirection.y = -100 * Time.deltaTime * 5;

        CurrRoleFSMMgr.CurrRoleCtrl.m_CharacterController.Move(moveDirection);//移动角色
    }

    /// <summary>
    /// 实现基类 离开状态
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToRun.ToString(), false);
    }
}
