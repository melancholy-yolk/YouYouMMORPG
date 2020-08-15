using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ְҵ��
/// </summary>
public class UISelectRoleJobItemView : MonoBehaviour 
{
    [SerializeField]
    private int m_JobId;//ְҵ���

    [SerializeField]
    private int m_RotateAngle;//�������ʾ��Ӧģ��Ŀ����ת�Ƕ�

    private Button btn;

    public Action<int, int> OnJobItemClick;

    private Vector3 m_MoveTargetPos;//�����ƶ�Ŀ���

    private int m_SelectJobId;

	void Awake () {
		btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);

        m_MoveTargetPos = transform.localPosition + new Vector3(50, 0, 0);
        transform.DOLocalMove(m_MoveTargetPos, 0.2f).SetAutoKill(false).SetEase(GlobalInit.Instance.UIAnimationCurve).Pause();

        SetSelected(m_SelectJobId);
	}

    public void SetSelected(int selectedJobId)
    {
        m_SelectJobId = selectedJobId;
        if (m_JobId == selectedJobId)
        {
            transform.DOPlayForward();
        }
        else
        {
            transform.DOPlayBackwards();
        }
    }

    private void OnBtnClick()
    {
        if (OnJobItemClick != null)
        {
            OnJobItemClick(m_JobId, m_RotateAngle);
        }
    }

    void OnDestroy()
    {
        btn = null;
        OnJobItemClick = null;
    }
}
