/// <summary>
/// 类名 : GameServerEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2020-03-12 15:27:21
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
public partial class GameServerEntity : MFAbstractEntity
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
    ///0=维护 1=流畅 2=爆满 
    /// </summary>
    public byte RunStatus { get; set; }

    /// <summary>
    ///是否推荐 
    /// </summary>
    public bool IsCommand { get; set; }

    /// <summary>
    ///是否新服 
    /// </summary>
    public bool IsNew { get; set; }

    /// <summary>
    ///名称 
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///Ip 
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    ///Port 
    /// </summary>
    public int Port { get; set; }

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
