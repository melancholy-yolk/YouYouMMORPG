using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleInfoMainPlayer : RoleInfoBase 
{
    public byte JobId; //职业编号
    public int Level; //等级
    public int TotalRechargeMoney; //总充值金额
    public int Money; //元宝
    public int Gold; //金币
    public int Exp;//经验

    public RoleInfoMainPlayer() : base()
    {
        
    }

    public RoleInfoMainPlayer(RoleOperation_SelectRoleInfoReturnProto proto)
    {
        RoleId = proto.RoldId; //角色编号
        RoleNickName = proto.RoleNickName; //角色昵称
        JobId = proto.JobId; //职业编号
        Level = proto.Level; //等级
        TotalRechargeMoney = proto.TotalRechargeMoney; //总充值金额
        Money = proto.Money; //元宝
        Gold = proto.Gold; //金币
        Exp = proto.Exp; //经验
        MaxHP = proto.MaxHP; //最大HP
        MaxMP = proto.MaxMP; //最大MP
        CurrHP = proto.CurrHP; //当前HP
        CurrMP = proto.CurrMP; //当前MP
        Attack = proto.Attack; //攻击力
        Defense = proto.Defense; //防御
        Hit = proto.Hit; //命中
        Dodge = proto.Dodge; //闪避
        Cri = proto.Cri; //暴击
        Res = proto.Res; //抗性
        Fighting = proto.Fighting; //综合战斗力

        SkillList = new List<RoleInfoSkill>();

    //public int LastInWorldMapId; //最后进入的世界地图编号
    //public string LastInWorldMapPos; //最后进入的世界地图坐标
    //public int Equip_Weapon; //穿戴武器
    //public int Equip_Pants; //穿戴护腿
    //public int Equip_Clothes; //穿戴衣服
    //public int Equip_Belt; //穿戴腰带
    //public int Equip_Cuff; //穿戴护腕
    //public int Equip_Necklace; //穿戴项链
    //public int Equip_Shoe; //穿戴鞋
    //public int Equip_Ring; //穿戴戒指
    //public int Equip_WeaponTableId; //穿戴武器
    //public int Equip_PantsTableId; //穿戴护腿
    //public int Equip_ClothesTableId; //穿戴衣服
    //public int Equip_BeltTableId; //穿戴腰带
    //public int Equip_CuffTableId; //穿戴护腕
    //public int Equip_NecklaceTableId; //穿戴项链
    //public int Equip_ShoeTableId; //穿戴鞋
    //public int Equip_RingTableId; //穿戴戒指
    }

    public void LoadSkill(RoleData_SkillReturnProto proto)
    {
        SkillList.Clear();
        for (int i = 0; i < proto.SkillCount; i++)
        {
            SkillList.Add(new RoleInfoSkill() {
                SkillId = proto.CurrSkillDataList[i].SkillId,
                SkillLevel = proto.CurrSkillDataList[i].SkillLevel,
                SlotsNo = proto.CurrSkillDataList[i].SlotsNo
            });
        }
    }

    

}
