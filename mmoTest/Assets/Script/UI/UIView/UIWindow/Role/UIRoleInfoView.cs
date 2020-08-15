using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoleInfoView : UIWindowViewBase 
{
    [SerializeField]
    private UIRoleInfoEquipView m_UIRoleInfoEquipView;//��ɫװ������ͼ
    [SerializeField]
    private UIRoleInfoDetailView m_UIRoleInfoDetailView;//��ɫ��ֵ����ͼ

    public void SetRoleInfo(TransferData data)
    {
        m_UIRoleInfoEquipView.SetUI(data);
        m_UIRoleInfoDetailView.SetUI(data);
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
    }
}
