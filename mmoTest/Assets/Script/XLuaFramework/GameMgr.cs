using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�߼���ʼ���
/// </summary>
public class GameMgr : MonoBehaviour 
{
    void Awake()
    {
        gameObject.AddComponent<LuaMgr>();
    }
	
	void Start () 
    {
        //����ִ�е�һ��lua�ű�
        LuaMgr.Instance.DoString("require 'Download/XLuaLogic/LuaTest'");
	}
	
	
	
}
