using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SettingWindow : EditorWindow {

	private List<MacorItem> m_List = new List<MacorItem>();//保存所有自定义宏的列表
	private Dictionary<string, bool> m_Dic = new Dictionary<string, bool> ();//保存自定义宏的开启状态
    private string m_Macor = null;//当前用户设置中开启的宏字符串 例如：DEBUG_MODEL;DEBUG_LOG;STAT_TD

    public SettingWindow()
	{
		m_List.Clear ();
        m_List.Add(new MacorItem() { Name = "DEBUG_MODEL", DisplayName = "调试模式", IsDebug = true, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "DEBUG_LOG", DisplayName = "打印日志", IsDebug = true, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "STAT_TD", DisplayName = "开启统计", IsDebug = false, IsRelease = true });
        m_List.Add(new MacorItem() { Name = "DEBUG_ROLE_STATE", DisplayName = "调试角色状态", IsDebug = false, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "DISABLE_ASSETBUNDLE", DisplayName = "禁用AssetBundle", IsDebug = false, IsRelease = false });
	}

	private void OnEnable()
	{
        m_Macor = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

        //初始化每个自定义宏的当前状态
        for (int i = 0; i < m_List.Count; i++)
        {
            if (!string.IsNullOrEmpty(m_Macor) && m_Macor.IndexOf(m_List[i].Name) != -1)
            {
                m_Dic[m_List[i].Name] = true;
            }
            else
            {
                m_Dic[m_List[i].Name] = false;
            }

        }
	}

	void OnGUI()
	{
		m_Macor = PlayerSettings.GetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android);

		for(int i = 0; i < m_List.Count; i++)
		{
			EditorGUILayout.BeginHorizontal ("box");
			m_Dic [m_List [i].Name] = GUILayout.Toggle (m_Dic[m_List[i].Name], m_List[i].DisplayName);//toggle(bool值，显示名称)
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.BeginHorizontal ("box");
		if(GUILayout.Button("保存", GUILayout.Width(100)))
		{
			SaveMacor ();
		}
		if(GUILayout.Button("调试模式", GUILayout.Width(100)))
		{
			for (int i = 0; i < m_List.Count; i++) 
            {
				m_Dic [m_List [i].Name] = m_List [i].IsDebug;
			}
			SaveMacor ();
		}
		if(GUILayout.Button("发布模式", GUILayout.Width(100)))
		{
			for (int i = 0; i < m_List.Count; i++) 
            {
				m_Dic [m_List [i].Name] = m_List [i].IsRelease;
			}
			SaveMacor ();
		}
		EditorGUILayout.EndHorizontal ();
	}

	private void SaveMacor()
	{
		m_Macor = string.Empty;
		foreach(var item in m_Dic)
		{
			if(item.Value)
			{
				m_Macor += string.Format ("{0};", item.Key);
			}

            if (item.Key.Equals("DISABLE_ASSETBUNDLE", System.StringComparison.CurrentCultureIgnoreCase))
            {
                //如果禁用AssetBundle 就让Download下的场景生效
                EditorBuildSettingsScene[] sceneArr = EditorBuildSettings.scenes;
                for (int i = 0; i < sceneArr.Length; i++)
                {
                    //Debug.Log(sceneArr[i].path);
                    if (sceneArr[i].path.IndexOf("download", System.StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        sceneArr[i].enabled = item.Value;
                    }
                }
                EditorBuildSettings.scenes = sceneArr;
            }
		}
		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, m_Macor);
		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, m_Macor);
		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Standalone, m_Macor);
	}

	public class MacorItem
	{
		public string Name;
		public string DisplayName;
		public bool IsDebug;
		public bool IsRelease;
	}
}
