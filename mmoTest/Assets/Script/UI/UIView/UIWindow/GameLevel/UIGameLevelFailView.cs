using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIGameLevelFailView : UIWindowViewBase 
{
    public Action OnReborn;//��ť����¼�ִ��ί�� �����߼���Controller�ϱ�д
    public Action OnReturnMainCity;

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
}
