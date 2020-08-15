using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIWorldMapView : UIWindowViewBase 
{
    [SerializeField]
    private Transform Container;

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();

        Container = null;
    }

    public void SetUI(TransferData data, Action<int> onWorldMapItemClick)
    {
        if (Container == null) return;

        List<TransferData> list = data.GetValue<List<TransferData>>(ConstDefine.WorldMap_ItemList);
        for (int i = 0; i < list.Count; i++)
        {
            TransferData worldMapEntityData = list[i];

            GameObject obj = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIItem, "WorldMap", "WorldMapItem");
            obj.transform.SetParent(Container);
            obj.transform.localScale = Vector3.one;

            Vector2 posInMap = worldMapEntityData.GetValue<Vector2>(ConstDefine.WorldMap_PosInMap);
            obj.transform.localPosition = new Vector3(posInMap.x, posInMap.y, 0);
            Debug.Log(posInMap);

            UIWorldMapItemView itemView = obj.GetComponent<UIWorldMapItemView>();
            itemView.SetUI(worldMapEntityData, onWorldMapItemClick);
        }
    }
}
