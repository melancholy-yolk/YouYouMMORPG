using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIWorldMapItemView : MonoBehaviour 
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Text text;
    private Button btn;

    private int m_WorldMapId;

    public Action<int> OnWorldMapItemClick;

    public void SetUI(TransferData data, Action<int> callback)
    {
        OnWorldMapItemClick = callback;//点击回调

        m_WorldMapId = data.GetValue<int>(ConstDefine.WorldMap_Id);//数据
        //UI相关
        text.SetText(data.GetValue<string>(ConstDefine.WorldMap_Name) + " " + m_WorldMapId);
        image.overrideSprite = GameUtil.LoadWorldMapItemIcon(data.GetValue<string>(ConstDefine.WorldMap_Icon));
        image.SetNativeSize();
    }

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
    }

    private void OnBtnClick()
    {
        if (OnWorldMapItemClick != null)
        {
            OnWorldMapItemClick(m_WorldMapId);
        }
    }
}
