using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(FunnelModifier))]
public class RoleCtrl : MonoBehaviour
{
    #region 变量

    #region 头顶血条和称号
    public Transform m_HeadBarPos;//角色头顶名称挂点
    private GameObject m_HeadBar;//角色头顶名称物体
    #endregion

    [HideInInspector]
    public Animator m_Animator;

    #region 角色移动
    [HideInInspector]
    public Vector3 TargetPos;//角色要前往的目标位置
    [HideInInspector]
    public CharacterController m_CharacterController;//角色控制器
    [HideInInspector]
    public float Speed = 10f;//移动速度
    [HideInInspector]
    public float m_RotateSpeed = 0.2f;//转身速度
    private Quaternion m_TargetQuaternion;
    #endregion

    [HideInInspector]
    public Vector3 BornPoint;//角色出生点

    public float ViewRange = 6;//视野范围
    public float PatrolRange = 10;//巡逻范围
    public float AttackRange = 3;//每个技能的攻击范围都不同 要根据技能编号读取

    public RoleAttackInfo CurrAttackInfo;//当前测试使用

    #region 角色信息
    [HideInInspector]
    public RoleType CurrRoleType;//角色类型
    public RoleInfoBase CurrRoleInfo;//角色信息
    public IRoleAI CurrRoleAI = null;
    [HideInInspector]
    public RoleCtrl LockEnemy;//锁定的敌人
    private bool m_IsInit = false;//角色是否已经初始化
    public bool IsDied;//角色是否已经死亡
    #endregion

    [HideInInspector]
    public RoleFSMMgr CurrRoleFSMMgr = null;//有限状态机管理器
    private RoleHeadBarView roleHeadBarView = null;//角色血条UI

    #region 角色A星寻路
    private Seeker seeker;//生成寻路数据
    public Seeker Seeker
    {
        get
        {
            return seeker;
        }
    }
    [HideInInspector]
    public ABPath AStarPath;//路径
    [HideInInspector]
    public float nextWaypointDistance = 3;//与下个目标点的距离
    [HideInInspector]
    public int AStarCurrentWaypoint = 0;//当前要去的路径点序号
    [HideInInspector]
    public float roleSlerp = 0f;//转身速度
    [HideInInspector]
    public AstarPath AstarPathController;
    #endregion

    #region 角色战斗
    //========== 战斗相关 ==========
    public RoleAttack Attack;//处理攻击逻辑 数值计算 视觉表现
    private RoleHurt m_Hurt;//处理受伤逻辑 数值计算 视觉表现

    public Action<RoleCtrl> OnDieHandler;//外界其他模块角色死亡相关逻辑要注册在这个委托上
    public Action<Transform> OnDestroyHandler;//外界其他模块角色销毁相关逻辑要注册在这个委托上

    [HideInInspector]
    public bool IsRigidity;//是否处于僵直状态（攻击中处于僵直状态）

    [HideInInspector]
    public float PreviousFightTime = 0f;//上次发生战斗的时间 用来处理待机动画的切换

    

    public delegate void OnValueChangeHandler(int type);//数值变化委托定义
    public OnValueChangeHandler OnHPChange;//角色HP改变委托
    public OnValueChangeHandler OnMPChange;//角色MP改变委托
    #endregion

    #endregion

    #region 角色数据初始化
    public void Init(RoleType roleType, RoleInfoBase roleInfo, IRoleAI ai)
    {
        CurrRoleType = roleType;
        CurrRoleInfo = roleInfo;
        CurrRoleAI = ai;

        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = true;
        }

