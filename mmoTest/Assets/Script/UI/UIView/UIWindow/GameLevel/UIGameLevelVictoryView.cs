using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameLevelVictoryView : UIWindowViewBase
{
    [SerializeField]
    private Text textPassTime;
    [SerializeField]
    private Text textExp;
    [SerializeField]
    private Text textGold;
    [SerializeField]
    private Image[] StarArr;
    [SerializeField]
    private List<UIGameLevelRewardView> RewardsList;

    protected override void OnAwake()
    {
        base.OnAwake();

        if (StarArr != null && StarArr.Length == 3)
        {
            for (int i = 0; i < StarArr.Length; i++)
            {
                StarArr[i].gameObject.SetActive(false);
            }
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);

        if (go.name == "BtnReturn")
        {
            //返回主城
            SceneMgr.Instance.LoadToWorldMap(SceneMgr.Instance.CurrWorldMapId);
        }
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
    }

    public void SetUI(TransferData data)
    {
        Debug.Log("set victory ui");
        //通关时间 经验 金币 评分星级 掉落物品
        textPassTime.SetText("Pass Time : " + (int)data.GetValue<float>(ConstDefine.GameLevelVictory_PassTime));
        textExp.SetText(data.GetValue<int>(ConstDefine.GameLevelVictory_EXP).ToString());
        textGold.SetText(data.GetValue<int>(ConstDefine.GameLevelVictory_Gold).ToString());

        SetStar(data.GetValue<int>(ConstDefine.GameLevelVictory_Star));

        #region 关卡中掉落物品
        //奖励物品列表
        //List<TransferData> rewardList = data.GetValue<List<TransferData>>(ConstDefine.GameLevelVictory_Reward);

        //for (int j = 0; j < RewardsList.Count; j++)
        //{
        //    RewardsList[j].gameObject.SetActive(false);
        //}

        //if (rewardList.Count > 0)
        //{
        //    for (int i = 0; i < rewardList.Count; i++)
        //    {
        //        RewardsList[i].gameObject.SetActive(true);
        //        RewardsList[i].SetUI(rewardList[i].GetValue<string>(ConstDefine.GoodsName), rewardList[i].GetValue<int>(ConstDefine.GoodsId), rewardList[i].GetValue<GoodsType>(ConstDefine.GoodsType));
        //    }
        //}
        #endregion
    }

    private void SetStar(int starNum)
    {
        if (StarArr != null && StarArr.Length == 3)
        {
            for (int i = 0; i < starNum; i++)
            {
                StarArr[i].gameObject.SetActive(true);
            }
        }
    }
}
