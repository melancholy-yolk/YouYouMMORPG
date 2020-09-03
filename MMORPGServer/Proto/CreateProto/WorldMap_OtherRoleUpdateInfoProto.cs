//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器广播角色更新信息消息
/// </summary>
public struct WorldMap_OtherRoleUpdateInfoProto : IProto
{
    public ushort ProtoCode { get { return 13014; } }

    public int RoldId; //角色编号
    public string RoleNickName; //角色昵称

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(RoldId);
            ms.WriteUTF8String(RoleNickName);
            return ms.ToArray();
        }
    }

    public static WorldMap_OtherRoleUpdateInfoProto GetProto(byte[] buffer)
    {
        WorldMap_OtherRoleUpdateInfoProto proto = new WorldMap_OtherRoleUpdateInfoProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.RoldId = ms.ReadInt();
            proto.RoleNickName = ms.ReadUTF8String();
        }
        return proto;
    }
}