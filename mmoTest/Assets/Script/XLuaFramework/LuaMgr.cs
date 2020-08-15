using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaMgr : MonoBehaviour
{
    public static LuaMgr Instance;

    /// <summary>
    /// 全局的xlua引擎
    /// </summary>
    public static LuaEnv luaEnv;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        //1.实例化xlua引擎
        luaEnv = new LuaEnv();

        //2.设置xlua的脚本路径
        luaEnv.DoString(string.Format("package.path = '{0}/?.lua'", Application.dataPath));
    }

    /// <summary>
    /// 执行lua脚本
    /// </summary>
    /// <param name="str"></param>
    public void DoString(string str)
    {
        luaEnv.DoString(str);
    }

	void Start () 
    {
		
	}
}
