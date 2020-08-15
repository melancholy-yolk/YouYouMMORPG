using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// ѡ���ɫ���� ɾ����ɫ������ͼ
/// </summary>
public class UISelectRoleDeleteRoleView : UIViewBase 
{
    [SerializeField]
    private Text textMessage;

    [SerializeField]
    private InputField inputOK;

    private Vector3 m_MoveTargetPos;

    private Action OnBtnOKClickCallback;

    protected override void OnAwake()
    {
        base.OnAwake();
        transform.localPosition = new Vector3(0, 1000, 0);
    }

    protected override void OnStart()
    {
        base.OnStart();
        transform.DOLocalMove(Vector3.zero, 0.2f).SetAutoKill(false).Pause();
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "BtnClose":
                Close();
                break;
            case "BtnOK":
                OnBtnOKClick();
                break;
        }
    }

    /// <summary>
    /// ���OK��ť
    /// </summary>
    private void OnBtnOKClick()
    {
        if (string.IsNullOrEmpty(inputOK.text) || !inputOK.text.Equals("ok", StringComparison.CurrentCultureIgnoreCase))
        {
            MessageController.Instance.Show("NOTICE", "Please Input:OK!");
            return;
        }
        if (OnBtnOKClickCallback != null)
        {
            OnBtnOKClickCallback();
        }
    }

    public void Show(string nickName, Action action)
    {
        OnBtnOKClickCallback = action;
        textMessage.text = string.Format("Are you sure delete role:{0}?", nickName);
        transform.DOPlayForward();
    }

    public void Close()
    {
        transform.DOPlayBackwards();
    }

    /// <summary>
    /// ������� �ͷ��ڴ�
    /// </summary>
    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();

        textMessage = null;
        inputOK = null;
        OnBtnOKClickCallback = null;
    }
}
