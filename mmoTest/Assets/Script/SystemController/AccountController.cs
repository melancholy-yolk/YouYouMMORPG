using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// 账户控制器
/// </summary>
public class AccountController : SystemControllerBase<AccountController>, ISystemController
{
    #region 属性
    private UILogOnView m_LogOnView;//登录窗口
    private UIRegView m_RegView;//注册窗口

    private bool isAutoLogOn = false;//是否自动登录
    #endregion

    #region 构造函数
    public AccountController()
    {
        this.AddEventListener(ConstDefine.UILogOnView_BtnLogOn, OnBtnLogOnClick);
        this.AddEventListener(ConstDefine.UILogOnView_BtnToReg, OnBtnToRegClick);
        this.AddEventListener(ConstDefine.UIRegView_BtnReg, OnBtnRegClick);
        this.AddEventListener(ConstDefine.UIRegView_BtnBackLogin, OnBtnBackLoginClick);
    }
    #endregion

    #region 快速登录
    /// <summary>
    /// 快速登录
    /// </summary>
    public void QuickLogOn()
    {
        //OpenView(WindowUIType.LogOn);
        //return;

        //首先判断本地帐号
        //没有本地帐号 进入注册view
        //存在本地帐号 自动登录 登录成功后 返回进入游戏的view
        if (!PlayerPrefs.HasKey(ConstDefine.LogOn_AccountId))
        {
            OpenView(WindowUIType.Reg);
        }
        else
        {
            //自动登录
            isAutoLogOn = true;
            JsonData jd = new JsonData();
            jd["Type"] = 1;
            jd["UserName"] = PlayerPrefs.GetString(ConstDefine.LogOn_AccountUserName);
            jd["Pwd"] = PlayerPrefs.GetString(ConstDefine.LogOn_AccountPwd);
            NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/account", OnLogOnCallBack, isPost: true, jsonData: jd);
        }
    }
    #endregion

    #region btn callback
    /// <summary>
    /// 登录按钮点击
    /// </summary>
    private void OnBtnLogOnClick(params object[] param)
    {
        if (string.IsNullOrEmpty(m_LogOnView.userNameInput.text))
        {
            this.ShowMessage("登录提示", "请输入账户", MessageViewType.OK);
            return;
        }
        if (string.IsNullOrEmpty(m_LogOnView.pwdInput.text))
        {
            this.ShowMessage("登录提示", "请输入密码", MessageViewType.OK);
            return;
        }

        isAutoLogOn = false;
        JsonData jd = new JsonData();
        jd["Type"] = 1;
        jd["UserName"] = m_LogOnView.userNameInput.text;
        jd["Pwd"] = m_LogOnView.pwdInput.text;
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/account", OnLogOnCallBack, isPost: true, jsonData: jd);
    }

