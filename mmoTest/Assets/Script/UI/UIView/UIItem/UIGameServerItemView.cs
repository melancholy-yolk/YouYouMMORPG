using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameServerItemView : MonoBehaviour 
{
    [SerializeField]
    private Image imageRunStatus;
    [SerializeField]
    private Text textServerName;

    private Button btn;

    [SerializeField]
    private Sprite[] gameServerStatusIcon;
    private Sprite currRunStatusIcon;

    private RetGameServerEntity curData;
    public Action<RetGameServerEntity> OnGameServerItemClick;

	void Awake () 
    {
		btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
	}

    private void OnBtnClick()
    {
        if (OnGameServerItemClick != null)
        {
            OnGameServerItemClick(curData);
        }
    }

    public void SetUI(RetGameServerEntity entity)
    {
        textServerName.text = entity.Name;
        currRunStatusIcon = gameServerStatusIcon[entity.RunStatus];
        imageRunStatus.overrideSprite = currRunStatusIcon;

        curData = entity;
    }
}
