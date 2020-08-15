using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// µÇÂ¼³¡¾°UIÊÓÍ¼
/// </summary>
public class UISceneLogOnView : UISceneViewBase 
{
    protected override void OnStart()
    {
        base.OnStart();
        StartCoroutine(OpenLogOnWindow());
    }

    private IEnumerator OpenLogOnWindow()
    {
        yield return new WaitForSeconds(0.2f);
        AccountController.Instance.QuickLogOn();
    }

}
