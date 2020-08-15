using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Menu 
{
	[MenuItem("CWBTools/Setting")]
	public static void Setting()
	{
//		SettingsWindow win = (SettingsWindow)EditorWindow.GetWindow (typeof(SettingsWindow));
//		win.titleContent = new GUIContent ("全局设置");
//		win.Show ();
        SettingWindow win = (SettingWindow)EditorWindow.GetWindow(typeof(SettingWindow));
		win.titleContent = new GUIContent ("全局设置");
		win.Show ();
	}

	[MenuItem("CWBTools/ResourcesManage/AssetBundleCreate")]
	public static void AssetBundleCreate()
	{
        //string path = Application.dataPath + "/../AssetBundles";
        //if(!Directory.Exists(path))
        //{
        //    Directory.CreateDirectory (path);
        //}
        
        AssetBundleWindow abWindow = EditorWindow.GetWindow<AssetBundleWindow>();
        abWindow.titleContent = new GUIContent("AB打包");
        abWindow.Show();
	}

    [MenuItem("CWBTools/ResourcesManage/初始资源拷贝到StreamingAssets")]
    public static void AssetBundleCopyToStreamingAssets()
    {
        string fromPath = Application.persistentDataPath;
        string toPath = Application.streamingAssetsPath + "/AssetBundles/";

        if (Directory.Exists(toPath))
        {
            Directory.Delete(toPath, true);
        }
        Directory.CreateDirectory(toPath);

        IOUtil.CopyDirectory(fromPath, toPath);
        AssetDatabase.Refresh();
        Debug.Log("copy complete");
    }

}
