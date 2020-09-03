/// <summary>
/// 类名 : GameServerConfigEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2017-07-23 20:14:20
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
public partial class GameServerConfigEntity : MFAbstractEntity
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
    ///配置编码 
    /// </summary>
    public string ConfigCode { get; set; }

    /// <summary>
    ///是否开启 
    /// </summary>
    public byte IsOpen { get; set; }

    /// <summary>
    ///参数 
    /// </summary>
    public string Param { get; set; }

    #endregion
}
