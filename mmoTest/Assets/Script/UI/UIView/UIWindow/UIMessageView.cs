using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageView : MonoBehaviour 
{
    [SerializeField]
    private Text textTitle;

    [SerializeField]
    private Text textContent;

    [SerializeField]
    private Button btnOK;

    [SerializeField]
    private Button btnCancel;

    public Action OnBtnOKClickHandler;
    public Action OnBtnCancelClickHandler;

    void Awake()
    {
        EventTriggerListener.Get(btnOK.gameObject).onClick = BtnOKClickCallBack;
        EventTriggerListener.Get(btnCancel.gameObject).onClick = BtnCancelClickCallBack;
    }

    private void BtnCancelClickCallBack(GameObject go)
    {
        if (OnBtnCancelClickHandler != null) OnBtnCancelClickHandler();
        Close();
    }

    private void BtnOKClickCallBack(GameObject go)
    {
        if (OnBtnOKClickHandler != null) OnBtnOKClickHandler();
        Close();
    }

    /// <summary>
    /// ��ʾ����
    /// </summary>
    /// <param name="title">����</param>
    /// <param name="content">����</param>
    /// <param name="type">����</param>
    /// <param name="okAction">ȷ���ص�</param>
    /// <param name="cancelAction">ȡ���ص�</param>
    public void Show(string title, string content, MessageViewType type = MessageViewType.OK, Action okAction = null, Action cancelAction = null)
    {
        gameObject.transform.localPosition = Vector3.zero;

        textTitle.text = title;
        textContent.text = content;

        switch (type)
        {
            case MessageViewType.OK:
                btnOK.transform.localPosition = Vector3.zero;
                btnCancel.gameObject.SetActive(false);
                break;
            case MessageViewType.OKAndCancel:
                btnOK.transform.localPosition = new Vector3(-90, 0, 0);
                btnCancel.gameObject.SetActive(true);
                break;
        }

        OnBtnOKClickHandler = okAction;
        OnBtnCancelClickHandler = cancelAction;
    }

    private void Close()
    {
        gameObject.transform.localPosition = new Vector3(0, 5000, 0);
    }

}
