using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��¼����UI��ͼ
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
