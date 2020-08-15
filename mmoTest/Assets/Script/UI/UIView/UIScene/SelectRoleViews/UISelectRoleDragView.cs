using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectRoleDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 m_DragBeginPos = Vector2.zero;//��ʼ��ק��λ��
    private Vector2 m_DragEndPos = Vector2.zero;//������ק��λ��
    public Action<int> OnSelectRoleDrag;//��קί�� ������ʾ��/��
	
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

        //20���ݴ�Χ ��ָˮƽ�ƶ����벻����20��������
        if (x > 20)//����
        {
            OnSelectRoleDrag(0);
        }
        else if (x < -20)//����
        {
            OnSelectRoleDrag(1);
        }
    }

    /// <summary>
    /// ����������ָ��� �����ڴ�
    /// </summary>
    void OnDestroy()
    {
        OnSelectRoleDrag = null;
    }
}
