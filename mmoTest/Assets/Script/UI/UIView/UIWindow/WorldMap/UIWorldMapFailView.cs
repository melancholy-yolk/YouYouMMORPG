using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIWorldMapFailView : UIWindowViewBase 
{

    public Action OnReborn;//按钮点击事件执行委托 具体逻辑在Controller上编写
    public Action OnReturnMainCity;

    private Text text;

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);

        switch (go.name)
        {
            case "BtnReborn":
                if (OnReborn != null)
                {
                    OnReborn();
                }
                break;
            case "BtnReturn":
                if (OnReturnMainCity != null)
                {
                    OnReturnMainCity();
                }
                break;
        }
    }

    public void SetUI(string enemyNickName)
    {
        text.SetText(enemyNickName);
    }
}
