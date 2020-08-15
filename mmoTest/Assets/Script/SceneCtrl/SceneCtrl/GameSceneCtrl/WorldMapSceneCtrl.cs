using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 世界地图 主城场景控制器 负责：主角初始化 NPC初始化
/// </summary>
public class WorldMapSceneCtrl : GameSceneCtrlBase
{
    public static WorldMapSceneCtrl Instance;//单例

    #region 变量
    [SerializeField]
    private Transform m_PlayerBornPos;//主角出生点

    private WorldMapEntity CurrWorldMapEntity;//当前世界地图场景excel配置实体

    private Dictionary<int, WorldMapTransCtrl> m_TransPointDic;//保存此场景中所有的传送点
    private Dictionary<int, RoleCtrl> m_AllRoleDic;//当前场景中所有角色

    private WorldMap_PosProto m_WorldMap_PosProto = new WorldMap_PosProto();
    private float m_NextSendTime = 0f;//向服务器同步主角位置的计时器
    #endregion

    public void MainPlayerLeaveScene()
    {
        m_AllRoleDic.Remove(GlobalInit.Instance.MainPlayer.CurrRoleInfo.RoleId);
    }

    #region 初始化世界地图场景
    /// <summary>
    /// 场景UI加载完成回调 UI加载完成后 再使用数据设置UI
    /// </summary>
    protected override void OnUISceneMainCityViewLoadComplete()
    {
        base.OnUISceneMainCityViewLoadComplete();

        if (GlobalInit.Instance == null) return;

        RoleMgr.Instance.InitMainPlayer();//实例化主角模型 初始化主角数据
        
        if (GlobalInit.Instance.MainPlayer != null)//设置主角出生点
        {
            CurrWorldMapEntity = WorldMapDBModel.Instance.GetEntity(SceneMgr.Instance.CurrWorldMapId);

            InitTransPoint();//根据excel表格数据 初始化场景中的传送点

            #region 设置主角在场景中的位置
            if (SceneMgr.Instance.TargetWorldMapTransPointId == 0)//未设置玩家目标场景传送点Id
            {
                
                //如果服务器告诉了客户端最后所在世界地图场景中的位置信息
                if (!string.IsNullOrEmpty(PlayerController.Instance.LastInWorldMapPos))
                {
                    #region 出生在上次登录最后所在的位置
                    string[] arr = PlayerController.Instance.LastInWorldMapPos.Split('_');
                    Vector3 pos = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));

                    if (pos == Vector3.zero)
                    {
                        if (CurrWorldMapEntity.OriginRoleBirthPos != Vector3.zero)
                        {
                            GlobalInit.Instance.MainPlayer.Born(CurrWorldMapEntity.OriginRoleBirthPos);
                            GlobalInit.Instance.MainPlayer.transform.eulerAngles = new Vector3(0, CurrWorldMapEntity.RoleBirthEulerAnglesY, 0);
                        }
                        else
                        {
                            GlobalInit.Instance.MainPlayer.Born(m_PlayerBornPos.position);
                        }
                    }
                    else
                    {
                        GlobalInit.Instance.MainPlayer.Born(pos);
                        GlobalInit.Instance.MainPlayer.transform.eulerAngles = new Vector3(0, float.Parse(arr[3]), 0);
                    }
                    #endregion
                }
                else
                {
                    #region 出生在excel中配置的位置
                    if (CurrWorldMapEntity.OriginRoleBirthPos != Vector3.zero)
                    {
                        GlobalInit.Instance.MainPlayer.Born(CurrWorldMapEntity.OriginRoleBirthPos);
                        GlobalInit.Instance.MainPlayer.transform.eulerAngles = new Vector3(0, CurrWorldMapEntity.RoleBirthEulerAnglesY, 0);
                    }
                    else
                    {
                        GlobalInit.Instance.MainPlayer.Born(m_PlayerBornPos.position);
                    }
                    #endregion
                }
            }
            else//设置了目标场景传送点Id 出生在传送点附近
            {
                #region 出生在传送点旁边
                if (m_TransPointDic.ContainsKey(SceneMgr.Instance.TargetWorldMapTransPointId))
                {
                    Vector3 newBornPoint = m_TransPointDic[SceneMgr.Instance.TargetWorldMapTransPointId].transform.forward.normalized * 3 + m_TransPointDic[SceneMgr.Instance.TargetWorldMapTransPointId].transform.position;
                    Vector3 lookAtPoint = m_TransPointDic[SceneMgr.Instance.TargetWorldMapTransPointId].transform.forward.normalized * 4 + m_TransPointDic[SceneMgr.Instance.TargetWorldMapTransPointId].transform.position;
                    
                    GlobalInit.Instance.MainPlayer.Born(newBornPoint);
                    GlobalInit.Instance.MainPlayer.transform.LookAt(lookAtPoint);

                    SceneMgr.Instance.TargetWorldMapTransPointId = 0;
                }
                #endregion
            }
            #endregion

