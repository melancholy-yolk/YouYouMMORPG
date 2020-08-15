using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NPCHeadBarView : MonoBehaviour 
{
    [SerializeField]
    private Text textNickName;
    [SerializeField]
    private Image imageTalkBG;
    [SerializeField]
    private Text textTalk;

    private Transform m_Target;
    private RectTransform rectTrans;

    private Tween m_ScaleTween;
    private Tween m_RotateTween;
    private float m_TalkStopTime = 0;//˵������ʱ��
    private bool m_IsTalk;//�Ƿ�˵��
    private string m_TalkText;

    void Awake()
    {
        imageTalkBG.gameObject.SetActive(false);
    }

    void Start()
    {
        rectTrans = SceneUIManager.Instance.CurrentUIScene.CurrCanvas.GetComponent<RectTransform>();

        imageTalkBG.transform.localScale = Vector3.zero;
        m_ScaleTween = imageTalkBG.transform.DOScale(Vector3.one, 0.2f).SetAutoKill(false).Pause().OnComplete(() =>
        {
            //���򲥷���ɻص�
            textTalk.DOText(m_TalkText, 0.5f);
        }).OnRewind(() => { 
            //���򲥷���ɻص�
            imageTalkBG.gameObject.SetActive(false);
        });

        imageTalkBG.transform.localEulerAngles = new Vector3(0,0,-10);
        m_RotateTween = imageTalkBG.transform.DOLocalRotate(new Vector3(0,0,20), 0.5f, RotateMode.LocalAxisAdd).SetAutoKill(false).Pause().SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutBack).OnComplete(() =>
        {

        }).OnRewind(() => {

        });
    }

    /// <summary>
    /// NPC��ʼ˵��
    /// </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
    public void Talk(string text, float time)
    {
        m_TalkStopTime = Time.time + time;
        m_IsTalk = true;
        textTalk.text = "";
        m_TalkText = text;

        
        imageTalkBG.gameObject.SetActive(true);

        m_ScaleTween.PlayForward();
        m_RotateTween.PlayForward();
    }

    void Update()
    {
        if (rectTrans == null || m_Target == null) return;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(m_Target.position);//�õ���Ļ����
        Vector3 pos;//���յ�UI��������
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, screenPos, UI_Camera.Instance.m_Camera, out pos))
        {
            transform.position = pos;
        }

        if (m_IsTalk && Time.time > m_TalkStopTime)
        {
            m_IsTalk = false;
            m_ScaleTween.PlayBackwards();
        }
    }

    public void Init(Transform trans, string nickName, bool isShowHPBar)
    {
        m_Target = trans;
        textNickName.text = nickName;
    }
}
