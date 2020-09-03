//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2020-03-15 10:58:27
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送出售物品给系统消息
/// </summary>
public struct Goods_SellToSysProto : IProto
{
    public ushort ProtoCode { get { return 16008; } }

    public int roleBackpackId; //背包项编号
    public byte GoodsType; //物品类型
    public int GoodsId; //物品编号
    public int GoodsServerId; //服务服务器端编号
    public int SellCount; //出售数量

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(roleBackpackId);
            ms.WriteByte(GoodsType);
            ms.WriteInt(GoodsId);
            ms.WriteInt(GoodsServerId);
            ms.WriteInt(SellCount);
            return ms.ToArray();
        }
    }

    public static Goods_SellToSysProto GetProto(byte[] buffer)
    {
        Goods_SellToSysProto proto = new Goods_SellToSysProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.roleBackpackId = ms.ReadInt();
            proto.GoodsType = (byte)ms.ReadByte();
            proto.GoodsId = ms.ReadInt();
            proto.GoodsServerId = ms.ReadInt();
            proto.SellCount = ms.ReadInt();
        }
        return proto;
    }
}