using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCityRoleInfoView : MonoBehaviour 
{
    public static UIMainCityRoleInfoView Instance;

    [SerializeField]
    private Image imageHeadPic;
    [SerializeField]
    private Text textNickName;
    [SerializeField]
    private Text textLevel;
    [SerializeField]
    private Text textYuanBao;
    [SerializeField]
    private Text textGold;
    [SerializeField]
    private Image imageHPBar;
    [SerializeField]
    private Image imageMPBar;
	
    void Awake()
    {
        Instance = this;
    }

    public void SetUI(string headPic, string nickName, int level, int yuanbao, int gold, int currHP, int maxHP, int currMP, int maxMP)
    {
        Sprite headPicSprite = RoleMgr.Instance.LoadHeadPic(headPic);
        if (headPicSprite != null)
        {
            imageHeadPic.overrideSprite = headPicSprite;
        }
        textNickName.text = nickName;
        textLevel.text = "LV." + level;
        textYuanBao.text = yuanbao.ToString();
        textGold.text = gold.ToString();
        imageHPBar.fillAmount = (float)currHP / maxHP;
        imageMPBar.fillAmount = (float)currMP / maxMP;
    }

    public void SetHP(int currHP, int maxHP)
    {
        Debug.Log(string.Format("HP change CurrHP={0} MaxHP={1}", currHP, maxHP));
        imageHPBar.fillAmount = (float)currHP / maxHP;
    }

    public void SetMP(int currMP, int maxMP)
    {
        Debug.Log(string.Format("MP change CurrMP={0} MaxMP={1}", currMP, maxMP));
        imageMPBar.fillAmount = (float)currMP / maxMP;
    }

    void OnDestroy()
    {
        imageHeadPic = null;
        textNickName = null;
        textLevel = null;
        textYuanBao = null;
        textGold = null;
        imageHPBar = null;
        imageMPBar = null;
    }
}
