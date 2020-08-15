using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UISceneLoadingView : UISceneViewBase 
{
    [SerializeField]
    private UILoadingView uiView;

    public void SetProgress(float value)
    {
        uiView.SetSlider(value);
    }

}
