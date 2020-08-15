using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FingerEventCenter : MonoBehaviour
{
    public static FingerEventCenter Instance;

    public enum FingerDragDir
    {
        None,
        Left,
        Rigth,
        Up,
        Down
    }

    public enum ZoomType
    {
        None,
        In,
        Out
    }

    private FingerDragDir m_FingerDragDir = FingerDragDir.None;
    private ZoomType m_ZoomType = ZoomType.None;

    public Action OnPlayerClickGround;

    public Action<FingerDragDir> OnFingerDrag;
    public Action<ZoomType> OnZoom;

    private Vector2 m_OldFingerPos;//��ָ��һ�ε�λ��
    private Vector2 m_Dir;//��ָ��������
    private int m_PreviousFinger;//��һ�β�������

    //�ƶ���˫ָ����
    private Vector2 m_TempFinger1Pos;
    private Vector2 m_TempFinger2Pos;
    private Vector2 m_OldFinger1Pos;
    private Vector2 m_OldFinger2Pos;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (OnZoom != null)
            {
                OnZoom(ZoomType.In);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (OnZoom != null)
            {
                OnZoom(ZoomType.Out);
            }
        }
#elif UNITY_ANDROID || UNITY_IPHONE
        if (Input.touchCount > 1)
        {
            //����һ����ָ���ƶ�
            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                m_TempFinger1Pos = Input.GetTouch(0).position;
                m_TempFinger2Pos = Input.GetTouch(1).position;

                if (Vector2.Distance(m_OldFinger1Pos, m_OldFinger2Pos) < Vector2.Distance(m_TempFinger1Pos, m_TempFinger2Pos))
                {
                    //�Ŵ�
                    OnZoom(ZoomType.In);
                }
                else
                {
                    //��С
                    OnZoom(ZoomType.Out);
                }

                m_OldFinger1Pos = m_TempFinger1Pos;
                m_OldFinger2Pos = m_TempFinger2Pos;
            }
        }
#endif


    }

    private void OnFingerDown(FingerDownEvent eventData)
    {
        m_PreviousFinger = 1;//��ָ����
    }

    private void OnFingerUp(FingerUpEvent eventData)
    {
        if (m_PreviousFinger == 1)
        {
            m_PreviousFinger = -1;
            if (OnPlayerClickGround != null)
            {
                OnPlayerClickGround();
            }
        }
    }

    int dragFingerIndex = -1;
    void OnDrag(DragGesture gesture)
    {
        // first finger
        FingerGestures.Finger finger = gesture.Fingers[0];

        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            // remember which finger is dragging dragObject
            dragFingerIndex = finger.Index;
            m_PreviousFinger = 2;//��ָ��ק��ʼ
            m_OldFingerPos = finger.Position;
        }
        else if (finger.Index == dragFingerIndex)  // gesture in progress, make sure that this event comes from the finger that is dragging our dragObject
        {
            if (gesture.Phase == ContinuousGesturePhase.Updated)
            {
                m_PreviousFinger = 3;//��ָ��ק��
                m_Dir = finger.Position - m_OldFingerPos;

                if (m_Dir.y < m_Dir.x && m_Dir.y > -m_Dir.x)//����
                {
                    m_FingerDragDir = FingerDragDir.Rigth;
                }
                else if (m_Dir.y > m_Dir.x && m_Dir.y < -m_Dir.x)//����
                {
                    m_FingerDragDir = FingerDragDir.Left;
                }
                else if (m_Dir.y > m_Dir.x && m_Dir.y > -m_Dir.x)//����
                {
                    m_FingerDragDir = FingerDragDir.Up;
                }
                else if (m_Dir.y < m_Dir.x && m_Dir.y < -m_Dir.x)//����
                {
                    m_FingerDragDir = FingerDragDir.Down;
                }

                if (OnFingerDrag != null)
                {
                    OnFingerDrag(m_FingerDragDir);
                }
            }
            else
            {
                // reset our drag finger index
                dragFingerIndex = -1;
                m_PreviousFinger = 4;//��ָ��ק����
            }
        }
    }

}
