using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowViewBase : UIViewBase 
{
    [HideInInspector]
    public WindowUIType CurrentUIType;//窗口类型
    public WindowUIContainerType containerType;//窗口容器类型
    public WindowUIShowStyle showStyle;//打开方式
    public float Duration = 0.2f;//打开或关闭动画持续时间
    private WindowUIType NextOpenType;//下一个要打开的窗口

    /// <summary>
    /// window型ui都具有的close按钮
    /// </summary>
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        if (go.name.Equals("ButtonClose", System.StringComparison.CurrentCultureIgnoreCase))
        {
            Close();
        }
    }

    public virtual void Close()
    {
        UIViewUtil.Instance.CloseWindow(CurrentUIType);
    }

    /// <summary>
    /// 关闭并且打开下一个窗口
    /// </summary>
    /// <param name="nextType"></param>
    public virtual void CloseAndOpenNext(WindowUIType nextType)
    {
        this.Close();
        NextOpenType = nextType;
        if (NextOpenType != WindowUIType.None)
        {
            UIViewManager.Instance.OpenView(NextOpenType);
        }
    }

    protected override void BeforeOnDestroy()
    {
        LayerUIManager.Instance.CheckOpenWindow();
    }
}
