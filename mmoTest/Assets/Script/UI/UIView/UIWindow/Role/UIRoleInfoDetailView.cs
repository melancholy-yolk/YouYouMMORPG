using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoleInfoDetailView : MonoBehaviour 
{
    [SerializeField]
    private Text textMoney;
    [SerializeField]
    private Text textGold;
    [SerializeField]
    private Image imageHP;
    [SerializeField]
    private Image imageMP;
    [SerializeField]
    private Image imageEXP;
    [SerializeField]
    private Text textHP;
    [SerializeField]
    private Text textMP;
    [SerializeField]
    private Text textEXP;
    [SerializeField]
    private Text textAttack;
    [SerializeField]
    private Text textDefense;
    [SerializeField]
    private Text textHit;
    [SerializeField]
    private Text textDodge;
    [SerializeField]
    private Text textCri;
    [SerializeField]
    private Text textRes;
	
	void Start () {
		
	}

    public void SetUI(TransferData data)
    {
        textMoney.SetText(data.GetValue<int>(ConstDefine.Money).ToString());
        textGold.SetText(data.GetValue<int>(ConstDefine.Gold).ToString());

        imageHP.SetImageFill((float)data.GetValue<int>(ConstDefine.CurrHP) / data.GetValue<int>(ConstDefine.MAXHP));
        imageMP.SetImageFill((float)data.GetValue<int>(ConstDefine.CurrMP) / data.GetValue<int>(ConstDefine.MAXMP));
        imageEXP.SetImageFill((float)data.GetValue<int>(ConstDefine.MAXEXP) / 1000000);

        textHP.SetText(string.Format("{0}/{1}", data.GetValue<int>(ConstDefine.CurrHP), data.GetValue<int>(ConstDefine.MAXHP)));
        textMP.SetText(string.Format("{0}/{1}", data.GetValue<int>(ConstDefine.CurrMP), data.GetValue<int>(ConstDefine.MAXMP)));
        textEXP.SetText(string.Format("{0}/{1}", data.GetValue<int>(ConstDefine.MAXEXP), 1000000));

        textAttack.SetText(data.GetValue<int>(ConstDefine.Attack).ToString());
        textDefense.SetText(data.GetValue<int>(ConstDefine.Defense).ToString());
        textHit.SetText(data.GetValue<int>(ConstDefine.Hit).ToString());
        textDodge.SetText(data.GetValue<int>(ConstDefine.Dodge).ToString());
        textCri.SetText(data.GetValue<int>(ConstDefine.Cri).ToString());
        textRes.SetText(data.GetValue<int>(ConstDefine.Res).ToString());
    }
}
