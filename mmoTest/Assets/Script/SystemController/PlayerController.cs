using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 使用主角数据来刷新主城UI RoleData-->Controller<--UIView
/// </summary>
public class PlayerController : SystemControllerBase<PlayerController>, ISystemController
{

    private UIRoleInfoView m_UIRoleInfoView;//角色详情窗口

    private RoleInfoMainPlayer m_RoleInfoMainPlayer;

    public int LastInWorldMapId;
    public string LastInWorldMapPos;

    public PlayerController()
    { 
        
    }

    #region 系统控制器接口方法实现
    /// <summary>
    /// 打开角色详情窗口 并给窗口传递数据
    /// </summary>
    private void OpenRoleInfoView()
    {
        m_UIRoleInfoView = UIViewUtil.Instance.OpenWindow(WindowUIType.RoleInfo).GetComponent<UIRoleInfoView>();

        TransferData data = new TransferData();
        data.SetValue<int>(ConstDefine.JobId, GlobalInit.Instance.MainPlayerInfo.JobId);
        data.SetValue<string>(ConstDefine.NickName, GlobalInit.Instance.MainPlayerInfo.RoleNickName);
        data.SetValue<int>(ConstDefine.Level, GlobalInit.Instance.MainPlayerInfo.Level);
        data.SetValue<int>(ConstDefine.Fighting, GlobalInit.Instance.MainPlayerInfo.Fighting);

        data.SetValue<int>(ConstDefine.Money, GlobalInit.Instance.MainPlayerInfo.Money);
        data.SetValue<int>(ConstDefine.Gold, GlobalInit.Instance.MainPlayerInfo.Gold);

        data.SetValue<int>(ConstDefine.MAXHP, GlobalInit.Instance.MainPlayerInfo.MaxHP);
        data.SetValue<int>(ConstDefine.CurrHP, GlobalInit.Instance.MainPlayerInfo.CurrHP);
        data.SetValue<int>(ConstDefine.MAXMP, GlobalInit.Instance.MainPlayerInfo.MaxMP);
        data.SetValue<int>(ConstDefine.CurrMP, GlobalInit.Instance.MainPlayerInfo.CurrMP);
        data.SetValue<int>(ConstDefine.MAXEXP, GlobalInit.Instance.MainPlayerInfo.Exp);

        data.SetValue<int>(ConstDefine.Attack, GlobalInit.Instance.MainPlayerInfo.Attack);
        data.SetValue<int>(ConstDefine.Defense, GlobalInit.Instance.MainPlayerInfo.Defense);
        data.SetValue<int>(ConstDefine.Hit, GlobalInit.Instance.MainPlayerInfo.Hit);
        data.SetValue<int>(ConstDefine.Dodge, GlobalInit.Instance.MainPlayerInfo.Dodge);
        data.SetValue<int>(ConstDefine.Cri, GlobalInit.Instance.MainPlayerInfo.Cri);
        data.SetValue<int>(ConstDefine.Res, GlobalInit.Instance.MainPlayerInfo.Res);

        m_UIRoleInfoView.SetRoleInfo(data);
    }

    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.RoleInfo:
                OpenRoleInfoView();
                break;
        }
    }
    #endregion

    /// <summary>
    /// 设置主城常驻UI
    /// </summary>
    public void SetMainCityRoleData()
    {
        SetMainCityRoleInfo();//右上角角色信息UI
        SetMainCityRoleSkillInfo();//左下角角色技能UI
        SetMainCitySmallMap();
    }

    #region 场景UI左上角的角色信息
    /// <summary>
    /// 使用当前登录游戏服角色信息 设置主城角色信息UI
    /// </summary>
    private void SetMainCityRoleInfo()
    {
        RoleInfoMainPlayer roleInfo = GlobalInit.Instance.MainPlayerInfo;
        string headPic = string.Empty;
        JobEntity entity = JobDBModel.Instance.GetEntity(roleInfo.JobId);
        if (entity != null)
        {
            headPic = entity.HeadPic;
        }

        GlobalInit.Instance.MainPlayer.OnHPChange = OnHPChangeCallBack;
        GlobalInit.Instance.MainPlayer.OnMPChange = OnMPChangeCallBack;

        //头像 昵称 等级 元宝 金币 血条 蓝条
        UIMainCityRoleInfoView.Instance.SetUI(headPic, roleInfo.RoleNickName, roleInfo.Level, roleInfo.Money, roleInfo.Gold, roleInfo.CurrHP, roleInfo.MaxHP, roleInfo.CurrMP, roleInfo.MaxMP);
    }

    private void OnHPChangeCallBack(int type)
    {
        UIMainCityRoleInfoView.Instance.SetHP(GlobalInit.Instance.MainPlayerInfo.CurrHP, GlobalInit.Instance.MainPlayerInfo.MaxHP);
    }

    private void OnMPChangeCallBack(int type)
    {
        UIMainCityRoleInfoView.Instance.SetMP(GlobalInit.Instance.MainPlayerInfo.CurrMP, GlobalInit.Instance.MainPlayerInfo.MaxMP);
    }
    #endregion

    #region 场景UI右下角的技能图标
    /// <summary>
    /// 设置角色技能UI
    /// </summary>
    private void SetMainCityRoleSkillInfo()
    {
        RoleInfoMainPlayer roleInfo = GlobalInit.Instance.MainPlayerInfo;

        //服务器保存玩家已经学会的技能列表 技能Id 技能等级 技能槽编号
        List<TransferData> list = new List<TransferData>();
        for (int i = 0; i < roleInfo.SkillList.Count; i++)
        {
            TransferData data = new TransferData();

            data.SetValue(ConstDefine.SkillSlotNo, roleInfo.SkillList[i].SlotsNo);
            data.SetValue(ConstDefine.SkillId, roleInfo.SkillList[i].SkillId);
            data.SetValue(ConstDefine.SkillLevel, roleInfo.SkillList[i].SkillLevel);

            //通过技能Id 从技能表中获取技能图标Icon
            SkillEntity entity = SkillDBModel.Instance.GetEntity(roleInfo.SkillList[i].SkillId);
            if (entity != null)
            {
                data.SetValue(ConstDefine.SkillPic, entity.SkillPic);
            }

            //通过技能Id 技能等级 从技能等级表中获取技能冷却时长
            SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndSkillLevel(roleInfo.SkillList[i].SkillId, roleInfo.SkillList[i].SkillLevel);
            if (skillLevelEntity != null)
            {
                data.SetValue(ConstDefine.SkillCDTime, skillLevelEntity.SkillCDTime);
            }

            list.Add(data);
        }

        UIMainCitySkillView.Instance.SetUI(list, OnSkillClick);
    }
    

    /// <summary>
    /// 点击技能按钮的回调 调用角色发起技能攻击方法 如果技能使用成功 该技能开始CD
    /// </summary>
    /// <param name="skillId"></param>
    public void OnSkillClick(int skillId)
    {
        bool isSuccess = GlobalInit.Instance.MainPlayer.ToAttackBySkillId(RoleAttackType.SkillAttack, skillId);
        if (isSuccess)
        {
            GlobalInit.Instance.MainPlayer.CurrRoleInfo.SetSkillCDEndTime(skillId);//给玩家使用的这个技能设置冷却结束时间=使用技能时刻 + 技能冷却时长
            UIMainCitySkillView.Instance.BeginCD(skillId);
        }
    }
    #endregion

    private void SetMainCitySmallMap()
    {
        if (SceneMgr.Instance.CurrentSceneType == SceneType.WorldMap)
        {
            WorldMapEntity entity = WorldMapDBModel.Instance.GetEntity(SceneMgr.Instance.CurrWorldMapId);
            if (entity != null)
            {
                UIMainCitySmallMapView.Instance.SetUI(entity.SmallMapImg, entity.Name);
            }
        }
        else if (SceneMgr.Instance.CurrentSceneType == SceneType.GameLevel)
        {
            GameLevelEntity entity = GameLevelDBModel.Instance.GetEntity(SceneMgr.Instance.CurrGameLevelId);
            if (entity != null)
            {
                UIMainCitySmallMapView.Instance.SetUI(entity.SmallMapImg, entity.Name);
            }
        }
        
        
    }
}
