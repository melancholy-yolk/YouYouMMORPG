using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipItemView : MonoBehaviour 
{
    [SerializeField]
    private Image imageIcon;
    [SerializeField]
    private Text textContent;
    [SerializeField]
    private Sprite[] spriteArr;

    public void SetUI(TipEntity entity)
    {
        if (spriteArr != null && spriteArr.Length == 2)
        {
            imageIcon.overrideSprite = spriteArr[entity.Type];
            imageIcon.SetNativeSize();
        }
        textContent.text = entity.Content;
    }
}
