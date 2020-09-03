//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2020-03-15 10:58:27
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送查询装备详情消息
/// </summary>
public struct Goods_SearchEquipDetailProto : IProto
{
    public ushort ProtoCode { get { return 16006; } }

    public int GoodsServerId; //物品服务器端编号

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(GoodsServerId);
            return ms.ToArray();
        }
    }

    public static Goods_SearchEquipDetailProto GetProto(byte[] buffer)
    {
        Goods_SearchEquipDetailProto proto = new Goods_SearchEquipDetailProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.GoodsServerId = ms.ReadInt();
        }
        return proto;
    }
}