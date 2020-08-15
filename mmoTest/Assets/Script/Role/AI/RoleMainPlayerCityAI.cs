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

    private int m_PhyIndex = 0;//物理攻击索引
    private List<Collider> m_SearchList = new List<Collider>();
    private float m_NextAttackTime = 0f;
    private Vector3 targetPoint;
    private Vector3 rayPoint;

    public void DoAI()
    {
        //执行AI
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
        //if (GameLevelSceneCtrl.Instance.CurrentRegionHasMonster == false)//如果当前区域已经没有怪了
        //{
        //    //如果当前是最后一个区域 直接返回
        //    if (GameLevelSceneCtrl.Instance.CurrentRegionIsLast)
        //    {
        //        return;
        //    }
        //    else//否则 进入下一个区域
        //    {
        //        //让主角移动到下一个区域的出生点
        //        CurrRole.MoveTo(GameLevelSceneCtrl.Instance.NextRegionPlayerBornPos);
        //    }
        //}
        //else
        {
            if (CurrRole.LockEnemy == null)//当前没有锁定敌人
            {
                #region 找离自己最近的怪 作为锁定怪
                //根据我的视野范围 找到最近的怪 作为锁定的敌人
                m_SearchList.Clear();
                m_SearchList = GameUtil.FindEnemy(CurrRole, 1000f);//找敌人
                m_SearchList = GameUtil.SortEnemyByDistance(m_SearchList, CurrRole);//给敌人排序
                if (m_SearchList.Count > 0)
                {
                    CurrRole.LockEnemy = m_SearchList[0].GetComponent<RoleCtrl>();//锁定最近的敌人
                }
                #endregion
            }
            else//当前有锁定敌人
            {
                if (CurrRole.LockEnemy.CurrRoleInfo.CurrHP <= 0)
                {
                    CurrRole.LockEnemy = null;
                    return;
                }

                #region 检查自己的技能使用情况 决定使用技能还是物理攻击
                //首先检测有没有可使用的技能 拿到对应技能的攻击范围
                int skillId = CurrRole.CurrRoleInfo.GetCanUseSkillId();//从角色信息类获取当前可以使用的技能
                RoleAttackType attackType;
                if (skillId > 0)
                {
                    //使用技能攻击
                    attackType = RoleAttackType.SkillAttack;
                }
                else
                {
                    //使用物理攻击
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

                //判断敌人是否在攻击范围内
                if (Vector3.Distance(CurrRole.transform.position, CurrRole.LockEnemy.transform.position) <= skillEntity.AttackRange)//在攻击范围内
                {
                    //发起攻击
                    //CurrRole.transform.LookAt(new Vector3(CurrRole.LockEnemy.transform.position.x, CurrRole.transform.position.y, CurrRole.LockEnemy.transform.position.z));

                    //if (Time.time > m_NextAttackTime && CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum != RoleState.Attack)
                    //{
                    //    m_NextAttackTime = Time.time + 1f;
                    //    if (attackType == RoleAttackType.SkillAttack)
                    //    {
                    //        PlayerController.Instance.OnSkillClick(skillId);//模拟点击技能图标
                    //    }
                    //    else
                    //    {
                    //        CurrRole.ToAttackBySkillId(attackType, skillId);
                    //    }
                        
                    //}
                    if (attackType == RoleAttackType.SkillAttack)
                    {
                        PlayerController.Instance.OnSkillClick(skillId);//模拟点击技能图标 开始技能冷却
                    }
                    else
                    {
                        CurrRole.ToAttackBySkillId(attackType, skillId);
                    }
                }
                else//不在攻击范围内
                {
                    //进行追击
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
    /// 非自动战斗时 如果当前锁定敌人不为空 自动对锁定敌人发动物理攻击
    /// </summary>
    private void NormalState()
    {
        //如果离上次战斗时间超过30s 切换为普通待机
        if (CurrRole.PreviousFightTime != 0)
        {
            if (Time.time > CurrRole.PreviousFightTime + 30f)
            {
                CurrRole.ToIdle();
                CurrRole.PreviousFightTime = 0;
            }
        }

        //1.每帧进行判断 如果我有锁定敌人 就行攻击
        if (CurrRole.LockEnemy != null)
        {
            if (CurrRole.LockEnemy.CurrRoleInfo.CurrHP <= 0)
            {
                CurrRole.LockEnemy = null;
                return;
            }

            SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(CurrRole.CurrRoleInfo.PhySkillIds[m_PhyIndex]);
            if (skillEntity == null) return;

            //判断敌人是否在攻击范围内
            if (Vector3.Distance(CurrRole.transform.position, CurrRole.LockEnemy.transform.position) <= 3)//在攻击范围内
            {
                if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Idle)
                {
                    if (CurrRole.Attack.FollowSkillId > 0)
                    {
                        PlayerController.Instance.OnSkillClick(CurrRole.Attack.FollowSkillId);//模拟点击技能
                    }
                    else
                    {
                        //物理攻击Id自增长 循环使用
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
            else//不在攻击范围内
            {
                //进行追击
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
