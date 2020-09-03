/// <summary>
/// 类名 : Log_GameLevelEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2016-10-30 21:08:28
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
public partial class Log_GameLevelEntity : MFAbstractEntity
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
    ///游戏关卡Id 
    /// </summary>
    public int GameLevelId { get; set; }

    /// <summary>
    ///0=普通1=困难2=地狱
    /// </summary>
    public byte Grade { get; set; }

    /// <summary>
    ///0=进入关卡1=成功2=失败
    /// </summary>
    public byte Action { get; set; }

    /// <summary>
    ///创建时间 
    /// </summary>
    public DateTime CreateTime { get; set; }

    #endregion
}
