using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel_RoleMonsterAI : IRoleAI 
{
    public RoleCtrl CurrRole
    {
        get;
        set;
    }

    private float m_NextPatrolTime = 0f;
    private float m_NextAttackTime = 0f;
    private RoleInfoMonster roleInfo;
    private RoleAttackType usedAttackType;
    private int usedSkillId = 0;//��ʹ�õļ���Id

    private Vector3 MoveToPoint;//Ҫ�ƶ�����Ŀ���
    private Vector3 RayPoint;//Ѱ·Ŀ�����Ч�Լ���������

    private float m_NextThinkTime = 0f;
    private bool m_IsDaze;//�Ƿ񷢴���

    public GameLevel_RoleMonsterAI(RoleCtrl roleCtrl, RoleInfoMonster info)
    {
        CurrRole = roleCtrl;
        roleInfo = info;
    }

    public void DoAI()
    {
        //�����ǰ��Ҳ����� ֱ�ӷ���
        if (GlobalInit.Instance == null || GlobalInit.Instance.MainPlayer == null) return;

        //��������Ѿ����� ֱ�ӷ���
        if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Die || CurrRole.IsRigidity) return;

        if(CurrRole.LockEnemy == null)
        {
            #region û����������
            if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Idle)
            {
                
                //��ʱѲ��
                if (Time.time > m_NextPatrolTime)
                {
                    m_NextPatrolTime = Time.time + 6f;
                    MoveToPoint = new Vector3(CurrRole.BornPoint.x + Random.Range(CurrRole.PatrolRange * -1, CurrRole.PatrolRange), 
                                                    CurrRole.BornPoint.y, 
                                                    CurrRole.BornPoint.z + Random.Range(CurrRole.PatrolRange * -1, CurrRole.PatrolRange));

                    //ҪѰ·ȥ����Ŀ���ֻ���ڵ�ǰ����
                    if (SceneMgr.Instance.CurrentSceneType == SceneType.GameLevel)
                    {
                        RayPoint = new Vector3(MoveToPoint.x, MoveToPoint.y + 50, MoveToPoint.z);
                        if (Physics.Raycast(RayPoint, Vector3.down, 100, 1 << LayerMask.NameToLayer("RegionMask")))
                        {
                            Debug.Log("hit invalid region!!!");
                            return;
                        }
                    }

                    CurrRole.MoveTo(MoveToPoint);
                }

                //�����ڹֵ���Ұ��Χ��
                if (Vector3.Distance(CurrRole.transform.position, GlobalInit.Instance.MainPlayer.transform.position) <= CurrRole.ViewRange)
                {
                    CurrRole.LockEnemy = GlobalInit.Instance.MainPlayer;//����������
                    m_NextAttackTime = Time.time + roleInfo.spriteEntity.DelaySec_Attack;
                }
            }
            #endregion
        }
        else
        {
            #region ����������
            //���������Ѿ����� ����Ŀ������Ϊnull Ȼ�󷵻�
            if (CurrRole.LockEnemy.CurrRoleInfo.CurrHP <= 0)
            {
                CurrRole.LockEnemy = null;
                return;
            }

            if (Time.time > m_NextThinkTime + UnityEngine.Random.Range(3, 3.5f))
            {
                //�ý�ɫ��Ϣ
                CurrRole.ToIdle(RoleIdleState.IdleFight);
                m_NextThinkTime = Time.time;
                m_IsDaze = true;
            }

            //��ɫ��Ϣ�� ֱ�ӷ���
            if (m_IsDaze)
            {
                if (Time.time > m_NextThinkTime + UnityEngine.Random.Range(1, 1.5f))
                {
                    m_IsDaze = false;
                }
                else
                {
                    return;
                }
            }

            //ֻ�д���״̬�Ž���˼��
            if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum != RoleState.Idle) return;

            //�����������˾��볬���ҵ���Ұ��Χ ��ȡ������
            //�����ܵÿ� �����˹ֵ���Ұ��Χ ��ȡ��׷��
            if (Vector3.Distance(CurrRole.transform.position, CurrRole.LockEnemy.transform.position) > CurrRole.ViewRange)
            {
                CurrRole.LockEnemy = null;
                return;
            }

            #region ���һ������Id
            //����������ĵ���
            //1.�õ���Ҫʹ�õļ���Id��������������
            //����������
            int random = Random.Range(0, 100);
            if (roleInfo.spriteEntity.PhysicalAttackRate >= random)
            {
                //˵��Ҫʹ��������
                usedAttackType = RoleAttackType.PhyAttack;
                usedSkillId = roleInfo.spriteEntity.UsedPhyAttackArr[Random.Range(0, roleInfo.spriteEntity.UsedPhyAttackArr.Length)];
            }
            else
            {
                //ʹ�ü��ܹ���
                usedAttackType = RoleAttackType.SkillAttack;
                usedSkillId = roleInfo.spriteEntity.UsedSkillAttackArr[Random.Range(0, roleInfo.spriteEntity.UsedSkillAttackArr.Length)];
            }
            #endregion


            //�õ��ü�����Ϣ
            SkillEntity entity = SkillDBModel.Instance.GetEntity(usedSkillId);
            if (entity == null) return;

            //2.�жϵ����Ƿ��ڸü��ܵĹ�����Χ�� 
            if (Vector3.Distance(CurrRole.transform.position, CurrRole.LockEnemy.transform.position) <= entity.AttackRange)
            {
                CurrRole.transform.LookAt(CurrRole.LockEnemy.transform);

                //�ڹ�����Χ֮�� ֱ�ӷ��𹥻�
                //�����ǰʱ�̴����´ι���ʱ�� ���ҵ�ǰ��ɫ�����ڹ���״̬
                if (Time.time > m_NextAttackTime && CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum != RoleState.Attack)
                {
                    m_NextAttackTime = Time.time + Random.Range(0f, 1f) + roleInfo.spriteEntity.Attack_Interval;//���ù������
                    CurrRole.ToAttackBySkillId(usedAttackType, usedSkillId);
                }
            }
            else
            {
                
                //�ڹ�����Χ֮�� ����׷��
                if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Idle)
                {
                    MoveToPoint = GameUtil.GetRandomPos(CurrRole.transform.position, CurrRole.LockEnemy.transform.position, entity.AttackRange);

                    //��Ϸ�ؿ��� ֻ�е����ǰ�������Ч
                    if (SceneMgr.Instance.CurrentSceneType == SceneType.GameLevel)
                    {
                        RayPoint = new Vector3(MoveToPoint.x, MoveToPoint.y + 50, MoveToPoint.z);
                        if (Physics.Raycast(RayPoint, Vector3.down, 100, 1 << LayerMask.NameToLayer("RegionMask"))) return;
                    }

                    CurrRole.MoveTo(MoveToPoint);
                }
            }
            #endregion
        }

    }
}
