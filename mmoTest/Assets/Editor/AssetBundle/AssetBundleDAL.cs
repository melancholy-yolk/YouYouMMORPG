using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;

/// <summary>
/// AssetBundleEntity管理器 从AssetBundleConfig.xml文件中读取出ab实体对象 数据访问层(DAL)
/// </summary>
public class AssetBundleDAL  
{
    /// <summary>
    /// xml路径
    /// </summary>
    private string m_Path;

    /// <summary>
    /// 返回的数据集合
    /// </summary>
    private List<AssetBundleEntity> m_List = null;

    public AssetBundleDAL(string path)
    {
        m_Path = path;
        m_List = new List<AssetBundleEntity>();
    }

    public List<AssetBundleEntity> GetList()
    {
        m_List.Clear();

        XDocument xDoc = XDocument.Load(m_Path);
        XElement root = xDoc.Root;

        XElement assetBundleNode = root.Element("AssetBundle");

        IEnumerable<XElement> lst = assetBundleNode.Elements("Item");

        int index = 0;
        foreach(XElement item in lst)
        {
            AssetBundleEntity entity = new AssetBundleEntity();
            entity.Key = "key" + ++index;
            entity.Name = item.Attribute("Name").Value;
            entity.Tag = item.Attribute("Tag").Value;
            entity.IsFolder = item.Attribute("IsFolder").Value.Equals("True", System.StringComparison.CurrentCultureIgnoreCase);
            entity.IsFirstData = item.Attribute("IsFirstData").Value.Equals("True", System.StringComparison.CurrentCultureIgnoreCase);

            IEnumerable<XElement> pathList = item.Elements("Path");
            foreach (XElement path in pathList)
            {
                entity.PathList.Add(path.Attribute("Value").Value);
            }

            m_List.Add(entity);
        }

        return m_List;
    }
}
