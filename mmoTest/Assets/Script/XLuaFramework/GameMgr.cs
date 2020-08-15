using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏逻辑开始入口
/// </summary>
public class GameMgr : MonoBehaviour 
{
    void Awake()
    {
        gameObject.AddComponent<LuaMgr>();
    }
	
	void Start () 
    {
        //这里执行第一个lua脚本
        LuaMgr.Instance.DoString("require 'Download/XLuaLogic/LuaTest'");
	}
	
	
	
}
