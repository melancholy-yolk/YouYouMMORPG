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
        m_MoveTargetPos = transform.localPosition + new Vector3(0, 100, 0);//Ҫ�ƶ�����Ŀ��λ��
        transform.DOLocalMove(m_MoveTargetPos, 0.2f).SetAutoKill(false).Pause().OnComplete(() =>
        {
            if (m_OnChangeSuccess != null) m_OnChangeSuccess();
            isBusy = false;//�����������
        }).OnRewind(() => {
            if (m_OnChangeSuccess != null) m_OnChangeSuccess();
            isBusy = false;
        });
    }

    public void ChangeState(Action OnChangeSuccess)
    {
        if (isBusy) return;//���������е����Ч
        isBusy = true;//��ʼ���Ŷ���
        m_OnChangeSuccess = OnChangeSuccess;
        if (isShow)//����
        {
            transform.DOPlayForward();
        }
        else//��ʾ
        {
            transform.DOPlayBackwards();
        }
        isShow = !isShow;
        
    }
}
