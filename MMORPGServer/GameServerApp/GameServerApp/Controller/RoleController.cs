using GameServerApp;
using Mmcoy.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class RoleController : Singleton<RoleController>
{
    public void Init()
    {
        //添加协议监听

        #region 系统相关
        //客户端发送客户端本地时间
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.System_SendLocalTime, OnSystem_SendLocalTime);
        #endregion

        #region 登录游戏服相关
        //登录区服消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_LogOnGameServer, OnLogOnGameServer);
        //创建角色消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_CreateRole, OnCreateRole);
        //删除角色消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_DeleteRole, OnDeleteRole);
        //进入游戏消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_EnterGame, OnEnterGame);
        //角色信息详情消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_SelectRoleInfo, OnSelectRoleInfo);
        #endregion

        #region 游戏关卡相关
        //进入关卡消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_Enter, OnGameLevel_Enter);
        //关卡胜利消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_Victory, OnGameLevel_Victory);
        //关卡失败消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_Fail, OnGameLevel_Fail);
        //关卡复活消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_Resurgence, OnGameLevel_Resurgence);
        #endregion

        #region 世界地图相关
        //进入世界地图场景消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_RoleEnter, OnWorldMap_RoleEnter);
        //自身坐标消息
        EventDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_Pos, OnWorldMap_Pos);
        #endregion
    }

    #region 和客户端对表
    private void OnSystem_SendLocalTime(Role role, byte[] buffer)
    {
        System_SendLocalTimeProto proto = System_SendLocalTimeProto.GetProto(buffer);
        float clientLocalTime = proto.LocalTime;

        SystemServerTimeReturn(role, clientLocalTime);
    }

    private void SystemServerTimeReturn(Role role, float clientLocalTime)
    {
        System_ServerTimeReturnProto retProto = new System_ServerTimeReturnProto();

        retProto.LocalTime = clientLocalTime;
        retProto.ServerTime = DateTime.Now.ToUniversalTime().Ticks / 10000;

        role.Client_Socket.SendMsg(retProto.ToArray());
    }
    #endregion

    #region 角色跳转世界地图场景请求处理
    private void OnWorldMap_RoleEnter(Role role, byte[] buffer)
    {
        WorldMap_RoleEnterProto proto = WorldMap_RoleEnterProto.GetProto(buffer);
        role.LastInWorldMapId = proto.WorldMapSceneId;

        WorldMap_RoleEnterReturnProto retProto = new WorldMap_RoleEnterReturnProto();
        retProto.IsSuccess = true;
        role.Client_Socket.SendMsg(retProto.ToArray());
    }

    private void OnWorldMap_Pos(Role role, byte[] buffer)
    {
        WorldMap_PosProto proto = WorldMap_PosProto.GetProto(buffer);
        role.LastInWorldMapPos = string.Format("{0}_{1}_{2}_{3}", proto.x, proto.y, proto.z, proto.yAngle);
    }
    #endregion

    #region 游戏关卡相关
    private void OnGameLevel_Resurgence(Role role, byte[] buffer)
    {
        GameLevel_ResurgenceReturnProto retProto = new GameLevel_ResurgenceReturnProto();
        retProto.IsSuccess = true;
        role.Client_Socket.SendMsg(retProto.ToArray());
    }

    private void OnGameLevel_Fail(Role role, byte[] buffer)
    {
        //如果有必要 需要验证体力 这里省略
        GameLevel_FailProto proto = GameLevel_FailProto.GetProto(buffer);

        //添加攻打关卡记录 在数据库里增加一条通关失败记录
        Log_GameLevelEntity m_Log_GameLevelEntity = new Log_GameLevelEntity();
        m_Log_GameLevelEntity.Status = Mmcoy.Framework.AbstractBase.EnumEntityStatus.Released;
        m_Log_GameLevelEntity.RoleId = role.RoleId;
        m_Log_GameLevelEntity.GameLevelId = proto.GameLevelId;
        m_Log_GameLevelEntity.Grade = proto.Grade;
        m_Log_GameLevelEntity.Action = 2;
        m_Log_GameLevelEntity.CreateTime = DateTime.Now;
        Log_GameLevelCacheModel.Instance.Create(m_Log_GameLevelEntity);

        GameLevel_FailReturnProto retProto = new GameLevel_FailReturnProto();
        retProto.IsSuccess = true;
        role.Client_Socket.SendMsg(retProto.ToArray());
    }

    private void OnGameLevel_Victory(Role role, byte[] buffer)
    {
        GameLevel_VictoryProto proto = GameLevel_VictoryProto.GetProto(buffer);

        //1.添加或者修改玩家过关详情
        int count = Role_PassGameLevelDetailCacheModel.Instance.GetCount(string.Format("[RoleId]={0} AND [GameLevelId]={1}", role.RoleId, proto.GameLevelId));
        if (count > 0)
        {
            //修改
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["@RoleId"] = role.RoleId;
            parameters["@GameLevelId"] = proto.GameLevelId;
            parameters["@Star"] = proto.Star;

            Role_PassGameLevelDetailCacheModel.Instance.Update("[Star]=@Star", "[RoleId]=@RoleId AND [GameLevelId]=@GameLevelId", parameters);
        }
        else
        {
            Role_PassGameLevelDetailEntity entity = new Role_PassGameLevelDetailEntity();
            entity.Status = Mmcoy.Framework.AbstractBase.EnumEntityStatus.Released;
            entity.RoleId = role.RoleId;
            entity.GameLevelId = proto.GameLevelId;
            entity.Grade = proto.Grade;
            entity.Star = proto.Star;
            entity.IsMopUp = 0;
            entity.CreateTime = DateTime.Now;

            Role_PassGameLevelDetailCacheModel.Instance.Create(entity);
        }

        //2.添加攻打关卡记录
        {
            Log_GameLevelEntity m_Log_GameLevelEntity = new Log_GameLevelEntity();
            m_Log_GameLevelEntity.Status = Mmcoy.Framework.AbstractBase.EnumEntityStatus.Released;
            m_Log_GameLevelEntity.RoleId = role.RoleId;
            m_Log_GameLevelEntity.GameLevelId = proto.GameLevelId;
            m_Log_GameLevelEntity.Grade = proto.Grade;
            m_Log_GameLevelEntity.Action = 1;
            m_Log_GameLevelEntity.CreateTime = DateTime.Now;
            Log_GameLevelCacheModel.Instance.Create(m_Log_GameLevelEntity); 
        }

        //3.添加杀怪记录
        List<GameLevel_VictoryProto.MonsterItem> killMonsterList = proto.KillMonsterList;
        for (int i = 0; i < killMonsterList.Count; i++)
        {
            Log_KillMonsterEntity m_Log_KillMonsterEntity = new Log_KillMonsterEntity();
            m_Log_KillMonsterEntity.Status = Mmcoy.Framework.AbstractBase.EnumEntityStatus.Released;
            m_Log_KillMonsterEntity.RoleId = role.RoleId;
            m_Log_KillMonsterEntity.PlayType = 0;
            m_Log_KillMonsterEntity.PlaySceneId = proto.GameLevelId;
            m_Log_KillMonsterEntity.Grade = proto.Grade;
            m_Log_KillMonsterEntity.SpriteId = killMonsterList[i].MonsterId;
            m_Log_KillMonsterEntity.SpriteCount = killMonsterList[i].MonsterCount;
            m_Log_KillMonsterEntity.CreateTime = DateTime.Now;

            Log_KillMonsterCacheModel.Instance.Create(m_Log_KillMonsterEntity);
        }

        //4.添加获得物品记录
        List<GameLevel_VictoryProto.GoodsItem> getGoodsList = proto.GetGoodsList;
        for (int i = 0; i < getGoodsList.Count; i++)
        {
            Log_ReceiveGoodsEntity m_Log_ReceiveGoodsEntity = new Log_ReceiveGoodsEntity();
            m_Log_ReceiveGoodsEntity.Status = Mmcoy.Framework.AbstractBase.EnumEntityStatus.Released;
            m_Log_ReceiveGoodsEntity.RoleId = role.RoleId;
            m_Log_ReceiveGoodsEntity.PlayType = 0;
            m_Log_ReceiveGoodsEntity.PlaySceneId = proto.GameLevelId;
            m_Log_ReceiveGoodsEntity.Grade = proto.Grade;
            m_Log_ReceiveGoodsEntity.GoodsType = getGoodsList[i].GoodsType;
            m_Log_ReceiveGoodsEntity.GoodsId = getGoodsList[i].GoodsId;
            m_Log_ReceiveGoodsEntity.GoodsCount = getGoodsList[i].GoodsCount;
            m_Log_ReceiveGoodsEntity.CreateTime = DateTime.Now;

            Log_ReceiveGoodsCacheModel.Instance.Create(m_Log_ReceiveGoodsEntity);
        }

        //给玩家添加经验和金币 设置最后通关的游戏关卡Id
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["@Id"] = role.RoleId;
            parameters["@Exp"] = proto.Exp;
            parameters["@Gold"] = proto.Gold;
            parameters["@LastPassGameLevelId"] = proto.GameLevelId;

            Role_PassGameLevelDetailCacheModel.Instance.Update("[Exp]=[Exp]+@Exp, [Gold]=[Gold]+@Gold, [LastPassGameLevelId]=@LastPassGameLevelId",
                "[Id]=@Id", parameters);
        }

        GameLevel_VictoryReturnProto retProto = new GameLevel_VictoryReturnProto();
        retProto.IsSuccess = true;
        role.Client_Socket.SendMsg(retProto.ToArray());
    }

    private void OnGameLevel_Enter(Role role, byte[] buffer)
    {
        //如果有必要 需要验证体力 这里省略
        GameLevel_EnterProto proto = GameLevel_EnterProto.GetProto(buffer);

        //添加攻打关卡记录
        Log_GameLevelEntity m_Log_GameLevelEntity = new Log_GameLevelEntity();
        m_Log_GameLevelEntity.Status = Mmcoy.Framework.AbstractBase.EnumEntityStatus.Released;
        m_Log_GameLevelEntity.RoleId = role.RoleId;
        m_Log_GameLevelEntity.GameLevelId = proto.GameLevelId;
        m_Log_GameLevelEntity.Grade = proto.Grade;
        m_Log_GameLevelEntity.Action = 0;
        m_Log_GameLevelEntity.CreateTime = DateTime.Now;
        Log_GameLevelCacheModel.Instance.Create(m_Log_GameLevelEntity);

        GameLevel_EnterReturnProto retProto = new GameLevel_EnterReturnProto();
        retProto.IsSuccess = true;
        role.Client_Socket.SendMsg(retProto.ToArray());
    }
    #endregion


    #region 进入游戏消息处理
    private void OnEnterGame(Role role, byte[] buffer)
    {
        RoleOperation_EnterGameProto proto = RoleOperation_EnterGameProto.GetProto(buffer);
        role.RoleId = proto.RoleId;

        EnterGameReturn(role);
    }

    private void EnterGameReturn(Role role)
    {
        RoleOperation_EnterGameReturnProto proto = new RoleOperation_EnterGameReturnProto();

        proto.IsSuccess = true;

        role.Client_Socket.SendMsg(proto.ToArray());

        OnSelectRoleInfoReturn(role);//向客户端返回角色信息详情
        OnRoleData_SkillReturn(role);//向客户端返回角色已经学会的技能列表
    }
    #endregion

    #region 客户端发送登录区服消息处理
    /// <summary>
    /// 客户端发送登录区服消息
    /// </summary>
    /// <param name="role"></param>
    /// <param name="buffer"></param>
    private void OnLogOnGameServer(Role role, byte[] buffer)
    {
        RoleOperation_LogOnGameServerProto proto = RoleOperation_LogOnGameServerProto.GetProto(buffer);
        int accountId = proto.AccountId;
        role.AccountId = accountId;
        LogOnGameServerReturn(role, accountId);
    }

    private void LogOnGameServerReturn(Role role, int accountId)
    {
        RoleOperation_LogOnGameServerReturnProto proto = new RoleOperation_LogOnGameServerReturnProto();

        //根据玩家账号 查询玩家角色
        List<RoleEntity> list = RoleCacheModel.Instance.GetList(condition: string.Format("AccountId={0}", accountId));
        if (list != null && list.Count > 0)
        {
            proto.RoleCount = list.Count;
            proto.RoleList = new List<RoleOperation_LogOnGameServerReturnProto.RoleItem>();
            for (int i = 0; i < list.Count; i++)
            {
                RoleOperation_LogOnGameServerReturnProto.RoleItem item = new RoleOperation_LogOnGameServerReturnProto.RoleItem();
                item.RoleId = list[i].Id.Value;
                item.RoleNickName = list[i].NickName;
                item.RoleLevel = list[i].Level;
                item.RoleJob = (byte)list[i].JobId;
                proto.RoleList.Add(item);
            }
        }
        else
        {
            proto.RoleCount = 0;
        }

        role.Client_Socket.SendMsg(proto.ToArray());
    }
    #endregion

    #region 创建角色消息处理
    /// <summary>
    /// 客户端发送创建角色消息
    /// </summary>
    private void OnCreateRole(Role role, byte[] buffer)
    {
        RoleOperation_CreateRoleProto proto = RoleOperation_CreateRoleProto.GetProto(buffer);

        RoleEntity entity = new RoleEntity();
        entity.Status = Mmcoy.Framework.AbstractBase.EnumEntityStatus.Released;

        entity.AccountId = role.AccountId;
        entity.JobId = proto.JobId;
        entity.NickName = proto.RoleNickName;
        entity.Sex = 0;
        entity.Level = 1;
        entity.TotalRechargeMoney = 0;
        entity.Money = 0;
        entity.Gold = 0;
        entity.Exp = 0;

        JobEntity jobEntity = JobDBModel.Instance.GetEntity(entity.JobId);
        JobLevelEntity jobLevelEntity = JobLevelDBModel.Instance.GetEntity(entity.Level);

        entity.MaxHP = jobLevelEntity.HP;
        entity.CurrHP = jobLevelEntity.HP;
        entity.MaxMP = jobLevelEntity.MP;
        entity.CurrMP = jobLevelEntity.MP;
        entity.Attack = (int)Math.Round(jobEntity.Attack * jobLevelEntity.Attack * 0.01f);
        entity.Defense = (int)Math.Round(jobEntity.Defense * jobLevelEntity.Defense * 0.01f);
        entity.Hit = (int)Math.Round(jobEntity.Hit * jobLevelEntity.Hit * 0.01f);
        entity.Dodge = (int)Math.Round(jobEntity.Dodge * jobLevelEntity.Dodge * 0.01f);
        entity.Cri = (int)Math.Round(jobEntity.Cri * jobLevelEntity.Cri * 0.01f);
        entity.Res = (int)Math.Round(jobEntity.Res * jobLevelEntity.Res * 0.01f);
        entity.Fighting = entity.Attack * 4 + entity.Defense * 4 + entity.Hit * 2 + entity.Dodge * 2 + entity.Cri + entity.Res;

        entity.LastPassGameLevelId = 0;
        entity.LastInWorldMapId = 1;//角色最后所在的世界地图场景Id
        entity.LastInWorldMapPos = "0_0_0_0";//角色最后所在的世界地图场景中的位置

        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;

        entity.Equip_Weapon = 0;
        entity.Equip_Clothes = 0;
        entity.Equip_Belt = 0;
        entity.Equip_Pants = 0;
        entity.Equip_Shoe = 0;
        entity.Equip_Necklace = 0;
        entity.Equip_Cuff = 0;
        entity.Equip_Ring = 0;

        MFReturnValue<object> mfr = null;
        //1.查询昵称是否存在
        int count = RoleCacheModel.Instance.GetCount(string.Format("NickName='{0}'", proto.RoleNickName));
        if (count == 0)
        {
            mfr = RoleCacheModel.Instance.Create(entity);
            int roleId = mfr.GetOutputValue<int>("Id");
            role.RoleId = roleId;
            role.NickName = entity.NickName;
            role.JobId = entity.JobId;
            role.Level = entity.Level;
            role.MaxHP = entity.MaxHP;
            role.MaxMP = entity.MaxMP;
            role.CurrHP = entity.CurrHP;
            role.CurrMP = entity.CurrMP;
            role.Attack = entity.Attack;
            role.Defense = entity.Defense;
            role.Hit = entity.Hit;
            role.Dodge = entity.Dodge;
            role.Cri = entity.Cri;
            role.Res = entity.Res;
            role.Fighting = entity.Fighting;
        }
        else
        {
            mfr = new MFReturnValue<object>();
            mfr.HasError = true;
            mfr.ReturnCode = 1000;
        }
        OnCreateRoleReturn(role, mfr);
    }

    /// <summary>
    /// 服务器返回创建角色消息
    /// </summary>
    private void OnCreateRoleReturn(Role role, MFReturnValue<object> mfr)
    {
        RoleOperation_CreateRoleReturnProto proto = new RoleOperation_CreateRoleReturnProto();
        if (!mfr.HasError)
        {
            proto.IsSuccess = true;
            //角色创建成功 向客户端返回角色详情信息 角色已经学会的技能信息
            OnSelectRoleInfoReturn(role);
            OnRoleData_SkillReturn(role);
        }
        else
        {
            proto.IsSuccess = false;
            proto.MsgCode = 1000;
        }

        role.Client_Socket.SendMsg(proto.ToArray());
        
    }
    #endregion

    #region 删除角色消息处理
    private void OnDeleteRole(Role role, byte[] buffer)
    {
        RoleOperation_DeleteRoleProto proto = RoleOperation_DeleteRoleProto.GetProto(buffer);
        MFReturnValue<object> mfr = RoleCacheModel.Instance.Delete(proto.RoleId);
        OnDeleteRoleReturn(role, mfr);
    }

    private void OnDeleteRoleReturn(Role role, MFReturnValue<object> mfr)
    {
        RoleOperation_DeleteRoleReturnProto proto = new RoleOperation_DeleteRoleReturnProto();
        if (!mfr.HasError)
        {
            proto.IsSuccess = true;
        }
        else
        {
            proto.IsSuccess = false;
            proto.MsgCode = 1000;
        }
        role.Client_Socket.SendMsg(proto.ToArray());
    }
    #endregion

    #region 角色详细信息
    private void OnSelectRoleInfo(Role role, byte[] buffer)
    {
        RoleOperation_SelectRoleInfoProto proto = RoleOperation_SelectRoleInfoProto.GetProto(buffer);
        role.RoleId = proto.RoldId;
        OnSelectRoleInfoReturn(role);
    }

    private void OnSelectRoleInfoReturn(Role role)
    {
        RoleOperation_SelectRoleInfoReturnProto proto = new RoleOperation_SelectRoleInfoReturnProto();

        RoleEntity entity = RoleCacheModel.Instance.GetEntity(role.RoleId);
        if (entity != null)
        {
            proto.IsSuccess = true;

            proto.RoldId = entity.Id.Value;
            proto.RoleNickName = entity.NickName;
            proto.JobId = (byte)entity.JobId;
            proto.Level = entity.Level;
            proto.TotalRechargeMoney = entity.TotalRechargeMoney;
            proto.Money = entity.Money;
            proto.Gold = entity.Gold;
            proto.Exp = entity.Exp;

            proto.MaxHP = entity.MaxHP;
            proto.MaxMP = entity.MaxMP;
            proto.CurrHP = entity.CurrHP;
            proto.CurrMP = entity.CurrMP;
            proto.Attack = entity.Attack;
            proto.Defense = entity.Defense;
            proto.Hit = entity.Hit;
            proto.Dodge = entity.Dodge;
            proto.Cri = entity.Cri;
            proto.Res = entity.Res;
            proto.Fighting = entity.Fighting = entity.Attack * 4 + entity.Defense * 4 + entity.Hit * 2 + entity.Dodge * 2 + entity.Cri + entity.Res;

            proto.LastInWorldMapId = entity.LastInWorldMapId;
            proto.LastInWorldMapPos = entity.LastInWorldMapPos;//协议的string类型的字段一定要赋值 否则服务器会报错

            //给服务器的role对象缓存赋值 频繁使用的字段避免反复读取数据库
            role.NickName = entity.NickName;
            role.JobId = entity.JobId;
            role.Level = entity.Level;
            role.MaxHP = entity.MaxHP;
            role.MaxMP = entity.MaxMP;
            role.CurrHP = entity.CurrHP;
            role.CurrMP = entity.CurrMP;
            role.Attack = entity.Attack;
            role.Defense = entity.Defense;
            role.Hit = entity.Hit;
            role.Dodge = entity.Dodge;
            role.Cri = entity.Cri;
            role.Res = entity.Res;
            role.Fighting = entity.Fighting;
            role.PreviousWorldMapId = entity.LastInWorldMapId;
            role.LastInWorldMapId = entity.LastInWorldMapId;
            role.LastInWorldMapPos = entity.LastInWorldMapPos;
        }
        else
        {
            proto.IsSuccess = false;
            proto.MsgCode = 1000;
        }
        role.Client_Socket.SendMsg(proto.ToArray());
    }
    #endregion

    #region 角色学会的技能
    /// <summary>
    /// 服务器返回角色学会的技能信息
    /// </summary>
    /// <param name="role"></param>
    private void OnRoleData_SkillReturn(Role role)
    {
        RoleData_SkillReturnProto proto = new RoleData_SkillReturnProto();

        List<RoleSkillEntity> list = RoleSkillCacheModel.Instance.GetList(condition:"RoleId="+role.RoleId);

        if (list != null)
        {
            proto.SkillCount = (byte)list.Count;
            proto.CurrSkillDataList = new List<RoleData_SkillReturnProto.SkillData>();

            for (int i = 0; i < list.Count; i++)
            {
                proto.CurrSkillDataList.Add(new RoleData_SkillReturnProto.SkillData() { 
                    SkillId = list[i].SkillId,
                    SkillLevel = list[i].SkillLevel,
                    SlotsNo = list[i].SlotsNo
                });
            }
        }
        


        role.Client_Socket.SendMsg(proto.ToArray());
    }
    #endregion

    public override void Dispose()
    {
        base.Dispose();
        //移除监听

        //登录区服消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_LogOnGameServer, OnLogOnGameServer);
        //创建角色消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_CreateRole, OnCreateRole);
        //删除角色消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_DeleteRole, OnDeleteRole);
        //进入游戏消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_EnterGame, OnEnterGame);
        //角色详情消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_SelectRoleInfo, OnSelectRoleInfo);
        //进入关卡消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.GameLevel_Enter, OnGameLevel_Enter);
        //关卡胜利消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.GameLevel_Victory, OnGameLevel_Victory);
        //关卡失败消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.GameLevel_Fail, OnGameLevel_Fail);
        //关卡复活消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.GameLevel_Resurgence, OnGameLevel_Resurgence);
        //进入世界地图场景消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_RoleEnter, OnWorldMap_RoleEnter);
        //自身坐标消息
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_Pos, OnWorldMap_Pos);
        //客户端发送客户端本地时间
        EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.System_SendLocalTime, OnSystem_SendLocalTime);
    }
}
