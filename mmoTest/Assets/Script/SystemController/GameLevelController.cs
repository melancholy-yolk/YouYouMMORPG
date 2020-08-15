using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�ؿ���ش��ڿ�����
/// </summary>
public class GameLevelController : SystemControllerBase<GameLevelController>, ISystemController
{
    #region ����
    private UIGameLevelMapView m_UIGameLevelMapView;
    private UIGameLevelDetailView m_UIGameLevelDetailView;
    private UIGameLevelVictoryView m_UIGameLevelVictoryView;
    private UIGameLevelFailView m_UIGameLevelFailView;

    #region ��ǰ�ؿ�����
    public int CurrGameLevelId;//��ǰ��Ϸ�ؿ�Id(������Ϸ�ؿ�������ֵ)
    public GameLevelGrade CurrGameLevelGrade;//��ǰ��Ϸ�ؿ��Ѷ�(������Ϸ�ؿ�������ֵ)
    public float CurrGameLevelPassTime;//��ǰ��Ϸ�ؿ�ͨ��ʱ��

    public int CurrGameLevelTotalExp;//��ǰ��Ϸ�ؿ���õ��ܾ���
    public int CurrGameLevelTotalGold;//��ǰ��Ϸ�ؿ���õ��ܽ��
    public Dictionary<int, int> CurrGameLevelKillMonsterDic = new Dictionary<int, int>();//��ǰ��Ϸ�ؿ�ɱ���Ĺ���
    public List<GetGoodsEntity> CurrGameLevelGetGoodsList = new List<GetGoodsEntity>();//��ǰ��Ϸ�ؿ������Ʒ�б�
    #endregion

    #endregion