        m_IsInit = true;
    }
    #endregion

    #region 引擎生命周期
    void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
        CurrRoleFSMMgr = new RoleFSMMgr(this, OnRoleDieCallBack, OnRoleDestroyCallBack);
    }

    void Start () 
    {
        //角色进入新场景 给A星寻路组件赋值
        if ((SceneMgr.Instance.CurrentSceneType == SceneType.GameLevel || SceneMgr.Instance.CurrentSceneType == SceneType.WorldMap) && AstarPathController == null)
        {
            AstarPathController = GameObject.Find("A*").GetComponent<AstarPath>();
        }
        seeker = GetComponent<Seeker>();
        
        //如果是主角 摄像机功能设置
        if (CurrRoleType == RoleType.MainPlayer)
        {
            if (CameraCtrl.Instance != null)
            {
                CameraCtrl.Instance.Init();
            }
        }

        //处理角色受伤逻辑类
        m_Hurt = new RoleHurt(CurrRoleFSMMgr);
        m_Hurt.OnRoleHurt = OnRoleHurtCallBack;

        //处理角色攻击逻辑类
        Attack.SetFSM(CurrRoleFSMMgr);
	}

    

    #region 角色状态改变回调
    /// <summary>
    /// 角色销毁回调
    /// </summary>
    private void OnRoleDestroyCallBack()
    {
        //角色死亡一定时间后 要把角色回收到对象池中 回收工作不由角色自己处理
        if (OnDestroyHandler != null)
        {
            OnDestroyHandler(this.transform);
        }
    }

    /// <summary>
    /// 角色死亡回调
    /// </summary>
    private void OnRoleDieCallBack()
    {
        //角色死亡后不能再被点击
        //角色复活后启用角色控制器组件
        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = false;
        }

        //角色死亡后HeadBar也要销毁
        if (roleHeadBarView != null)
        {
            Destroy(roleHeadBarView.gameObject);
        }

        //执行其他模块与角色死亡相关的方法
        if (OnDieHandler != null)
        {
            OnDieHandler(this);
        }
    }

    /// <summary>
    /// 角色受伤回调
    /// </summary>
    private void OnRoleHurtCallBack()
    {
        if (roleHeadBarView != null)
        {
            roleHeadBarView.SetSliderHP((float)this.CurrRoleInfo.CurrHP/this.CurrRoleInfo.MaxHP);
        }
        if (OnHPChange != null)
        {
            OnHPChange(-1);
        }
    }
    #endregion

    #endregion

    #region 角色出生 设置角色出生时在场景中的位置和朝向 初始化HeadBar 初始化摄像机跟随
    /// <summary>
    /// 角色出生
    /// </summary>
    /// <param name="pos">出生点坐标</param>
    public void Born(Vector3 pos)
    {
        BornPoint = pos;
        transform.position = pos;
        InitHeadBar();//实例化血条和名称 并赋值
        ToIdle();

        if (CurrRoleType == RoleType.MainPlayer)
        {
            if (CameraCtrl.Instance != null)
            {
                CameraCtrl.Instance.Init();
            }
            
        }
    }
    #endregion

    void Update () 
    {
        
        //如果角色没有AI 直接返回
        if (CurrRoleAI != null)
        {
            CurrRoleAI.DoAI();
        }

        if (CurrRoleFSMMgr != null)
        {
            CurrRoleFSMMgr.OnUpdate();
        }

        //角色每次出生 初始化一次状态
        if (m_IsInit)
        {
            m_IsInit = false;

            if (CurrRoleInfo.CurrHP <= 0)
            {
                ToDie(isDied:true);
            }
            else
            {
                if (CurrRoleType == RoleType.Monster)//怪物只有战斗待机
                {
                    ToIdle(RoleIdleState.IdleFight);
                }
                else
                {
                    ToIdle();//默认为Idle状态
                }
            }
            
        }

        if (m_CharacterController == null) return;

        //让角色贴着地面
        //if (m_CharacterController.isGrounded == false)
        //{
        //    m_CharacterController.Move((transform.position + new Vector3(0, -1000, 0)) - transform.position);
        //}

        //摄像机跟随主角
        if (CurrRoleType == RoleType.MainPlayer)
        {
            CameraAutoFollow();
            AutoSmallMap();
        }

        
	}

    #region 小地图
    /// <summary>
    /// 小地图相关逻辑
    /// </summary>
    private void AutoSmallMap()
    {
        if (SmallMapHelper.Instance == null || UIMainCitySmallMapView.Instance == null) return;

        SmallMapHelper.Instance.gameObject.transform.position = transform.position;
        Transform trans = SmallMapHelper.Instance.gameObject.transform;
        UIMainCitySmallMapView.Instance.imageSmallMap.transform.localPosition = new Vector3(trans.localPosition.x * -512, trans.localPosition.z * -512, 1);
        UIMainCitySmallMapView.Instance.imageArrow.transform.localEulerAngles = new Vector3(0, 0, 360 - transform.eulerAngles.y);
    }
    #endregion

    #region HeadBar
    private void InitHeadBar()
    {
        if (RoleHeadBarRoot.Instance == null) return;
        if (CurrRoleInfo == null) return;
        if (m_HeadBarPos == null) return;

        m_HeadBar = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIItem, "MainCity", "RoleHeadBar");
        
        m_HeadBar.transform.SetParent(RoleHeadBarRoot.Instance.gameObject.transform);
        m_HeadBar.transform.localScale = Vector3.one;
        m_HeadBar.transform.localPosition = Vector3.zero;

        roleHeadBarView = m_HeadBar.GetComponent<RoleHeadBarView>();
        roleHeadBarView.Init(m_HeadBarPos, CurrRoleInfo.RoleNickName, isShowHPBar: (CurrRoleType == RoleType.Monster ? true : false), sliderValue:(float)this.CurrRoleInfo.CurrHP/this.CurrRoleInfo.MaxHP);
    }
    #endregion

    #region 控制角色方法

    public void ToIdle(RoleIdleState state = RoleIdleState.IdleNormal)
    {
        CurrRoleFSMMgr.ToIdleState = state;
        CurrRoleFSMMgr.ChangeState(RoleState.Idle);
    }

    public void ToRun()//临时测试用
    {
        CurrRoleFSMMgr.ChangeState(RoleState.Run);
    }

    public void MoveTo(Vector3 targetPos)
    {
        if (CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Die) return;
        if (IsRigidity) return;//攻击中不能移动
        if (targetPos == Vector3.zero) return;//如果目标点不是原点 进行移动

        TargetPos = targetPos;

        if (this.seeker != null)
        {
            //给出起点和终点 开始A*寻路计算 计算完成时将会回调OnPathComplete函数
            this.seeker.StartPath(this.transform.position, TargetPos, OnPathComplete);
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            AStarPath = (ABPath)p;
            if (Vector3.Distance(AStarPath.endPoint, new Vector3(AStarPath.originalEndPoint.x, AStarPath.endPoint.y, AStarPath.originalEndPoint.z)) > 0.5f)
            {
                Debug.Log("Can not arrive target point!");
                AStarPath = null;
                return;
            }

            if (CurrRoleType == RoleType.MainPlayer)
            {
                //PVP发送消息给服务器
                SendPVPMove(TargetPos, AStarPath.vectorPath);
            }

            AStarCurrentWaypoint = 1;
            CurrRoleFSMMgr.ChangeState(RoleState.Run);
        }
        else
        {
            AppDebug.Log("Astar find path fail!");
            AStarPath = null;
        }
    }

    private void SendPVPMove(Vector3 targetPos, List<Vector3> path)
    {
        if (SceneMgr.Instance.CurrPlayType == PlayType.PVP)
        {
            float pathLength = 0f;

            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1) continue;

                float dis = Vector3.Distance(path[i], path[i+1]);
                pathLength += dis;
            }

            float needTime = pathLength / Speed;//走完路径需要的时间

            WorldMap_CurrRoleMoveProto proto = new WorldMap_CurrRoleMoveProto();
            proto.TargetPosX = targetPos.x;
            proto.TargetPosY = targetPos.y;
            proto.TargetPosZ = targetPos.z;
            proto.ServerTime = GlobalInit.Instance.GetCurrServerTime();
            proto.NeedTime = (int)(needTime * 1000);

            NetWorkSocket.Instance.SendMsg(proto.ToArray());
        }
    }

    public void ToAttackByIndex(RoleAttackType type, int index)
    {
        Attack.ToAttackByIndex(type, index);
    }

    /// <summary>
    /// 外界调用角色的攻击接口 具体实现逻辑由RoleAttack类进行处理
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public bool ToAttackBySkillId(RoleAttackType type, int skillId)
    {
        return Attack.ToAttack(type, skillId);
    }

    /// <summary>
    /// PVP服务器广播 让攻击者演出攻击动画和特效 不进行数据运算
    /// </summary>
    public void PlayAttack(int skillId)
    {
        Attack.PlayAttack(skillId);
    }

    public void ToHurt(RoleTransferAttackInfo attackInfo)
    {
        StartCoroutine(m_Hurt.ToHurt(attackInfo));
    }

    public void ToDie(bool isDied = false)
    {
        IsDied = isDied;
        CurrRoleInfo.CurrHP = 0;
        CurrRoleFSMMgr.ChangeState(RoleState.Die);
    }

    public void ToSelect()
    {
        CurrRoleFSMMgr.ChangeState(RoleState.Select);
    }
    #endregion

    #region OnDestroy 销毁
    /// <summary>
    /// 销毁
    /// </summary>
    void OnDestroy()
    {
        if (m_HeadBar != null)
        {
            Destroy(m_HeadBar);
        }
    }
    #endregion

    #region CameraAutoFollow 摄像机自动跟随
    /// <summary>
    /// 摄像机自动跟随
    /// </summary>
    private void CameraAutoFollow()
    {
        if (CameraCtrl.Instance == null) return;

        CameraCtrl.Instance.transform.position = transform.position;
        CameraCtrl.Instance.AutoLookAt(transform.position);
    }
    #endregion

    void OnGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CurrAttackInfo.AttackRange);
    }

    public void RoleReborn()//角色复活
    {
        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = true;
        }

        PreviousFightTime = 0;
        CurrRoleInfo.CurrHP = CurrRoleInfo.MaxHP;
        CurrRoleInfo.CurrMP = CurrRoleInfo.MaxMP;
        LockEnemy = null;

        if (OnHPChange != null)
        {
            OnHPChange(1);
        }

        if (OnMPChange != null)
        {
            OnMPChange(1);
        }

        ToIdle();
    }

    /// <summary>
    /// PVP中同步其他玩家移动的修正速度 要比其他玩家的实际移动速度快 目的是为了能追得上 其他玩家实际的移动
    /// </summary>
    public float ModifySpeed { get; set; }
}
