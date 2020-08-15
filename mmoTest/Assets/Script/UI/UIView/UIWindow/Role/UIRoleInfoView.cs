using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoleInfoView : UIWindowViewBase 
{
    [SerializeField]
    private UIRoleInfoEquipView m_UIRoleInfoEquipView;//角色装备子视图
    [SerializeField]
    private UIRoleInfoDetailView m_UIRoleInfoDetailView;//角色数值子视图

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
