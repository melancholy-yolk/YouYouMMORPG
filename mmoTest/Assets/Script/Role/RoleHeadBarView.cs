using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleHeadBarView : MonoBehaviour 
{
    [SerializeField]
    private Text textNickName;
    [SerializeField]
    private Slider sliderHP;

    private Transform m_Target;
    private RectTransform rectTrans;
	
	void Start () {
        rectTrans = SceneUIManager.Instance.CurrentUIScene.CurrCanvas.GetComponent<RectTransform>();
	}
	
	
	void Update () {
        if (rectTrans == null || m_Target == null) return;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(m_Target.position);
        Vector3 pos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, screenPos, UI_Camera.Instance.m_Camera, out pos))
        {
            transform.position = pos;
        }
	}

    public void Init(Transform trans, string nickName, bool isShowHPBar = false, float sliderValue = 1)
    {
        m_Target = trans;
        textNickName.text = nickName;

        sliderHP.gameObject.SetActive(isShowHPBar);
        sliderHP.value = sliderValue;
    }

    public void SetSliderHP(float sliderValue)
    {
        sliderHP.value = sliderValue;
    }

}
