using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 职业项
/// </summary>
public class UISelectRoleJobItemView : MonoBehaviour 
{
    [SerializeField]
    private int m_JobId;//职业编号

    [SerializeField]
    private int m_RotateAngle;//摄像机显示对应模型目标旋转角度

    private Button btn;

    public Action<int, int> OnJobItemClick;

    private Vector3 m_MoveTargetPos;//动画移动目标点

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
