using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 技能槽视图
/// </summary>
public class UIMainCitySkillSlotView : MonoBehaviour 
{
    public int SlotNo;

    
    public int SkillId = -1;

    [SerializeField]
    private Image SkillImg;

    [SerializeField]
    private Image CDImg;

    private float m_CDTime = 0f;//冷却时间
    private float timer = 0;
    private bool isCD = false;//技能是否在冷却中



    private Action<int> OnSkillClick;

    public void SetUI(int skillId, string skillPic, float cdTime, Action<int> onSkillClick)
    {
        if (skillId == -1)
        {
            return;
        }


        SkillId = skillId;
        SkillImg.gameObject.SetActive(true);
        SkillImg.sprite = RoleMgr.Instance.LoadSkikllPic(skillPic);
        m_CDTime = cdTime;

        OnSkillClick = onSkillClick;
    }

    public void BeginCD()
    {
        isCD = true;
        CDImg.fillAmount = 1;
    }

    void Awake()
    {
        SkillImg.gameObject.SetActive(false);
        CDImg.fillAmount = 0;
    }

	void Start () 
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
	}

    private void OnBtnClick()
    {
        Debug.Log("click btn skill=" + SkillId);
        if (SkillId < 1) return;

        if (isCD) return;

        if (OnSkillClick != null)
        {
            OnSkillClick(SkillId);
        }
    }

	void Update () 
    {
        if (isCD)
        {
            timer += Time.deltaTime;
            if (timer >= m_CDTime)
            {
                isCD = false;
                timer = 0;
                CDImg.fillAmount = 0;
            }
            else
            {
                CDImg.fillAmount = (m_CDTime - timer) / m_CDTime;
            }
        }
	}
}
