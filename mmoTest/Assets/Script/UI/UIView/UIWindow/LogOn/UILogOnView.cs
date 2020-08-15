using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogOnView : UIWindowViewBase 
{
    public InputField userNameInput;
    public InputField pwdInput;

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        if (go.name == "BtnToReg")
        {
            UIDispatcher.Instance.Dispatch(ConstDefine.UILogOnView_BtnToReg);
        }
        else if (go.name == "BtnLogin")
        {
            UIDispatcher.Instance.Dispatch(ConstDefine.UILogOnView_BtnLogOn);
        }
    }

}
