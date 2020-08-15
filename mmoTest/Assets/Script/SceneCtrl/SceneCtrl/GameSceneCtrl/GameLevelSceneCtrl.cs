using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelSceneCtrl : GameSceneCtrlBase
{
    public static GameLevelSceneCtrl Instance;

    #region 变量
    [SerializeField]
    private GameLevelRegionCtrl[] AllRegion;//此关卡所有的区域控制器

    [SerializeField]
    private AstarPath m_AstarPath;//此关卡的A星寻路组件

    

    private int m_CurrGameLevelId;// 当前关卡Id
    private GameLevelGrade m_CurrGrade;// 当前关卡的难度等级

    private float m_FightTime = 0f;//战斗时长
    private bool m_IsFighting;//是否在战斗中


    #region 关卡数据
    private List<GameLevelRegionEntity> m_RegionList;// 表格中配置的区域列表(关卡区域顺序 区域按顺序排列 表示主角进入区域的顺序)
    private int m_AllMonsterCount;// 本关卡怪物总数
    private int[] m_MonsterIdArr;// 保存该关卡所有的怪物Id
    private SpawnPool m_MonsterPool;//怪物池
    #endregion

    #region 当前进入区域数据
    private int m_CurrRegionIndex;// 当前进入的区域索引号
    private int m_CurrRegionId;
    private GameLevelRegionCtrl m_CurrRegionCtrl;// 当前的区域控制器

    private int m_CurrRegionMonsterTotalCount;// 当前区域怪物总数
    private int m_CurrRegionCreatedMonsterCount;// 当前区域已经创建的怪物数量
    private int m_CurrRegionKilledMonsterCount;//当前区域已经杀死的怪物数量

    
    private Dictionary<int, int> m_RegionMonsterDic;//这个字典保存的信息是：那种怪 有多少

    private float m_NextCreateMonsterTime = 0;//间隔几秒刷下一只怪
    #endregion

    #endregion


    #region 生命周期
    protected override void OnAwake()
    {
        base.OnAwake();

        Instance = this;
    }

    protected override void OnStart()
    {
        base.OnStart();

        //给游戏关卡场景主控制器的变量赋值
        m_CurrGameLevelId = SceneMgr.Instance.CurrGameLevelId;//当前关卡编号
        m_CurrGrade = SceneMgr.Instance.CurrGameLevelGrade;//当前关卡难度

        //给游戏关卡UI控制器的变量赋值
        GameLevelController.Instance.CurrGameLevelId = m_CurrGameLevelId;
        GameLevelController.Instance.CurrGameLevelGrade = m_CurrGrade;

        m_IsFighting = true;//开始关卡战斗 此标志位为false时 表示关卡结束（通关成功/失败）

        m_RegionMonsterDic = new Dictionary<int, int>();//当前区域怪物详情 <怪物Id,怪物数量>
        spriteList = new List<int>();//当前区域怪物Id列表

        GameLevelController.Instance.CurrGameLevelTotalExp = 0;//当前关卡获得的总经验
        GameLevelController.Instance.CurrGameLevelTotalGold = 0;//当前关卡获得的总金币
        GameLevelController.Instance.CurrGameLevelKillMonsterDic.Clear();
        GameLevelController.Instance.CurrGameLevelGetGoodsList.Clear();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        //当前关卡还未通关
        if (m_IsFighting)
        {
            m_FightTime += Time.deltaTime;

            //间隔一段时间刷怪
            if (m_CurrRegionCreatedMonsterCount < m_CurrRegionMonsterTotalCount)
            {
                if (Time.time > m_NextCreateMonsterTime)
                {
                    m_NextCreateMonsterTime = Time.time + 1f;
                    CreateMonster();
                }
            }
        }

    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
    }
    #endregion

    #region 游戏关卡场景 场景UI加载完成回调
    /// <summary>
    /// 场景UI加载完成回调
    /// </summary>
    protected override void OnUISceneMainCityViewLoadComplete()
    {
        base.OnUISceneMainCityViewLoadComplete();

        //设置场景常驻UI
        PlayerController.Instance.SetMainCityRoleData();//controller--->view 控制器使用数据给视图赋值


        //根据游戏关卡编号返回excel中配置的区域列表 游戏关卡中区域的先后排列顺序
        m_RegionList = GameLevelRegionDBModel.Instance.GetListByGameLevelId(m_CurrGameLevelId);

        //场景怪物总数
        m_AllMonsterCount = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterTotalCount(m_CurrGameLevelId, m_CurrGrade);

        //场景怪物种类集合
        m_MonsterIdArr = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterId(m_CurrGameLevelId, m_CurrGrade);

        #region 怪物池
        //创建一个怪物池
        m_MonsterPool = PoolManager.Pools.Create("Monster");
        m_MonsterPool.group.parent = null;
        m_MonsterPool.group.localPosition = Vector3.zero;

        //从ab包中 加载每种怪物的预制体
        for (int i = 0; i < m_MonsterIdArr.Length; i++)
        {
            PrefabPool prefabPool = new PrefabPool(RoleMgr.Instance.LoadSprite(m_MonsterIdArr[i]).transform);
            prefabPool.preloadAmount = 5;//预加载数量
            prefabPool.cullDespawned = true;//是否开启缓存池自动清理
            prefabPool.cullAbove = 5;//缓存池始终保持对象
            prefabPool.cullDelay = 2;//多长时间清理一次
            prefabPool.cullMaxPerPass = 2;//每次清理几个

            m_MonsterPool.CreatePrefabPool(prefabPool);
        }
        #endregion

        //默认进入序号为0的区域
        m_CurrRegionIndex = 0;
        EnterRegion(m_CurrRegionIndex);
    }
    #endregion

    #region 属性 当前区域是否还有存活的怪
    /// <summary>
    /// 当前区域是否有怪
    /// </summary>
    public bool CurrentRegionHasMonster
    {
        get
        {
            return m_CurrRegionKilledMonsterCount < m_CurrRegionMonsterTotalCount;
        }
    }
    #endregion

    #region 属性 当前区域是否是当前关卡最后一个怪
    /// <summary>
    /// 当前区域是否是最后一个区域
    /// </summary>
    public bool CurrentRegionIsLast
    {
        get
        {
            return m_CurrRegionIndex == m_RegionList.Count;
        }
    }
    #endregion

    #region 属性 下一个区域主角出生点
    /// <summary>
    /// 下一个区域主角出生点
    /// </summary>
    public Vector3 NextRegionPlayerBornPos
    {
        get
        {
            GameLevelRegionEntity entity = GetRegionEntityByIndex(m_CurrRegionIndex);//通过excel中区域实体的排序索引 得到对应的区域实体
            if (entity == null) return Vector3.zero;

            int regionId = entity.RegionId;//要进入的新区域的区域编号
            return GetRegionCtrlByRegionId(regionId).RoleBornPos.position;
        }
    }
    #endregion

    /// <summary>
    /// 从GameLevelRegion表中得到按照先后顺序排列的区域列表 通过区域的先后排序索引 获得对应regionEntity
    /// </summary>
    private GameLevelRegionEntity GetRegionEntityByIndex(int index)
    {
        GameLevelRegionEntity entity = null;
        for (int i = 0; i < m_RegionList.Count; i++)
        {
            if (i == index)
            {
                entity = m_RegionList[i];
            }
        }
        return entity;
    }

    /// <summary>
    /// 通过区域Id 获得区域控制器
    /// </summary>
    private GameLevelRegionCtrl GetRegionCtrlByRegionId(int regionId)
    {
        GameLevelRegionCtrl ctrl = null;
        for (int i = 0; i < AllRegion.Length; i++)
        {
            if (AllRegion[i].RegionId == regionId)
            {
                ctrl = AllRegion[i];
            }
        }
        return ctrl;
    }

    #region 进入关卡中某个区域 传入参数为区域先后排序索引
    /// <summary>
    /// 进入区域 要做的事：重置区域变量 销毁区域遮挡 打开区域门 从excel读取区域数据
    /// </summary>
    private void EnterRegion(int regionIndex)
    {
        GameLevelRegionEntity entity = GetRegionEntityByIndex(regionIndex);//通过excel中区域实体的排序索引 得到对应的区域实体
        if (entity == null) return;

        int regionId = entity.RegionId;//要进入的新区域的区域编号
        m_CurrRegionCtrl = GetRegionCtrlByRegionId(regionId);
        if (m_CurrRegionCtrl == null) return;

        m_CurrRegionCreatedMonsterCount = 0;//当前进入区域已经刷出的怪物数量
        m_CurrRegionKilledMonsterCount = 0;//当前进入区域已经杀死的怪物数量

        //销毁本区域的遮挡物体
        if (m_CurrRegionCtrl.RegionMask != null)
        {
            Destroy(m_CurrRegionCtrl.RegionMask);
        }

        //打开上一个区域的门
        if (regionIndex != 0)
        {
            GameLevelDoorCtrl doorCtrl = m_CurrRegionCtrl.GetToNextRegionDoor(m_CurrRegionId);//上次的区域编号
            if (doorCtrl != null)
            {
                doorCtrl.gameObject.SetActive(false);
                if (doorCtrl.ConnectToDoor != null)
                {
                    doorCtrl.ConnectToDoor.gameObject.SetActive(false);
                }
            }
        }
        m_CurrRegionId = regionId;

        #region 进入一个关卡的第一个区域
        //如果是 当前关卡的第一个区域
        if (regionIndex == 0)
        {
            if (GlobalInit.Instance.MainPlayer != null)
            {
                GlobalInit.Instance.MainPlayer.Born(m_CurrRegionCtrl.RoleBornPos.position);//将主角位置设置到该区域的主角出生点位置
                GlobalInit.Instance.MainPlayer.AstarPathController = m_AstarPath;//给主角寻路组件添加引用
                GlobalInit.Instance.MainPlayer.ToIdle(RoleIdleState.IdleFight);//主角默认进入战斗待机
                GlobalInit.Instance.MainPlayer.OnDieHandler = (RoleCtrl roleCtrl) => { StartCoroutine(ShowFailView()); };//给主角死亡事件绑定回调
            }
            if (DelegateDefine.Instance.OnSceneLoadOK != null) DelegateDefine.Instance.OnSceneLoadOK();//通知新场景加载完成
        }
        #endregion

        //刷怪
        m_CurrRegionMonsterTotalCount = GameLevelMonsterDBModel.Instance.GetRegionMonsterTotalCount(m_CurrGameLevelId, m_CurrGrade, entity.RegionId);//获取当前区域怪物总数
        List<GameLevelMonsterEntity> m_RegionMonsterList = GameLevelMonsterDBModel.Instance.GetGameLevelMonster(m_CurrGameLevelId, m_CurrGrade, entity.RegionId);//获取当前区域怪物列表(一种怪一个实体 包括该种类怪物的编号 数量)
        for (int i = 0; i < m_RegionMonsterList.Count; i++)
        {
            if (m_RegionMonsterDic.ContainsKey(m_RegionMonsterList[i].SpriteId))
            {
                m_RegionMonsterDic[m_RegionMonsterList[i].SpriteId] += m_RegionMonsterList[i].SpriteCount; 
            }
            else
            {
                m_RegionMonsterDic[m_RegionMonsterList[i].SpriteId] = m_RegionMonsterList[i].SpriteCount;
            }
        }
        
    }

    /// <summary>
    /// 主角死亡 显示通关失败窗口
    /// </summary>
    private IEnumerator ShowFailView()
    {
        yield return new WaitForSeconds(3f);
        GameLevelController.Instance.OpenView(WindowUIType.GameLevelFail);
    }
    #endregion

    #region 刷出一个怪
    private int m_Index = 0;//随机索引
    private int m_MonsterTempId = 0;//临时怪物Id
    List<int> spriteList;//保存怪物Id的列表

    /// <summary>
    /// 创建怪物
    /// </summary>
    private void CreateMonster()
    {
        //if (m_CurrRegionCreatedMonsterCount >= 1) return;

        #region 随机拿到一个精灵Id
        spriteList.Clear();
        foreach (var item in m_RegionMonsterDic)
        {
            spriteList.Add(item.Key);
        }
        m_Index = Random.Range(0, spriteList.Count);
        int monsterId = spriteList[m_Index];
        #endregion

        //从缓存池中取出怪物
        Transform trans = m_MonsterPool.Spawn(SpriteDBModel.Instance.GetEntity(monsterId).PrefabName);

        //随机选择该区域的怪物出生点中的一个 作为怪物的出生点
        Transform monsterBornPos = m_CurrRegionCtrl.MonsterBornPos[Random.Range(0, m_CurrRegionCtrl.MonsterBornPos.Length)];

        //初始化怪物角色控制器
        RoleCtrl roleMonsterCtrl = trans.GetComponent<RoleCtrl>();

        SpriteEntity spriteEntity = SpriteDBModel.Instance.GetEntity(monsterId);//从本地怪物excel表中获取到此怪物编号对应的怪物实体

        RoleInfoMonster monsterInfo = new RoleInfoMonster(spriteEntity);//怪物的角色信息
        monsterInfo.RoleId = ++m_MonsterTempId;

        roleMonsterCtrl.ViewRange = spriteEntity.Range_View;//怪物视野范围
        roleMonsterCtrl.PatrolRange = 3;//怪物巡逻范围
        roleMonsterCtrl.Speed = spriteEntity.MoveSpeed;//怪物移动速度

        roleMonsterCtrl.Init(RoleType.Monster, monsterInfo, new GameLevel_RoleMonsterAI(roleMonsterCtrl, monsterInfo));//初始化角色控制器
        roleMonsterCtrl.OnDieHandler = OnDieHandler;//设置角色死亡回调
        roleMonsterCtrl.OnDestroyHandler = OnDestroyHandler;//设置角色销毁回调


        roleMonsterCtrl.ToIdle(RoleIdleState.IdleFight);//怪物默认为战斗待机状态
        roleMonsterCtrl.Born(monsterBornPos.TransformPoint(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)));//设置怪物出生位置

        m_RegionMonsterDic[monsterId]--;//要生成的此种类怪物数量减一
        if (m_RegionMonsterDic[monsterId] <= 0)
        {
            m_RegionMonsterDic.Remove(monsterId);
        }

        m_CurrRegionCreatedMonsterCount++;//已经刷出的怪物数量加一
    }
    #endregion

    #region 每杀死一个怪 进行此回调 判断关卡区域、是否完成关卡
    /// <summary>
    /// 角色死亡时回调 
    /// </summary>
    /// <param name="roleId"></param>
    private void OnDieHandler(RoleCtrl roleCtrl)
    {
        m_CurrRegionKilledMonsterCount++;//当前区域死亡怪物数量加一

        #region 怪物死亡掉落
        RoleInfoMonster monsterInfo = (RoleInfoMonster)roleCtrl.CurrRoleInfo;
        //处理掉落经验和金币
        GameLevelMonsterEntity entity = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterEntity(m_CurrGameLevelId, m_CurrGrade, m_CurrRegionId, monsterInfo.spriteEntity.Id);
        if (entity != null)
        {
            if (entity.Exp > 0)
            {
                UITipView.Instance.ShowTip(0, string.Format("+{0}", entity.Exp));
                GameLevelController.Instance.CurrGameLevelTotalExp += entity.Exp;
            }
            if (entity.Gold > 0)
            {
                UITipView.Instance.ShowTip(1, string.Format("+{0}", entity.Gold));
                GameLevelController.Instance.CurrGameLevelTotalGold += entity.Gold;
            }
        }
        #endregion

        //添加当前关卡杀怪详情
        if (GameLevelController.Instance.CurrGameLevelKillMonsterDic.ContainsKey(entity.SpriteId))
        {
            GameLevelController.Instance.CurrGameLevelKillMonsterDic[entity.SpriteId] += 1;
        }
        else
        {
            GameLevelController.Instance.CurrGameLevelKillMonsterDic[entity.SpriteId] = 1;
        }

        //当前区域怪物全部被杀死 可以进入下一个区域了
        if (m_CurrRegionKilledMonsterCount >= m_CurrRegionCreatedMonsterCount)
        {
            m_CurrRegionIndex++;
            //判断是否是最后一个区域
            if (CurrentRegionIsLast)
            {
                m_IsFighting = false;//通关
                GameLevelController.Instance.CurrGameLevelPassTime = m_FightTime;//给游戏关卡的UI控制器传值

                TimeMgr.Instance.ChangeTimeScale(0.5f, 3f);
                //弹出胜利界面
                Debug.Log("Game Win!");
                StartCoroutine(ShowVictory());
                return;
            }

            m_CurrRegionMonsterTotalCount = 0;// 当前区域怪物总数
            m_CurrRegionCreatedMonsterCount = 0;// 当前区域已经创建的怪物数量
            m_CurrRegionKilledMonsterCount = 0;//当前区域已经杀死的怪物数量

            //进入下一个区域
            EnterRegion(m_CurrRegionIndex);
        }
    }
    #endregion

    #region 每个怪死亡后 过一段时间回收到对象池
    /// <summary>
    /// 角色销毁
    /// </summary>
    private void OnDestroyHandler(Transform obj)
    {
        //角色回收
        m_MonsterPool.Despawn(obj);
    }
    #endregion

    private IEnumerator ShowVictory()
    {
        yield return new WaitForSeconds(3f);
        GameLevelController.Instance.OpenView(WindowUIType.GameLevelVictory);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);

        if (AllRegion != null && AllRegion.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < AllRegion.Length; i++)
            {
                Gizmos.DrawLine(this.transform.position, AllRegion[i].transform.position);
            }
        }
        
       
    }
#endif

}
