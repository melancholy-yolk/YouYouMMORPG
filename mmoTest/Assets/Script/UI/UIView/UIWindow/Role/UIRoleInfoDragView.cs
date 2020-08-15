using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


public class UIRoleInfoDragView : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private Transform m_Target;

    private Vector2 m_DragBeginPos = Vector2.zero;//��ʼ��ק��λ��
    private Vector2 m_DragEndPos = Vector2.zero;//������ק��λ��

    private float m_Speed = 300f;//��ת�ٶ�

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_DragBeginPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_Target == null)
        {
            return;
        }
        m_DragEndPos = eventData.position;
        float x = m_DragBeginPos.x - m_DragEndPos.x;
        m_Target.Rotate(0, Time.deltaTime * m_Speed * (x > 0 ? 1 : -1), 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    /// <summary>
    /// ����������ָ��� �����ڴ�
    /// </summary>
    void OnDestroy()
    {

    }
}
