//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2018-02-25 22:40:38
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器广播其他角色死亡消息
/// </summary>
public struct WorldMap_OtherRoleDieProto : IProto
{
    public ushort ProtoCode { get { return 13012; } }

    public int AttackRoleId; //发起攻击角色Id
    public int DieCount; //死亡角色数量
    public List<int> RoleIdList; //角色编号

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(AttackRoleId);
            ms.WriteInt(DieCount);
            for (int i = 0; i < DieCount; i++)
            {
                ms.WriteInt(RoleIdList[i]);
            }
            return ms.ToArray();
        }
    }

    public static WorldMap_OtherRoleDieProto GetProto(byte[] buffer)
    {
        WorldMap_OtherRoleDieProto proto = new WorldMap_OtherRoleDieProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.AttackRoleId = ms.ReadInt();
            proto.DieCount = ms.ReadInt();
            proto.RoleIdList = new List<int>();
            for (int i = 0; i < proto.DieCount; i++)
            {
                int _RoleId = ms.ReadInt();  //角色编号
                proto.RoleIdList.Add(_RoleId);
            }
        }
        return proto;
    }
}