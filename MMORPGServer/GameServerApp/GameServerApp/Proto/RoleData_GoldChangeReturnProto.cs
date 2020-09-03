//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2020-03-15 10:58:27
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器返回金币更新消息
/// </summary>
public struct RoleData_GoldChangeReturnProto : IProto
{
    public ushort ProtoCode { get { return 11005; } }

    public int OldGold; //更新前金币
    public int CurrGold; //更新后当前金币
    public byte ChangeType; //更新方式 0=增加 1=减少
    public byte AddType; //增加的方式
    public byte ReduceType; //减少方式
    public byte GoodsType; //物品类型
    public int GoodsId; //物品编号

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(OldGold);
            ms.WriteInt(CurrGold);
            ms.WriteByte(ChangeType);
            ms.WriteByte(AddType);
            ms.WriteByte(ReduceType);
            ms.WriteByte(GoodsType);
            ms.WriteInt(GoodsId);
            return ms.ToArray();
        }
    }

    public static RoleData_GoldChangeReturnProto GetProto(byte[] buffer)
    {
        RoleData_GoldChangeReturnProto proto = new RoleData_GoldChangeReturnProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.OldGold = ms.ReadInt();
            proto.CurrGold = ms.ReadInt();
            proto.ChangeType = (byte)ms.ReadByte();
            proto.AddType = (byte)ms.ReadByte();
            proto.ReduceType = (byte)ms.ReadByte();
            proto.GoodsType = (byte)ms.ReadByte();
            proto.GoodsId = ms.ReadInt();
        }
        return proto;
    }
}