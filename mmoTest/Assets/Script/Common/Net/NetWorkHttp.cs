using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using LitJson;

//使用http与账户服务器交互 访问账户数据库
public class NetWorkHttp : MonoBehaviour
{
    #region 单例
    private static NetWorkHttp instance;
	public static NetWorkHttp Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject obj = new GameObject("NetWorkHttp");
				DontDestroyOnLoad(obj);
				instance = obj.AddComponent<NetWorkHttp>();
			}
			return instance;
		}
	}
    #endregion

    private Action<CallBackArgs> m_CallBack;
	private CallBackArgs m_CallBackArgs;
	private bool isBusy = false;
	public bool IsBusy
	{
		get{ return isBusy; }
	}

	void Start()
	{
		m_CallBackArgs = new CallBackArgs();
	}

    /// <summary>
    /// 向web服务器发送请求 
    /// </summary>
    /// <param name="url">账号服务器url</param>
    /// <param name="callBack">收到服务器响应结果时的回调</param>
    /// <param name="isPost">是否是post请求</param>
    /// <param name="json">收到的数据</param>
	public void SendData(string url, Action<CallBackArgs> callBack, bool isPost = false, JsonData jsonData = null)
	{
		if (isBusy)
		{
			return;
		}
		isBusy = true;
		m_CallBack = callBack;//服务器响应回调函数
		if (isPost)
		{
            //web加密
            if (jsonData != null)
            {
                //客户端设备Id
                jsonData["DeviceIdentifier"] = DeviceUtil.DeviceIdentifier;
                //客户端设备型号
                jsonData["DeviceModel"] = DeviceUtil.DeviceModel;

                long t = GlobalInit.Instance.CurrServerTime;
                jsonData["sign"] = MFEncryptUtil.Md5(string.Format("{0}:{1}", t, DeviceUtil.DeviceIdentifier));//签名

                jsonData["t"] = t;//时间戳
            }
            PostUrl(url, jsonData == null ? "" : JsonMapper.ToJson(jsonData));
		}
		else
		{
			GetUrl(url);
		}
	}

	private void GetUrl(string url)
	{
		WWW data = new WWW(url);
		StartCoroutine(Request(data));
	}

	private IEnumerator Request(WWW data)
	{
		yield return data;

		isBusy = false;

		if (string.IsNullOrEmpty(data.error))//未出错
		{
			if (data.text == "null")
			{
				m_CallBackArgs.HasError = true;
				m_CallBackArgs.ErrorMsg = "未请求到数据";

				m_CallBack(m_CallBackArgs);
			}
			else
			{
				if (m_CallBack != null)
				{
					m_CallBackArgs.HasError = false;
					m_CallBackArgs.Data = data.text;

					m_CallBack(m_CallBackArgs);
				}
			}
		}
		else//出错
		{
			if (m_CallBack != null)
			{
				m_CallBackArgs.HasError = true;
				m_CallBackArgs.ErrorMsg = data.error;

				m_CallBack(m_CallBackArgs);
			}
		}
	}

    /// <summary>
    /// 想服务器发起post请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="json"></param>
	private void PostUrl(string url, string json)
	{
		WWWForm form = new WWWForm();
		form.AddField("", json);
		WWW data = new WWW(url, form);
		StartCoroutine(Request(data));
	}

    /// <summary>
    /// 参数数据结构
    /// </summary>
	public class CallBackArgs : EventArgs
	{
		public bool HasError;//是否有错
		public string ErrorMsg;//错误原因
		public string Data;//数据
	}
	
}
