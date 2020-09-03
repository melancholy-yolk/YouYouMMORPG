/// <summary>
/// 类名 : RoleSkillEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2016-09-19 22:31:52
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
public partial class RoleSkillEntity : MFAbstractEntity
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
    /// 
    /// </summary>
    public int SkillId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int SkillLevel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public byte SlotsNo { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime UpdateTime { get; set; }

    #endregion
}
