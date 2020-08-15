using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 登录 场景控制器
/// </summary>
public class LogOnSceneCtrl : MonoBehaviour 
{
    GameObject obj;
	
    void Awake()
    {
        //先加载出场景UI
        SceneUIManager.Instance.LoadSceneUI(SceneUIType.LogOn);
    }

	void Start () 
    {
        if (DelegateDefine.Instance.OnSceneLoadOK != null)
        {
            DelegateDefine.Instance.OnSceneLoadOK();
        }
	}
}
