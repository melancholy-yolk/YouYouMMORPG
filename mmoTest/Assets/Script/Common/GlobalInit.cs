using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalInit : MonoBehaviour
{
    public static GlobalInit Instance;

    public delegate void OnReceiveProtoHandler(ushort protoCode, byte[] buffer);
	public OnReceiveProtoHandler OnReceiveProto;

    public const string MMO_NICKNAME = "MMO_NICKNAME";
    public const string MMO_PWD = "MMO_PWD";
    public const string WebAccountUrl = "http://127.0.0.1:8080/";//账号服务器
    public const string SocketIP = "127.0.0.1";//游戏服
    public const ushort Port = 1037;
    
    public AnimationCurve UIAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

    public long ServerTime = 0;//登录到账号服务器那一刻的服务器时间
    public long CurrServerTime//当前账号服务器时间
    {
        get
        {
            return ServerTime + (long)Time.time;
        }
    }

    public RetGameServerEntity CurrentSelectGameServer;//当前选择的区服
    public RetAccountEntity CurrentAccount;//当前账号

    public List<JobEntity> jobEntityList = new List<JobEntity>();//本地职业数据
    public Dictionary<int, GameObject> jobPrefabDic = new Dictionary<int, GameObject>();//职业镜像字典 <职业id,prefab>

    public RoleInfoMainPlayer MainPlayerInfo;
    public RoleCtrl MainPlayer;

    #region unity生命周期
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //请求服务器时间
        NetWorkHttp.Instance.SendData(WebAccountUrl+"api/time", OnGetTimeCallBack);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.F))
        {
            PlayerPrefs.DeleteAll();
            AppDebug.Log("清空本地缓存");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (GlobalInit.Instance.MainPlayer == null)
            {
                return;
            }
            Transform trans = GlobalInit.Instance.MainPlayer.transform;
            string pos = string.Format("{0}_{1}_{2}_{3}", trans.position.x, trans.position.y, trans.position.z, trans.rotation.eulerAngles.y);
            AppDebug.Log(pos);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneMgr.Instance.LoadToWorldMap(3);
        }
    }
    #endregion

    private void OnGetTimeCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (!obj.HasError)
        {
            ServerTime = long.Parse(obj.Data);
        }
    }

    public long GetLocalTimeStamp()
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds;
        return timeStamp;
    }

    /// <summary>
    /// 和服务器对表的时刻（向服务器发送客户端本地时间的时刻）
    /// </summary>
    public float CheckServerTime { get; set; }

    /// <summary>
    /// ping值
    /// </summary>
    public int PingValue { get; set; }

    /// <summary>
    /// 客户端计算出来的服务器时间
    /// </summary>
    public long GameServerTime { get; set; }

    /// <summary>
    /// 获取当前的游戏服时间
    /// </summary>
    /// <returns></returns>
    public long GetCurrServerTime()
    {
        return (long)((Time.realtimeSinceStartup - CheckServerTime) * 1000) + GameServerTime;
    }
}
