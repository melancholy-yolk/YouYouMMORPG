/// <summary>
/// 类名 : RechargeProductEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2017-08-16 21:00:46
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
public partial class RechargeProductEntity : MFAbstractEntity
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
    ///渠道类型 
    /// </summary>
    public byte ChannelType { get; set; }

    /// <summary>
    ///充值产品编号 
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    ///充值产品类型 
    /// </summary>
    public byte ProductType { get; set; }

    /// <summary>
    ///充值产品描述 
    /// </summary>
    public string ProductDesc { get; set; }

    /// <summary>
    ///售价 
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    ///获得虚拟货币 
    /// </summary>
    public int Virtual { get; set; }

    #endregion
}
