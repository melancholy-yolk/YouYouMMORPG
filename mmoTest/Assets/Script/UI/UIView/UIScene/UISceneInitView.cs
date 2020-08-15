using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISceneInitView : MonoBehaviour 
{
    public static UISceneInitView Instance;

    [SerializeField]
    private Slider m_Progress;
    [SerializeField]
    private Text m_Text;

    void Awake()
    {
        Instance = this;
    }

    public void SetProgress(string content, float value)
    {
        m_Text.SetText(content);
        m_Progress.SetSliderValue(value);
    }
}
