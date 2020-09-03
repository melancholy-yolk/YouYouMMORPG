//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送使用道具消息
/// </summary>
public struct Goods_UseItemProto : IProto
{
    public ushort ProtoCode { get { return 16010; } }

    public int BackpackItemId; //背包项编号
    public int GoodsId; //物品编号

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(BackpackItemId);
            ms.WriteInt(GoodsId);
            return ms.ToArray();
        }
    }

    public static Goods_UseItemProto GetProto(byte[] buffer)
    {
        Goods_UseItemProto proto = new Goods_UseItemProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.BackpackItemId = ms.ReadInt();
            proto.GoodsId = ms.ReadInt();
        }
        return proto;
    }
}