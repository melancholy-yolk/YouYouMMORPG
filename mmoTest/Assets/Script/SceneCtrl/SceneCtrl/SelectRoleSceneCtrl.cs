using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectRoleSceneCtrl : MonoBehaviour
{
    #region 变量区
    [SerializeField]
    private List<Transform> roleModelPosList = new List<Transform>();//实例化出来的模型位置
    
    private Dictionary<int, GameObject> jobRoleModelDic = new Dictionary<int, GameObject>();//新建角色界面 实例化出来的模型

    UISceneSelectRoleView m_UISceneSelectRoleView;//场景UI视图

    [SerializeField]
    private Transform[] CreateRoleSceneModel;//新建角色所需的场景模型

    private bool m_IsCreateRole;//是否新建角色:新建角色界面 和 选择已有角色界面，点击开始游戏按钮，逻辑不同
    #endregion

    void Awake()
    {
        m_UISceneSelectRoleView = SceneUIManager.Instance.LoadSceneUI(SceneUIType.SelectRole).GetComponent<UISceneSelectRoleView>();
    }

	void Start () 
    {
        #region 子视图事件委托赋值
        if (m_UISceneSelectRoleView != null)
        {
            //UIDragView拖拽事件委托赋值(view不处理具体逻辑 具体逻辑写在控制器上)
            m_UISceneSelectRoleView.m_UISelectRoleDragView.OnSelectRoleDrag = OnSelectRoleDrag;

            //JobItem点击委托
            if (m_UISceneSelectRoleView.JobItems != null && m_UISceneSelectRoleView.JobItems.Length > 0)
            {
                for (int i = 0; i < m_UISceneSelectRoleView.JobItems.Length; i++)
                {
                    m_UISceneSelectRoleView.JobItems[i].OnJobItemClick = OnJobItemClickCallback;
                }
            }

            //其他按钮委托
            m_UISceneSelectRoleView.OnBtnBeginGame = OnBtnBeginGameClick;//开始游戏按钮点击委托
            m_UISceneSelectRoleView.OnBtnDeleteRole = OnBtnDeleteRoleClick;//删除角色按钮点击委托
            m_UISceneSelectRoleView.OnBtnReturn = OnBtnReturnClick;//返回按钮点击委托
            m_UISceneSelectRoleView.OnBtnCreateRole = OnBtnCreateRoleClick;//创建角色按钮点击委托
        }

        #endregion

        #region 协议监听
        //服务器返回登录区服消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_LogOnGameServerReturn, OnLogOnGameServerReturn);
        //服务器返回创建角色消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_CreateRoleReturn, OnCreateRoleReturn);
        //服务器返回进入游戏消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_EnterGameReturn, OnEnterGameReturn);
        //服务器返回删除角色消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_DeleteRoleReturn, OnDeleteRoleReturn);
        //服务器返回角色详情消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_SelectRoleInfoReturn, OnSelectRoleInfoReturn);
        //服务器返回角色技能消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleData_SkillReturn, OnSkillReturn);
        #endregion

        LoadJobPrefab();//excel->assetbundle->加载角色镜像

        OnLogOnGameServer();//登录到游戏服
	}

    

    #region 从ab包中根据excel中的数据 加载角色模型
    /// <summary>
    /// 从ab包加载角色prefab
    /// </summary>
    private void LoadJobPrefab()
    {
        //从本地excel文件加载职业配置
        GlobalInit.Instance.jobEntityList = JobDBModel.Instance.GetList();
        for (int i = 0; i < GlobalInit.Instance.jobEntityList.Count; i++)
        {
            GameObject obj = AssetBundleMgr.Instance.Load(string.Format("Download/Prefab/RolePrefab/Player/{0}.assetbundle", GlobalInit.Instance.jobEntityList[i].PrefabName), GlobalInit.Instance.jobEntityList[i].PrefabName);
            if (obj != null)
            {
                GlobalInit.Instance.jobPrefabDic[GlobalInit.Instance.jobEntityList[i].Id] = obj;
            }
            AppDebug.Log("JobPrefabName=" + GlobalInit.Instance.jobEntityList[i].PrefabName);
        }
    }
    #endregion

    #region 一进入场景 就自动 登录区服
    /// <summary>
    /// 登录区服消息
    /// </summary>
    private void OnLogOnGameServer()
    {
        RoleOperation_LogOnGameServerProto proto = new RoleOperation_LogOnGameServerProto();
        proto.AccountId = GlobalInit.Instance.CurrentAccount.Id;
        NetWorkSocket.Instance.SendMsg(proto.ToArray());
    }

    /// <summary>
    /// 服务器返回登录消息 返回此账户的角色列表
    /// </summary>
    private void OnLogOnGameServerReturn(byte[] buffer)
    {
        RoleOperation_LogOnGameServerReturnProto proto = RoleOperation_LogOnGameServerReturnProto.GetProto(buffer);
        int roleCount = proto.RoleCount;//服务器已存在角色
        if (roleCount == 0)
        {
            Procedure_CreateRole();//创建角色流程
        }
        else
        {
            Procedure_SelectRole(proto.RoleList);//选择角色流程
        }
        if (DelegateDefine.Instance.OnSceneLoadOK != null) DelegateDefine.Instance.OnSceneLoadOK();
    }
    #endregion

    //================================新建角色相关===================================

    #region 实例化模型
    /// <summary>
    /// 新建角色界面实例化出四个模型
    /// </summary>
	private void CloneCreateRole()
    {
        if (jobRoleModelDic != null && jobRoleModelDic.Count == 4)
        {
            foreach (var kv in jobRoleModelDic)
            {
                kv.Value.gameObject.SetActive(true);
            }
            return;
        }
        for (int i = 0; i < GlobalInit.Instance.jobEntityList.Count; i++)
        {
            int jobId = GlobalInit.Instance.jobEntityList[i].Id;

            GameObject obj = Instantiate<GameObject>(GlobalInit.Instance.jobPrefabDic[jobId]);
            obj.transform.SetParent(roleModelPosList[i]);//不同职业模型在不同位置
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = new Vector3(0, i * -90, 0);
            
            jobRoleModelDic[jobId] = obj;
        }
    }
    #endregion

    #region 拖拽选择职业方法
    [SerializeField]
    private Transform DragTarget;//拖拽旋转目标物体 场景摄像机
    private float m_RotateAngle = 90;//每次旋转角度
    private float m_TargetAngle = 0;//目标角度
    private bool m_IsRotating;//是否正在旋转中
    private float m_RotateSpeed = 180;//旋转速度
    private int m_CurrSelectJobId;//当前选中的职业id

    /// <summary>
    /// 选择职业拖拽回调 0=左 1=右
    /// </summary>
    /// <param name="dir"></param>
    private void OnSelectRoleDrag(int dir)
    {
        if (m_IsRotating) return;

        m_RotateAngle = Mathf.Abs(m_RotateAngle) * (dir == 0 ? -1 : 1);
        m_IsRotating = true;
        m_TargetAngle = DragTarget.eulerAngles.y + m_RotateAngle;

        //处理当前选择的职业id
        if (dir == 0)
        {
            //向左 逆时针旋转
            m_CurrSelectJobId++;
            if (m_CurrSelectJobId > 4)
            {
                m_CurrSelectJobId = 1;
            }
        }
        else
        {
            //向右 顺时针旋转
            m_CurrSelectJobId--;
            if (m_CurrSelectJobId <= 0)
            {
                m_CurrSelectJobId = 4;
            }
        }

        SetJobDesc();//m_CurrSelectJobId改变 JobItemView JobDescView都要刷新
    }
    #endregion

    #region JobItem点击选择职业方法
    /// <summary>
    /// UI上点击职业项后的回调
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void OnJobItemClickCallback(int jobId, int rotateAngle)
    {
        AppDebug.Log("JobId=" + jobId);
        AppDebug.Log("RotateAngle=" + rotateAngle);

        if (m_IsRotating) return;
        m_CurrSelectJobId = jobId;
        m_IsRotating = true;
        m_TargetAngle = rotateAngle;

        SetJobDesc();//m_CurrSelectJobId改变 JobItemView JobDescView都要刷新
    }
    #endregion

    #region 刷新当前选中职业描述
    /// <summary>
    /// m_CurrSelectJobId改变 JobItemView JobDescView都要刷新
    /// </summary>
    private void SetJobDesc()
    {
        AppDebug.Log("Refresh Job Desc!");
        //更新选择的职业信息
        for (int i = 0; i < GlobalInit.Instance.jobEntityList.Count; i++)
        {
            if (GlobalInit.Instance.jobEntityList[i].Id == m_CurrSelectJobId)
            {
                m_UISceneSelectRoleView.m_UISelectRoleJobDescView.SetUI(GlobalInit.Instance.jobEntityList[i].Name, GlobalInit.Instance.jobEntityList[i].Desc);
                break;
            }
        }

        for (int i = 0; i < m_UISceneSelectRoleView.JobItems.Length; i++)
        {
            m_UISceneSelectRoleView.JobItems[i].SetSelected(m_CurrSelectJobId);
        }
    }
    #endregion

    #region 新建角色场景模型显示与隐藏
    /// <summary>
    /// 新建角色场景模型显示与隐藏
    /// </summary>
    /// <param name="isShow"></param>
    private void SetCreateRoleSceneModelShow(bool isShow)
    {
        if (CreateRoleSceneModel != null && CreateRoleSceneModel.Length > 0)
        {
            for (int i = 0; i < CreateRoleSceneModel.Length; i++)
            {
                CreateRoleSceneModel[i].gameObject.SetActive(isShow);
            }
        }
    }
    #endregion

    //================================已有角色相关===================================
    #region 已有角色列表
    
    private List<RoleOperation_LogOnGameServerReturnProto.RoleItem> m_RoleList;//服务器返回的已存在角色列表
    private GameObject m_CurrSelectRoleModel;//当前选中的角色模型
    private int m_CurrSelectRoleId;//当前选中的角色id

    #region 通过RoleId查找role数据
    /// <summary>
    /// 根据roleid获取已有角色信息
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    private RoleOperation_LogOnGameServerReturnProto.RoleItem GetRoleItem(int roleId)
    {
        if (m_RoleList != null)
        {
            for (int i = 0; i < m_RoleList.Count; i++)
            {
                if (m_RoleList[i].RoleId == roleId)
                {
                    return m_RoleList[i];
                }
            }
        }
        
        return default(RoleOperation_LogOnGameServerReturnProto.RoleItem);
    }
    #endregion

    #region 在场景中显示出与roleid对应的模型
    /// <summary>
    /// 传入RoleId 场景中显示出对应的角色模型
    /// </summary>
    /// <param name="roleId"></param>
    private void SetSelectedRole(int roleId)
    {
        if (m_CurrSelectRoleId == roleId && m_CurrSelectRoleModel != null)
        {
            return;
        }

        m_CurrSelectRoleId = roleId;
        if (m_CurrSelectRoleModel != null)
        {
            Destroy(m_CurrSelectRoleModel);
        }

        RoleOperation_LogOnGameServerReturnProto.RoleItem item = GetRoleItem(roleId);

        //根据角色的职业id 实例化角色
        m_CurrSelectRoleModel = Instantiate<GameObject>(GlobalInit.Instance.jobPrefabDic[item.RoleJob]);
        m_CurrSelectRoleModel.transform.SetParent(roleModelPosList[0]);
        m_CurrSelectRoleModel.transform.localPosition = Vector3.zero;
        m_CurrSelectRoleModel.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    #endregion

    #region 已有角色列表单个项被点击
    /// <summary>
    /// 已有角色列表单个项 点击回调 在场景中显示出对应的模型
    /// </summary>
    /// <param name="roleId"></param>
    private void SelectRoleCallBack(int roleId)
    {
        AppDebug.Log("RoleId=" + roleId);
        SetSelectedRole(roleId);
        m_UISceneSelectRoleView.SetSelected(roleId);//UI tween动画
    }
    #endregion

    #endregion

    //================================删除角色相关===================================
    #region 删除角色相关
    /// <summary>
    /// 点击删除角色按钮回调
    /// </summary>
    private void OnBtnDeleteRoleClick()
    {
        m_UISceneSelectRoleView.DeleteSelectRole(GetRoleItem(m_CurrSelectRoleId).RoleNickName, EnsureDeleteRole);
    }

    /// <summary>
    /// 删除角色窗口 点击确定按钮 向服务器发送删除角色协议
    /// </summary>
    private void EnsureDeleteRole()
    {
        RoleOperation_DeleteRoleProto proto = new RoleOperation_DeleteRoleProto();
        proto.RoleId = m_CurrSelectRoleId;
        NetWorkSocket.Instance.SendMsg(proto.ToArray());
    }

    /// <summary>
    /// 收到服务器返回删除角色结果
    /// </summary>
    private void OnDeleteRoleReturn(byte[] buffer)
    {
        RoleOperation_DeleteRoleReturnProto proto = RoleOperation_DeleteRoleReturnProto.GetProto(buffer);
        if (proto.IsSuccess)//删除角色成功
        {
            AppDebug.Log("delete role success!!!");
            DeleteRole(m_CurrSelectRoleId);
            m_UISceneSelectRoleView.CloseDeleteRoleView();//关闭删除角色窗口
        }
        else//删除角色失败
        {
            MessageController.Instance.Show("NOTICE", "Delete role failed!");
        }
    }

    /// <summary>
    /// 删除角色成功后 从本地列表删除角色 刷新已有角色列表UI
    /// </summary>
    private void DeleteRole(int roleId)
    {
        //从本地角色列表中移除已删除的role
        for (int i = m_RoleList.Count-1; i >= 0; i--)
        {
            if (m_RoleList[i].RoleId == roleId)
            {
                m_RoleList.RemoveAt(i);
            }
        }

        m_UISceneSelectRoleView.SetRoleList(m_RoleList, SelectRoleCallBack);//设置已有角色UI列表

        if (m_RoleList.Count == 0)
        {
            //切换到新建角色界面
            Procedure_CreateRole();
        }
        else
        {
            SetSelectedRole(m_RoleList[0].RoleId);//默认显示 一个已有角色模型
        }
    }
    #endregion
    

    //================================进入游戏相关===================================

    #region begin game按钮
    /// <summary>
    /// 点击开始游戏按钮 向服务器发送 创建角色/进入游戏 消息
    /// </summary>
    private void OnBtnBeginGameClick()
    {
        
        if (m_IsCreateRole)//创建角色视图
        {
            #region 新建角色 开始按钮逻辑 向服务器发送CreateRole消息 参数为当前选择JobId和输入的昵称
            RoleOperation_CreateRoleProto proto = new RoleOperation_CreateRoleProto();
            proto.JobId = (byte)m_CurrSelectJobId;
            proto.RoleNickName = m_UISceneSelectRoleView.inputNickName.text;

            if (string.IsNullOrEmpty(proto.RoleNickName))
            {
                MessageController.Instance.Show("提示", "角色名不能为空!");
                return;
            }

            NetWorkSocket.Instance.SendMsg(proto.ToArray());
            #endregion
            AppDebug.Log("click begin game btn!!!");
        }
        else//选择角色视图
        {
            #region 选择已有角色 开始按钮逻辑 向服务器发送EnterGame消息 参数为当前选择角色id
            RoleOperation_EnterGameProto proto = new RoleOperation_EnterGameProto();

            proto.RoleId = m_CurrSelectRoleId;

            NetWorkSocket.Instance.SendMsg(proto.ToArray());
            #endregion
        }
    }

    /// <summary>
    /// 服务器返回 创建角色消息
    /// </summary>
    private void OnCreateRoleReturn(byte[] buffer)
    {
        RoleOperation_CreateRoleReturnProto proto = RoleOperation_CreateRoleReturnProto.GetProto(buffer);
        if (proto.IsSuccess)
        {
            AppDebug.Log("create role success");
        }
        else
        {
            MessageController.Instance.Show("notice", "create role failure");
        }
    }

    /// <summary>
    /// 服务器返回 进入游戏消息
    /// </summary>
    private void OnEnterGameReturn(byte[] buffer)
    {
        RoleOperation_EnterGameReturnProto proto = RoleOperation_EnterGameReturnProto.GetProto(buffer);
        if (proto.IsSuccess)
        {
            AppDebug.Log("enter game success");
            //TODO:创建真正的角色 跳转场景 服务器收到进入游戏协议 会自动返回角色信息协议 客户端收到角色信息协议后 跳转到下一个游戏场景
            //SceneMgr.Instance.LoadToCity();
        }
        else
        {
            MessageController.Instance.Show("notice", "enter game fail");
        }
    }
    #endregion

    private int m_LastInWorldMapId;//最后所在世界地图Id

    /// <summary>
    /// 服务器返回角色详情
    /// </summary>
    private void OnSelectRoleInfoReturn(byte[] buffer)
    {
        RoleOperation_SelectRoleInfoReturnProto proto = RoleOperation_SelectRoleInfoReturnProto.GetProto(buffer);
        if (proto.IsSuccess)
        {
            GlobalInit.Instance.MainPlayerInfo = new RoleInfoMainPlayer(proto);//给角色信息赋值
            m_LastInWorldMapId = proto.LastInWorldMapId;

            PlayerController.Instance.LastInWorldMapId = m_LastInWorldMapId;
            PlayerController.Instance.LastInWorldMapPos = proto.LastInWorldMapPos;
        }
    }

    /// <summary>
    /// 服务器返回角色学会的技能信息
    /// </summary>
    private void OnSkillReturn(byte[] p)
    {
        RoleData_SkillReturnProto proto = RoleData_SkillReturnProto.GetProto(p);
        GlobalInit.Instance.MainPlayerInfo.LoadSkill(proto);//加载角色学会的技能信息

        SceneMgr.Instance.LoadToWorldMap(m_LastInWorldMapId);
    }

    #region btn return
    /// <summary>
    /// 点击返回按钮回调
    /// </summary>
    private void OnBtnReturnClick()
    {
        if (m_IsCreateRole)//当前是新建角色流程
        {
            if (m_RoleList == null || m_RoleList.Count == 0)//返回选择区服场景
            {
                NetWorkSocket.Instance.Disconnect();//与socket游戏服断开连接
                SceneMgr.Instance.LoadToLogOn();//切换场景
            }
            else
            {
                Procedure_SelectRole(m_RoleList);
            }
        }
        else//当前是选择角色流程
        {
            NetWorkSocket.Instance.Disconnect();
            SceneMgr.Instance.LoadToLogOn();
        }
    }
    #endregion

    #region btn create role
    /// <summary>
    /// 点击新建角色按钮委托
    /// </summary>
    private void OnBtnCreateRoleClick()
    {
        if (m_RoleList.Count >= 4)
        {
            return;
        }
        Procedure_CreateRole();
    }
    #endregion

    #region update方法
    void Update () {
        if (m_IsRotating == true)
        {
            //插值计算出每帧需要旋转的角度
            float toAngle = Mathf.MoveTowardsAngle(DragTarget.eulerAngles.y, m_TargetAngle, Time.deltaTime * m_RotateSpeed);
            DragTarget.eulerAngles = Vector3.up * toAngle;
            if (Mathf.RoundToInt(m_TargetAngle) == Mathf.RoundToInt(toAngle) || Mathf.RoundToInt(m_TargetAngle + 360) == Mathf.RoundToInt(toAngle))
            {
                AppDebug.Log("Current Job Id=" + m_CurrSelectJobId);
                m_IsRotating = false;
                DragTarget.eulerAngles = Vector3.up * m_TargetAngle;
            }
        }
    }
    #endregion

    void OnDestroy()
    {
        #region 移除协议监听
        //服务器返回登录区服消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_LogOnGameServerReturn, OnLogOnGameServerReturn);
        //服务器返回创建角色消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_CreateRoleReturn, OnCreateRoleReturn);
        //服务器返回进入游戏消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_EnterGameReturn, OnEnterGameReturn);
        //服务器返回删除角色消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_DeleteRoleReturn, OnDeleteRoleReturn);
        //服务器返回角色详情消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_SelectRoleInfoReturn, OnSelectRoleInfoReturn);
        //服务器返回角色技能消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleData_SkillReturn, OnSkillReturn);
        #endregion
    }

    private void Procedure_CreateRole()
    {
        //销毁已有角色模型
        if (m_CurrSelectRoleModel != null)
        {
            Destroy(m_CurrSelectRoleModel);
        }

        #region 新建角色
        m_IsCreateRole = true;
        m_UISceneSelectRoleView.SetUICreateRoleShow(true);//显示新建角色UI
        m_UISceneSelectRoleView.SetUISelectRoleShow(false);//隐藏新建角色UI
        SetCreateRoleSceneModelShow(true);//显示新建角色的三个柱子（有一个柱子始终存在）

        CloneCreateRole();//实例化出四个职业的角色模型
        m_CurrSelectJobId = 1;//默认选择职业Id=1
        DragTarget.eulerAngles = Vector3.zero;//摄像机旋转归零
        SetJobDesc();//设置JobItemView JobDescView

        m_UISceneSelectRoleView.RandomName();//默认给出一个随机角色昵称
        #endregion
    }

    private void Procedure_SelectRole(List<RoleOperation_LogOnGameServerReturnProto.RoleItem> returnRoleList)
    {
        //隐藏新建角色模型
        if (jobRoleModelDic != null && jobRoleModelDic.Count > 0)
        {
            foreach (var kv in jobRoleModelDic)
            {
                kv.Value.gameObject.SetActive(false);
            }
        }

        #region 选择已有角色
        m_IsCreateRole = false;
        m_UISceneSelectRoleView.SetUICreateRoleShow(false);//隐藏新建角色UI
        m_UISceneSelectRoleView.SetUISelectRoleShow(true);//显示选择角色UI
        SetCreateRoleSceneModelShow(false);//隐藏新建角色场景模型
        DragTarget.eulerAngles = Vector3.zero;//摄像机旋转归零

        if (returnRoleList != null)
        {
            m_RoleList = returnRoleList;
            m_UISceneSelectRoleView.SetRoleList(m_RoleList, SelectRoleCallBack);//设置已有角色UI列表
            //SetSelectedRole(m_RoleList[0].RoleId);//默认显示的已有角色模型
            SelectRoleCallBack(m_RoleList[0].RoleId);
        }
        #endregion
    }

}
