//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2020-03-15 10:58:27
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送角色已经进入世界地图场景消息
/// </summary>
public struct WorldMap_RoleAlreadyEnterProto : IProto
{
    public ushort ProtoCode { get { return 13004; } }

    public int TargetWorldMapSceneId; //目标场景编号
    public float RolePosX; //角色坐标X
    public float RolePosY; //角色坐标Y
    public float RolePosZ; //角色坐标Z
    public float RoleYAngle; //角色Y轴旋转

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(TargetWorldMapSceneId);
            ms.WriteFloat(RolePosX);
            ms.WriteFloat(RolePosY);
            ms.WriteFloat(RolePosZ);
            ms.WriteFloat(RoleYAngle);
            return ms.ToArray();
        }
    }

    public static WorldMap_RoleAlreadyEnterProto GetProto(byte[] buffer)
    {
        WorldMap_RoleAlreadyEnterProto proto = new WorldMap_RoleAlreadyEnterProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.TargetWorldMapSceneId = ms.ReadInt();
            proto.RolePosX = ms.ReadFloat();
            proto.RolePosY = ms.ReadFloat();
            proto.RolePosZ = ms.ReadFloat();
            proto.RoleYAngle = ms.ReadFloat();
        }
        return proto;
    }
}