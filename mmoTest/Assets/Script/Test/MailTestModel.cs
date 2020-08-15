using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailTestModel : MonoBehaviour 
{
	void Start () {
		SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.Mail_Get_Detail, OnGetMailDetail);
	}

	void Update () {
		
	}

	public void OnGetMailDetail(byte[] buffer)//收到服务器特定协议的回调
	{
		Mail_Get_DetailProto proto = Mail_Get_DetailProto.GetProto(buffer);
		Debug.Log(proto.IsSuccess);
		Debug.Log(proto.MailTitle);
		Debug.Log(proto.MailContent);
	}
}
