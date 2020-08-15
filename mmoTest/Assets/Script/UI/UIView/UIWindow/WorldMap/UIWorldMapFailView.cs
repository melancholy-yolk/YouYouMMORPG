using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIWorldMapFailView : UIWindowViewBase 
{

    public Action OnReborn;//��ť����¼�ִ��ί�� �����߼���Controller�ϱ�д
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
