using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AssetBundle实体
/// </summary>
public class AssetBundleEntity  
{
    public string Key;
    public string Name;//ab包名
    public string Tag;//类型标签
    public bool IsFolder;//是否文件夹
    public bool IsFirstData;//是否初始资源
    public bool IsChecked;//是否被选中

    private List<string> m_PathList = new List<string>();//要打ab包的文件地址列表

    public List<string> PathList
    {
        get { return m_PathList; }
    }
}
