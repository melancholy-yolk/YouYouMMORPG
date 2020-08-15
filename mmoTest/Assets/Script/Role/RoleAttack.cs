using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoleAttack
{
#region 变量
    [Header("==== Physics Attack Info ====")]
    public List<RoleAttackInfo> PhyAttackInfoList;//物理攻击信息列表

    [Header("==== Skill Attack Info ====")]
    public List<RoleAttackInfo> SkillAttackInfoList;//技能攻击信息列表

    private RoleFSMMgr m_RoleFSMMgr = null;
    private RoleCtrl m_CurrRoleCtrl = null;

    private List<RoleCtrl> m_EnemyList = null;//真正要承受攻击的敌人列表
    private List<Collider> m_SearchList = null;//碰撞检测到的技能范围内的敌人

    /*
     * 玩家锁定敌人时会默认一直进行物理攻击 此时点击技能图标 由于处于攻击中的僵直状态 技能无法释放 所以我们将点击的技能记录下来 下次进行AI攻击时会进行判断
     * 如果缓存了要释放的后续技能 就进行技能攻击
     */
    private int m_FollowSkillId;//后续技能编号:锁定敌人自动发动物理攻击时按下的技能会成为后续技能
    [HideInInspector]
    public int FollowSkillId { get { return m_FollowSkillId; } }
    [HideInInspector]
    public bool IsAutoFight;//玩家是否开启自动战斗

    public string EffectPath;//角色技能特效的路径
#endregion


    public void SetFSM(RoleFSMMgr fsm)//序列化的类没有构造函数 使用此函数进行初始化
    {
        m_RoleFSMMgr = fsm;
        m_CurrRoleCtrl = m_RoleFSMMgr.CurrRoleCtrl;
        m_EnemyList = new List<RoleCtrl>();
        m_SearchList = new List<Collider>();
    }

    

    private RoleStateAttack m_RoleStateAttack;//角色攻击状态机



    public void ToAttackByIndex(RoleAttackType type, int index)
    {
#if DEBUG_ROLE_STATE
        //如果 有限状态机 或 玩家正在攻击中
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

        //震屏
        if (CameraCtrl.Instance != null)
        {
            CameraCtrl.Instance.CameraShake(info.CameraShakeDelay, info.CameraShakeDuration, info.CameraShakeStrength, info.CameraShakeVibrto);
        }

        if (m_RoleStateAttack == null)
        {
            //获取攻击状态
            //为什么要获取这个状态：因为要修改攻击状态机中的参数
            m_RoleStateAttack = m_RoleFSMMgr.GetRoleState(RoleState.Attack) as RoleStateAttack;
        }

        m_RoleStateAttack.AnimatorCondition = type == RoleAttackType.PhyAttack ? "ToPhyAttack" : "ToSkill";
        m_RoleStateAttack.AnimatorConditionValue = index;
        m_RoleStateAttack.CurrAnimatorState = GameUtil.GetRoleAnimatorState(type, index);

        //切换成攻击状态
        m_RoleFSMMgr.ChangeState(RoleState.Attack);
#endif
    }//测试攻击使用

    public bool ToAttack(RoleAttackType type, int skillId)
    {
        //如果 有限状态机不为空 或 玩家正在攻击中(僵直状态)
        if (m_RoleFSMMgr == null || m_RoleFSMMgr.CurrRoleCtrl.IsRigidity == true) 
        {
            if (type == RoleAttackType.SkillAttack)
            {
                m_FollowSkillId = skillId;//设置后续技能
            }
            return false;
        }

        m_FollowSkillId = -1;

        #region 数值相关
        //1.只有主角和怪物才参与数值计算(PVE伤害判定在客户端进行运算)
        if (m_CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer || m_CurrRoleCtrl.CurrRoleType == RoleType.Monster)
        {
            //2.获取技能信息
            SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(skillId);
            if (skillEntity == null) return false;

            int skillLevel = m_CurrRoleCtrl.CurrRoleInfo.GetSkillLevelBySkillId(skillId);//通过技能编号 从角色已经学会的技能列表中查询技能等级
            SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndSkillLevel(skillId, skillLevel);

            m_EnemyList.Clear();

            //如果是主角 才找敌人
            if (m_CurrRoleCtrl.CurrRoleType == RoleType.MainPlayer)
            {
                #region 找敌人
                //4.寻找敌人
                int attackTargetCount = skillEntity.AttackTargetCount;

                if (attackTargetCount == 1)
                {
                    #region 单体攻击
                    //单体攻击
                    if (m_CurrRoleCtrl.LockEnemy != null)
                    {
                        //如果当前有锁定的敌人 加入到敌人列表中
                        m_EnemyList.Add(m_CurrRoleCtrl.LockEnemy);
                    }
                    else
                    {
                        //球形检测
                        m_SearchList = FindEnemy(m_CurrRoleCtrl.transform.position, skillEntity.AttackRange, m_CurrRoleCtrl);
                        if (m_SearchList.Count == 0)
                        {
                            Debug.Log("no enemy");
                            return false;//附近没有敌人 直接返回
                        }

                        //根据距离排序
                        m_SearchList = SortEnemyByDistance(m_SearchList, m_CurrRoleCtrl);

                        m_CurrRoleCtrl.LockEnemy = m_SearchList[0].gameObject.GetComponent<RoleCtrl>();
                        m_EnemyList.Add(m_CurrRoleCtrl.LockEnemy);
                    }
                    #endregion
                }
                else
                {
                    #region 群攻
                    int needAttack = attackTargetCount;//需要攻击的数量

                    //球形检测
                    m_SearchList = FindEnemy(m_CurrRoleCtrl.transform.position, skillEntity.AttackRange, m_CurrRoleCtrl);
                    if (m_SearchList.Count == 0)
                    {
                        Debug.Log("no enemy");
                        return false;//附近没有敌人 直接返回
                    }

                    //根据距离排序
                    m_SearchList = SortEnemyByDistance(m_SearchList, m_CurrRoleCtrl);

                    #region 填充敌人列表
                    if (m_CurrRoleCtrl.LockEnemy != null)
                    {
                        //如果当前有锁定的敌人 加入到敌人列表中
                        m_EnemyList.Add(m_CurrRoleCtrl.LockEnemy);
                        needAttack--;//需要攻击的数量-1

                        //计算其他需要伤害的敌人
                        for (int i = 0; i < m_SearchList.Count; i++)
                        {
                            RoleCtrl ctrl = m_SearchList[i].gameObject.GetComponent<RoleCtrl>();
                            if (ctrl.CurrRoleInfo.RoleId != m_CurrRoleCtrl.LockEnemy.CurrRoleInfo.RoleId && ctrl.CurrRoleInfo.RoleId != m_CurrRoleCtrl.CurrRoleInfo.RoleId)//避免当前锁定敌人重复加入
                            {
                                if ((i + 1) > needAttack) break;
                                m_EnemyList.Add(ctrl);
                            }
                        }
                    }
                    else
                    {
                        m_CurrRoleCtrl.LockEnemy = m_SearchList[0].GetComponent<RoleCtrl>();
                        //计算其他需要伤害的敌人
                        for (int i = 0; i < m_SearchList.Count; i++)
                        {
                            //玩家自己不能攻击自己
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

            //拿到了敌人列表m_EnemyList

            //========== PVE和PVP的区别 ==========
            //1.PVE是直接让敌人受伤
            //2.PVP是发送消息给服务器

            if (SceneMgr.Instance.CurrPlayType == PlayType.PVE)
            {
                //1.PVE是直接让敌人受伤
                for (int i = 0; i < m_EnemyList.Count; i++)
                {
                    RoleTransferAttackInfo roleTransferAttackInfo = CalculateHurtValue(m_EnemyList[i], skillLevelEntity);
                    m_EnemyList[i].ToHurt(roleTransferAttackInfo);
                }
            }
            else if (SceneMgr.Instance.CurrPlayType == PlayType.PVP)
            {
                //2.PVP是发送消息给服务器
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

            //5.让敌人受伤
            

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

        //如果是PVE播放动画 如果是PVP只是发送消息给服务器 等服务器通知客户端使用技能后在播放动画
        if (SceneMgr.Instance.CurrPlayType == PlayType.PVE)
        {
            PlayAttack(skillId);
        }

        return true;
    }

    public void PlayAttack(int skillId)
    {
        RoleAttackType type = SkillDBModel.Instance.GetEntity(skillId).IsPhyAttack == 1 ? RoleAttackType.PhyAttack : RoleAttackType.SkillAttack;

        RoleAttackInfo attackInfo = GetRoleAttackInfo(type, skillId);//获取攻击相关信息

        //从AssetBundle中 加载特效
        if (attackInfo == null) return;

        m_RoleFSMMgr.CurrRoleCtrl.CurrAttackInfo = attackInfo;

        if (!string.IsNullOrEmpty(attackInfo.EffectName))
        {
            Transform obj = EffectMgr.Instance.PlayEffect(EffectPath, attackInfo.EffectName);//特效池取出特效
            obj.position = m_RoleFSMMgr.CurrRoleCtrl.transform.position;
            obj.rotation = m_RoleFSMMgr.CurrRoleCtrl.transform.rotation;
            EffectMgr.Instance.DestroyEffect(obj, attackInfo.EffectLifeTime);//特效池回收特效
        }

        if (CameraCtrl.Instance != null)
        {
            CameraCtrl.Instance.CameraShake(attackInfo.CameraShakeDelay, attackInfo.CameraShakeDuration, attackInfo.CameraShakeStrength, attackInfo.CameraShakeVibrto);
        }

        if (m_RoleStateAttack == null)
        {
            //获取攻击状态
            //为什么要获取这个状态：因为要修改攻击状态机中的参数
            m_RoleStateAttack = m_RoleFSMMgr.GetRoleState(RoleState.Attack) as RoleStateAttack;
        }

        m_RoleStateAttack.AnimatorCondition = type == RoleAttackType.PhyAttack ? "ToPhyAttack" : "ToSkill";
        m_RoleStateAttack.AnimatorConditionValue = attackInfo.Index;
        m_RoleStateAttack.CurrAnimatorState = GameUtil.GetRoleAnimatorState(type, attackInfo.Index);

        //动画切换成攻击状态
        m_RoleFSMMgr.ChangeState(RoleState.Attack);
    }

    /// <summary>
    /// 通过公式计算最终伤害信息
    /// </summary>
    private RoleTransferAttackInfo CalculateHurtValue(RoleCtrl enemy, SkillLevelEntity skillLevelEntity)
    {
        if (enemy == null || skillLevelEntity == null) return null;

        SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(skillLevelEntity.SkillId);
        if (skillEntity == null) return null;

        RoleTransferAttackInfo roleTransferAttackInfo = new RoleTransferAttackInfo();

        roleTransferAttackInfo.AttackRoleId = m_CurrRoleCtrl.CurrRoleInfo.RoleId;//发起攻击者
        roleTransferAttackInfo.BeAttackedRoleId = enemy.CurrRoleInfo.RoleId;//被攻击者
        roleTransferAttackInfo.SkillId = skillEntity.Id;
        roleTransferAttackInfo.SkillLevel = skillLevelEntity.Level;
        roleTransferAttackInfo.AttackRolePos = m_CurrRoleCtrl.transform.position;//攻击者的位置
        roleTransferAttackInfo.IsAbnormal = (skillEntity.AbnormalState == 1);//是否附加异常状态

        //计算伤害
        

        //1.攻击数值 = 攻方综合战斗力 * （技能伤害率 * 0.01f）
        float attackValue = m_CurrRoleCtrl.CurrRoleInfo.Fighting * (skillLevelEntity.HurtValueRate * 0.01f);

        //2.基础伤害  = 攻击数值 * 攻击数值 / （攻击数值 + 被攻击方的防御）
        float baseHurt = attackValue * attackValue / (attackValue + enemy.CurrRoleInfo.Defense);

        //3.暴击概率 = 0.05f + （攻方暴击 / （攻方暴击 + 防御方抗性）* 0.1f）
        float cri = 0.05f + (m_CurrRoleCtrl.CurrRoleInfo.Cri / (m_CurrRoleCtrl.CurrRoleInfo.Cri + enemy.CurrRoleInfo.Res) * 0.1f);

        //暴击概率 = 暴击概率 > 0.5f ? 0.5f : 暴击概率
        cri = cri > 0.5f ? 0.5f : cri;

        //4.是否暴击 = 0-1随机数 <= 暴击概率
        bool isCri = Random.Range(0f, 1f) <= cri;

        //5.暴击伤害倍率 = 有暴击 ? 1.5f : 1f
        float criHurt = isCri ? 1.5f : 1f;

        //6.随机数 = 0.9f-1.1f之间
        float random = Random.Range(0.9f, 1.1f);

        //7.最终伤害 = 基础伤害 * 暴击伤害倍率 * 随机数
        int hurtValue = Mathf.RoundToInt(baseHurt * criHurt * random);
        hurtValue = hurtValue < 1 ? 1 : hurtValue;

        roleTransferAttackInfo.HurtValue = hurtValue;
        roleTransferAttackInfo.IsCri = isCri;

        return roleTransferAttackInfo;
    }

    /// <summary>
    /// 通过攻击类型 和 攻击索引 找到对应的攻击信息
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
    /// 通过攻击类型 和 技能Id 找到对应的攻击信息
    /// </summary>
    private RoleAttackInfo GetRoleAttackInfo(RoleAttackType type, int skillId)
    {
        if (type == RoleAttackType.PhyAttack)//物理攻击
        {
            for (int i = 0; i < PhyAttackInfoList.Count; i++)
            {
                if (PhyAttackInfoList[i].SkillId == skillId)
                {
                    return PhyAttackInfoList[i];
                }
            }
        }
        else//技能攻击
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

    #region 球形检测 寻找敌人
    /// <summary>
    /// 查找角色固定范围内的所有敌人
    /// </summary>
    /// <param name="pos">角色位置</param>
    /// <param name="attackRange">范围</param>
    /// <param name="currRoleCtrl">角色控制器</param>
    /// <returns></returns>
    private List<Collider> FindEnemy(Vector3 pos, float attackRange, RoleCtrl currRoleCtrl)
    {
        //如果当前没有锁定敌人 找离得最近的敌人
        //以主角为中心 进行球形重叠检测 要注意：玩家自己一定会被检测到
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
                    //排除自身
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

    #region 按照敌人与主角的距离来排序
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

    #region 填充最终要计算伤害的敌人列表
    private void FillEnemyList(List<RoleCtrl> enemyList)
    {
        //TODO
    }
    #endregion
}
