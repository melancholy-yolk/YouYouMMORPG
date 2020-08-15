using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AssetBundleʵ��
/// </summary>
public class AssetBundleEntity  
{
    public string Key;
    public string Name;//ab����
    public string Tag;//���ͱ�ǩ
    public bool IsFolder;//�Ƿ��ļ���
    public bool IsFirstData;//�Ƿ��ʼ��Դ
    public bool IsChecked;//�Ƿ�ѡ��

    private List<string> m_PathList = new List<string>();//Ҫ��ab�����ļ���ַ�б�

    public List<string> PathList
    {
        get { return m_PathList; }
    }
}
