using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ʹ������������ˢ������UI RoleData-->Controller<--UIView
/// </summary>
public class PlayerController : SystemControllerBase<PlayerController>, ISystemController
{

    private UIRoleInfoView m_UIRoleInfoView;//��ɫ���鴰��

    private RoleInfoMainPlayer m_RoleInfoMainPlayer;

    public int LastInWorldMapId;
    public string LastInWorldMapPos;

    public PlayerController()
    { 
        
    }

    #region ϵͳ�������ӿڷ���ʵ��
    /// <summary>
    /// �򿪽�ɫ���鴰�� �������ڴ�������
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
    /// �������ǳ�פUI
    /// </summary>
    public void SetMainCityRoleData()
    {
        SetMainCityRoleInfo();//���Ͻǽ�ɫ��ϢUI
        SetMainCityRoleSkillInfo();//���½ǽ�ɫ����UI
        SetMainCitySmallMap();
    }

    #region ����UI���ϽǵĽ�ɫ��Ϣ
    /// <summary>
    /// ʹ�õ�ǰ��¼��Ϸ����ɫ��Ϣ �������ǽ�ɫ��ϢUI
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

        //ͷ�� �ǳ� �ȼ� Ԫ�� ��� Ѫ�� ����
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

    #region ����UI���½ǵļ���ͼ��
    /// <summary>
    /// ���ý�ɫ����UI
    /// </summary>
    private void SetMainCityRoleSkillInfo()
    {
        RoleInfoMainPlayer roleInfo = GlobalInit.Instance.MainPlayerInfo;

        //��������������Ѿ�ѧ��ļ����б� ����Id ���ܵȼ� ���ܲ۱��
        List<TransferData> list = new List<TransferData>();
        for (int i = 0; i < roleInfo.SkillList.Count; i++)
        {
            TransferData data = new TransferData();

            data.SetValue(ConstDefine.SkillSlotNo, roleInfo.SkillList[i].SlotsNo);
            data.SetValue(ConstDefine.SkillId, roleInfo.SkillList[i].SkillId);
            data.SetValue(ConstDefine.SkillLevel, roleInfo.SkillList[i].SkillLevel);

            //ͨ������Id �Ӽ��ܱ��л�ȡ����ͼ��Icon
            SkillEntity entity = SkillDBModel.Instance.GetEntity(roleInfo.SkillList[i].SkillId);
            if (entity != null)
            {
                data.SetValue(ConstDefine.SkillPic, entity.SkillPic);
            }

            //ͨ������Id ���ܵȼ� �Ӽ��ܵȼ����л�ȡ������ȴʱ��
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
    /// ������ܰ�ť�Ļص� ���ý�ɫ�����ܹ������� �������ʹ�óɹ� �ü��ܿ�ʼCD
    /// </summary>
    /// <param name="skillId"></param>
    public void OnSkillClick(int skillId)
    {
        bool isSuccess = GlobalInit.Instance.MainPlayer.ToAttackBySkillId(RoleAttackType.SkillAttack, skillId);
        if (isSuccess)
        {
            GlobalInit.Instance.MainPlayer.CurrRoleInfo.SetSkillCDEndTime(skillId);//�����ʹ�õ��������������ȴ����ʱ��=ʹ�ü���ʱ�� + ������ȴʱ��
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
