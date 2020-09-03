/// <summary>
/// 类名 : Role_EquipEntity
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2017-10-23 12:44:24
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
public partial class Role_EquipEntity : MFAbstractEntity
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
    ///基础装备编号 
    /// </summary>
    public int EquipId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public byte Type { get; set; }

    /// <summary>
    ///品质 
    /// </summary>
    public byte Quality { get; set; }

    /// <summary>
    ///星级 
    /// </summary>
    public byte Star { get; set; }

    /// <summary>
    ///强化等级 
    /// </summary>
    public int EnchantLevel { get; set; }

    /// <summary>
    ///强化次数 
    /// </summary>
    public int EnchantCount { get; set; }

    /// <summary>
    ///基础属性1类型 
    /// </summary>
    public byte BaseAttr1Type { get; set; }

    /// <summary>
    ///基础属性1值 
    /// </summary>
    public int BaseAttr1Value { get; set; }

    /// <summary>
    ///基础属性2类型 
    /// </summary>
    public byte BaseAttr2Type { get; set; }

    /// <summary>
    ///基础属性2值 
    /// </summary>
    public int BaseAttr2Value { get; set; }

    /// <summary>
    ///HP 
    /// </summary>
    public int HP { get; set; }

    /// <summary>
    ///MP 
    /// </summary>
    public int MP { get; set; }

    /// <summary>
    ///攻击 
    /// </summary>
    public int Attack { get; set; }

    /// <summary>
    ///防御 
    /// </summary>
    public int Defense { get; set; }

    /// <summary>
    ///命中 
    /// </summary>
    public int Hit { get; set; }

    /// <summary>
    ///闪避 
    /// </summary>
    public int Dodge { get; set; }

    /// <summary>
    ///暴击 
    /// </summary>
    public int Cri { get; set; }

    /// <summary>
    ///抗性 
    /// </summary>
    public int Res { get; set; }

    /// <summary>
    ///创建时间 
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    ///更新时间 
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    ///是否已经穿戴 
    /// </summary>
    public bool IsPutOn { get; set; }

    #endregion
}
