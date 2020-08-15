using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISceneSelectRoleView : UISceneViewBase 
{
    [Header("====Create Role UI====")]
    public UISelectRoleDragView m_UISelectRoleDragView;//拖拽区域视图
    public UISelectRoleJobItemView[] JobItems;//职业项
    public UISelectRoleJobDescView m_UISelectRoleJobDescView;//当前选中的职业 职业描述视图
    public InputField inputNickName;//角色昵称输入框
    [SerializeField]
    private Transform[] UICreateRole;//新建角色显示的UI


    [Header("====Select Role UI====")]
    [SerializeField]
    private Transform[] UISelectRole;//选择角色显示的UI
    #region 已有角色列表相关
    [SerializeField]
    private GameObject m_RoleItemPrefab;
    [SerializeField]
    private Transform m_RoleListContainer;//已有角色列表容器
    [SerializeField]
    private Sprite[] m_RoleHeadPic;//角色头像
    #endregion
    public List<UISelectRoleRoleItemView> roleItemList = new List<UISelectRoleRoleItemView>();//角色项


    [Header("====Delete Role UI====")]
    public UISelectRoleDeleteRoleView m_UISelectRoleDeleteRoleView;//删除角色窗口

    public Action OnBtnBeginGame;//开始游戏委托
    public Action OnBtnDeleteRole;//删除角色委托
    public Action OnBtnReturn;//返回委托
    public Action OnBtnCreateRole;//新建角色按钮委托

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "BtnReturn"://返回按钮
                AppDebug.Log("return");
                if (OnBtnReturn != null) OnBtnReturn();
                break;
            case "BtnBeginGame"://进入游戏按钮
                AppDebug.Log("begin game");
                if (OnBtnBeginGame != null) OnBtnBeginGame();
                break;
            case "BtnRandomName"://随机名称按钮
                AppDebug.Log("random name");
                RandomName();
                break;
            case "BtnDeleteRole"://删除角色按钮
                if (OnBtnDeleteRole != null) OnBtnDeleteRole();
                break;
            case "BtnCreateRole"://新建角色按钮
                if (OnBtnCreateRole != null) OnBtnCreateRole();
                break;
        }
    }

    public void RandomName()
    {
        inputNickName.text = GameUtil.RandomName();
    }

    #region 显示/隐藏 创建角色UI
    /// </summary>
    /// <summary>
    /// 显示/隐藏创建角色UI
    /// </summary>
    /// <param name="isShow"></param>
    public void SetUICreateRoleShow(bool isShow)
    {
        if (UICreateRole != null && UICreateRole.Length > 0)
        {
            for (int i = 0; i < UICreateRole.Length; i++)
            {
                UICreateRole[i].gameObject.SetActive(isShow);
            }
        }
    }
    #endregion

    #region 显示/隐藏 选择角色UI
    /// </summary>
    /// <summary>
    /// 显示/隐藏创建角色UI
    /// </summary>
    /// <param name="isShow"></param>
    public void SetUISelectRoleShow(bool isShow)
    {
        if (UISelectRole != null && UISelectRole.Length > 0)
        {
            for (int i = 0; i < UISelectRole.Length; i++)
            {
                UISelectRole[i].gameObject.SetActive(isShow);
            }
        }
    }
    #endregion

    #region 实例化已有角色列表
    /// <summary>
    /// 已有角色 实例化已有角色列表
    /// </summary>
    /// <param name="list"></param>
    /// <param name="OnSelectRole"></param>
    public void SetRoleList(List<RoleOperation_LogOnGameServerReturnProto.RoleItem> list, Action<int> OnSelectRole)
    {

        if (roleItemList.Count > 0)
        {
            for (int i = 0; i < roleItemList.Count; i++)
            {
                Destroy(roleItemList[i].gameObject);
            }
            roleItemList.Clear();
        }

        if (list != null && list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                GameObject obj = Instantiate<GameObject>(m_RoleItemPrefab);
                UISelectRoleRoleItemView view = obj.GetComponent<UISelectRoleRoleItemView>();
                roleItemList.Add(view);
                if (view != null)
                {
                    view.SetUI(list[i].RoleId, list[i].RoleNickName, list[i].RoleLevel, list[i].RoleJob, m_RoleHeadPic[list[i].RoleJob - 1], OnSelectRole);
                }
                obj.transform.SetParent(m_RoleListContainer);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero + new Vector3(0, -100 * i, 0);
            }
        }
    }
    #endregion

    /// <summary>
    /// 在已有角色列表中 选中一个角色后 item动画
    /// </summary>
    /// <param name="roleId"></param>
    public void SetSelected(int roleId)
    {
        if (roleItemList != null && roleItemList.Count > 0)
        {
            for (int i = 0; i < roleItemList.Count; i++)
            {
                roleItemList[i].SetSelected(roleId);
            }
        }
    }

    /// <summary>
    /// 点击删除角色按钮
    /// </summary>
    public void DeleteSelectRole(string nickName, Action ensureDelete)
    {
        m_UISelectRoleDeleteRoleView.Show(nickName, ensureDelete);
    }

    /// <summary>
    /// 关闭 删除角色窗口
    /// </summary>
    public void CloseDeleteRoleView()
    {
        m_UISelectRoleDeleteRoleView.Close();
    }

    /// <summary>
    /// 清空引用 回收内存
    /// </summary>
    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();

        m_UISelectRoleDragView = null;
        JobItems.SetNull();
        m_UISelectRoleJobDescView = null;
        inputNickName = null;
        UICreateRole.SetNull();
        UISelectRole.SetNull();
        m_RoleItemPrefab = null;
        m_RoleListContainer = null;
        m_RoleHeadPic.SetNull();
        roleItemList.SetNull();
        m_UISelectRoleDeleteRoleView = null;
        OnBtnBeginGame = null;
        OnBtnDeleteRole = null;
        OnBtnReturn = null;
        OnBtnCreateRole = null;
    }
}
