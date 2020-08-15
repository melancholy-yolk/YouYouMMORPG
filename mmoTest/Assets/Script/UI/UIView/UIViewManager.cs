using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIViewManager : Singleton<UIViewManager> 
{

    private Dictionary<WindowUIType, ISystemController> m_SystemCtrollerDic = new Dictionary<WindowUIType, ISystemController>();

    public UIViewManager()
    {
        m_SystemCtrollerDic.Add(WindowUIType.LogOn, AccountController.Instance);
        m_SystemCtrollerDic.Add(WindowUIType.Reg, AccountController.Instance);

        m_SystemCtrollerDic.Add(WindowUIType.GameServerEnter, GameServerController.Instance);
        m_SystemCtrollerDic.Add(WindowUIType.GameServerSelect, GameServerController.Instance);

        m_SystemCtrollerDic.Add(WindowUIType.RoleInfo, PlayerController.Instance);

        m_SystemCtrollerDic.Add(WindowUIType.GameLevelMap, GameLevelController.Instance);
        m_SystemCtrollerDic.Add(WindowUIType.GameLevelDetail, GameLevelController.Instance);

        m_SystemCtrollerDic.Add(WindowUIType.WorldMap, WorldMapController.Instance);
    }

    public void OpenView(WindowUIType type)
    {
        m_SystemCtrollerDic[type].OpenView(type);
    }

}
