//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2020-03-15 10:58:27
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端查询角色信息
/// </summary>
public struct RoleOperation_SelectRoleInfoProto : IProto
{
    public ushort ProtoCode { get { return 10009; } }

    public int RoldId; //角色编号

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(RoldId);
            return ms.ToArray();
        }
    }

    public static RoleOperation_SelectRoleInfoProto GetProto(byte[] buffer)
    {
        RoleOperation_SelectRoleInfoProto proto = new RoleOperation_SelectRoleInfoProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.RoldId = ms.ReadInt();
        }
        return proto;
    }
}