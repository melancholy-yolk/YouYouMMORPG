using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleMainPlayerCityAI : IRoleAI 
{
    public RoleCtrl CurrRole
    {
        get;
        set;
    }

    public RoleMainPlayerCityAI(RoleCtrl roleCtrl)
    {
        CurrRole = roleCtrl;
    }

    private int m_PhyIndex = 0;//����������
    private List<Collider> m_SearchList = new List<Collider>();
    private float m_NextAttackTime = 0f;
    private Vector3 targetPoint;
    private Vector3 rayPoint;

    public void DoAI()
    {
        //ִ��AI
        if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Die)
        {
            return;
        }

        if (CurrRole.Attack.IsAutoFight)
        {
            AutoFightState();
        }
        else
        {
            NormalState();
        }
    }

    private void AutoFightState()
    {
        //if (GameLevelSceneCtrl.Instance.CurrentRegionHasMonster == false)//�����ǰ�����Ѿ�û�й���
        //{
        //    //�����ǰ�����һ������ ֱ�ӷ���
        //    if (GameLevelSceneCtrl.Instance.CurrentRegionIsLast)
        //    {
        //        return;
        //    }
        //    else//���� ������һ������
        //    {
        //        //�������ƶ�����һ������ĳ�����
        //        CurrRole.MoveTo(GameLevelSceneCtrl.Instance.NextRegionPlayerBornPos);
        //    }
        //}
        //else
        {
            if (CurrRole.LockEnemy == null)//��ǰû����������
            {
                #region �����Լ�����Ĺ� ��Ϊ������
                //�����ҵ���Ұ��Χ �ҵ�����Ĺ� ��Ϊ�����ĵ���
                m_SearchList.Clear();
                m_SearchList = GameUtil.FindEnemy(CurrRole, 1000f);//�ҵ���
                m_SearchList = GameUtil.SortEnemyByDistance(m_SearchList, CurrRole);//����������
                if (m_SearchList.Count > 0)
                {
                    CurrRole.LockEnemy = m_SearchList[0].GetComponent<RoleCtrl>();//��������ĵ���
                }
                #endregion
            }
            else//��ǰ����������
            {
                if (CurrRole.LockEnemy.CurrRoleInfo.CurrHP <= 0)
                {
                    CurrRole.LockEnemy = null;
                    return;
                }

                #region ����Լ��ļ���ʹ����� ����ʹ�ü��ܻ���������
                //���ȼ����û�п�ʹ�õļ��� �õ���Ӧ���ܵĹ�����Χ
                int skillId = CurrRole.CurrRoleInfo.GetCanUseSkillId();//�ӽ�ɫ��Ϣ���ȡ��ǰ����ʹ�õļ���
                RoleAttackType attackType;
                if (skillId > 0)
                {
                    //ʹ�ü��ܹ���
                    attackType = RoleAttackType.SkillAttack;
                }
                else
                {
                    //ʹ��������
                    skillId = CurrRole.CurrRoleInfo.PhySkillIds[m_PhyIndex];
                    m_PhyIndex++;
                    if (m_PhyIndex >= CurrRole.CurrRoleInfo.PhySkillIds.Length)
                    {
                        m_PhyIndex = 0;
                    }
                    attackType = RoleAttackType.PhyAttack;
                }
                #endregion

                SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(skillId);
                if (skillEntity == null) return;

                //�жϵ����Ƿ��ڹ�����Χ��
                if (Vector3.Distance(CurrRole.transform.position, CurrRole.LockEnemy.transform.position) <= skillEntity.AttackRange)//�ڹ�����Χ��
                {
                    //���𹥻�
                    //CurrRole.transform.LookAt(new Vector3(CurrRole.LockEnemy.transform.position.x, CurrRole.transform.position.y, CurrRole.LockEnemy.transform.position.z));

                    //if (Time.time > m_NextAttackTime && CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum != RoleState.Attack)
                    //{
                    //    m_NextAttackTime = Time.time + 1f;
                    //    if (attackType == RoleAttackType.SkillAttack)
                    //    {
                    //        PlayerController.Instance.OnSkillClick(skillId);//ģ��������ͼ��
                    //    }
                    //    else
                    //    {
                    //        CurrRole.ToAttackBySkillId(attackType, skillId);
                    //    }
                        
                    //}
                    if (attackType == RoleAttackType.SkillAttack)
                    {
                        PlayerController.Instance.OnSkillClick(skillId);//ģ��������ͼ�� ��ʼ������ȴ
                    }
                    else
                    {
                        CurrRole.ToAttackBySkillId(attackType, skillId);
                    }
                }
                else//���ڹ�����Χ��
                {
                    //����׷��
                    if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Idle)
                    {
                        targetPoint = GameUtil.GetRandomPos(CurrRole.transform.position, CurrRole.LockEnemy.transform.position, skillEntity.AttackRange);
                        rayPoint = new Vector3(targetPoint.x, targetPoint.y + 50, targetPoint.z);
                        if (Physics.Raycast(rayPoint, Vector3.down, 1000f, 1 << LayerMask.NameToLayer("RegionMask")))
                        {
                            return;
                        }
                        CurrRole.MoveTo(targetPoint);
                    }
                }
            }
        }

    }

    /// <summary>
    /// ���Զ�ս��ʱ �����ǰ�������˲�Ϊ�� �Զ����������˷���������
    /// </summary>
    private void NormalState()
    {
        //������ϴ�ս��ʱ�䳬��30s �л�Ϊ��ͨ����
        if (CurrRole.PreviousFightTime != 0)
        {
            if (Time.time > CurrRole.PreviousFightTime + 30f)
            {
                CurrRole.ToIdle();
                CurrRole.PreviousFightTime = 0;
            }
        }

        //1.ÿ֡�����ж� ��������������� ���й���
        if (CurrRole.LockEnemy != null)
        {
            if (CurrRole.LockEnemy.CurrRoleInfo.CurrHP <= 0)
            {
                CurrRole.LockEnemy = null;
                return;
            }

            SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(CurrRole.CurrRoleInfo.PhySkillIds[m_PhyIndex]);
            if (skillEntity == null) return;

            //�жϵ����Ƿ��ڹ�����Χ��
            if (Vector3.Distance(CurrRole.transform.position, CurrRole.LockEnemy.transform.position) <= 3)//�ڹ�����Χ��
            {
                if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Idle)
                {
                    if (CurrRole.Attack.FollowSkillId > 0)
                    {
                        PlayerController.Instance.OnSkillClick(CurrRole.Attack.FollowSkillId);//ģ��������
                    }
                    else
                    {
                        //������Id������ ѭ��ʹ��
                        int skillId = CurrRole.CurrRoleInfo.PhySkillIds[m_PhyIndex];
                        CurrRole.ToAttackBySkillId(RoleAttackType.PhyAttack, skillId);
                        m_PhyIndex++;

                        if (m_PhyIndex >= CurrRole.CurrRoleInfo.PhySkillIds.Length)
                        {
                            m_PhyIndex = 0;
                        }
                    }
                }
            }
            else//���ڹ�����Χ��
            {
                //����׷��
                if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Idle)
                {
                    targetPoint = GameUtil.GetRandomPos(CurrRole.transform.position, CurrRole.LockEnemy.transform.position, skillEntity.AttackRange);
                    rayPoint = new Vector3(targetPoint.x, targetPoint.y + 50, targetPoint.z);
                    if (Physics.Raycast(rayPoint, Vector3.down, 1000f, 1 << LayerMask.NameToLayer("RegionMask")))
                    {
                        return;
                    }
                    CurrRole.MoveTo(targetPoint);
                }
            }

        }
    }

}
