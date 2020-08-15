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
/// ��Ϸ�д���ö��
/// </summary>
public enum WindowUIType
{
    None,
    LogOn,//��¼
    Reg,//ע��
    GameServerEnter,//��¼��ѡ����
    GameServerSelect,//ѡ������
    RoleInfo,//��ɫ����
    GameLevelMap,//����ؿ���ͼ
    GameLevelDetail,//����ؿ�����
    GameLevelVictory,//ͨ�سɹ�
    GameLevelFail,//ͨ��ʧ��
    WorldMap,//�����ͼ
    WorldMapFail//�����ͼ�� PVP����ɱ��
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
    MainPlayer,//����
    Monster,//����
    OtherPlayer//�������
}

/// <summary>
/// ��ɫ����״̬��״̬
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
/// Animator��������
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
/// ��ɫ����״̬
/// </summary>
public enum RoleIdleState
{
    IdleNormal,
    IdleFight
}

/// <summary>
/// ��ɫ��������
/// </summary>
public enum RoleAttackType
{
    PhyAttack,//������
    SkillAttack//���ܹ���
}

public enum GameLevelGrade
{
    Normal = 0,//��ͨ
    Hard = 1,//����
    Hell = 2,//����
}

public enum GoodsType
{
    Equip=0,
    Item=1,
    Material=2
}