//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2018-02-25 22:40:38
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器广播其他角色移动消息
/// </summary>
public struct WorldMap_OtherRoleMoveProto : IProto
{
    public ushort ProtoCode { get { return 13009; } }

    public int RoleId; //角色编号
    public float TargetPosX; //目标坐标X
    public float TargetPosY; //目标坐标Y
    public float TargetPosZ; //目标坐标Z
    public long ServerTime; //客户端发包时的服务器时间
    public int NeedTime; //客户端移动所需时间

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(RoleId);
            ms.WriteFloat(TargetPosX);
            ms.WriteFloat(TargetPosY);
            ms.WriteFloat(TargetPosZ);
            ms.WriteLong(ServerTime);
            ms.WriteInt(NeedTime);
            return ms.ToArray();
        }
    }

    public static WorldMap_OtherRoleMoveProto GetProto(byte[] buffer)
    {
        WorldMap_OtherRoleMoveProto proto = new WorldMap_OtherRoleMoveProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.RoleId = ms.ReadInt();
            proto.TargetPosX = ms.ReadFloat();
            proto.TargetPosY = ms.ReadFloat();
            proto.TargetPosZ = ms.ReadFloat();
            proto.ServerTime = ms.ReadLong();
            proto.NeedTime = ms.ReadInt();
        }
        return proto;
    }
}