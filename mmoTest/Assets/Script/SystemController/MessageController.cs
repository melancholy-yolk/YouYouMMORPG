using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MessageController : Singleton<MessageController>
{
    private GameObject messageView;

    public void Show(string title, string content, MessageViewType type = MessageViewType.OK, Action okAction = null, Action cancelAction = null)
    {
        if (messageView == null)
        {
            messageView = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIWindow, "Panel_Message");
        }

        messageView.transform.SetParent(SceneUIManager.Instance.CurrentUIScene.ContainerCenter);
        messageView.transform.localPosition = Vector3.zero;
        messageView.transform.localScale = Vector3.one;
        messageView.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        UIMessageView view = messageView.GetComponent<UIMessageView>();
        if (view != null)
        {
            view.Show(title, content, type, okAction, cancelAction);
        }
    }
}
