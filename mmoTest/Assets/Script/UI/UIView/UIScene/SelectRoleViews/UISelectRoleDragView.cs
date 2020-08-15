using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectRoleDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 m_DragBeginPos = Vector2.zero;//开始拖拽的位置
    private Vector2 m_DragEndPos = Vector2.zero;//结束拖拽的位置
    public Action<int> OnSelectRoleDrag;//拖拽委托 参数表示左/右
	
	void Start () {
		
	}
	
	
	void Update () {
		
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_DragBeginPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_DragEndPos = eventData.position;
        float x = m_DragBeginPos.x - m_DragEndPos.x;

        //20是容错范围 手指水平移动距离不超过20不做处理
        if (x > 20)//向左
        {
            OnSelectRoleDrag(0);
        }
        else if (x < -20)//向右
        {
            OnSelectRoleDrag(1);
        }
    }

    /// <summary>
    /// 将引用类型指向空 回收内存
    /// </summary>
    void OnDestroy()
    {
        OnSelectRoleDrag = null;
    }
}
