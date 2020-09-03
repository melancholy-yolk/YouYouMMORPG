/// <summary>
/// 类名 : Role_MaterialEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2017-10-23 12:52:59
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
public partial class Role_MaterialEntity : MFAbstractEntity
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
    ///创建时间 
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    ///更新时间 
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    ///基础材料编号 
    /// </summary>
    public int MaterialId { get; set; }

    /// <summary>
    ///是否已经镶嵌 
    /// </summary>
    public bool IsInlay { get; set; }

    /// <summary>
    ///镶嵌到的装备编号 
    /// </summary>
    public int InlayEquipServerId { get; set; }

    #endregion
}
