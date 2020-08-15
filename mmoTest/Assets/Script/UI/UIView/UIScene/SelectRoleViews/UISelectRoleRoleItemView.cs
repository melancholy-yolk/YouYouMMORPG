using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class UISelectRoleRoleItemView : MonoBehaviour 
{
    private int m_RoleId;

    [SerializeField]
    private Text textNickName;

    [SerializeField]
    private Text textLevel;

    [SerializeField]
    private Text textJob;

    [SerializeField]
    private Image imageHeadPic;

    private Action<int> OnSelectRole;

    private Button btn;

    private Vector3 m_MoveTargetPos;//动画移动目标点

    private int m_CurrSelectRoleId;

	void Awake () {
		btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);

        
	}

    void Start()
    {
        m_MoveTargetPos = transform.localPosition + new Vector3(-50, 0, 0);
        transform.DOLocalMove(m_MoveTargetPos, 0.2f).SetAutoKill(false).Pause();
        SetSelected(m_CurrSelectRoleId);
    }

    public void SetSelected(int roleId)
    {
        m_CurrSelectRoleId = roleId;
        if (m_RoleId == roleId)
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
        if (OnSelectRole != null)
        {
            OnSelectRole(m_RoleId);
        }
    }

    public void SetUI(int roleId, string nickName, int level, byte jobId, Sprite headPic, Action<int> onSelectRole)
    {
        m_RoleId = roleId;
        textNickName.text = nickName;
        textLevel.text = string.Format("LV.{0}", level);
        textJob.text = JobDBModel.Instance.GetEntity(jobId).Name;
        imageHeadPic.overrideSprite = headPic;
        OnSelectRole = onSelectRole;
    }

    void OnDestroy()
    {
        textNickName = null;
        textLevel = null;
        textJob = null;
        imageHeadPic = null;
        OnSelectRole = null;
        btn = null;
    }
}
