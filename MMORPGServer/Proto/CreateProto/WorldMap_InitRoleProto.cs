//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器广播当前场景角色
/// </summary>
public struct WorldMap_InitRoleProto : IProto
{
    public ushort ProtoCode { get { return 13007; } }

    public int RoleCount; //角色数量
    public List<RoleItem> ItemList; //角色列表

    /// <summary>
    /// 角色列表
    /// </summary>
    public struct RoleItem
    {
        public int RoleId; //角色编号
        public string RoleNickName; //角色昵称
        public int RoleLevel; //角色等级
        public int RoleMaxHP; //最大血量
        public int RoleCurrHP; //当前血量
        public int RoleMaxMP; //最大魔法
        public int RoleCurrMP; //当前魔法
        public int RoleJobId; //角色职业编号
        public float RolePosX; //角色坐标X
        public float RolePosY; //角色坐标Y
        public float RolePosZ; //角色坐标Z
        public float RoleYAngle; //角色Y轴旋转
    }

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(RoleCount);
            for (int i = 0; i < RoleCount; i++)
            {
                ms.WriteInt(ItemList[i].RoleId);
                ms.WriteUTF8String(ItemList[i].RoleNickName);
                ms.WriteInt(ItemList[i].RoleLevel);
                ms.WriteInt(ItemList[i].RoleMaxHP);
                ms.WriteInt(ItemList[i].RoleCurrHP);
                ms.WriteInt(ItemList[i].RoleMaxMP);
                ms.WriteInt(ItemList[i].RoleCurrMP);
                ms.WriteInt(ItemList[i].RoleJobId);
                ms.WriteFloat(ItemList[i].RolePosX);
                ms.WriteFloat(ItemList[i].RolePosY);
                ms.WriteFloat(ItemList[i].RolePosZ);
                ms.WriteFloat(ItemList[i].RoleYAngle);
            }
            return ms.ToArray();
        }
    }

    public static WorldMap_InitRoleProto GetProto(byte[] buffer)
    {
        WorldMap_InitRoleProto proto = new WorldMap_InitRoleProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.RoleCount = ms.ReadInt();
            proto.ItemList = new List<RoleItem>();
            for (int i = 0; i < proto.RoleCount; i++)
            {
                RoleItem _Item = new RoleItem();
                _Item.RoleId = ms.ReadInt();
                _Item.RoleNickName = ms.ReadUTF8String();
                _Item.RoleLevel = ms.ReadInt();
                _Item.RoleMaxHP = ms.ReadInt();
                _Item.RoleCurrHP = ms.ReadInt();
                _Item.RoleMaxMP = ms.ReadInt();
                _Item.RoleCurrMP = ms.ReadInt();
                _Item.RoleJobId = ms.ReadInt();
                _Item.RolePosX = ms.ReadFloat();
                _Item.RolePosY = ms.ReadFloat();
                _Item.RolePosZ = ms.ReadFloat();
                _Item.RoleYAngle = ms.ReadFloat();
                proto.ItemList.Add(_Item);
            }
        }
        return proto;
    }
}