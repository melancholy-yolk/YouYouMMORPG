using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class GameObjectUtil 
{
    public static T GetOrCreateComponent<T>(this GameObject obj) where T:MonoBehaviour
    {
        T t = obj.GetComponent<T>();
        if (t == null)
        {
            t = obj.AddComponent<T>();
        }
        return t;
    }

    /// <summary>
    /// 设置物体的父节点
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parentTrans"></param>
    public static void SetParent(this GameObject obj, Transform parentTrans)
    {
        obj.transform.SetParent(parentTrans);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.localEulerAngles = Vector3.zero;
    }

    /// <summary>
    /// 设置物体的层
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerName"></param>
    public static void SetLayer(this GameObject obj, string layerName)
    {
        Transform[] transArr = obj.transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transArr.Length; i++)
        {
            transArr[i].gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    public static void SetNull(this MonoBehaviour[] arr)
    {
        if (arr != null)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = null;
            }
            arr = null;
        }
    }

    public static void SetNull(this Transform[] arr)
    {
        if (arr != null)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = null;
            }
            arr = null;
        }
    }

    public static void SetNull(this Sprite[] arr)
    {
        if (arr != null)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = null;
            }
            arr = null;
        }
    }

    public static void SetNull<T>(this List<T> list) where T : MonoBehaviour
    {
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = null;
            }
            list = null;
        }
    }

    //==========================UI扩展============================
    public static void SetText(this Text textComponent, string content, bool isAnimation = false, ScrambleMode scrambleMode = ScrambleMode.None)
    {
        if (textComponent != null)
        {
            if (isAnimation)
            {
                textComponent.text = "";
                textComponent.DOText(content, 0.2f, scrambleMode: scrambleMode);
            }
            else
            {
                textComponent.text = content;
            }
        }
    }

    public static void SetImageFill(this Image imageComponent, float value)
    {
        if (imageComponent != null)
        {
            imageComponent.fillAmount = value;
        }
    }

    public static void SetSliderValue(this Slider slider, float value)
    {
        if (slider != null)
        {
            slider.value = value;
        }
    }

    //========== 组件扩展 ==========
    public static T GetOrCreatComponent<T>(this GameObject obj) where T : Component
    {
        T t = obj.GetComponent<T>();
        if (t == null)
        {
            t = obj.AddComponent<T>();
        }
        return t;
    }
}
