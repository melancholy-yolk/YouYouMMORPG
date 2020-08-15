using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoleAttack
{
#region ����
    [Header("==== Physics Attack Info ====")]
    public List<RoleAttackInfo> PhyAttackInfoList;//��������Ϣ�б�

    [Header("==== Skill Attack Info ====")]
    public List<RoleAttackInfo> SkillAttackInfoList;//���ܹ�����Ϣ�б�

    private RoleFSMMgr m_RoleFSMMgr = null;
    private RoleCtrl m_CurrRoleCtrl = null;

    private List<RoleCtrl> m_EnemyList = null;//����Ҫ���ܹ����ĵ����б�
    private List<Collider> m_SearchList = null;//��ײ��⵽�ļ��ܷ�Χ�ڵĵ���

    /*
     * �����������ʱ��Ĭ��һֱ���������� ��ʱ�������ͼ�� ���ڴ��ڹ����еĽ�ֱ״̬ �����޷��ͷ� �������ǽ�����ļ��ܼ�¼���� �´ν���AI����ʱ������ж�
     * ���������Ҫ�ͷŵĺ������� �ͽ��м��ܹ���
     */
    private int m_FollowSkillId;//�������ܱ��:���������Զ�����������ʱ���µļ��ܻ��Ϊ��������
    [HideInInspector]
    public int FollowSkillId { get { return m_FollowSkillId; } }
    [HideInInspector]
    public bool IsAutoFight;//����Ƿ����Զ�ս��

    public string EffectPath;//��ɫ������Ч��·��
#endregion


    public void SetFSM(RoleFSMMgr fsm)//���л�����û�й��캯�� ʹ�ô˺������г�ʼ��
    {
        m_RoleFSMMgr = fsm;
        m_CurrRoleCtrl = m_RoleFSMMgr.CurrRoleCtrl;
        m_EnemyList = new List<RoleCtrl>();
        m_SearchList = new List<Collider>();
    }

    

    private RoleStateAttack m_RoleStateAttack;//��ɫ����״̬��



    public void ToAttackByIndex(RoleAttackType type, int index)
    {
#if DEBUG_ROLE_STATE
        //��� ����״̬�� �� ������ڹ�����
        if (m_RoleFSMMgr == null || m_RoleFSMMgr.CurrRoleCtrl.IsRigidity == true) return;

        RoleAttackInfo info = GetRoleAttackInfoByIndex(type, index);

        if (info != null)
        {
            m_RoleFSMMgr.CurrRoleCtrl.CurrAttackInfo = info;

            GameObject obj = Object.Instantiate<GameObject>(info.EffectObject);
            obj.transform.position = m_RoleFSMMgr.CurrRoleCtrl.transform.position;
            obj.transform.rotation = m_RoleFSMMgr.CurrRoleCtrl.transform.rotation;
            Object.Destroy(obj, info.EffectLifeTime);
        }

        //����
        if (CameraCtrl.Instance != null)
        {
            CameraCtrl.Instance.CameraShake(info.CameraShakeDelay, info.CameraShakeDuration, info.CameraShakeStrength, info.CameraShakeVibrto);
        }

        if (m_RoleStateAttack == null)
        {
            //��ȡ����״̬
            //ΪʲôҪ��ȡ���״̬����ΪҪ�޸Ĺ���״̬���еĲ���
            m_RoleStateAttack = m_RoleFSMMgr.GetRoleState(RoleState.Attack) as RoleStateAttack;
        }

        m_RoleStateAttack.AnimatorCondition = type == RoleAttackType.PhyAttack ? "ToPhyAttack" : "ToSkill";
        m_RoleStateAttack.AnimatorConditionValue = index;
        m_RoleStateAttack.CurrAnimatorState = GameUtil.GetRoleAnimatorState(type, index);

        //�л��ɹ���״̬
        m_RoleFSMMgr.ChangeState(RoleState.Attack);
#endif
    }//���Թ���ʹ��

    public bool ToAttack(RoleAttackType type, int skillId)
    {
        //��� ����״̬����Ϊ�� �� ������ڹ�����(��ֱ״̬)
        if (m_RoleFSMMgr == null || m_RoleFSMMgr.CurrRoleCtrl.IsRigidity == true) 
        {
            if (type == RoleAttackType.SkillAttack)
            {
                m_FollowSkillId = skillId;//���ú�������
            }
            return false;
        }

        m_FollowSkillId = -1;

        #region ��ֵ���
        //1.ֻ�����Ǻ͹���Ų�����ֵ����(PVE�˺��ж��ڿͻ��˽�������)
        if (m_CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer || m_CurrRoleCtrl.CurrRoleType == RoleType.Monster)
        {
            //2.��ȡ������Ϣ
            SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(skillId);
            if (skillEntity == null) return false;

            int skillLevel = m_CurrRoleCtrl.CurrRoleInfo.GetSkillLevelBySkillId(skillId);//ͨ�����ܱ�� �ӽ�ɫ�Ѿ�ѧ��ļ����б��в�ѯ���ܵȼ�
            SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndSkillLevel(skillId, skillLevel);

            m_EnemyList.Clear();

            //��������� ���ҵ���
            if (m_CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer)
            {
                #region �ҵ���
                //4.Ѱ�ҵ���
                int attackTargetCount = skillEntity.AttackTargetCount;

                if (attackTargetCount == 1)
                {
                    #region ���幥��
                    //���幥��
                    if (m_CurrRoleCtrl.LockEnemy != null)
                    {
                        //�����ǰ�������ĵ��� ���뵽�����б���
                        m_EnemyList.Add(m_CurrRoleCtrl.LockEnemy);
                    }
                    else
                    {
                        //���μ��
                        m_SearchList = FindEnemy(m_CurrRoleCtrl.transform.position, skillEntity.AttackRange, m_CurrRoleCtrl);
                        if (m_SearchList.Count == 0)
                        {
                            Debug.Log("no enemy");
                            return false;//����û�е��� ֱ�ӷ���
                        }

                        //���ݾ�������
                        m_SearchList = SortEnemyByDistance(m_SearchList, m_CurrRoleCtrl);

                        m_CurrRoleCtrl.LockEnemy = m_SearchList[0].gameObject.GetComponent<RoleCtrl>();
                        m_EnemyList.Add(m_CurrRoleCtrl.LockEnemy);
                    }
                    #endregion
                }
                else
                {
                    #region Ⱥ��
                    int needAttack = attackTargetCount;//��Ҫ����������

                    //���μ��
                    m_SearchList = FindEnemy(m_CurrRoleCtrl.transform.position, skillEntity.AttackRange, m_CurrRoleCtrl);
                    if (m_SearchList.Count == 0)
                    {
                        Debug.Log("no enemy");
                        return false;//����û�е��� ֱ�ӷ���
                    }

                    //���ݾ�������
                    m_SearchList = SortEnemyByDistance(m_SearchList, m_CurrRoleCtrl);

                    #region �������б�
                    if (m_CurrRoleCtrl.LockEnemy != null)
                    {
                        //�����ǰ�������ĵ��� ���뵽�����б���
                        m_EnemyList.Add(m_CurrRoleCtrl.LockEnemy);
                        needAttack--;//��Ҫ����������-1

                        //����������Ҫ�˺��ĵ���
                        for (int i = 0; i < m_SearchList.Count; i++)
                        {
                            RoleCtrl ctrl = m_SearchList[i].gameObject.GetComponent<RoleCtrl>();
                            if (ctrl.CurrRoleInfo.RoleId != m_CurrRoleCtrl.LockEnemy.CurrRoleInfo.RoleId && ctrl.CurrRoleInfo.RoleId != m_CurrRoleCtrl.CurrRoleInfo.RoleId)//���⵱ǰ���������ظ�����
                            {
                                if ((i + 1) > needAttack) break;
                                m_EnemyList.Add(ctrl);
                            }
                        }
                    }
                    else
                    {
                        m_CurrRoleCtrl.LockEnemy = m_SearchList[0].GetComponent<RoleCtrl>();
                        //����������Ҫ�˺��ĵ���
                        for (int i = 0; i < m_SearchList.Count; i++)
                        {
                            //����Լ����ܹ����Լ�
                            RoleCtrl ctrl = m_SearchList[i].gameObject.GetComponent<RoleCtrl>();
                            if (ctrl.CurrRoleInfo.RoleId != m_CurrRoleCtrl.CurrRoleInfo.RoleId)
                            {
                                if ((i + 1) > needAttack) break;
                                m_EnemyList.Add(ctrl);
                            }
                        }

                    }
                    #endregion

                    #endregion
                }
                #endregion
            }
            else if (m_CurrRoleCtrl.CurrRoleType == RoleType.Monster)
            {
                if (m_CurrRoleCtrl.LockEnemy != null)
                {
                    m_EnemyList.Add(m_CurrRoleCtrl.LockEnemy);   
                }
                
            }

            //�õ��˵����б�m_EnemyList

            //========== PVE��PVP������ ==========
            //1.PVE��ֱ���õ�������
            //2.PVP�Ƿ�����Ϣ��������

            if (SceneMgr.Instance.CurrPlayType == PlayType.PVE)
            {
                //1.PVE��ֱ���õ�������
                for (int i = 0; i < m_EnemyList.Count; i++)
                {
                    RoleTransferAttackInfo roleTransferAttackInfo = CalculateHurtValue(m_EnemyList[i], skillLevelEntity);
                    m_EnemyList[i].ToHurt(roleTransferAttackInfo);
                }
            }
            else if (SceneMgr.Instance.CurrPlayType == PlayType.PVP)
            {
                //2.PVP�Ƿ�����Ϣ��������
                WorldMap_CurrRoleUseSkillProto proto = new WorldMap_CurrRoleUseSkillProto();

                proto.SkillId = skillId;
                proto.SkillLevel = skillLevel;
                proto.RolePosX = m_CurrRoleCtrl.transform.position.x;
                proto.RolePosY = m_CurrRoleCtrl.transform.position.y;
                proto.RolePosZ = m_CurrRoleCtrl.transform.position.z;
                proto.RoleYAngle = m_CurrRoleCtrl.transform.eulerAngles.y;

                proto.BeAttackCount = m_EnemyList.Count;
                proto.ItemList = new List<WorldMap_CurrRoleUseSkillProto.BeAttackItem>();

                for (int i = 0; i < m_EnemyList.Count; i++)
                {
                    proto.ItemList.Add(new WorldMap_CurrRoleUseSkillProto.BeAttackItem() {BeAttackRoleId = m_EnemyList[i].CurrRoleInfo.RoleId});
                }

                NetWorkSocket.Instance.SendMsg(proto.ToArray());
            }

            //5.�õ�������
            

            m_CurrRoleCtrl.CurrRoleInfo.CurrMP -= skillLevelEntity.SpendMP;
            if (m_CurrRoleCtrl.CurrRoleInfo.CurrMP <= 0)
            {
                m_CurrRoleCtrl.CurrRoleInfo.CurrMP = 0;
            }
            if (m_CurrRoleCtrl.OnMPChange != null)
            {
                m_CurrRoleCtrl.OnMPChange(-1);
            }
        }
        #endregion

        //�����PVE���Ŷ��� �����PVPֻ�Ƿ�����Ϣ�������� �ȷ�����֪ͨ�ͻ���ʹ�ü��ܺ��ڲ��Ŷ���
        if (SceneMgr.Instance.CurrPlayType == PlayType.PVE)
        {
            PlayAttack(skillId);
        }

        return true;
    }

    public void PlayAttack(int skillId)
    {
        RoleAttackType type = SkillDBModel.Instance.GetEntity(skillId).IsPhyAttack == 1 ? RoleAttackType.PhyAttack : RoleAttackType.SkillAttack;

        RoleAttackInfo attackInfo = GetRoleAttackInfo(type, skillId);//��ȡ���������Ϣ

        //��AssetBundle�� ������Ч
        if (attackInfo == null) return;

        m_RoleFSMMgr.CurrRoleCtrl.CurrAttackInfo = attackInfo;

        if (!string.IsNullOrEmpty(attackInfo.EffectName))
        {
            Transform obj = EffectMgr.Instance.PlayEffect(EffectPath, attackInfo.EffectName);//��Ч��ȡ����Ч
            obj.position = m_RoleFSMMgr.CurrRoleCtrl.transform.position;
            obj.rotation = m_RoleFSMMgr.CurrRoleCtrl.transform.rotation;
            EffectMgr.Instance.DestroyEffect(obj, attackInfo.EffectLifeTime);//��Ч�ػ�����Ч
        }

        if (CameraCtrl.Instance != null)
        {
            CameraCtrl.Instance.CameraShake(attackInfo.CameraShakeDelay, attackInfo.CameraShakeDuration, attackInfo.CameraShakeStrength, attackInfo.CameraShakeVibrto);
        }

        if (m_RoleStateAttack == null)
        {
            //��ȡ����״̬
            //ΪʲôҪ��ȡ���״̬����ΪҪ�޸Ĺ���״̬���еĲ���
            m_RoleStateAttack = m_RoleFSMMgr.GetRoleState(RoleState.Attack) as RoleStateAttack;
        }

        m_RoleStateAttack.AnimatorCondition = type == RoleAttackType.PhyAttack ? "ToPhyAttack" : "ToSkill";
        m_RoleStateAttack.AnimatorConditionValue = attackInfo.Index;
        m_RoleStateAttack.CurrAnimatorState = GameUtil.GetRoleAnimatorState(type, attackInfo.Index);

        //�����л��ɹ���״̬
        m_RoleFSMMgr.ChangeState(RoleState.Attack);
    }

    /// <summary>
    /// ͨ����ʽ���������˺���Ϣ
    /// </summary>
    private RoleTransferAttackInfo CalculateHurtValue(RoleCtrl enemy, SkillLevelEntity skillLevelEntity)
    {
        if (enemy == null || skillLevelEntity == null) return null;

        SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(skillLevelEntity.SkillId);
        if (skillEntity == null) return null;

        RoleTransferAttackInfo roleTransferAttackInfo = new RoleTransferAttackInfo();

        roleTransferAttackInfo.AttackRoleId = m_CurrRoleCtrl.CurrRoleInfo.RoleId;//���𹥻���
        roleTransferAttackInfo.BeAttackedRoleId = enemy.CurrRoleInfo.RoleId;//��������
        roleTransferAttackInfo.SkillId = skillEntity.Id;
        roleTransferAttackInfo.SkillLevel = skillLevelEntity.Level;
        roleTransferAttackInfo.AttackRolePos = m_CurrRoleCtrl.transform.position;//�����ߵ�λ��
        roleTransferAttackInfo.IsAbnormal = (skillEntity.AbnormalState == 1);//�Ƿ񸽼��쳣״̬

        //�����˺�
        

        //1.������ֵ = �����ۺ�ս���� * �������˺��� * 0.01f��
        float attackValue = m_CurrRoleCtrl.CurrRoleInfo.Fighting * (skillLevelEntity.HurtValueRate * 0.01f);

        //2.�����˺�  = ������ֵ * ������ֵ / ��������ֵ + ���������ķ�����
        float baseHurt = attackValue * attackValue / (attackValue + enemy.CurrRoleInfo.Defense);

        //3.�������� = 0.05f + ���������� / ���������� + ���������ԣ�* 0.1f��
        float cri = 0.05f + (m_CurrRoleCtrl.CurrRoleInfo.Cri / (m_CurrRoleCtrl.CurrRoleInfo.Cri + enemy.CurrRoleInfo.Res) * 0.1f);

        //�������� = �������� > 0.5f ? 0.5f : ��������
        cri = cri > 0.5f ? 0.5f : cri;

        //4.�Ƿ񱩻� = 0-1����� <= ��������
        bool isCri = Random.Range(0f, 1f) <= cri;

        //5.�����˺����� = �б��� ? 1.5f : 1f
        float criHurt = isCri ? 1.5f : 1f;

        //6.����� = 0.9f-1.1f֮��
        float random = Random.Range(0.9f, 1.1f);

        //7.�����˺� = �����˺� * �����˺����� * �����
        int hurtValue = Mathf.RoundToInt(baseHurt * criHurt * random);
        hurtValue = hurtValue < 1 ? 1 : hurtValue;

        roleTransferAttackInfo.HurtValue = hurtValue;
        roleTransferAttackInfo.IsCri = isCri;

        return roleTransferAttackInfo;
    }

    /// <summary>
    /// ͨ���������� �� �������� �ҵ���Ӧ�Ĺ�����Ϣ
    /// </summary>
    private RoleAttackInfo GetRoleAttackInfoByIndex(RoleAttackType type, int index)
    {
        if (type == RoleAttackType.PhyAttack)
        {
            for (int i = 0; i < PhyAttackInfoList.Count; i++)
            {
                if (PhyAttackInfoList[i].Index == index)
                {
                    return PhyAttackInfoList[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < SkillAttackInfoList.Count; i++)
            {
                if (SkillAttackInfoList[i].Index == index)
                {
                    return SkillAttackInfoList[i];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// ͨ���������� �� ����Id �ҵ���Ӧ�Ĺ�����Ϣ
    /// </summary>
    private RoleAttackInfo GetRoleAttackInfo(RoleAttackType type, int skillId)
    {
        if (type == RoleAttackType.PhyAttack)//������
        {
            for (int i = 0; i < PhyAttackInfoList.Count; i++)
            {
                if (PhyAttackInfoList[i].SkillId == skillId)
                {
                    return PhyAttackInfoList[i];
                }
            }
        }
        else//���ܹ���
        {
            for (int i = 0; i < SkillAttackInfoList.Count; i++)
            {
                if (SkillAttackInfoList[i].SkillId == skillId)
                {
                    return SkillAttackInfoList[i];
                }
            }
        }

        return null;
    }

    #region ���μ�� Ѱ�ҵ���
    /// <summary>
    /// ���ҽ�ɫ�̶���Χ�ڵ����е���
    /// </summary>
    /// <param name="pos">��ɫλ��</param>
    /// <param name="attackRange">��Χ</param>
    /// <param name="currRoleCtrl">��ɫ������</param>
    /// <returns></returns>
    private List<Collider> FindEnemy(Vector3 pos, float attackRange, RoleCtrl currRoleCtrl)
    {
        //�����ǰû���������� ���������ĵ���
        //������Ϊ���� ���������ص���� Ҫע�⣺����Լ�һ���ᱻ��⵽
        Collider[] colliderArr = Physics.OverlapSphere(pos, attackRange, 1 << LayerMask.NameToLayer("Role"));

        List<Collider> colliderList = new List<Collider>();
        colliderList.Clear();
        if (colliderArr != null && colliderArr.Length > 0)
        {
            for (int i = 0; i < colliderArr.Length; i++)
            {
                RoleCtrl ctrl = colliderArr[i].GetComponent<RoleCtrl>();
                if (ctrl != null)
                {
                    //�ų�����
                    if (ctrl.CurrRoleInfo.RoleId != currRoleCtrl.CurrRoleInfo.RoleId)
                    {
                        colliderList.Add(colliderArr[i]);
                    }
                }
            }
        }
        return colliderList;
    }
    #endregion

    #region ���յ��������ǵľ���������
    private List<Collider> SortEnemyByDistance(List<Collider> list, RoleCtrl currRoleCtrl)
    {
        list.Sort((c1, c2) =>
        {
            int ret = 0;
            if (Vector3.Distance(c1.gameObject.transform.position, currRoleCtrl.transform.position) <
                Vector3.Distance(c2.gameObject.transform.position, currRoleCtrl.transform.position))
            {
                ret = -1;
            }
            else
            {
                ret = 1;
            }
            return ret;
        });
        return list;
    }
    #endregion

    #region �������Ҫ�����˺��ĵ����б�
    private void FillEnemyList(List<RoleCtrl> enemyList)
    {
        //TODO
    }
    #endregion
}
