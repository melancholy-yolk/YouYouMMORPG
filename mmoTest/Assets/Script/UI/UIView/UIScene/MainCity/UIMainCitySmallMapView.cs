using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCitySmallMapView : MonoBehaviour 
{
    public static UIMainCitySmallMapView Instance;

    public Image imageSmallMap;
    public Image imageArrow;
    private Button btn;
    [SerializeField]
    private Text textWorldMapSceneName;
	
    void Awake()
    {
        Instance = this;
    }

    public void SetUI(string picName, string name)
    {
        Sprite sprite = GameUtil.LoadSmallMap(picName);
        if (sprite != null)
        {
            imageSmallMap.overrideSprite = sprite;
            imageSmallMap.SetNativeSize();
        }
        textWorldMapSceneName.text = name;
    }
}
