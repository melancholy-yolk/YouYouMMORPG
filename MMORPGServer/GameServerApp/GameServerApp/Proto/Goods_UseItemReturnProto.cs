//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2020-03-15 10:58:27
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器返回使用道具消息
/// </summary>
public struct Goods_UseItemReturnProto : IProto
{
    public ushort ProtoCode { get { return 16011; } }

    public bool IsSuccess; //是否成功
    public int MsgCode; //消息码
    public int GoodsId; //物品编号

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteBool(IsSuccess);
            if(IsSuccess)
            {
                ms.WriteInt(GoodsId);
            }
            else
            {
                ms.WriteInt(MsgCode);
            }
            return ms.ToArray();
        }
    }

    public static Goods_UseItemReturnProto GetProto(byte[] buffer)
    {
        Goods_UseItemReturnProto proto = new Goods_UseItemReturnProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.IsSuccess = ms.ReadBool();
            if(proto.IsSuccess)
            {
                proto.GoodsId = ms.ReadInt();
            }
            else
            {
                proto.MsgCode = ms.ReadInt();
            }
        }
        return proto;
    }
}