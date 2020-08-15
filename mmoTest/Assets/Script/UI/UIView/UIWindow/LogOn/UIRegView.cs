using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegView : UIWindowViewBase 
{
    public InputField userNameInput;
    public InputField pwdInput;


    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        if (go.name == "BtnReg")
        {
            UIDispatcher.Instance.Dispatch(ConstDefine.UIRegView_BtnReg);
        }
        else if (go.name == "BtnBackLogin")
        {
            UIDispatcher.Instance.Dispatch(ConstDefine.UIRegView_BtnBackLogin);
        }
    }
}
