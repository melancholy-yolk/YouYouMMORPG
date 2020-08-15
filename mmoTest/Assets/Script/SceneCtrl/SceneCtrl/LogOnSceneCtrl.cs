using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��¼ ����������
/// </summary>
public class LogOnSceneCtrl : MonoBehaviour 
{
    GameObject obj;
	
    void Awake()
    {
        //�ȼ��س�����UI
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
