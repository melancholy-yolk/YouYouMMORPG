using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UIMainCityMenusView : MonoBehaviour
{
    public static UIMainCityMenusView Instance;

    private Vector3 m_MoveTargetPos;

    private bool isShow;

    private Action m_OnChangeSuccess;
    private bool isBusy = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        isShow = true;
        m_MoveTargetPos = transform.localPosition + new Vector3(0, 100, 0);//要移动到的目标位置
        transform.DOLocalMove(m_MoveTargetPos, 0.2f).SetAutoKill(false).Pause().OnComplete(() =>
        {
            if (m_OnChangeSuccess != null) m_OnChangeSuccess();
            isBusy = false;//动画播放完毕
        }).OnRewind(() => {
            if (m_OnChangeSuccess != null) m_OnChangeSuccess();
            isBusy = false;
        });
    }

    public void ChangeState(Action OnChangeSuccess)
    {
        if (isBusy) return;//动画播放中点击无效
        isBusy = true;//开始播放动画
        m_OnChangeSuccess = OnChangeSuccess;
        if (isShow)//隐藏
        {
            transform.DOPlayForward();
        }
        else//显示
        {
            transform.DOPlayBackwards();
        }
        isShow = !isShow;
        
    }
}
