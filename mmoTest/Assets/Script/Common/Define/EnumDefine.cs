using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayType
{
    PVE,
    PVP
}

public enum SceneType
{
    LogOn,
    SelectRole,
    WorldMap,
    GameLevel,
}

public enum SceneUIType
{
    Init,
    Loading,
    LogOn,
    SelectRole,
    MainCity
}

public enum MessageViewType
{
    OK,
    OKAndCancel
}

/// <summary>
/// 游戏中窗口枚举
/// </summary>
public enum WindowUIType
{
    None,
    LogOn,//登录
    Reg,//注册
    GameServerEnter,//登录所选区服
    GameServerSelect,//选择屈服
    RoleInfo,//角色详情
    GameLevelMap,//剧情关卡地图
    GameLevelDetail,//剧情关卡详情
    GameLevelVictory,//通关成功
    GameLevelFail,//通关失败
    WorldMap,//世界地图
    WorldMapFail//世界地图中 PVP被人杀死
}

public enum WindowUIContainerType
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
}

public enum WindowUIShowStyle
{
    Normal,
    CenterToBig,
    FromTop,
    FromDown,
    FromLeft,
    FromRight,
}

public enum RoleType
{
    None,
    MainPlayer,//主角
    Monster,//怪物
    OtherPlayer//其他玩家
}

/// <summary>
/// 角色有限状态机状态
/// </summary>
public enum RoleState
{
    None=0,
    Idle=1,
    Run=2,
    Attack=3,
    Hurt=4,
    Die=5,
    Select,
    Skill
}




/// <summary>
/// Animator参数名称
/// </summary>
public enum AnimatorParameter
{
    None,
    ToIdleNormal,
    ToIdleFight,
    ToXiuXian,
    ToSelect,
    ToRun,
    ToHurt,
    ToDie,
    ToPhyAttack,
    ToSkill,
    CurrState,
    ToDied
}

public enum RoleAnimatorState
{
    Idle_Normal=1,
    Idle_Fight=2,
    Run=3,
    Hurt=4,
    Die=5,
    Select=6,
    XiuXian=7,
    Died=8,
    PhyAttack1=11,
    PhyAttack2=12,
    PhyAttack3=13,
    Skill1=14,
    Skill2=15,
    Skill3=16,
    Skill4=17,
    Skill5=18,
    Skill6=19
}

/// <summary>
/// 角色待机状态
/// </summary>
public enum RoleIdleState
{
    IdleNormal,
    IdleFight
}

/// <summary>
/// 角色攻击类型
/// </summary>
public enum RoleAttackType
{
    PhyAttack,//物理攻击
    SkillAttack//技能攻击
}

public enum GameLevelGrade
{
    Normal = 0,//普通
    Hard = 1,//困难
    Hell = 2,//地狱
}

public enum GoodsType
{
    Equip=0,
    Item=1,
    Material=2
}