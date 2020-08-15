using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏关卡相关窗口控制器
/// </summary>
public class GameLevelController : SystemControllerBase<GameLevelController>, ISystemController
{
    #region 变量
    private UIGameLevelMapView m_UIGameLevelMapView;
    private UIGameLevelDetailView m_UIGameLevelDetailView;
    private UIGameLevelVictoryView m_UIGameLevelVictoryView;
    private UIGameLevelFailView m_UIGameLevelFailView;

    #region 当前关卡数据
    public int CurrGameLevelId;//当前游戏关卡Id(进入游戏关卡场景后赋值)
    public GameLevelGrade CurrGameLevelGrade;//当前游戏关卡难度(进入游戏关卡场景后赋值)
    public float CurrGameLevelPassTime;//当前游戏关卡通关时间

    public int CurrGameLevelTotalExp;//当前游戏关卡获得的总经验
    public int CurrGameLevelTotalGold;//当前游戏关卡获得的总金币
    public Dictionary<int, int> CurrGameLevelKillMonsterDic = new Dictionary<int, int>();//当前游戏关卡杀死的怪物
    public List<GetGoodsEntity> CurrGameLevelGetGoodsList = new List<GetGoodsEntity>();//当前游戏关卡获得物品列表
    #endregion

    #endregion

