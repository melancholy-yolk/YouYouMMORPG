using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SystemControllerBase<T> : IDisposable where T : new()
{
    #region ����
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
    #endregion

    public virtual void Dispose()
    {
        
    }

    /// <summary>
    /// ������ʾ����
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    /// <param name="type"></param>
    /// <param name="okAction"></param>
    /// <param name="cancelAction"></param>
    protected void ShowMessage(string title, string content, MessageViewType type = MessageViewType.OK, Action okAction = null, Action cancelAction = null)
    {
        MessageController.Instance.Show(title, content, type, okAction, cancelAction);
    }

    /// <summary>
    /// ��Ӽ���
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    protected void AddEventListener(string key, DispatcherBase<UIDispatcher, object[], string>.OnActionHandler handler)
    {
        UIDispatcher.Instance.AddEventListener(key, handler);
    }

    /// <summary>
    /// �Ƴ�����
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    protected void RemoveEventListener(string key, DispatcherBase<UIDispatcher, object[], string>.OnActionHandler handler)
    {
        UIDispatcher.Instance.RemoveEventListener(key, handler);
    }
}
