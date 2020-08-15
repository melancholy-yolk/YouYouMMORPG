using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIGameLevelMapView : UIWindowViewBase
{
    #region 变量
    [SerializeField]
    private Text textChapterName;
    [SerializeField]
    private RawImage imgMap;
    [SerializeField]
    private Transform linkPointContainer;

    private int m_ChapterId;

    private List<Transform> m_GameLevelItems = new List<Transform>();//关卡项transform组件集合

    private List<TransferData> m_LevelDataList;

    public Action<int> OnGameLevelItemClickCallBack;
    #endregion


    protected override void OnStart()
    {
        base.OnStart();

        StartCoroutine(CreateItem());
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();

        textChapterName = null;
        imgMap = null;
    }

    public void SetUI(TransferData data, Action<int> callback)
    {
        OnGameLevelItemClickCallBack = callback;

        m_ChapterId = data.GetValue<int>(ConstDefine.ChapterId);
        textChapterName.SetText(data.GetValue<string>(ConstDefine.ChapterName));
        imgMap.texture = GameUtil.LoadGameLevelMapPic(data.GetValue<string>(ConstDefine.ChapterMapPic));

        m_LevelDataList = data.GetValue<List<TransferData>>(ConstDefine.GameLevelList);
    }

    private IEnumerator CreateItem()
    {
        if (m_LevelDataList == null) yield break;

        if (m_LevelDataList != null && m_LevelDataList.Count > 0)
        {
            m_GameLevelItems.Clear();

            for (int i = 0; i < m_LevelDataList.Count; i++)
            {
                //======== 关卡项 ========
                GameObject obj = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIItem, "GameLevel", "GameLevelMapItem");

                obj.transform.SetParent(imgMap.transform);
                Vector2 pos = m_LevelDataList[i].GetValue<Vector2>(ConstDefine.GameLevel_Position);
                obj.transform.localPosition = new Vector3(pos.x, pos.y, 0);
                obj.transform.localScale = Vector3.one;

                UIGameLevelMapItemView itemView = obj.GetComponent<UIGameLevelMapItemView>();
                itemView.SetUI(m_LevelDataList[i], OnGameLevelItemClickCallBack);

                m_GameLevelItems.Add(obj.transform);

                yield return null;//等待一帧
            }
        }

        //======== 生成连线点 ========
        for (int i = 0; i < m_GameLevelItems.Count; i++)
        {
            if (i == m_GameLevelItems.Count - 1) yield break;
            Transform transBegin = m_GameLevelItems[i];//起始点
            Transform transEnd = m_GameLevelItems[i + 1];//结束点

            //计算两点的距离
            float distance = Vector2.Distance(transBegin.localPosition, transEnd.localPosition);

            //计算生成多少个连线点
            int createCount = Mathf.FloorToInt(distance / 20f);

            float xLen = transEnd.localPosition.x - transBegin.localPosition.x;
            float yLen = transEnd.localPosition.y - transBegin.localPosition.y;

            //x y的递增
            float stepX = xLen / createCount;
            float stepY = yLen / createCount;

            //创建点
            for (int j = 0; j < createCount; j++)
            {
                //首尾不创建点
                if (j < 1 || j > createCount - 2) continue;

                GameObject pointObj = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIItem, "GameLevel", "GameLevelMapPoint");

                pointObj.SetParent(linkPointContainer);
                pointObj.transform.localPosition = new Vector3(transBegin.localPosition.x + stepX * j, transBegin.localPosition.y + stepY * j, 0);
                pointObj.transform.localScale = Vector3.one;

                UIGameLevelMapPointView pointView = pointObj.GetComponent<UIGameLevelMapPointView>();
                if (pointView != null) pointView.SetUI(true);

                yield return null;//等待一帧
            }
        }
    }

}
