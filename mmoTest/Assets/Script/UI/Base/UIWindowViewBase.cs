using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowViewBase : UIViewBase 
{
    [HideInInspector]
    public WindowUIType CurrentUIType;//��������
    public WindowUIContainerType containerType;//������������
    public WindowUIShowStyle showStyle;//�򿪷�ʽ
    public float Duration = 0.2f;//�򿪻�رն�������ʱ��
    private WindowUIType NextOpenType;//��һ��Ҫ�򿪵Ĵ���

    /// <summary>
    /// window��ui�����е�close��ť
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
    /// �رղ��Ҵ���һ������
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
