using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameServerEnterView : UIWindowViewBase 
{
    public Text textDefaultGameServer;

    public void SetUI(string gameServerName)
    {
        textDefaultGameServer.text = gameServerName;
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "BtnSelectGameServer":
                UIDispatcher.Instance.Dispatch(ConstDefine.UIGameServerEnterView_BtnSelectGameServer);
                break;
            case "BtnEnterGame":
                UIDispatcher.Instance.Dispatch(ConstDefine.UIGameServerEnterView_BtnEnterGame);
                break;
        }
    }
}
