/// <summary>
/// 类名 : Role_PassGameLevelDetailEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2016-10-31 20:05:37
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
public partial class Role_PassGameLevelDetailEntity : MFAbstractEntity
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
    /// 
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    ///章编号 
    /// </summary>
    public int ChapterId { get; set; }

    /// <summary>
    ///关卡Id 
    /// </summary>
    public int GameLevelId { get; set; }

    /// <summary>
    ///难度等级 
    /// </summary>
    public byte Grade { get; set; }

    /// <summary>
    ///星级 
    /// </summary>
    public byte Star { get; set; }

    /// <summary>
    ///是否扫荡中 
    /// </summary>
    public byte IsMopUp { get; set; }

    /// <summary>
    ///创建时间 
    /// </summary>
    public DateTime CreateTime { get; set; }

    #endregion
}
