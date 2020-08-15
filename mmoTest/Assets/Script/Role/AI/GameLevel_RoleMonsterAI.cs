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
    private int usedSkillId = 0;//用使用的技能Id

    private Vector3 MoveToPoint;//要移动到的目标点
    private Vector3 RayPoint;//寻路目标点有效性检测射线起点

    private float m_NextThinkTime = 0f;
    private bool m_IsDaze;//是否发呆中

    public GameLevel_RoleMonsterAI(RoleCtrl roleCtrl, RoleInfoMonster info)
    {
        CurrRole = roleCtrl;
        roleInfo = info;
    }

    public void DoAI()
    {
        //如果当前玩家不存在 直接返回
        if (GlobalInit.Instance == null || GlobalInit.Instance.MainPlayer == null) return;

        //如果怪物已经死亡 直接返回
        if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Die || CurrRole.IsRigidity) return;

        if(CurrRole.LockEnemy == null)
        {
            #region 没有锁定敌人
            if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Idle)
            {
                
                //定时巡逻
                if (Time.time > m_NextPatrolTime)
                {
                    m_NextPatrolTime = Time.time + 6f;
                    MoveToPoint = new Vector3(CurrRole.BornPoint.x + Random.Range(CurrRole.PatrolRange * -1, CurrRole.PatrolRange), 
                                                    CurrRole.BornPoint.y, 
                                                    CurrRole.BornPoint.z + Random.Range(CurrRole.PatrolRange * -1, CurrRole.PatrolRange));

                    //要寻路去往的目标点只能在当前区域
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

                //主角在怪的视野范围内
                if (Vector3.Distance(CurrRole.transform.position, GlobalInit.Instance.MainPlayer.transform.position) <= CurrRole.ViewRange)
                {
                    CurrRole.LockEnemy = GlobalInit.Instance.MainPlayer;//怪锁定敌人
                    m_NextAttackTime = Time.time + roleInfo.spriteEntity.DelaySec_Attack;
                }
            }
            #endregion
        }
        else
        {
            #region 有锁定敌人
            //锁定敌人已经死亡 锁定目标设置为null 然后返回
            if (CurrRole.LockEnemy.CurrRoleInfo.CurrHP <= 0)
            {
                CurrRole.LockEnemy = null;
                return;
            }

            if (Time.time > m_NextThinkTime + UnityEngine.Random.Range(3, 3.5f))
            {
                //让角色休息
                CurrRole.ToIdle(RoleIdleState.IdleFight);
                m_NextThinkTime = Time.time;
                m_IsDaze = true;
            }

            //角色休息中 直接返回
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

            //只有待机状态才进行思考
            if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum != RoleState.Idle) return;

            //我与锁定敌人距离超过我的视野范围 则取消锁定
            //主角跑得快 脱离了怪的视野范围 怪取消追击
            if (Vector3.Distance(CurrRole.transform.position, CurrRole.LockEnemy.transform.position) > CurrRole.ViewRange)
            {
                CurrRole.LockEnemy = null;
                return;
            }

            #region 随机一个技能Id
            //如果有锁定的敌人
            //1.得到怪要使用的技能Id（包括物理攻击）
            //物理攻击概率
            int random = Random.Range(0, 100);
            if (roleInfo.spriteEntity.PhysicalAttackRate >= random)
            {
                //说明要使用物理攻击
                usedAttackType = RoleAttackType.PhyAttack;
                usedSkillId = roleInfo.spriteEntity.UsedPhyAttackArr[Random.Range(0, roleInfo.spriteEntity.UsedPhyAttackArr.Length)];
            }
            else
            {
                //使用技能攻击
                usedAttackType = RoleAttackType.SkillAttack;
                usedSkillId = roleInfo.spriteEntity.UsedSkillAttackArr[Random.Range(0, roleInfo.spriteEntity.UsedSkillAttackArr.Length)];
            }
            #endregion


            //得到该技能信息
            SkillEntity entity = SkillDBModel.Instance.GetEntity(usedSkillId);
            if (entity == null) return;

            //2.判断敌人是否在该技能的攻击范围内 
            if (Vector3.Distance(CurrRole.transform.position, CurrRole.LockEnemy.transform.position) <= entity.AttackRange)
            {
                CurrRole.transform.LookAt(CurrRole.LockEnemy.transform);

                //在攻击范围之内 直接发起攻击
                //如果当前时刻大于下次攻击时刻 并且当前角色不处于攻击状态
                if (Time.time > m_NextAttackTime && CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum != RoleState.Attack)
                {
                    m_NextAttackTime = Time.time + Random.Range(0f, 1f) + roleInfo.spriteEntity.Attack_Interval;//设置攻击间隔
                    CurrRole.ToAttackBySkillId(usedAttackType, usedSkillId);
                }
            }
            else
            {
                
                //在攻击范围之外 进行追击
                if (CurrRole.CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Idle)
                {
                    MoveToPoint = GameUtil.GetRandomPos(CurrRole.transform.position, CurrRole.LockEnemy.transform.position, entity.AttackRange);

                    //游戏关卡中 只有点击当前区域才有效
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
