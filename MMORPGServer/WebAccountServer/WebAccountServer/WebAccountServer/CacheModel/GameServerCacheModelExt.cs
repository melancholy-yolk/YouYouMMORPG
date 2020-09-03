using Mmcoy.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class GameServerCacheModel
{

    /// <summary>
    /// 获得区服分页页签列表
    /// </summary>
    public List<RetGameServerPageEntity> GetGameServerPageList()
    {
        List<RetGameServerPageEntity> lst = new List<RetGameServerPageEntity>();

        List<GameServerEntity> gameServerLst = this.GetList(isDesc:false);

        RetGameServerPageEntity page = null;
        int pageIndex = 1;
        for (int i = 0; i < gameServerLst.Count; i++)
        {
            //每十个服务器一组
            if (i % 10 == 0)
            {
                //10个一组的第一个
                page = new RetGameServerPageEntity();
                page.PageIndex = pageIndex;
                pageIndex++;

                page.Name = gameServerLst[i].Id.ToString();
                lst.Add(page);
            }

            if ((i+1)%10==0 || i==gameServerLst.Count-1)
            {
                //10个一组的最后一个
                if (page != null)
                {
                    page.Name += "——" + gameServerLst[i].Id.ToString()+"服";
                }
            }
        }
        //int num = (int)Math.Ceiling(gameServerLst.Count / 10.0f);
        //for (int i = 1; i <= num; i++)
        //{
        //    RetGameServerPageEntity page = new RetGameServerPageEntity();
        //    page.PageIndex = i;
        //    page.Name = string.Format("{0}——{1}", 1+10*(i-1), 10*i);
        //    if (i == num)
        //    {
        //        page.Name = string.Format("{0}——{1}", 1+10*(i-1), gameServerLst.Count);
        //    }
        //    lst.Add(page);
        //}

        return lst.OrderByDescending(p=>p.PageIndex).ToList();
    }

    /// <summary>
    /// 根据页签序号获取区服列表
    /// </summary>
    public List<RetGameServerEntity> GetGameServerList(int pageIndex)
    {
        List<RetGameServerEntity> retList = new List<RetGameServerEntity>();

        if (pageIndex == 0)
        {
            //推荐服务器
            //新区 玩家有账号的区

            //临时假数据方案 返回最新前三个
            MFReturnValue<List<GameServerEntity>> retValue = this.GetPageList(pageSize: 3, pageIndex:1);
            if (!retValue.HasError)
            {
                List<GameServerEntity> lst = retValue.Value;
                for (int i = 0; i < lst.Count; i++)
                {
                    RetGameServerEntity entity = new RetGameServerEntity()
                    {
                        Id = lst[i].Id.Value,
                        RunStatus = lst[i].RunStatus,
                        IsCommand = lst[i].IsCommand,
                        IsNew = lst[i].IsNew,
                        Name = lst[i].Name,
                        Ip = lst[i].Ip,
                        Port = lst[i].Port
                    };
                    retList.Add(entity);
                }
            }
        }
        else
        {
            MFReturnValue<List<GameServerEntity>> retValue = this.GetPageList(pageSize: 10, pageIndex: pageIndex, isDesc:false);
            if (!retValue.HasError)
            {
                List<GameServerEntity> lst = retValue.Value;
                for (int i = 0; i < lst.Count; i++)
                {
                    RetGameServerEntity entity = new RetGameServerEntity()
                    {
                        Id = lst[i].Id.Value,
                        RunStatus = lst[i].RunStatus,
                        IsCommand = lst[i].IsCommand,
                        IsNew = lst[i].IsNew,
                        Name = lst[i].Name,
                        Ip = lst[i].Ip,
                        Port = lst[i].Port
                    };
                    retList.Add(entity);
                }
            }
        }

        return retList.OrderByDescending( p=>p.Id ).ToList();
    }
}
    