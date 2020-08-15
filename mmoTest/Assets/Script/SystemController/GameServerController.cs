using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

/// <summary>
/// 区服控制器
/// </summary>
public class GameServerController : SystemControllerBase<GameServerController>, ISystemController
{
    #region 属性
    private UIGameServerEnterView m_UIGameServerEnterView;//进入游戏窗口
    private UIGameServerSelectView m_UIGameServerSelectView;//选择区服窗口
    #endregion

    #region 构造函数
    public GameServerController()
    {
        AddEventListener(ConstDefine.UIGameServerEnterView_BtnEnterGame, GameServerEnterViewBtnEnterGameClick);
        AddEventListener(ConstDefine.UIGameServerEnterView_BtnSelectGameServer, GameServerEnterViewBtnSelectGameServerClick);
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.System_ServerTimeReturn, OnSystemServerTimeReturn);

        NetWorkSocket.Instance.OnConnectOK = OnSocketConnectOKCallBack;
    }
    #endregion

    #region btn callback
    #region 点击进入选择区服按钮
    /// <summary>
    /// 选择区服方法 点击选择区服按钮 打开选择区服窗口 需要页签和区服数据
    /// </summary>
    /// <param name="p"></param>
    private void GameServerEnterViewBtnSelectGameServerClick(object[] p)
    {
        //点击 选择区服按钮 就打开视图
        m_UIGameServerSelectView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameServerSelect, () => {
            m_UIGameServerSelectView.SetSelectedGameServerUI(GlobalInit.Instance.CurrentSelectGameServer);
            GetGameServerPage();//当窗口打开时 请求数据
        }).GetComponent<UIGameServerSelectView>();

        m_UIGameServerSelectView.OnGameServerPageItemClickCallBack = OnPageClick;
        m_UIGameServerSelectView.OnGameServerItemClickCallBack = OnGameServerItemClick;
    }

    /// <summary>
    /// 点击了单个服务器item
    /// </summary>
    /// <param name="entity"></param>
    private void OnGameServerItemClick(RetGameServerEntity entity)
    {
        //选择了服务器
        m_UIGameServerSelectView.SetSelectedGameServerUI(entity);
        GlobalInit.Instance.CurrentSelectGameServer = entity;
        m_UIGameServerEnterView.SetUI(entity.Name);
        AppDebug.Log(entity.Ip + "===" + entity.Port);
    }

    /// <summary>
    /// 点击了单个服务器页签item
    /// </summary>
    /// <param name="pageIndex"></param>
    private void OnPageClick(int pageIndex)
    {
        GetGameServer(pageIndex);
    }
    #endregion

    #region 向服务器发起请求

    //缓存每个页签对应的区服列表 避免多次向服务器请求数据
    private Dictionary<int, List<RetGameServerEntity>> m_GameServerDic = new Dictionary<int, List<RetGameServerEntity>>();
    private int m_CurrClickPageIndex = 0;
    private bool m_IsBusy = false;

    /// <summary>
    /// 向服务器请求页签数据
    /// </summary>
    private void GetGameServerPage()
    {
        JsonData jd = new JsonData();
        jd["Type"] = 0;
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", OnGameServerPageCallBack, isPost: true, jsonData: jd);
    }

    /// <summary>
    /// 向服务器请求区服数据
    /// </summary>
    private void GetGameServer(int pageIndex)
    {
        if (m_GameServerDic.ContainsKey(pageIndex))
        {
            if (m_UIGameServerSelectView != null)
            {
                m_UIGameServerSelectView.SetGameServerUI(m_GameServerDic[pageIndex]);
            }
            return;
        }

        m_CurrClickPageIndex = pageIndex;
        if (m_IsBusy) return;
        m_IsBusy = true;

        JsonData jd = new JsonData();
        jd["Type"] = 1;
        jd["PageIndex"] = pageIndex;
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", OnGameServerCallBack, isPost: true, jsonData: jd);
    }

    /// <summary>
    /// 更新最后登录服务器
    /// </summary>
    /// <param name="curAccount">当前账户数据</param>
    /// <param name="curServer">当前区服数据</param>
    private void UpdateLastLogOnServer(RetAccountEntity curAccount, RetGameServerEntity curServer)
    {
        JsonData jd = new JsonData();
        jd["Type"] = 2;
        jd["UserId"] = curAccount.Id;
        jd["LastServerId"] = curServer.Id;
        jd["LastServerName"] = curServer.Name;
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", UpdateLastLogOnServerCallBack, isPost: true, jsonData: jd);
    }

    /// <summary>
    /// 获取区服 http响应
    /// </summary>
    private void OnGameServerCallBack(NetWorkHttp.CallBackArgs obj)
    {
        m_IsBusy = false;
        if (obj.HasError)//http通讯发生错误
        {
            this.ShowMessage("获取区服", obj.ErrorMsg, MessageViewType.OK);
        }
        else
        {
            List<RetGameServerEntity> list = JsonMapper.ToObject<List<RetGameServerEntity>>(obj.Data);
            m_GameServerDic[m_CurrClickPageIndex] = list;
            if (list != null && m_UIGameServerSelectView != null)
            {
                m_UIGameServerSelectView.SetGameServerUI(list);
            }
        }
    }

    /// <summary>
    /// 获取区服页签 http响应
    /// </summary>
    private void OnGameServerPageCallBack(NetWorkHttp.CallBackArgs obj)
    {

        if (obj.HasError)//http通讯发生错误
        {
            this.ShowMessage("获取区服页签", obj.ErrorMsg, MessageViewType.OK);
        }
        else
        {
            List<RetGameServerPageEntity> pageList = JsonMapper.ToObject<List<RetGameServerPageEntity>>(obj.Data);

            if (pageList != null && m_UIGameServerSelectView != null)
            {
                AppDebug.Log("页签数量=" + pageList.Count);

                pageList.Insert(0, new RetGameServerPageEntity() { Name="推荐区服", PageIndex=0});
                //给视图赋值
                m_UIGameServerSelectView.SetGameServerPageUI(pageList);

                GetGameServer(0);
            }
        }
    }
    #endregion

    /// <summary>
    /// 更新最后登录区服数据 http响应
    /// </summary>
    private void UpdateLastLogOnServerCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (obj.HasError)//http通讯发生错误
        {
            this.ShowMessage("更新服务器信息", obj.ErrorMsg, MessageViewType.OK);
        }
        else
        {
            AppDebug.Log("更新最后登录服务器完毕");
        }
    }

    /// <summary>
    /// 进入游戏方法
    /// </summary>
    /// <param name="p"></param>
    private void GameServerEnterViewBtnEnterGameClick(object[] p)
    {
        //开始连接区服
        NetWorkSocket.Instance.Connect(GlobalInit.Instance.CurrentSelectGameServer.Ip, GlobalInit.Instance.CurrentSelectGameServer.Port);

        //NetWorkSocket.Instance.Connect("127.0.0.1", 1037);
    }

    /// <summary>
    /// 和游戏服连接成功 向游戏服发送客户端本地时间
    /// </summary>
    private void OnSocketConnectOKCallBack()
    {
        System_SendLocalTimeProto proto = new System_SendLocalTimeProto();
        proto.LocalTime = Time.realtimeSinceStartup * 1000;

        GlobalInit.Instance.CheckServerTime = Time.realtimeSinceStartup;
        NetWorkSocket.Instance.SendMsg(proto.ToArray());
    }
    #endregion

    private void OnSystemServerTimeReturn(byte[] buffer)
    {
        System_ServerTimeReturnProto retProto = System_ServerTimeReturnProto.GetProto(buffer);
        float localTime = retProto.LocalTime;
        long serverTime = retProto.ServerTime;

        GlobalInit.Instance.PingValue = (int)((Time.realtimeSinceStartup * 1000 - localTime) * 0.5f);//ping值
        GlobalInit.Instance.GameServerTime = serverTime - GlobalInit.Instance.PingValue;//客户端计算出来的服务器时间

        //更新账号服务器保存的最后登录服务器相关数据
        UpdateLastLogOnServer(GlobalInit.Instance.CurrentAccount, GlobalInit.Instance.CurrentSelectGameServer);
        AppDebug.Log("和服务器连接成功！！！");
        //跳转到选择角色场景
        SceneMgr.Instance.LoadToSelectRole();
    }

    #region 系统控制器接口实现
    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.GameServerEnter:
                OpenGameServerEnterView();
                break;
            case WindowUIType.GameServerSelect:
                OpenGameServerSelectView();
                break;
        }
    }

    private void OpenGameServerEnterView()
    {
        m_UIGameServerEnterView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameServerEnter, () => {
            m_UIGameServerEnterView.SetUI(GlobalInit.Instance.CurrentSelectGameServer.Name);
            }).GetComponent<UIGameServerEnterView>();
        
    }

    private void OpenGameServerSelectView()
    {
        m_UIGameServerSelectView = UIViewUtil.Instance.OpenWindow(WindowUIType.GameServerSelect).GetComponent<UIGameServerSelectView>();
    }
    #endregion

    #region 释放
    /// <summary>
    /// 释放
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        RemoveEventListener(ConstDefine.UIGameServerEnterView_BtnEnterGame, GameServerEnterViewBtnEnterGameClick);
        RemoveEventListener(ConstDefine.UIGameServerEnterView_BtnSelectGameServer, GameServerEnterViewBtnSelectGameServerClick);
    }
    #endregion
}