    public GameLevelController()
    {
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_EnterReturn, OnGameLevel_EnterReturn);
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_ResurgenceReturn, OnGameLevel_ResurgenceReturn);
    }

    #region ����������Э��ص�����
    /// <summary>
    /// ���������ظ���Э��
    /// </summary>
    /// <param name="p"></param>
    private void OnGameLevel_ResurgenceReturn(byte[] p)
    {
        GameLevel_ResurgenceReturnProto retProto = GameLevel_ResurgenceReturnProto.GetProto(p);
        if (retProto.IsSuccess)
        {
            m_UIGameLevelFailView.Close();
            GlobalInit.Instance.MainPlayer.RoleReborn();
        }
        else
        {
            ShowMessage("Resurgence Fail", "Resurgence Fail, Please Return Main City!");
        }
    }

    /// <summary>
    /// ���������ؽ�����Ϸ�ؿ�Э��
    /// </summary>
    /// <param name="p"></param>
    private void OnGameLevel_EnterReturn(byte[] p)
    {
        GameLevel_EnterReturnProto retProto = GameLevel_EnterReturnProto.GetProto(p);
        if (retProto.IsSuccess)
        {
            SceneMgr.Instance.LoadToGameLevel(tempGameLevelId, tempGameLevelGrade);//��ת����
        }
    }
    #endregion

    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.GameLevelMap:
                OpenGameLevelMapView();
                break;
            case WindowUIType.GameLevelVictory:
                OpenGameLevelVictoryView();
                break;
            case WindowUIType.GameLevelFail:
                OpenGameLevelFailView();
                break;
        }
    }

    #region ����Ϸ�ؿ���ͼ����
    /// <summary>
    /// ����Ϸ�ؿ���ͼ����
    /// </summary>
    private void OpenGameLevelMapView()
    {
        m_UIGameLevelMapView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameLevelMap).GetComponent<UIGameLevelMapView>();

        TransferData data = new TransferData();

        ChapterEntity entity = ChapterDBModel.Instance.GetEntity(1);

        if (entity == null) return;

        data.SetValue(ConstDefine.ChapterId, entity.Id);
        data.SetValue(ConstDefine.ChapterName, entity.ChapterName);
        data.SetValue(ConstDefine.ChapterMapPic, entity.BG_Pic);

        List<GameLevelEntity> gameLevelEntityList = GameLevelDBModel.Instance.GetListByChapterId(entity.Id);
        if (gameLevelEntityList != null && gameLevelEntityList.Count > 0)
        {
            List<TransferData> lst = new List<TransferData>();

            for (int i = 0; i < gameLevelEntityList.Count; i++)
            {
                TransferData childData = new TransferData();

                childData.SetValue(ConstDefine.GameLevel_Id, gameLevelEntityList[i].Id);
                childData.SetValue(ConstDefine.GameLevel_Name, gameLevelEntityList[i].Name);
                childData.SetValue(ConstDefine.GameLevel_Ico, gameLevelEntityList[i].Ico);
                childData.SetValue(ConstDefine.GameLevel_Position, gameLevelEntityList[i].Position);
                childData.SetValue(ConstDefine.GameLevel_SceneName, gameLevelEntityList[i].SceneName);
                childData.SetValue(ConstDefine.GameLevel_IsBoss, gameLevelEntityList[i].isBoss);

                lst.Add(childData);
            }

            data.SetValue(ConstDefine.GameLevelList, lst);
        }

        m_UIGameLevelMapView.SetUI(data, OnGameLevelItemClickCallback);
    }

    /// <summary>
    /// ��Ϸ�ؿ���ͼ�� �ؿ������ص� �򿪹ؿ����鴰��
    /// </summary>
    /// <param name="gameLevelId"></param>
    private void OnGameLevelItemClickCallback(int gameLevelId)
    {
        //�򿪹ؿ����鴰��
        OpenGameLevelDetailView(gameLevelId);
    }
    #endregion

    #region �򿪹ؿ����鴰��
    /// <summary>
    /// ����Ϸ�ؿ����鴰��
    /// </summary>
    /// <param name="gameLevelId">�ؿ����</param>
    private void OpenGameLevelDetailView(int gameLevelId)
    {
        m_UIGameLevelDetailView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameLevelDetail).GetComponent<UIGameLevelDetailView>();
        m_UIGameLevelDetailView.OnBtnGradeClick = OnGameLevelChangeGrade;//�󶨸ı�ؿ��ѶȻص�
        m_UIGameLevelDetailView.OnBtnEnterClick = OnEnterGameLevel;//�󶨽�����Ϸ�ؿ���ť�ص�

        //��������������
        SetGameLevelDetailData(gameLevelId, GameLevelGrade.Normal);

        AppDebug.Log("click game level item, levelid=" + gameLevelId);
    }

    private int tempGameLevelId;//��ʱ����Ҫ����Ĺؿ���Ϣ
    private GameLevelGrade tempGameLevelGrade;

    /// <summary>
    /// ����ؿ�����
    /// </summary>
    private void OnEnterGameLevel(int gameLevelId, GameLevelGrade grade)
    {
        //��Э�� ���߷�����Ҫ����ĳ���ؿ�
        GameLevel_EnterProto proto = new GameLevel_EnterProto();
        proto.GameLevelId = gameLevelId;
        proto.Grade = (byte)grade;
        NetWorkSocket.Instance.SendMsg(proto.ToArray());

        tempGameLevelId = gameLevelId;
        tempGameLevelGrade = grade;
    }

    /// <summary>
    /// �ı�ؿ��Ѷ�
    /// </summary>
    private void OnGameLevelChangeGrade(int gameLevelId, GameLevelGrade grade)
    {
        SetGameLevelDetailData(gameLevelId, grade);
        AppDebug.Log("Change level grade=" + grade.ToString());
    }

    /// <summary>
    /// ���ùؿ����鴰������
    /// </summary>
    private void SetGameLevelDetailData(int gameLevelId, GameLevelGrade grade)
    {
        //��ȡ��Ϸ�ؿ���
        GameLevelEntity gameLevelEntity = GameLevelDBModel.Instance.GetEntity(gameLevelId);

        //��ȡ��Ϸ�ؿ��Ѷȵȼ���
        GameLevelGradeEntity gameLevelGradeEntity = GameLevelGradeDBModel.Instance.GetEntityByIdAndGrade(gameLevelId, grade);

        AppDebug.Log(string.Format("Id={0} Name={1} Pic={2}", gameLevelEntity.Id, gameLevelEntity.Name, gameLevelEntity.DlgPic));

        if (gameLevelEntity == null)
        {
            AppDebug.Log("gameLevelEntity is null");
            return;
        }

        if (gameLevelGradeEntity == null)
        {
            AppDebug.Log("gameLevelGradeEntity is null");
            return;
        }

        TransferData data = new TransferData();

        data.SetValue(ConstDefine.GameLevel_Id, gameLevelEntity.Id);//�ؿ����
        data.SetValue(ConstDefine.GameLevel_Name, gameLevelEntity.Name);//�ؿ�����
        data.SetValue(ConstDefine.GameLevel_DlgPic, gameLevelEntity.DlgPic);//�ؿ�Ԥ��ͼ

        data.SetValue(ConstDefine.GameLevelGrade_Gold, gameLevelGradeEntity.Gold);//���ؽ������
        data.SetValue(ConstDefine.GameLevelGrade_Exp, gameLevelGradeEntity.Exp);//���ؽ�������
        data.SetValue(ConstDefine.GameLevelGrade_Desc, gameLevelGradeEntity.Desc);//�ؿ�����
        data.SetValue(ConstDefine.GameLevelGrade_ConditionDesc, gameLevelGradeEntity.ConditionDesc);//��������
        data.SetValue(ConstDefine.GameLevelGrade_CommendFighting, gameLevelGradeEntity.CommendFighting);//�Ƽ�ս��

        List<TransferData> rewardList = new List<TransferData>();

        #region Equip �ùؿ����ܵ����װ��
        if (gameLevelGradeEntity.EquipList.Count > 0)
        {
            gameLevelGradeEntity.EquipList.Sort((entity1, entity2) =>
            {
                if (entity1.Probability < entity2.Probability)
                {
                    return -1;
                }
                else if (entity1.Probability == entity2.Probability)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            GoodsEntity goodsEquip = gameLevelGradeEntity.EquipList[0];//�õ����������С��װ��ʵ��

            TransferData equipReward = new TransferData();
            equipReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            equipReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            equipReward.SetValue(ConstDefine.GoodsType, GoodsType.Equip);
            rewardList.Add(equipReward);
        }
        #endregion

        #region Item �ùؿ����ܵ������Ʒ
        if (gameLevelGradeEntity.ItemList.Count > 0)
        {
            gameLevelGradeEntity.ItemList.Sort((entity1, entity2) =>
            {
                if (entity1.Probability < entity2.Probability)
                {
                    return -1;
                }
                else if (entity1.Probability == entity2.Probability)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            GoodsEntity goodsEquip = gameLevelGradeEntity.ItemList[0];

            TransferData itemReward = new TransferData();
            itemReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            itemReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            itemReward.SetValue(ConstDefine.GoodsType, GoodsType.Item);
            rewardList.Add(itemReward);
        }
        #endregion

        #region Material �ùؿ����ܵ���Ĳ���
        if (gameLevelGradeEntity.MaterialList.Count > 0)
        {
            gameLevelGradeEntity.MaterialList.Sort((entity1, entity2) =>
            {
                if (entity1.Probability < entity2.Probability)
                {
                    return -1;
                }
                else if (entity1.Probability == entity2.Probability)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            GoodsEntity goodsEquip = gameLevelGradeEntity.MaterialList[0];//���յ�����ʴ�С�������� �õ�������͵Ĳ���

            TransferData materialReward = new TransferData();
            materialReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            materialReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            materialReward.SetValue(ConstDefine.GoodsType, GoodsType.Material);
            rewardList.Add(materialReward);
        }
        #endregion

        data.SetValue(ConstDefine.GameLevelGrade_RewardList, rewardList);

        m_UIGameLevelDetailView.SetUI(data);//controller��view��������
    }
    #endregion

    /// <summary>
    /// ����Ϸ�ؿ�ͨ�ش���
    /// </summary>
    private void OpenGameLevelVictoryView()
    {
        #region ��UI���� ��UI��ֵ
        m_UIGameLevelVictoryView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameLevelVictory).GetComponent<UIGameLevelVictoryView>();

        GameLevelEntity gameLevelEntity = GameLevelDBModel.Instance.GetEntity(CurrGameLevelId);
        GameLevelGradeEntity gameLevelGradeEntity = GameLevelGradeDBModel.Instance.GetEntityByIdAndGrade(CurrGameLevelId, CurrGameLevelGrade);

        if (gameLevelEntity == null || gameLevelGradeEntity == null) return;

        TransferData data = new TransferData();
        data.SetValue(ConstDefine.GameLevelVictory_PassTime, CurrGameLevelPassTime);//����ʱ��
        data.SetValue(ConstDefine.GameLevelVictory_EXP, gameLevelGradeEntity.Exp);//ͨ�ػ�þ���
        data.SetValue(ConstDefine.GameLevelVictory_Gold, gameLevelGradeEntity.Gold);//ͨ�ػ�ý��

        int starNum = 1;
        if (CurrGameLevelPassTime <= gameLevelGradeEntity.Star2)
        {
            starNum = 3;
        }
        else  if (CurrGameLevelPassTime <= gameLevelGradeEntity.Star1)
        {
            starNum = 2;
        }
        
        data.SetValue(ConstDefine.GameLevelVictory_Star, starNum);//��������

        //������Ʒ
        List<TransferData> rewardList = new List<TransferData>();

        #region Equip �ùؿ����ܵ����װ��
        if (gameLevelGradeEntity.EquipList.Count > 0)
        {
            gameLevelGradeEntity.EquipList.Sort((entity1, entity2) =>
            {
                if (entity1.Probability < entity2.Probability)
                {
                    return -1;
                }
                else if (entity1.Probability == entity2.Probability)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            GoodsEntity goodsEquip = gameLevelGradeEntity.EquipList[0];//�õ����������С��װ��ʵ��

            TransferData equipReward = new TransferData();

            equipReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            equipReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            equipReward.SetValue(ConstDefine.GoodsType, GoodsType.Equip);

            rewardList.Add(equipReward);

            CurrGameLevelGetGoodsList.Add(new GetGoodsEntity() { GoodsType = 0, GoodsId = goodsEquip.Id, GoodsCount = 1});
        }
        #endregion

        #region Item �ùؿ����ܵ������Ʒ
        if (gameLevelGradeEntity.ItemList.Count > 0)
        {
            gameLevelGradeEntity.ItemList.Sort((entity1, entity2) =>
            {
                if (entity1.Probability < entity2.Probability)
                {
                    return -1;
                }
                else if (entity1.Probability == entity2.Probability)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            GoodsEntity goodsEquip = gameLevelGradeEntity.ItemList[0];

            TransferData itemReward = new TransferData();

            itemReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            itemReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            itemReward.SetValue(ConstDefine.GoodsType, GoodsType.Item);

            rewardList.Add(itemReward);

            CurrGameLevelGetGoodsList.Add(new GetGoodsEntity() { GoodsType = 1, GoodsId = goodsEquip.Id, GoodsCount = 1 });
        }
        #endregion

        #region Material �ùؿ����ܵ���Ĳ���
        if (gameLevelGradeEntity.MaterialList.Count > 0)
        {
            gameLevelGradeEntity.MaterialList.Sort((entity1, entity2) =>
            {
                if (entity1.Probability < entity2.Probability)
                {
                    return -1;
                }
                else if (entity1.Probability == entity2.Probability)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            GoodsEntity goodsEquip = gameLevelGradeEntity.MaterialList[0];//���յ�����ʴ�С�������� �õ�������͵Ĳ���

            TransferData materialReward = new TransferData();

            materialReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            materialReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            materialReward.SetValue(ConstDefine.GoodsType, GoodsType.Material);

            rewardList.Add(materialReward);

            CurrGameLevelGetGoodsList.Add(new GetGoodsEntity() { GoodsType = 2, GoodsId = goodsEquip.Id, GoodsCount = 1 });
        }
        #endregion

        data.SetValue(ConstDefine.GameLevelVictory_Reward, rewardList);

        m_UIGameLevelVictoryView.SetUI(data);//��view��������
        #endregion

        #region �������ͨ��
        GameLevel_VictoryProto proto = new GameLevel_VictoryProto();
        proto.GameLevelId = CurrGameLevelId;
        proto.Grade = (byte)CurrGameLevelGrade;
        proto.Star = (byte)starNum;//�Ǽ�

        CurrGameLevelTotalExp += gameLevelGradeEntity.Exp;//����
        proto.Exp = CurrGameLevelTotalExp;

        CurrGameLevelTotalGold += gameLevelGradeEntity.Gold;////���
        proto.Gold = CurrGameLevelTotalGold;

        //ɱ��
        proto.KillTotalMonsterCount = CurrGameLevelKillMonsterDic.Count;
        proto.KillMonsterList = new List<GameLevel_VictoryProto.MonsterItem>();
        foreach (var item in CurrGameLevelKillMonsterDic)
        {
            GameLevel_VictoryProto.MonsterItem monsterItem = new GameLevel_VictoryProto.MonsterItem();
            monsterItem.MonsterId = item.Key;
            monsterItem.MonsterCount = item.Value;

            proto.KillMonsterList.Add(monsterItem);
        }

        //�����Ʒ
        proto.GoodsTotalCount = CurrGameLevelGetGoodsList.Count;
        proto.GetGoodsList = new List<GameLevel_VictoryProto.GoodsItem>();
        for (int i = 0; i < CurrGameLevelGetGoodsList.Count; i++)
        {
            GameLevel_VictoryProto.GoodsItem goodsItem = new GameLevel_VictoryProto.GoodsItem();
            goodsItem.GoodsType = CurrGameLevelGetGoodsList[i].GoodsType;
            goodsItem.GoodsId = CurrGameLevelGetGoodsList[i].GoodsId;
            goodsItem.GoodsCount = CurrGameLevelGetGoodsList[i].GoodsCount;

            proto.GetGoodsList.Add(goodsItem);
        }

        NetWorkSocket.Instance.SendMsg(proto.ToArray());
        #endregion
    }

    /// <summary>
    /// ����Ϸ�ؿ�ʧ�ܴ���
    /// </summary>
    private void OpenGameLevelFailView()
    {
        #region ��UI���� ��UI��ֵ
        m_UIGameLevelFailView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameLevelFail).GetComponent<UIGameLevelFailView>();

        //��ͨ��ʧ�ܴ�����Ѹ��ť�ĵ���¼��ص�
        m_UIGameLevelFailView.OnReborn = () => 
        {
            //����������͸���Э�� ������������ظ���ɹ� ��ô�͸�������
            GameLevel_ResurgenceProto rebornProto = new GameLevel_ResurgenceProto();
            rebornProto.GameLevelId = CurrGameLevelId;
            rebornProto.Grade = (byte)CurrGameLevelGrade;
            rebornProto.Type = 0;//��Ѹ���
            NetWorkSocket.Instance.SendMsg(rebornProto.ToArray());
        };

        //��ͨ��ʧ�ܴ��ڷ������ǰ�ť�ĵ���¼��ص�
        m_UIGameLevelFailView.OnReturnMainCity = () => 
        {
            SceneMgr.Instance.LoadToWorldMap(SceneMgr.Instance.CurrWorldMapId);//��������
            GlobalInit.Instance.MainPlayer.RoleReborn();
        };
        #endregion

        #region �������ͨ��
        //����ս��ʧ��Э��
        GameLevel_FailProto failProto = new GameLevel_FailProto();
        failProto.GameLevelId = CurrGameLevelId;
        failProto.Grade = (byte)CurrGameLevelGrade;
        NetWorkSocket.Instance.SendMsg(failProto.ToArray());
        #endregion
    }

    public override void Dispose()
    {
        base.Dispose();

        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.GameLevel_EnterReturn, OnGameLevel_EnterReturn);
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.GameLevel_ResurgenceReturn, OnGameLevel_ResurgenceReturn);
    }
}
