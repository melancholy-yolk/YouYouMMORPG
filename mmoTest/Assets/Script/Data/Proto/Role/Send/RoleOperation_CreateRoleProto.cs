//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2020-03-15 10:58:27
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送创建角色消息
/// </summary>
public struct RoleOperation_CreateRoleProto : IProto
{
    public ushort ProtoCode { get { return 10003; } }

    public byte JobId; //职业ID
    public string RoleNickName; //角色名称

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteByte(JobId);
            ms.WriteUTF8String(RoleNickName);
            return ms.ToArray();
        }
    }

    public static RoleOperation_CreateRoleProto GetProto(byte[] buffer)
    {
        RoleOperation_CreateRoleProto proto = new RoleOperation_CreateRoleProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.JobId = (byte)ms.ReadByte();
            proto.RoleNickName = ms.ReadUTF8String();
        }
        return proto;
    }
}