    public GameLevelController()
    {
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_EnterReturn, OnGameLevel_EnterReturn);
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_ResurgenceReturn, OnGameLevel_ResurgenceReturn);
    }

    #region 服务器返回协议回调函数
    /// <summary>
    /// 服务器返回复活协议
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
    /// 服务器返回进入游戏关卡协议
    /// </summary>
    /// <param name="p"></param>
    private void OnGameLevel_EnterReturn(byte[] p)
    {
        GameLevel_EnterReturnProto retProto = GameLevel_EnterReturnProto.GetProto(p);
        if (retProto.IsSuccess)
        {
            SceneMgr.Instance.LoadToGameLevel(tempGameLevelId, tempGameLevelGrade);//跳转场景
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

    #region 打开游戏关卡地图窗口
    /// <summary>
    /// 打开游戏关卡地图窗口
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
    /// 游戏关卡地图上 关卡项点击回调 打开关卡详情窗口
    /// </summary>
    /// <param name="gameLevelId"></param>
    private void OnGameLevelItemClickCallback(int gameLevelId)
    {
        //打开关卡详情窗口
        OpenGameLevelDetailView(gameLevelId);
    }
    #endregion

    #region 打开关卡详情窗口
    /// <summary>
    /// 打开游戏关卡详情窗口
    /// </summary>
    /// <param name="gameLevelId">关卡编号</param>
    private void OpenGameLevelDetailView(int gameLevelId)
    {
        m_UIGameLevelDetailView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameLevelDetail).GetComponent<UIGameLevelDetailView>();
        m_UIGameLevelDetailView.OnBtnGradeClick = OnGameLevelChangeGrade;//绑定改变关卡难度回调
        m_UIGameLevelDetailView.OnBtnEnterClick = OnEnterGameLevel;//绑定进入游戏关卡按钮回调

        //给窗口设置数据
        SetGameLevelDetailData(gameLevelId, GameLevelGrade.Normal);

        AppDebug.Log("click game level item, levelid=" + gameLevelId);
    }

    private int tempGameLevelId;//暂时保存要进入的关卡信息
    private GameLevelGrade tempGameLevelGrade;

    /// <summary>
    /// 进入关卡场景
    /// </summary>
    private void OnEnterGameLevel(int gameLevelId, GameLevelGrade grade)
    {
        //发协议 告诉服务器要进入某个关卡
        GameLevel_EnterProto proto = new GameLevel_EnterProto();
        proto.GameLevelId = gameLevelId;
        proto.Grade = (byte)grade;
        NetWorkSocket.Instance.SendMsg(proto.ToArray());

        tempGameLevelId = gameLevelId;
        tempGameLevelGrade = grade;
    }

    /// <summary>
    /// 改变关卡难度
    /// </summary>
    private void OnGameLevelChangeGrade(int gameLevelId, GameLevelGrade grade)
    {
        SetGameLevelDetailData(gameLevelId, grade);
        AppDebug.Log("Change level grade=" + grade.ToString());
    }

    /// <summary>
    /// 设置关卡详情窗口数据
    /// </summary>
    private void SetGameLevelDetailData(int gameLevelId, GameLevelGrade grade)
    {
        //读取游戏关卡表
        GameLevelEntity gameLevelEntity = GameLevelDBModel.Instance.GetEntity(gameLevelId);

        //读取游戏关卡难度等级表
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

        data.SetValue(ConstDefine.GameLevel_Id, gameLevelEntity.Id);//关卡编号
        data.SetValue(ConstDefine.GameLevel_Name, gameLevelEntity.Name);//关卡名称
        data.SetValue(ConstDefine.GameLevel_DlgPic, gameLevelEntity.DlgPic);//关卡预览图

        data.SetValue(ConstDefine.GameLevelGrade_Gold, gameLevelGradeEntity.Gold);//过关奖励金币
        data.SetValue(ConstDefine.GameLevelGrade_Exp, gameLevelGradeEntity.Exp);//过关奖励经验
        data.SetValue(ConstDefine.GameLevelGrade_Desc, gameLevelGradeEntity.Desc);//关卡描述
        data.SetValue(ConstDefine.GameLevelGrade_ConditionDesc, gameLevelGradeEntity.ConditionDesc);//过关条件
        data.SetValue(ConstDefine.GameLevelGrade_CommendFighting, gameLevelGradeEntity.CommendFighting);//推荐战力

        List<TransferData> rewardList = new List<TransferData>();

        #region Equip 该关卡可能掉落的装备
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

            GoodsEntity goodsEquip = gameLevelGradeEntity.EquipList[0];//拿到掉落概率最小的装备实体

            TransferData equipReward = new TransferData();
            equipReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            equipReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            equipReward.SetValue(ConstDefine.GoodsType, GoodsType.Equip);
            rewardList.Add(equipReward);
        }
        #endregion

        #region Item 该关卡可能掉落的物品
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

        #region Material 该关卡可能掉落的材料
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

            GoodsEntity goodsEquip = gameLevelGradeEntity.MaterialList[0];//按照掉落概率从小到大排序 得到掉率最低的材料

            TransferData materialReward = new TransferData();
            materialReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            materialReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            materialReward.SetValue(ConstDefine.GoodsType, GoodsType.Material);
            rewardList.Add(materialReward);
        }
        #endregion

        data.SetValue(ConstDefine.GameLevelGrade_RewardList, rewardList);

        m_UIGameLevelDetailView.SetUI(data);//controller给view传递数据
    }
    #endregion

    /// <summary>
    /// 打开游戏关卡通关窗口
    /// </summary>
    private void OpenGameLevelVictoryView()
    {
        #region 打开UI窗口 给UI赋值
        m_UIGameLevelVictoryView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameLevelVictory).GetComponent<UIGameLevelVictoryView>();

        GameLevelEntity gameLevelEntity = GameLevelDBModel.Instance.GetEntity(CurrGameLevelId);
        GameLevelGradeEntity gameLevelGradeEntity = GameLevelGradeDBModel.Instance.GetEntityByIdAndGrade(CurrGameLevelId, CurrGameLevelGrade);

        if (gameLevelEntity == null || gameLevelGradeEntity == null) return;

        TransferData data = new TransferData();
        data.SetValue(ConstDefine.GameLevelVictory_PassTime, CurrGameLevelPassTime);//过关时间
        data.SetValue(ConstDefine.GameLevelVictory_EXP, gameLevelGradeEntity.Exp);//通关获得经验
        data.SetValue(ConstDefine.GameLevelVictory_Gold, gameLevelGradeEntity.Gold);//通关获得金币

        int starNum = 1;
        if (CurrGameLevelPassTime <= gameLevelGradeEntity.Star2)
        {
            starNum = 3;
        }
        else  if (CurrGameLevelPassTime <= gameLevelGradeEntity.Star1)
        {
            starNum = 2;
        }
        
        data.SetValue(ConstDefine.GameLevelVictory_Star, starNum);//过关评级

        //掉落物品
        List<TransferData> rewardList = new List<TransferData>();

        #region Equip 该关卡可能掉落的装备
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

            GoodsEntity goodsEquip = gameLevelGradeEntity.EquipList[0];//拿到掉落概率最小的装备实体

            TransferData equipReward = new TransferData();

            equipReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            equipReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            equipReward.SetValue(ConstDefine.GoodsType, GoodsType.Equip);

            rewardList.Add(equipReward);

            CurrGameLevelGetGoodsList.Add(new GetGoodsEntity() { GoodsType = 0, GoodsId = goodsEquip.Id, GoodsCount = 1});
        }
        #endregion

        #region Item 该关卡可能掉落的物品
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

        #region Material 该关卡可能掉落的材料
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

            GoodsEntity goodsEquip = gameLevelGradeEntity.MaterialList[0];//按照掉落概率从小到大排序 得到掉率最低的材料

            TransferData materialReward = new TransferData();

            materialReward.SetValue(ConstDefine.GoodsId, goodsEquip.Id);
            materialReward.SetValue(ConstDefine.GoodsName, goodsEquip.Name);
            materialReward.SetValue(ConstDefine.GoodsType, GoodsType.Material);

            rewardList.Add(materialReward);

            CurrGameLevelGetGoodsList.Add(new GetGoodsEntity() { GoodsType = 2, GoodsId = goodsEquip.Id, GoodsCount = 1 });
        }
        #endregion

        data.SetValue(ConstDefine.GameLevelVictory_Reward, rewardList);

        m_UIGameLevelVictoryView.SetUI(data);//给view传递数据
        #endregion

        #region 与服务器通信
        GameLevel_VictoryProto proto = new GameLevel_VictoryProto();
        proto.GameLevelId = CurrGameLevelId;
        proto.Grade = (byte)CurrGameLevelGrade;
        proto.Star = (byte)starNum;//星级

        CurrGameLevelTotalExp += gameLevelGradeEntity.Exp;//经验
        proto.Exp = CurrGameLevelTotalExp;

        CurrGameLevelTotalGold += gameLevelGradeEntity.Gold;////金币
        proto.Gold = CurrGameLevelTotalGold;

        //杀怪
        proto.KillTotalMonsterCount = CurrGameLevelKillMonsterDic.Count;
        proto.KillMonsterList = new List<GameLevel_VictoryProto.MonsterItem>();
        foreach (var item in CurrGameLevelKillMonsterDic)
        {
            GameLevel_VictoryProto.MonsterItem monsterItem = new GameLevel_VictoryProto.MonsterItem();
            monsterItem.MonsterId = item.Key;
            monsterItem.MonsterCount = item.Value;

            proto.KillMonsterList.Add(monsterItem);
        }

        //获得物品
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
    /// 打开游戏关卡失败窗口
    /// </summary>
    private void OpenGameLevelFailView()
    {
        #region 打开UI窗口 给UI赋值
        m_UIGameLevelFailView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameLevelFail).GetComponent<UIGameLevelFailView>();

        //绑定通关失败窗口免费复活按钮的点击事件回调
        m_UIGameLevelFailView.OnReborn = () => 
        {
            //向服务器发送复活协议 如果服务器返回复活成功 那么就复活主角
            GameLevel_ResurgenceProto rebornProto = new GameLevel_ResurgenceProto();
            rebornProto.GameLevelId = CurrGameLevelId;
            rebornProto.Grade = (byte)CurrGameLevelGrade;
            rebornProto.Type = 0;//免费复活
            NetWorkSocket.Instance.SendMsg(rebornProto.ToArray());
        };

        //绑定通关失败窗口返回主城按钮的点击事件回调
        m_UIGameLevelFailView.OnReturnMainCity = () => 
        {
            SceneMgr.Instance.LoadToWorldMap(SceneMgr.Instance.CurrWorldMapId);//返回主城
            GlobalInit.Instance.MainPlayer.RoleReborn();
        };
        #endregion

        #region 与服务器通信
        //发送战斗失败协议
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
