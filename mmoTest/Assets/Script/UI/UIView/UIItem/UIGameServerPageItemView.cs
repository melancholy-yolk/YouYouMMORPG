using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameServerPageItemView : MonoBehaviour 
{
    private int m_PageIndex;
    private Text textName;
    private Button btn;
    public Action<int> OnGameServerPageClick;
	
	void Awake () {
		textName = transform.Find("Text").GetComponent<Text>();
        btn = GetComponent<Button>();
        btn.onClick.AddListener(GameServerPageClick);
	}

    private void GameServerPageClick()
    {
        if (OnGameServerPageClick != null)
        {
            OnGameServerPageClick(m_PageIndex);
        }
    }

    public void SetUI(RetGameServerPageEntity entity)
    {
        m_PageIndex = entity.PageIndex;
        textName.text = entity.Name;
    }
}
