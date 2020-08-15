using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UISceneViewBase : UIViewBase 
{
    [SerializeField]
    public Transform ContainerCenter;

    public Action OnLoadComplete;

    public Canvas CurrCanvas;

    public bl_HUDText HUDText;
}
