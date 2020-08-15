using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstDefine 
{
    //========================== 场景名称 ==========================
    public const string Scene_Init = "Scene_Init";
    public const string Scene_Loading = "Scene_Loading";
    public const string Scene_LogOn = "Scene_LogOn";
    public const string Scene_SelectRole = "Scene_SelectRole";

    //========================== PlayerPrefs键的名称 ==========================
    public const string LogOn_AccountId = "LogOn_AccountId";
    public const string LogOn_AccountUserName = "LogOn_AccountUserName";
    public const string LogOn_AccountPwd = "LogOn_AccountPwd";

    //========================== UI事件名称 ==========================
    public const string UILogOnView_BtnLogOn = "UILogOnView_BtnLogOn";
    public const string UILogOnView_BtnToReg = "UILogOnView_BtnToReg";

    public const string UIRegView_BtnReg = "UIRegView_BtnReg";
    public const string UIRegView_BtnBackLogin = "UIRegView_BtnBackLogin";

    public const string UIGameServerEnterView_BtnSelectGameServer = "UIGameServerEnterView_BtnSelectGameServer";
    public const string UIGameServerEnterView_BtnEnterGame = "UIGameServerEnterView_BtnEnterGame";

    //==========================属性名称术语===============================
    public const string JobId = "JobId";
    public const string NickName = "NickName";
    public const string Level = "Level";
    public const string Fighting = "Fighting";

    public const string Money = "Money";
    public const string Gold = "Gold";

    public const string MAXHP = "MAXHP";
    public const string CurrHP = "CurrHP";
    public const string MAXMP = "MAXMP";
    public const string CurrMP = "CurrMP";
    public const string MAXEXP = "MAXEXP";
    public const string CurrEXP = "CurrEXP";

    public const string Attack = "Attack";
    public const string Defense = "Defense";
    public const string Hit = "Hit";
    public const string Dodge = "Dodge";
    public const string Cri = "Cri";
    public const string Res = "Res";

    //=================================剧情关卡======================================
    public const string ChapterId = "ChapterId";
    public const string ChapterName = "ChapterName";
    public const string ChapterMapPic = "ChapterMapPic";
    public const string GameLevelList = "GameLevelList";

    public const string GameLevel_Id = "GameLevel_Id";
    public const string GameLevel_Name = "GameLevel_Name";
    public const string GameLevel_Ico = "GameLevel_Ico";
    public const string GameLevel_Position = "GameLevel_Position";
    public const string GameLevel_SceneName = "GameLevel_SceneName";
    public const string GameLevel_IsBoss = "GameLevel_IsBoss";
    public const string GameLevel_DlgPic = "GameLevel_DlgPic";
    

    //======== 关卡详情 ========
    public const string GameLevelGrade_Gold = "GameLevelGrade_Gold";
    public const string GameLevelGrade_Exp = "GameLevelGrade_Exp";
    public const string GameLevelGrade_Desc = "GameLevelGrade_Desc";
    public const string GameLevelGrade_ConditionDesc = "GameLevelGrade_ConditionDesc";
    public const string GameLevelGrade_CommendFighting = "GameLevelGrade_CommendFighting";
    public const string GameLevelGrade_RewardList = "GameLevelGrade_RewardList";

    public const string GoodsId = "GoodsId";
    public const string GoodsName = "GoodsName";
    public const string GoodsType = "GoodsType";

    //======== 技能相关 ========
    public const string SkillSlotNo = "SkillSlotNo";
    public const string SkillId = "SkillId";
    public const string SkillLevel = "SkillLevel";
    public const string SkillPic = "SkillPic";
    public const string SkillCDTime = "SkillCDTime";

    //======== 关卡胜利 ========
    public const string GameLevelVictory_PassTime = "GameLevelVictory_PassTime";
    public const string GameLevelVictory_EXP = "GameLevelVictory_EXP";
    public const string GameLevelVictory_Gold = "GameLevelVictory_Gold";
    public const string GameLevelVictory_Star = "GameLevelVictory_Star";
    public const string GameLevelVictory_Reward = "GameLevelVictory_Reward";

    //======== 世界地图 ========
    public const string WorldMap_ItemList = "WorldMap_ItemList";
    public const string WorldMap_Id = "WorldMap_Id";
    public const string WorldMap_Name = "WorldMap_Name";
    public const string WorldMap_Icon = "WorldMap_Icon";
    public const string WorldMap_PosInMap = "WorldMap_PosInMap";

}
