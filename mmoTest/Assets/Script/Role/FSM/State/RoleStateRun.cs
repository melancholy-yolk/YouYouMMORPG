using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStateRun : RoleStateAbstract 
{
    /// <summary>
    /// ת���ٶ�
    /// </summary>
    private float m_RotationSpeed = 0.2f;

    /// <summary>
    /// ת���Ŀ�귽��
    /// </summary>
    private Quaternion m_TargetQuaternion;
    private float m_MoveSpeed = 0f;
    public RoleStateRun(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr) { }

    /// <summary>
    /// ʵ�ֻ��� ����״̬
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();

        m_RotationSpeed = 0;
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToRun.ToString(), true);
    }

    /// <summary>
    /// ʵ�ֻ��� ִ��״̬
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();

        //���ö���״̬������ ��ֹƵ������ͬһ������״̬
        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Run.ToString()))
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), (int)RoleAnimatorState.Run);
        }
        else
        {
            CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetInteger(AnimatorParameter.CurrState.ToString(), 0);
        }

        //==== ���Դ��� ====
        //if (CurrRoleFSMMgr.CurrRoleCtrl.AStarPath == null)
        //{
        //    return;
        //}
        //==== ���Դ��� ====

        //���A��·��Ϊ��
        if (CurrRoleFSMMgr.CurrRoleCtrl.AStarPath == null)
        {
            CurrRoleFSMMgr.CurrRoleCtrl.ToIdle();
            return;
        }

        //������A��Ѱ·
        if (CurrRoleFSMMgr.CurrRoleCtrl.AStarCurrentWaypoint >= CurrRoleFSMMgr.CurrRoleCtrl.AStarPath.vectorPath.Count)
        {
            CurrRoleFSMMgr.CurrRoleCtrl.AStarPath = null;

            if (CurrRoleFSMMgr.CurrRoleCtrl.PreviousFightTime != 0)//�ϴ�ս��ʱ�䲻Ϊ��
            {
                if (Time.time > CurrRoleFSMMgr.CurrRoleCtrl.PreviousFightTime + 30f)//�����ϴ�ս��ʱ�䳬����30s ����л�����ͨ����״̬
                {
                    CurrRoleFSMMgr.CurrRoleCtrl.ToIdle();
                    CurrRoleFSMMgr.CurrRoleCtrl.PreviousFightTime = 0;
                }
                else//�����ϴ�ս��ʱ��û�г���30s ����л���ս������״̬
                {
                    CurrRoleFSMMgr.CurrRoleCtrl.ToIdle(RoleIdleState.IdleFight);
                }
            }
            else//�ϴ�ս��ʱ��Ϊ0 ˵�����û�н��й�ս��
            {
                CurrRoleFSMMgr.CurrRoleCtrl.ToIdle();
            }
            

            return;
        }

        //�ƶ�����
        Vector3 moveDirection = Vector3.zero;
        //Ҫ�����Ŀ���λ��
        Vector3 tempvv = new Vector3(CurrRoleFSMMgr.CurrRoleCtrl.AStarPath.vectorPath[CurrRoleFSMMgr.CurrRoleCtrl.AStarCurrentWaypoint].x,
                                     CurrRoleFSMMgr.CurrRoleCtrl.transform.position.y,
                                     CurrRoleFSMMgr.CurrRoleCtrl.AStarPath.vectorPath[CurrRoleFSMMgr.CurrRoleCtrl.AStarCurrentWaypoint].z);

        //���㷽��
        moveDirection = (tempvv - CurrRoleFSMMgr.CurrRoleCtrl.transform.position).normalized;//��һ��
        m_MoveSpeed = CurrRoleFSMMgr.CurrRoleCtrl.ModifySpeed > 0 ? CurrRoleFSMMgr.CurrRoleCtrl.ModifySpeed : CurrRoleFSMMgr.CurrRoleCtrl.Speed;
        moveDirection = moveDirection * (Time.deltaTime * m_MoveSpeed);//�ƶ��ٶ�
        moveDirection.y = 0;

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            //����ת��
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
        //�����������ٶȣ��ý�ɫ�������ڵ�����
        moveDirection.y = -100 * Time.deltaTime * 5;

        CurrRoleFSMMgr.CurrRoleCtrl.m_CharacterController.Move(moveDirection);//�ƶ���ɫ
    }

    /// <summary>
    /// ʵ�ֻ��� �뿪״̬
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        CurrRoleFSMMgr.CurrRoleCtrl.m_Animator.SetBool(AnimatorParameter.ToRun.ToString(), false);
    }
}
