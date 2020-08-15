using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISelectRoleJobDescView : MonoBehaviour 
{
    private Text textJobName;
    private Text textJobDesc;

    private Vector3 m_MoveTargetPos;

    private bool m_IsShow = false;

    void Awake()
    {
        textJobName = transform.Find("TextJobName").GetComponent<Text>();
        textJobDesc = transform.Find("TextJobDesc").GetComponent<Text>();

        m_MoveTargetPos = transform.localPosition;
        Vector3 from = m_MoveTargetPos + new Vector3(0, 500, 0);
        transform.localPosition = from;
        transform.DOLocalMove(m_MoveTargetPos, 0.2f).SetAutoKill(false).Pause().OnComplete(() =>
        {
            m_IsShow = true;
        }).OnRewind(() =>
        {
            transform.DOPlayForward();
        });

        //DoAnim();
    }

    void Start()
    {
        
    }

    public void SetUI(string jobName, string jobDesc)
    {
        textJobName.text = jobName;
        textJobDesc.text = jobDesc;

        DoAnim();
    }

    /// <summary>
    /// ²¥·ÅUI tween¶¯»­
    /// </summary>
    private void DoAnim()
    {
        if (!m_IsShow)
        {
            transform.DOPlayForward();
        }
        else
        {
            transform.DOPlayBackwards();
        }
    }

    void OnDestroy()
    {
        textJobName = null;
        textJobDesc = null;
    }
}
