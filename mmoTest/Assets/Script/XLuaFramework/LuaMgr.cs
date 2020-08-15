using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaMgr : MonoBehaviour
{
    public static LuaMgr Instance;

    /// <summary>
    /// ȫ�ֵ�xlua����
    /// </summary>
    public static LuaEnv luaEnv;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        //1.ʵ����xlua����
        luaEnv = new LuaEnv();

        //2.����xlua�Ľű�·��
        luaEnv.DoString(string.Format("package.path = '{0}/?.lua'", Application.dataPath));
    }

    /// <summary>
    /// ִ��lua�ű�
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
