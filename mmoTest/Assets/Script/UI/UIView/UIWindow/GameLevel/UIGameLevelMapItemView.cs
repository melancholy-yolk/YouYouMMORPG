using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIGameLevelMapItemView : MonoBehaviour 
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Text text;
    private Button btn;

    private int m_GameLevelId;

    public Action<int> OnGameLevelItemClick;
	
    public void SetUI(TransferData data, Action<int> callback)
    {
        OnGameLevelItemClick = callback;

        m_GameLevelId = data.GetValue<int>(ConstDefine.GameLevel_Id);

        text.SetText(data.GetValue<string>(ConstDefine.GameLevel_Name));
        image.overrideSprite = GameUtil.LoadGameLevelMapItemIcon(data.GetValue<string>(ConstDefine.GameLevel_Ico));
        image.SetNativeSize();
    }

	void Start () 
    {
		btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
	}

    private void OnBtnClick()
    {
        if (OnGameLevelItemClick != null)
        {
            OnGameLevelItemClick(m_GameLevelId);
        }
    }

}
