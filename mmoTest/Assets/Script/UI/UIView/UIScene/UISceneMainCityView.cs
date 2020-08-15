using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 主城View
/// </summary>
public class UISceneMainCityView : UISceneViewBase 
{
    [SerializeField]
    private Toggle m_AutoFightToggle;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();

        if (OnLoadComplete != null)
        {
            OnLoadComplete();
        }

        #region 主角自动战斗按钮
        m_AutoFightToggle.onValueChanged.AddListener(OnToggleValueChange);
        m_AutoFightToggle.isOn = false;
        GlobalInit.Instance.MainPlayer.Attack.IsAutoFight = m_AutoFightToggle.isOn;

        if (GameLevelSceneCtrl.Instance == null)
        {
            m_AutoFightToggle.gameObject.SetActive(false);
        }
        else
        {
            m_AutoFightToggle.gameObject.SetActive(true);
        }
        #endregion
    }

    private void OnToggleValueChange(bool arg0)
    {
        GlobalInit.Instance.MainPlayer.Attack.IsAutoFight = m_AutoFightToggle.isOn;
    }



    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "BtnTopMenu":
                ChangeMenuState(go);
                break;
            case "Role":
                UIViewManager.Instance.OpenView(WindowUIType.RoleInfo);
                break;
            case "GameLevel":
                UIViewManager.Instance.OpenView(WindowUIType.GameLevelMap);
                break;
            case "SmallMap":
                UIViewManager.Instance.OpenView(WindowUIType.WorldMap);
                break;
        }
    }

    private void ChangeMenuState(GameObject go)
    {
        AppDebug.Log("btn fold click!");
        UIMainCityMenusView.Instance.ChangeState(() => {
            go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y * -1, go.transform.localScale.z);
        });
    }
    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
    }
}