            SendRoleAlreadyEnter(SceneMgr.Instance.CurrWorldMapId, GlobalInit.Instance.MainPlayer.transform.position, GlobalInit.Instance.MainPlayer.transform.eulerAngles.y);

            //设置场景常驻UI的数据(主角信息 技能)
            PlayerController.Instance.SetMainCityRoleData();//controller--->view 控制器使用数据给视图赋值
        }

        if (DelegateDefine.Instance.OnSceneLoadOK != null) DelegateDefine.Instance.OnSceneLoadOK();//执行场景加载完成回调

        StartCoroutine(InitNPC());//读取excel表格 刷NPC
        AutoMove();//世界地图跨场景寻路
    }

    #region 初始化NPC
    /// <summary>
    /// 加载excel表中 此场景中所有的NPC
    /// </summary>
    private IEnumerator InitNPC()
    {
        yield return null;//等待一帧

        if (CurrWorldMapEntity.NPCWorldMapList == null) yield break;

        for (int i = 0; i < CurrWorldMapEntity.NPCWorldMapList.Count; i++)
        {
            NPCWorldMapData data = CurrWorldMapEntity.NPCWorldMapList[i];
            NPCEntity npcEntity = NPCDBModel.Instance.GetEntity(data.NPCId);

            string prefabName = npcEntity.PrefabName;
            GameObject obj = RoleMgr.Instance.LoadNPC(prefabName);
            obj.transform.position = data.NPCPosition;
            obj.transform.eulerAngles = new Vector3(0, data.NPCRotationY, 0);

            NPCCtrl npcCtrl = obj.GetComponent<NPCCtrl>();
            npcCtrl.Init(data);
        }
    }
    #endregion

    #region 初始化传送点
    /// <summary>
    /// 初始化传送点
    /// </summary>
    private void InitTransPoint()
    {
        if (m_TransPointDic == null)
        {
            m_TransPointDic = new Dictionary<int, WorldMapTransCtrl>();
        }
        for (int i = 0; i < CurrWorldMapEntity.TransPointList.Count; i++)
        {
            TransPointWorldMapData data = CurrWorldMapEntity.TransPointList[i];

            //实例化传送点
            GameObject obj = ResourcesManager.Instance.LoadCommonPrefab("Effect_Trans");
            obj.transform.position = data.TransPointPos;
            obj.transform.localEulerAngles = new Vector3(0,data.RotationY,0);

            //设置传送点数据
            WorldMapTransCtrl ctrl = obj.GetComponent<WorldMapTransCtrl>();
            if (ctrl != null)
            {
                ctrl.SetParam(data);
            }

            m_TransPointDic[data.TransPointId] = ctrl;
        }
    }
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

        if (GlobalInit.Instance.MainPlayer != null) GlobalInit.Instance.MainPlayer.Attack.IsAutoFight = false;//回到世界地图时 关闭角色自动战斗

        m_TransPointDic = new Dictionary<int, WorldMapTransCtrl>();
        m_AllRoleDic = new Dictionary<int, RoleCtrl>();

        AddEventListener();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (Time.time > m_NextSendTime)
        {
            m_NextSendTime += 1f;
            SendPlayerPos();
        }
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();

        RemoveEventListener();
    }
    #endregion

    #region 世界地图跨场景寻路移动
    /// <summary>
    /// 世界地图跨场景寻路移动
    /// </summary>
    public void AutoMove()
    {
        if (!WorldMapController.Instance.IsAutoMove) return;
        if (SceneMgr.Instance.CurrWorldMapId == WorldMapController.Instance.ToSceneId)
        {
            //到达了目标场景 前往场景中目标位置
            AppDebug.Log("arrive target scene");
            if (WorldMapController.Instance.ToScenePos != Vector3.zero)
            {
                GlobalInit.Instance.MainPlayer.MoveTo(WorldMapController.Instance.ToScenePos);
            }
            
            WorldMapController.Instance.IsAutoMove = false;
            return;
        }
        //找到当前场景中 主角要前往的传送点
        foreach (var item in m_TransPointDic)
        {
            if (item.Value.TargetTransSceneId == WorldMapController.Instance.ToSceneId)
            {
                //这个传送点是我们要走的
                //让主角移动到传送点
                GlobalInit.Instance.MainPlayer.MoveTo(item.Value.transform.position);
            }
        }
        WorldMapController.Instance.CurrSceneId = WorldMapController.Instance.ToSceneId;
        if (WorldMapController.Instance.SceneIdQueue.Count > 0)
        {
            WorldMapController.Instance.ToSceneId = WorldMapController.Instance.SceneIdQueue.Dequeue();
        }
    }
    #endregion

    #region 每隔1s就向服务器上传主角当前在世界地图场景中的位置信息
    private void SendPlayerPos()//向服务器同步主角位置信息
    {
        if (GlobalInit.Instance != null && GlobalInit.Instance.MainPlayer != null)
        {
            m_WorldMap_PosProto.x = GlobalInit.Instance.MainPlayer.transform.position.x;
            m_WorldMap_PosProto.y = GlobalInit.Instance.MainPlayer.transform.position.y;
            m_WorldMap_PosProto.z = GlobalInit.Instance.MainPlayer.transform.position.z;
            m_WorldMap_PosProto.yAngle = GlobalInit.Instance.MainPlayer.transform.eulerAngles.y;
            NetWorkSocket.Instance.SendMsg(m_WorldMap_PosProto.ToArray());
        }
    }
    #endregion

    #region 客户端发送已经进入世界地图场景消息
    private WorldMap_RoleAlreadyEnterProto m_RoleAlreadyEnterProto = new WorldMap_RoleAlreadyEnterProto();
    private void SendRoleAlreadyEnter(int worldMapSceneId, Vector3 currRolePos, float currRoleYAngle)
    {
        m_AllRoleDic[GlobalInit.Instance.MainPlayer.CurrRoleInfo.RoleId] = GlobalInit.Instance.MainPlayer;//将自己加入当前场景的角色字典

        m_RoleAlreadyEnterProto.TargetWorldMapSceneId = worldMapSceneId;
        m_RoleAlreadyEnterProto.RolePosX = currRolePos.x;
        m_RoleAlreadyEnterProto.RolePosY = currRolePos.y;
        m_RoleAlreadyEnterProto.RolePosZ = currRolePos.z;
        m_RoleAlreadyEnterProto.RoleYAngle = currRoleYAngle;

        NetWorkSocket.Instance.SendMsg(m_RoleAlreadyEnterProto.ToArray());
    }
    #endregion

    private void AddEventListener()
    {
        //服务器返回当前场景中的其他玩家
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_InitRole, OnWorldMap_InitRole);

        //服务器广播有其他玩家进入了场景
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleEnter, OnWorldMap_OtherRoleEnter);
        //服务器广播有其他玩家离开了场景
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleLeave, OnWorldMap_OtherRoleLeave);
        //服务器广播有其他玩家移动
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleMove, OnWorldMap_OtherRoleMove);
        //服务器广播有其他玩家使用技能
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleUseSkill, OnWorldMap_OtherRoleUseSkill);
        //服务器广播其他玩家死亡
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleDie, OnWorldMap_OtherRoleDie);
        //服务器广播其他玩家复活
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleResurgence, OnWorldMap_OtherRoleResurgence);
        //服务器广播其他玩家信息更新
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleUpdateInfo, OnWorldMap_OtherRoleUpdateInfo);
    }
    
    /// <summary>
    /// 初始化场景中已经存在的其他玩家
    /// </summary>
    private void OnWorldMap_InitRole(byte[] buffer)
    {
        WorldMap_InitRoleProto proto = WorldMap_InitRoleProto.GetProto(buffer);
        int roleCount = proto.RoleCount;
        Debug.Log("other role count=" + roleCount);

        List<WorldMap_InitRoleProto.RoleItem> list = proto.ItemList;
        if (list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            int roleId = list[i].RoleId;
            string roleNickName = list[i].RoleNickName;
            int roleLevel = list[i].RoleLevel;
            int roleJobId = list[i].RoleJobId;
            Vector3 rolePos = new Vector3(list[i].RolePosX, list[i].RolePosY, list[i].RolePosZ);
            float roleYAngle = list[i].RoleYAngle;
            int maxHP = list[i].RoleMaxHP;
            int currHP = list[i].RoleCurrHP;
            int maxMP = list[i].RoleMaxMP;
            int currMP = list[i].RoleCurrMP;
            Debug.Log("init scene role currhp==="+currHP);

            CreateOtherPlayer(roleId, roleNickName, roleLevel, roleJobId, rolePos, roleYAngle, maxHP, currHP, maxMP, currMP);
        }
    }

    /// <summary>
    /// 服务器广播同场景中有其他玩家进入
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_OtherRoleEnter(byte[] buffer)
    {
        WorldMap_OtherRoleEnterProto proto = WorldMap_OtherRoleEnterProto.GetProto(buffer);

        int roleId = proto.RoleId;
        string roleNickName = proto.RoleNickName;
        int roleLevel = proto.RoleLevel;
        int roleJobId = proto.RoleJobId;
        Vector3 rolePos = new Vector3(proto.RolePosX, proto.RolePosY, proto.RolePosZ);
        float roleYAngle = proto.RoleYAngle;
        int maxHP = proto.RoleMaxHP;
        int currHP = proto.RoleCurrHP;
        int maxMP = proto.RoleMaxMP;
        int currMP = proto.RoleCurrMP;

        CreateOtherPlayer(roleId, roleNickName, roleLevel, roleJobId, rolePos, roleYAngle, maxHP, currHP, maxMP, currMP);
    }

    /// <summary>
    /// 服务器广播通常经中有其他玩家离开
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_OtherRoleLeave(byte[] buffer)
    {
        WorldMap_OtherRoleLeaveProto proto = WorldMap_OtherRoleLeaveProto.GetProto(buffer);
        int leaveRoleId = proto.RoleId;
        DestroyOtherRole(leaveRoleId);
    }

    /// <summary>
    /// 服务器广播同场景中有其他玩家移动
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_OtherRoleMove(byte[] buffer)
    {
        WorldMap_OtherRoleMoveProto proto = WorldMap_OtherRoleMoveProto.GetProto(buffer);
        int roleId = proto.RoleId;
        Vector3 targetPos = new Vector3(proto.TargetPosX, proto.TargetPosY, proto.TargetPosZ);
        long serverTime = proto.ServerTime;
        int needTime = proto.NeedTime;

        if (m_AllRoleDic.ContainsKey(roleId))
        {
            //m_AllRoleDic[roleId].MoveTo(targetPos);
            ((OtherRoleAI)m_AllRoleDic[roleId].CurrRoleAI).MoveTo(targetPos, serverTime, needTime);
        }
    }

    /// <summary>
    /// 服务器广播其他玩家使用技能
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_OtherRoleUseSkill(byte[] buffer)
    {
        WorldMap_OtherRoleUseSkillProto proto = WorldMap_OtherRoleUseSkillProto.GetProto(buffer);

        //1.处理攻击者
        //让攻击者使用技能
        //如果攻击者存在
        if (m_AllRoleDic.ContainsKey(proto.AttackRoleId))
        {
            RoleCtrl attackRole = m_AllRoleDic[proto.AttackRoleId];

            //修正攻击者的位置
            attackRole.transform.position = new Vector3(proto.RolePosX, proto.RolePosY, proto.RolePosZ);
            attackRole.transform.eulerAngles = new Vector3(0, proto.RoleYAngle, 0);

            //攻击者表演攻击
            attackRole.PlayAttack(proto.SkillId);
        }

        //2.处理被攻击者
        if (proto.ItemList != null && proto.ItemList.Count > 0)
        {
            for (int i = 0; i < proto.ItemList.Count; i++)
            {
                WorldMap_OtherRoleUseSkillProto.BeAttackItem item = proto.ItemList[i];//拿到被攻击者
                if (m_AllRoleDic.ContainsKey(item.BeAttackRoleId))
                {
                    RoleCtrl beAttackRole = m_AllRoleDic[item.BeAttackRoleId];

                    RoleTransferAttackInfo attackInfo = new RoleTransferAttackInfo();
                    attackInfo.AttackRoleId = proto.AttackRoleId;//发起攻击者编号
                    attackInfo.AttackRolePos = Vector3.zero;//攻击者的位置
                    attackInfo.BeAttackedRoleId = item.BeAttackRoleId;//被攻击者编号
                    attackInfo.HurtValue = item.ReduceHp;//伤害值
                    attackInfo.SkillId = proto.SkillId;//攻击者使用的技能编号
                    attackInfo.SkillLevel = proto.SkillLevel;//攻击者使用的技能等级
                    attackInfo.IsAbnormal = false;//是否附加异常状态
                    attackInfo.IsCri = item.IsCri == 1;//是否暴击

                    //给被攻击者传递攻击信息
                    beAttackRole.ToHurt(attackInfo);
                }
            }
        }
    }

    private void OnWorldMap_OtherRoleDie(byte[] buffer)
    {
        WorldMap_OtherRoleDieProto proto = WorldMap_OtherRoleDieProto.GetProto(buffer);
        if (proto.RoleIdList != null && proto.RoleIdList.Count > 0)
        {
            for (int i = 0; i < proto.RoleIdList.Count; i++)
            {
                int dieRoleId = proto.RoleIdList[i];
                
                if (m_AllRoleDic.ContainsKey(dieRoleId))
                {
                    m_AllRoleDic[dieRoleId].CurrRoleInfo.CurrHP = 0;
                    m_AllRoleDic[dieRoleId].ToDie();

                    if (m_AllRoleDic[dieRoleId].CurrRoleType == RoleType.MainPlayer)
                    {
                        WorldMapController.Instance.EnemyNickName = m_AllRoleDic[proto.AttackRoleId].CurrRoleInfo.RoleNickName;
                        WorldMapController.Instance.OpenView(WindowUIType.WorldMapFail);
                        Debug.Log("<color=green>other die===" + m_AllRoleDic[dieRoleId].CurrRoleInfo.RoleNickName + "</color>");
                    }
                }
            }
        }
    }

    private void OnWorldMap_OtherRoleResurgence(byte[] buffer)
    {
        WorldMap_OtherRoleResurgenceProto proto = WorldMap_OtherRoleResurgenceProto.GetProto(buffer);
        if (m_AllRoleDic.ContainsKey(proto.RoleId))
        {
            m_AllRoleDic[proto.RoleId].RoleReborn();
        }
    }

    private void OnWorldMap_OtherRoleUpdateInfo(byte[] buffer)
    {

    }

    #region 场景中所有角色的管理（实例化与销毁）
    
    private void CreateOtherPlayer(int roleId, string roleNickName, int roleLevel, int roleJobId, Vector3 rolePos, float roleYAngle, int maxHP, int currHP, int maxMP, int currMP)
    {
        RoleCtrl roleCtrl = RoleMgr.Instance.LoadOtherRole(roleId, roleNickName, roleLevel, roleJobId, maxHP, currHP, maxMP, currMP);

        roleCtrl.Born(rolePos);
        roleCtrl.transform.eulerAngles = new Vector3(0, roleYAngle, 0);

        m_AllRoleDic[roleId] = roleCtrl;
    }

    private void DestroyOtherRole(int roleId)
    {
        if (m_AllRoleDic.ContainsKey(roleId))
        {
            Destroy(m_AllRoleDic[roleId].gameObject);
            m_AllRoleDic.Remove(roleId);
        }
    }
    #endregion

    private void RemoveEventListener()
    {
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_InitRole, OnWorldMap_InitRole);

        //服务器广播有其他玩家进入了场景
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleEnter, OnWorldMap_OtherRoleEnter);
        //服务器广播有其他玩家离开了场景
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleLeave, OnWorldMap_OtherRoleLeave);
        //服务器广播有其他玩家移动
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleMove, OnWorldMap_OtherRoleMove);
        //服务器广播有其他玩家使用技能
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleUseSkill, OnWorldMap_OtherRoleUseSkill);
        //服务器广播其他玩家死亡
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleDie, OnWorldMap_OtherRoleDie);
        //服务器广播其他玩家复活
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleResurgence, OnWorldMap_OtherRoleResurgence);
        //服务器广播其他玩家信息更新
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleUpdateInfo, OnWorldMap_OtherRoleUpdateInfo);
    }
}
