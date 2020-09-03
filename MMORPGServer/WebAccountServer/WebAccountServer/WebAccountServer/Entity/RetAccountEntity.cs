using System.Collections;
using System.Collections.Generic;
using System;

public class RetAccountEntity
{
    public RetAccountEntity()
    {

    }

    public RetAccountEntity(AccountEntity entity)
    {
        Id = entity.Id.Value;
        UserName = entity.UserName;
        YuanBao = entity.Money;
        LastServerId = entity.LastLogOnServerId;
        LastServerName = entity.LastLogOnServerName;

        if (LastServerId == 0)//首次注册的玩家 此值一定为零 默认给一个服务器
        {
            Mmcoy.Framework.MFReturnValue<List<GameServerEntity>> mfr = GameServerCacheModel.Instance.GetPageList(isDesc:true, pageSize:1);
            if (!mfr.HasError)
            {
                List<GameServerEntity> list = mfr.Value;
                if (list != null && list.Count > 0)
                {
                    LastServerId = list[0].Id.Value;
                    LastServerName = list[0].Name;
                    LastServerIP = list[0].Ip;
                    LastServerPort = list[0].Port.ToString();
                }
            }
        }
        else
        {
            GameServerEntity entityGameServer = GameServerCacheModel.Instance.GetEntity(entity.LastLogOnServerId);
            if (entityGameServer != null)
            {
                LastServerIP = entityGameServer.Ip;
                LastServerPort = entityGameServer.Port.ToString();
            }
        }
        
    }

    public int Id
    {
        get;
        set;
    }
    public string UserName
    {
        get;
        set;
    }
    public string Pwd
    {
        get;
        set;
    }
    public int YuanBao
    {
        get;
        set;
    }
    public int LastServerId
    {
        get;
        set;
    }
    public string LastServerName
    {
        get;
        set;
    }
    public string LastServerIP
    {
        get;
        set;
    }
    public string LastServerPort
    {
        get;
        set;
    }
    public DateTime CreateTime
    {
        get;
        set;
    }
    public DateTime UpdateTime
    {
        get;
        set;
    }
}
