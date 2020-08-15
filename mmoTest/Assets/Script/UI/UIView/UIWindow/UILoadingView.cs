using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingView : UIWindowViewBase 
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text text;

    public void SetSlider(float value)
    {
        slider.value = value;
        text.text = (int)(value * 100) + "%";
    }

}
