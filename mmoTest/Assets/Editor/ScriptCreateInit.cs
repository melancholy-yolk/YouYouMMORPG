using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

public class ScriptCreateInit : UnityEditor.AssetModificationProcessor 
{
	private static void OnWillCreateAsset(string path)
	{
		path = path.Replace (".meta", "");
		if(path.EndsWith(".cs"))
		{
			string strContent = File.ReadAllText (path);
			strContent = strContent.Replace ("#AuthorName#", "CWB").Replace ("#CreateTime#", DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"));
			File.WriteAllText (path, strContent);
			AssetDatabase.Refresh ();
		}
	}
}
