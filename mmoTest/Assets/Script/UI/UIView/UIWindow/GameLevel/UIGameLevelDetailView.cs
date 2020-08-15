using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameLevelDetailView : UIWindowViewBase 
{
    [SerializeField]
    private Text textTitle;
    [SerializeField]
    private Image imgPreview;
    [SerializeField]
    private UIGameLevelRewardView[] rewards;
    [SerializeField]
    private Text textGold;
    [SerializeField]
    private Text textEXP;
    [SerializeField]
    private Text textDesc;
    [SerializeField]
    private Text textConditionDesc;
    [SerializeField]
    private Text textRecommendFighting;

    private Color selectedGradeColor = Color.green;//已选中难度等级按钮颜色
    private Color normalGradeColor = Color.white;//默认难度等级按钮颜色
    [SerializeField]
    private Image[] btnGrades;

    private int m_GameLevelId;//当前关卡编号

    //关卡难度选择
    public delegate void OnBtnGradeClickHandler(int gameLevelId, GameLevelGrade grade);
    public OnBtnGradeClickHandler OnBtnGradeClick;

    //进入关卡
    public delegate void OnBtnEnterClickHandler(int gameLevelId, GameLevelGrade grade);
    public OnBtnEnterClickHandler OnBtnEnterClick;

    private GameLevelGrade m_CurrGrade;//当前选择关卡难度

    protected override void OnStart()
    {
        base.OnStart();

        if (btnGrades.Length > 0)
        {
            btnGrades[0].color = Color.green;
        }
    }

    /// <summary>
    /// 重置难度等级按钮颜色
    /// </summary>
    private void ResetBtnGradeColor()
    {
        for (int i = 0; i < btnGrades.Length; i++)
        {
            btnGrades[i].color = normalGradeColor;
        }

        
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);

        switch (go.name)
        {
            case "BtnNormal":
                BtnGradeClick(GameLevelGrade.Normal);
                break;
            case "BtnHard":
                BtnGradeClick(GameLevelGrade.Hard);
                break;
            case "BtnHell":
                BtnGradeClick(GameLevelGrade.Hell);
                break;
            case "BtnEnter":
                //进入关卡场景
                if (OnBtnEnterClick != null)
                {
                    OnBtnEnterClick(m_GameLevelId, m_CurrGrade);
                }
                break;
        }
    }

    private void BtnGradeClick(GameLevelGrade grade)
    {
        if (m_CurrGrade == grade) return;

        m_CurrGrade = grade;
        ResetBtnGradeColor();
        if (OnBtnGradeClick != null)
        {
            OnBtnGradeClick(m_GameLevelId, grade);
        }
        if (btnGrades.Length == 3)
        {
            btnGrades[(int)grade].color = selectedGradeColor;
        }
    }

    public void SetUI(TransferData data)
    {
        m_GameLevelId = data.GetValue<int>(ConstDefine.GameLevel_Id);//关卡编号
        textTitle.SetText(data.GetValue<string>(ConstDefine.GameLevel_Name));//关卡名称
        imgPreview.overrideSprite = GameUtil.LoadGameLevelDetailPic(data.GetValue<string>(ConstDefine.GameLevel_DlgPic));//关卡预览图

        textGold.SetText(data.GetValue<int>(ConstDefine.GameLevelGrade_Gold).ToString(), true);//过关奖励金币
        textEXP.SetText(data.GetValue<int>(ConstDefine.GameLevelGrade_Exp).ToString(), true);//过关奖励经验
        textDesc.SetText(data.GetValue<string>(ConstDefine.GameLevelGrade_Desc), true);//关卡描述
        textConditionDesc.SetText(data.GetValue<string>(ConstDefine.GameLevelGrade_ConditionDesc), true);//过关条件
        textRecommendFighting.SetText(data.GetValue<int>(ConstDefine.GameLevelGrade_CommendFighting).ToString(), true);//推荐战力

        //奖励物品列表
        List<TransferData> rewardList = data.GetValue<List<TransferData>>(ConstDefine.GameLevelGrade_RewardList);

        AppDebug.Log("reward count=" + rewardList.Count);

        for (int j = 0; j < rewards.Length; j++)
        {
            rewards[j].gameObject.SetActive(false);
        }

        if (rewardList.Count > 0)
        {
            for (int i = 0; i < rewardList.Count; i++)
            {
                rewards[i].gameObject.SetActive(true);
                rewards[i].SetUI(rewardList[i].GetValue<string>(ConstDefine.GoodsName), rewardList[i].GetValue<int>(ConstDefine.GoodsId), rewardList[i].GetValue<GoodsType>(ConstDefine.GoodsType));
            }
        }
    }
}