    private void OnLogOnCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (obj.HasError)//http通讯发生错误
        {
            this.ShowMessage("登录提示", obj.ErrorMsg, MessageViewType.OK);
        }
        else
        {
            ReturnValue ret = JsonMapper.ToObject<ReturnValue>(obj.Data);
            if (ret.HasError)
            {
                this.ShowMessage("登录提示", ret.ErrorMsg, MessageViewType.OK);
            }
            else
            {
                RetAccountEntity entity = JsonMapper.ToObject<RetAccountEntity>(ret.ReturnData.ToString());

                SetCurrentSelectGameServer(entity);

                string userName = string.Empty;
                if (isAutoLogOn)//快速登录
                {
                    userName = PlayerPrefs.GetString(ConstDefine.LogOn_AccountUserName);
                    UIViewManager.Instance.OpenView(WindowUIType.GameServerEnter);
                }
                else//手动登录
                {
                    //本地存储
                    PlayerPrefs.SetInt(ConstDefine.LogOn_AccountId, entity.Id);
                    PlayerPrefs.SetString(ConstDefine.LogOn_AccountUserName, m_LogOnView.userNameInput.text);
                    PlayerPrefs.SetString(ConstDefine.LogOn_AccountPwd, m_LogOnView.pwdInput.text);

                    userName = m_LogOnView.userNameInput.text;
                    m_LogOnView.CloseAndOpenNext(WindowUIType.GameServerEnter);
                }
                Stat.LogOn(entity.Id, userName);
                
            }
        }
    }

    /// <summary>
    /// 跳转到注册窗口
    /// </summary>
    private void OnBtnToRegClick(params object[] param)
    {
        m_LogOnView.CloseAndOpenNext(WindowUIType.Reg);
    }

    /// <summary>
    /// 注册按钮点击
    /// </summary>
    private void OnBtnRegClick(params object[] param)
    {
        if (string.IsNullOrEmpty(m_RegView.userNameInput.text))
        {
            this.ShowMessage("注册提示", "请输入账户", MessageViewType.OK);
            return;
        }
        if (string.IsNullOrEmpty(m_RegView.pwdInput.text))
        {
            this.ShowMessage("注册提示", "请输入密码", MessageViewType.OK);
            return;
        }

        JsonData jd = new JsonData();
        jd["Type"] = 0;
        jd["UserName"] = m_RegView.userNameInput.text;
        jd["Pwd"] = m_RegView.pwdInput.text;
        jd["ChannelId"] = 0;
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/account", OnRegCallBack, isPost: true, jsonData: jd);
    }

    private void OnRegCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (obj.HasError)//http通讯发生错误
        {
            this.ShowMessage("注册提示", obj.ErrorMsg, MessageViewType.OK);
        }
        else
        {
            ReturnValue ret = JsonMapper.ToObject<ReturnValue>(obj.Data);
            if (ret.HasError)//服务器注册行为失败
            {
                this.ShowMessage("注册提示", ret.ErrorMsg, MessageViewType.OK);
            }
            else
            {
                RetAccountEntity entity =  JsonMapper.ToObject<RetAccountEntity>(ret.ReturnData.ToString());
                SetCurrentSelectGameServer(entity);
                //Stat.Reg((int)ret.ReturnData, m_RegView.userNameInput.text);//注册打点

                //本地存储
                PlayerPrefs.SetInt(ConstDefine.LogOn_AccountId, entity.Id);
                PlayerPrefs.SetString(ConstDefine.LogOn_AccountUserName, m_RegView.userNameInput.text);
                PlayerPrefs.SetString(ConstDefine.LogOn_AccountPwd, m_RegView.pwdInput.text);

                m_RegView.CloseAndOpenNext(WindowUIType.GameServerEnter);
            }
            
        }
    }

    /// <summary>
    /// 跳转到登录窗口
    /// </summary>
    private void OnBtnBackLoginClick(params object[] param)
    {
        m_RegView.CloseAndOpenNext(WindowUIType.LogOn);
    }
    #endregion

    #region 系统控制器接口实现相关
    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.LogOn:
                OpenLogOnView();
                break;
            case WindowUIType.Reg:
                OpenRegView();
                break;
        }
    }

    /// <summary>
    /// 打开登录窗口
    /// </summary>
    private void OpenLogOnView()
    {
        m_LogOnView = UIViewUtil.Instance.OpenWindow(WindowUIType.LogOn).GetComponent<UILogOnView>();
        //m_LogOnView.OnViewClose = () =>
        //{
        //    OpenRegView();
        //};
    }

    /// <summary>
    /// 打开注册窗口
    /// </summary>
    private void OpenRegView()
    {
        m_RegView = UIViewUtil.Instance.OpenWindow(WindowUIType.Reg).GetComponent<UIRegView>();
        //m_RegView.OnViewClose = () => 
        //{ 
        //    OpenLogOnView(); 
        //};

    }
    #endregion

    #region 释放
    /// <summary>
    /// 释放
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        this.RemoveEventListener(ConstDefine.UILogOnView_BtnLogOn, OnBtnLogOnClick);
        this.RemoveEventListener(ConstDefine.UILogOnView_BtnToReg, OnBtnToRegClick);
        this.RemoveEventListener(ConstDefine.UIRegView_BtnReg, OnBtnRegClick);
        this.RemoveEventListener(ConstDefine.UIRegView_BtnBackLogin, OnBtnBackLoginClick);
    }
    #endregion

    /// <summary>
    /// 注册 登录成功后 都会刷新本地记录的账号实体数据
    /// </summary>
    /// <param name="entity"></param>
    private void SetCurrentSelectGameServer(RetAccountEntity entity)
    {
        RetGameServerEntity currGameServerEntity = new RetGameServerEntity();
        currGameServerEntity.Id = entity.LastServerId;
        currGameServerEntity.Name = entity.LastServerName;
        currGameServerEntity.Ip = entity.LastServerIP;
        currGameServerEntity.Port = int.Parse(entity.LastServerPort);

        GlobalInit.Instance.CurrentAccount = entity;
        GlobalInit.Instance.CurrentSelectGameServer = currGameServerEntity;
    }
}
