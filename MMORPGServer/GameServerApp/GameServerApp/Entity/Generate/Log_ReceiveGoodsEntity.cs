/// <summary>
/// 类名 : Log_ReceiveGoodsEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2016-10-30 21:13:57
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mmcoy.Framework.AbstractBase;

/// <summary>
/// 
/// </summary>
[Serializable]
public partial class Log_ReceiveGoodsEntity : MFAbstractEntity
{
    #region 重写基类属性
    /// <summary>
    /// 主键
    /// </summary>
    public override int? PKValue
    {
        get
        {
            return this.Id;
        }
        set
        {
            this.Id = value;
        }
    }
    #endregion

    #region 实体属性

    /// <summary>
    /// 编号
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public EnumEntityStatus Status { get; set; }

    /// <summary>
    ///角色Id 
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    ///0=游戏关卡（PVE）1=世界地图野外场景(PVP)
    /// </summary>
    public byte PlayType { get; set; }

    /// <summary>
    ///0：游戏关卡Id1：世界地图Id这个类型对应的场景Id，比如类型是游戏关卡 则对应游戏关卡Id
    /// </summary>
    public int PlaySceneId { get; set; }

    /// <summary>
    ///难度等级 仅用于游戏关卡
    /// </summary>
    public byte Grade { get; set; }

    /// <summary>
    ///物品类型 
    /// </summary>
    public byte GoodsType { get; set; }

    /// <summary>
    ///物品编号 
    /// </summary>
    public int GoodsId { get; set; }

    /// <summary>
    ///物品数量 
    /// </summary>
    public int GoodsCount { get; set; }

    /// <summary>
    ///创建时间 
    /// </summary>
    public DateTime CreateTime { get; set; }

    #endregion
}
