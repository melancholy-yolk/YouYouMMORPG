using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUIManager : Singleton<LayerUIManager> 
{
    private int orderInLayer = 50;

    public void Reset()
    {
        orderInLayer = 50;
    }

    /// <summary>
    /// 窗口被销毁时 会被调用
    /// </summary>
    public void CheckOpenWindow()
    {
        orderInLayer--;
        if (UIViewUtil.Instance.OpenWindowCount == 0)
        {
            Reset();
        }
    }

    /// <summary>
    /// 每次打开一个新的窗口时 排序加一 保证新打开的窗口在最上层显示
    /// </summary>
    /// <param name="obj"></param>
    public void SetLayer(GameObject obj)
    {
        orderInLayer++;
        Canvas m_Canvas = obj.GetComponent<Canvas>();
        m_Canvas.sortingOrder = orderInLayer;
    }

}
