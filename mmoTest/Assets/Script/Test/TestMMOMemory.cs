//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using LitJson;

//public class TestMMOMemory : MonoBehaviour 
//{
//    void Start () {
//        // Item item = new Item(){ Id = 1, Name = "测试" };

//        // byte[] arr = null;
//        // using (MMO_MemoryStream ms = new MMO_MemoryStream())
//        // {
//        // 	ms.WriteInt(item.Id);
//        // 	ms.WriteUTF8String(item.Name);

//        // 	arr = ms.ToArray();
//        // }
//        // // if (arr != null)
//        // // {
//        // // 	for (int i = 0; i < arr.Length; i++)
//        // // 	{
//        // // 		Debug.Log(string.Format("{0}={1}", i, arr[i]));
//        // // 	}
//        // // }
//        // Item item2 = new Item();
//        // using (MMO_MemoryStream ms = new MMO_MemoryStream(arr))
//        // {
//        // 	item2.Id = ms.ReadInt();
//        // 	item2.Name = ms.ReadUTF8String();
//        // }
//        // Debug.Log(string.Format("item2 Id={0}", item2.Id));
//        // Debug.Log(string.Format("item2 Name={0}", item2.Name));
//        //List<ProductEntity> lst = ProductDBModel.Instance.GetList();
//        //ProductEntity entity = ProductDBModel.Instance.GetEntity(5);
//        //for (int i = 0; i < lst.Count; i++)
//        //{
//        //    Debug.Log(string.Format("Id={0} Name={1} Price={2} PicName={3} Desc={4}\n", lst[i].Id, lst[i].Name, lst[i].Price, lst[i].PicName, lst[i].Desc));
//        //}
//        // List<JobEntity> lst = JobDBModel.Instance.GetList();
//        // for (int i = 0; i < lst.Count; i++)
//        // {
//        //     Debug.Log(string.Format("Id={0} Name={1} HeadPic={2} JobPic={3} PrefabName={4} Desc={5}\n", lst[i].Id, lst[i].Name, lst[i].HeadPic, lst[i].JobPic, lst[i].PrefabName, lst[i].Desc));
//        // }
//        // if (!NetWorkHttp.Instance.IsBusy)
//        // {
//        // 	NetWorkHttp.Instance.SendData("localhost:8080/api/account?id=2", CallBack);
//        // }
//        // if (!NetWorkHttp.Instance.IsBusy)//模拟注册账号
//        // {
//        // 	JsonData jsonData = new JsonData();
//        // 	jsonData["Type"] = 0;
//        // 	jsonData["UserName"] = "eee";
//        // 	jsonData["Pwd"] = "eee";
//        // 	NetWorkHttp.Instance.SendData("localhost:8080/api/account", PostCallBack, isPost:true, json:jsonData.ToJson());
//        // }

//        // //1.与服务器建立tcp连接
//        // NetWorkSocket.Instance.Connect("127.0.0.1", 1011);
		
//        // //2.发消息
//        // // SendMsg("");
//        // TestProto proto = new TestProto();
//        // proto.Id = 1;
//        // proto.Name = "测试";
//        // proto.Type = 0;
//        // proto.Price = 99.5f;

//        // byte[] buffer = null;
//        // //1.对象转json转byte数组
//        // // string json = LitJson.JsonMapper.ToJson(proto);
//        // // using (MMO_MemoryStream ms = new MMO_MemoryStream())
//        // // {
//        // // 	ms.WriteUTF8String(json);//将json字符串写入到数据流中
//        // // 	buffer = ms.ToArray();
//        // // }

//        // //2.自定义
//        // buffer = proto.ToArray();
//        // Debug.Log(buffer.Length);

//        // TestProto proto2 = TestProto.GetProto(buffer);
//        // Debug.Log(proto2.Name);
////		NetWorkSocket.Instance.Connect("127.0.0.1", 1011);
//        Debug.Log("dataPath="+Application.dataPath);
//        Debug.Log("persistentDataPath="+Application.persistentDataPath);
//        Debug.Log("streamingAssetsPath="+Application.streamingAssetsPath);
//        Debug.Log("temporaryCachePath="+Application.temporaryCachePath);
//    }

//    void Update() 
//    {
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            TestProto proto = new TestProto();
//            proto.Id = 100;
//            proto.Name = "测试协议";
//            proto.Type = 80;
//            proto.price = 56.5f;

//            NetWorkSocket.Instance.SendMsg(proto.ToArray());
//        }
//        if (Input.GetKeyDown(KeyCode.B))
//        {
//            Mail_Request_ListProto proto2 = new Mail_Request_ListProto();
//            NetWorkSocket.Instance.SendMsg(proto2.ToArray());
//        }
//    }

//    private void CallBack(NetWorkHttp.CallBackArgs cb)//http get请求回调
//    {
//        if (cb.HasError)
//        {
//            Debug.Log("未查找到用户");
//        }
//        else
//        {
//            AccountEntity accountEntity = LitJson.JsonMapper.ToObject<AccountEntity>(cb.Data);
//            Debug.Log(accountEntity.UserName);
//        }
		
//    }

//    private void PostCallBack(NetWorkHttp.CallBackArgs cb)//http post请求回调
//    {
//        if (cb.HasError)
//        {
//            Debug.Log("未查找到用户");
//        }
//        else
//        {
//            Debug.Log(cb.Data);
//        }
//    }

//}

//// public class Item
//// {
//// 	public int Id;
//// 	public string Name;
//// }
