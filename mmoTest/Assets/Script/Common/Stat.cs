using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏数据统计
/// </summary>
public class Stat 
{
    public static void Init()
    {
        //appid
    }

    public static void Reg(int userId, string nickName)
    {

    }

    public static void LogOn(int userId, string nickName)
    {

    }

    public static void ChangeNickName(string nickName)
    {

    }

    public static void UpLevel(int level)
    {

    }

    //=========================================任务
    /// <summary>
    /// 任务开始
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskName"></param>
    public static void TaskBegin(int taskId, string taskName)
    {

    }

    /// <summary>
    /// 任务结束
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskName"></param>
    /// <param name="status"></param>
    public static void TaskEnd(int taskId, string taskName, int status)
    {

    }

    //=========================================关卡
    /// <summary>
    /// 关卡开始
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="gameLevelName"></param>
    public static void GameLevelBegin(int gameLevelId, string gameLevelName)
    {

    }

    /// <summary>
    /// 关卡结束
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="gameLevelName"></param>
    /// <param name="status"></param>
    /// <param name="star"></param>
    public static void GameLevelEnd(int gameLevelId, string gameLevelName, int status, int star)
    {

    }

    /// <summary>
    /// 充值开始
    /// </summary>
    /// <param name="orderId">订单号</param>
    /// <param name="productId">产品编号</param>
    /// <param name="money">充值金额</param>
    /// <param name="type">货币种类</param>
    /// <param name="virtualMoney">虚拟货币</param>
    /// <param name="channelId">充值渠道</param>
    public static void ChargeBegin(string orderId, string productId, double money, string type, double virtualMoney, string channelId)
    {

    }

    /// <summary>
    /// 充值完成
    /// </summary>
    public static void ChargeEnd()
    {
        //参数从上个方法取
    }

    /// <summary>
    /// 购买道具
    /// </summary>
    /// <param name="itemId">道具编号</param>
    /// <param name="itemName">道具名称</param>
    /// <param name="price">价格</param>
    /// <param name="count">数量</param>
    public static void BuyItem(int itemId, string itemName, int price, int count)
    {

    }

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="itemId">道具编号</param>
    /// <param name="itemName">道具名称</param>
    /// <param name="count">数量</param>
    /// <param name="usedType">使用类型</param>
    public static void ItemUsed(int itemId, string itemName, int count, int usedType)
    {

    }

    /// <summary>
    /// 自定义事件
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddEvent(string key, string value)
    {

    }
}
