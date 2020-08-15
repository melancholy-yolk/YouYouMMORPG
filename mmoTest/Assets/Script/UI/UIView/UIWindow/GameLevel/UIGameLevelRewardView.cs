using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameLevelRewardView : MonoBehaviour 
{
    [SerializeField]
    private Image imgIcon;
    [SerializeField]
    private Text textName;
	
    public void SetUI(string name, int goodsId, GoodsType type)
    {
        imgIcon.overrideSprite = GameUtil.LoadGoodsIcon(goodsId, type);
        textName.SetText(name);
    }

	void Start () 
    {
		
	}

    void OnDestroy()
    {
        imgIcon = null;
        textName = null;
    }

}
