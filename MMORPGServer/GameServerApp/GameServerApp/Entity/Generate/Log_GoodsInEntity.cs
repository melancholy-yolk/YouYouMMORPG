/// <summary>
/// 类名 : Log_GoodsInEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2017-10-23 12:54:30
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
public partial class Log_GoodsInEntity : MFAbstractEntity
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
    ///类型 
    /// </summary>
    public byte Type { get; set; }

    /// <summary>
    ///物品类型 
    /// </summary>
    public byte GoodsType { get; set; }

    /// <summary>
    ///物品Id 
    /// </summary>
    public int GoodsId { get; set; }

    /// <summary>
    ///物品服务器端编号 
    /// </summary>
    public int GoodsServerId { get; set; }

    /// <summary>
    ///创建时间 
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    ///更新时间 
    /// </summary>
    public DateTime UpdateTime { get; set; }

    #endregion
}
