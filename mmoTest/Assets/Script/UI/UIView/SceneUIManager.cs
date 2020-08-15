using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 管理所有的场景View
/// </summary>
public class SceneUIManager : Singleton<SceneUIManager> 
{
    

    public UISceneViewBase CurrentUIScene;

    public GameObject LoadSceneUI(SceneUIType type, Action OnLoadComplete = null)
    {
        GameObject obj = null;
        switch (type)
        {
            case SceneUIType.Loading:
                break;
            case SceneUIType.LogOn:
                obj = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIScene, "UI_Root_LogOn");
                CurrentUIScene = obj.GetComponent<UISceneViewBase>();
                break;
            case SceneUIType.SelectRole:
                obj = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIScene, "UI_Root_SelectRole");
                CurrentUIScene = obj.GetComponent<UISceneViewBase>();
                break;
            case SceneUIType.MainCity:
                obj = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIScene, "UI_Root_MainCity");
                CurrentUIScene = obj.GetComponent<UISceneViewBase>();
                break;
        }
        CurrentUIScene.OnLoadComplete = OnLoadComplete;
        return obj;
    }
}
