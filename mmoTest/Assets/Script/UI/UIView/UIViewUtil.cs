using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIViewUtil : Singleton<UIViewUtil>
{
    private Dictionary<WindowUIType, UIWindowViewBase> m_DicWindow = new Dictionary<WindowUIType, UIWindowViewBase>();

    

    public int OpenWindowCount//当前打开的窗口数量
    {
        get { return m_DicWindow.Count; }
    }

    /// <summary>
    /// 关闭全部窗口
    /// </summary>
    public void CloseAllWindow()
    {
        if (m_DicWindow != null)
        {
            m_DicWindow.Clear();
        }
    }

    #region OpenWindow 如果窗口游戏物体存在 直接显示 没有则实例化后显示
    public GameObject OpenWindow(WindowUIType type, System.Action onShow = null)
    {
        if (type == WindowUIType.None) return null;

        GameObject obj = null;

        //如果窗口字典中不存在 或者为空
        if (!m_DicWindow.ContainsKey(type) || m_DicWindow[type] == null)
        {
            //将指定UI预制体实例化出来
            obj = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIWindow, string.Format("Panel_{0}", type.ToString()));
            if (obj == null) return null;

            UIWindowViewBase windowBase = obj.GetComponent<UIWindowViewBase>();
            if (windowBase == null) return null;

            if (m_DicWindow.ContainsKey(type))
            {
                m_DicWindow[type] = windowBase;
            }
            else
            {
                m_DicWindow.Add(type, windowBase);
            }
            
            windowBase.OnShow = onShow;
            windowBase.CurrentUIType = type;
            Transform transParent = null;
            switch (windowBase.containerType)
            {
                case WindowUIContainerType.Center:
                    transParent = SceneUIManager.Instance.CurrentUIScene.ContainerCenter;
                    break;
            }
            obj.transform.SetParent(transParent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.gameObject.SetActive(false);
            obj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            ShowOrHideWindowAnimation(windowBase, true);
        }
        else
        {
            obj = m_DicWindow[type].gameObject;

            ShowOrHideWindowAnimation(m_DicWindow[type], true);
        }

        LayerUIManager.Instance.SetLayer(obj);
        return obj;
    }
    #endregion

    /// <summary>
    /// 播放关闭动画 关闭窗口
    /// </summary>
    /// <param name="type"></param>
    public void CloseWindow(WindowUIType type)
    {
        if (m_DicWindow.ContainsKey(type))
        {
            ShowOrHideWindowAnimation(m_DicWindow[type], false);
        }
    }

    
    /// <summary>
    /// 打开/关闭 窗口动画
    /// </summary>
    /// <param name="windowBase"></param>
    /// <param name="isOpen"></param>
    private void ShowOrHideWindowAnimation(UIWindowViewBase windowBase, bool isOpen)
    {
        switch (windowBase.showStyle)
        {
            case WindowUIShowStyle.Normal:
                StartShowWindow(windowBase, isOpen);
                break;
            case WindowUIShowStyle.CenterToBig:
                ShowCenterToBig(windowBase, isOpen);
                break;
            case WindowUIShowStyle.FromTop:
                ShowFromDir(windowBase, 0, isOpen);
                break;
            case WindowUIShowStyle.FromDown:
                ShowFromDir(windowBase, 1, isOpen);
                break;
            case WindowUIShowStyle.FromLeft:
                ShowFromDir(windowBase, 2, isOpen);
                break;
            case WindowUIShowStyle.FromRight:
                ShowFromDir(windowBase, 3, isOpen);
                break;
        }
    }

    private void StartShowWindow(UIWindowViewBase windowBase, bool flag)
    {
        windowBase.gameObject.SetActive(flag);
    }

    /// <summary>
    /// 窗口从中心显示和隐藏
    /// </summary>
    private void ShowCenterToBig(UIWindowViewBase windowBase, bool isOpen)
    {
        windowBase.gameObject.SetActive(true);
        windowBase.transform.localScale = Vector3.zero;

        //OnRewind是tween倒播放完毕时的回调
        windowBase.transform.DOScale(Vector3.one, windowBase.Duration).SetAutoKill(false).Pause().OnRewind(() => {
            DestroyWindow(windowBase);
        });

        if (isOpen) windowBase.transform.DOPlayForward();
        else windowBase.transform.DOPlayBackwards();
    }

    /// <summary>
    /// 窗口从不同方向显示和隐藏
    /// </summary>
    private void ShowFromDir(UIWindowViewBase windowBase, int dir, bool isOpen)
    {
        Vector3 from = Vector3.zero;
        switch (dir)
        {
            case 0:
                from = new Vector3(0, 1000, 0);
                break;
            case 1:
                from = new Vector3(0, -1000, 0);
                break;
            case 2:
                from = new Vector3(-1400, 0, 0);
                break;
            case 3:
                from = new Vector3(1400, 0, 0);
                break;
        }
        windowBase.gameObject.SetActive(true);
        windowBase.transform.localPosition = from;
        windowBase.transform.DOLocalMove(Vector3.zero, windowBase.Duration).SetAutoKill(false).Pause().OnRewind(() =>{
            DestroyWindow(windowBase);
        });

        if (isOpen) windowBase.transform.DOPlayForward();
        else windowBase.transform.DOPlayBackwards();
    }

    private void DestroyWindow(UIWindowViewBase windowBase)
    {
        if (m_DicWindow.ContainsValue(windowBase))
        {
            m_DicWindow.Remove(windowBase.CurrentUIType);
            GameObject.Destroy(windowBase.gameObject);
        }
        
    }

}


