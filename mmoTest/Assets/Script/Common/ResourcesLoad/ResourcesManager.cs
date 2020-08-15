using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager> 
{
    private Hashtable PrefabTable;

    public ResourcesManager()
    {
        PrefabTable = new Hashtable();
    }

    public enum ResourceType
    {
        None,
        UIWindow,
        UIScene,
        Role,
        UIItem,
    }

    public GameObject Load(ResourceType type, string name)
    {
        GameObject obj = null;
        GameObject temp = Resources.Load<GameObject>("UIPrefab/" + type.ToString() + "/" + name);
        if (temp != null)
        {
            obj = GameObject.Instantiate<GameObject>(temp);
        }
        return obj;
    }

    public GameObject Load(ResourceType type, string module, string name)
    {
        GameObject obj = null;
        GameObject temp = Resources.Load<GameObject>(string.Format("UIPrefab/{0}/{1}/{2}", type.ToString(), module, name));
        if (temp != null)
        {
            obj = GameObject.Instantiate<GameObject>(temp);
        }
        return obj;
    }

    /// <summary>
    /// 从Resources文件夹下加载资源
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="path">资源路径</param>
    /// <param name="cache">是否缓存</param>
    /// <param name="returnClone">是否返回克隆体</param>
    /// <returns></returns>
    public GameObject Load(ResourceType type, string path, bool cache = false, bool returnClone = true)
    {
        GameObject obj = null;
        if (PrefabTable.Contains(path))
        {
            obj = PrefabTable[path] as GameObject;
        }
        else
        {
            StringBuilder sbr = new StringBuilder();
            switch (type)
            {
                case ResourceType.UIWindow:
                    sbr.Append("UIPrefab/UIWindow/");
                    break;
                case ResourceType.UIScene:
                    sbr.Append("UIPrefab/UIScene/");
                    break;
                case ResourceType.UIItem:
                    sbr.Append("UIPrefab/UIItem/");
                    break;
            }
            sbr.Append(path);

            obj = Resources.Load(sbr.ToString()) as GameObject;
            if (cache)
            {
                PrefabTable.Add(path, obj);
            }
        }

        return returnClone ? Object.Instantiate<GameObject>(obj) : obj;
    }

    public GameObject LoadCommonPrefab(string prefabName)
    {
        GameObject obj = Resources.Load("Prefab/Common/" + prefabName) as GameObject;
        return Object.Instantiate<GameObject>(obj);
    }
}
