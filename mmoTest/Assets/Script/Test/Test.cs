//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using LitJson;

//public class Test : MonoBehaviour 
//{
//    void Start () {
//        //@"Role\role_mainplayer_zhanshi.assetbundle", "role_mainplayer_zhanshi"
//        //@"Scene\shangu.unity3d", "shangu"
//        //1.同步加载
//        //GameObject obj = AssetBundleMgr.Instance.LoadClone(@"Role\role_mainplayer_zhanshi.assetbundle", "role_mainplayer_zhanshi");

//        //2.异步加载
//        AssetBundleMgr.Instance.LoadAsync(@"Role\role_mainplayer_zhanshi.assetbundle", "role_mainplayer_zhanshi").OnLoadComplete = OnLoadAsyncComplete;
//        //NetWorkHttp.Instance.SendData("http://localhost:8080/api/account?id=1", GetRequestCallBack);
        
//        if(NetWorkHttp.Instance.IsBusy)
//        {

//        }
//        else
//        {
//            //NetWorkHttp.Instance.SendData("http://localhost:8080/api/account", PostRequestCallBack, isPost:true, json:"");
//        }
//    }

//    /// <summary>
//    /// 异步加载回调
//    /// </summary>
//    /// <param name="obj"></param>
//    private void OnLoadAsyncComplete(Object obj)
//    {
//        Instantiate((GameObject)obj);
//    }

//    void Update () {
	
//    }

//    public void GetRequestCallBack(NetWorkHttp.CallBackArgs args) 
//    {
//        if (!string.IsNullOrEmpty(args.Data))
//        {
            
//            AccountEntity entity = JsonMapper.ToObject<AccountEntity>(args.Data);
//            Debug.Log(entity.UserName);
//        }
//        else
//        {
//            Debug.Log("no result");
//        }
//    }

//    public void PostRequestCallBack(NetWorkHttp.CallBackArgs args)
//    {
//        if (args.HasError)
//        {
//            Debug.Log(args.ErrorMsg);
//        }
//        else
//        {
//            //ReturnValue ret = JsonMapper.ToObject<ReturnValue>(args.Json);
//            //if(!ret.HasError)
//            //{
//            //    Debug.Log("新注册的用户编号=" + ret.ReturnData);
//            //}
//            Debug.Log(args.Data);
//        }
//    }
//}
