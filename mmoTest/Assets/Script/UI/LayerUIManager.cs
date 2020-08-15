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
    /// ���ڱ�����ʱ �ᱻ����
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
    /// ÿ�δ�һ���µĴ���ʱ �����һ ��֤�´򿪵Ĵ��������ϲ���ʾ
    /// </summary>
    /// <param name="obj"></param>
    public void SetLayer(GameObject obj)
    {
        orderInLayer++;
        Canvas m_Canvas = obj.GetComponent<Canvas>();
        m_Canvas.sortingOrder = orderInLayer;
    }

}
