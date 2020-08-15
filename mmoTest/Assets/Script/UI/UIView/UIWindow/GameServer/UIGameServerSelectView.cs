using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameServerSelectView : UIWindowViewBase
{
    private List<GameObject> gameServerObjList = new List<GameObject>();

    protected override void OnStart()
    {
        base.OnStart();

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate<GameObject>(GameServerItemPrefab);
            obj.transform.SetParent(GameServerItemParent);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            gameServerObjList.Add(obj);
        }
    }

    #region ҳǩ
    [SerializeField]
    private GameObject GameServerPageItemPrefab;//ҳǩԤ��

    [SerializeField]
    private Transform GameServerPageItemParent;

    public Action<int> OnGameServerPageItemClickCallBack;

    public void SetGameServerPageUI(List<RetGameServerPageEntity> list)
    {
        if (list == null || GameServerPageItemPrefab == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            GameObject obj = Instantiate<GameObject>(GameServerPageItemPrefab);
            obj.transform.SetParent(GameServerPageItemParent);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            UIGameServerPageItemView view = obj.GetComponent<UIGameServerPageItemView>();
            if (view != null)
            {
                view.SetUI(list[i]);
                view.OnGameServerPageClick = OnGameServerPageClick;
            }
        }
    }

    private void OnGameServerPageClick(int pageIndex)
    {
        AppDebug.Log("�������" + pageIndex + "ҳ");
        if (OnGameServerPageItemClickCallBack != null)
        {
            OnGameServerPageItemClickCallBack(pageIndex);
        }
    }
    #endregion

    #region ����
    [SerializeField]
    private GameObject GameServerItemPrefab;//����Ԥ��

    [SerializeField]
    private Transform GameServerItemParent;

    public Action<RetGameServerEntity> OnGameServerItemClickCallBack;

    private List<UIGameServerItemView> gameServerViewList = new List<UIGameServerItemView>();

    public void SetGameServerUI(List<RetGameServerEntity> list)
    {
        if (list == null || GameServerItemPrefab == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            GameObject obj = gameServerObjList[i];
            gameServerObjList[i].SetActive(true);

            UIGameServerItemView view = obj.GetComponent<UIGameServerItemView>();
            gameServerViewList.Add(view);
            if (view != null)
            {
                view.SetUI(list[i]);
                view.OnGameServerItemClick = OnGameServerItemClick;
            }
        }
        for (int i = list.Count; i < 10; i++)
        {
            gameServerObjList[i].SetActive(false);
        }
    }

    private void OnGameServerItemClick(RetGameServerEntity entity)
    {
        AppDebug.Log("�������" + entity.Name);
        if (OnGameServerItemClickCallBack != null)
        {
            OnGameServerItemClickCallBack(entity);
        }
    }
    #endregion

    #region ��ѡ�������
    [SerializeField]
    private UIGameServerItemView selected;

    public void SetSelectedGameServerUI(RetGameServerEntity entity)
    {
        if (entity == null) return;
        selected.gameObject.SetActive(true);
        selected.SetUI(entity);
    }
    #endregion
}
