//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器返回背包物品更新消息
/// </summary>
public struct Backpack_GoodsChangeReturnProto : IProto
{
    public ushort ProtoCode { get { return 16003; } }

    public int BackpackItemChangeCount; //更新的种类数量
    public List<ChangeItem> ItemList; //更改项

    /// <summary>
    /// 更改项
    /// </summary>
    public struct ChangeItem
    {
        public int BackpackId; //背包项编号
        public byte ChangeType; //改变类型
        public byte GoodsType; //物品类型
        public int GoodsId; //物品编号
        public int GoodsCount; //物品数量
        public int GoodsServerId; //物品服务器端Id
    }

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(BackpackItemChangeCount);
            for (int i = 0; i < BackpackItemChangeCount; i++)
            {
                ms.WriteInt(ItemList[i].BackpackId);
                ms.WriteByte(ItemList[i].ChangeType);
                ms.WriteByte(ItemList[i].GoodsType);
                ms.WriteInt(ItemList[i].GoodsId);
                ms.WriteInt(ItemList[i].GoodsCount);
                ms.WriteInt(ItemList[i].GoodsServerId);
            }
            return ms.ToArray();
        }
    }

    public static Backpack_GoodsChangeReturnProto GetProto(byte[] buffer)
    {
        Backpack_GoodsChangeReturnProto proto = new Backpack_GoodsChangeReturnProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.BackpackItemChangeCount = ms.ReadInt();
            proto.ItemList = new List<ChangeItem>();
            for (int i = 0; i < proto.BackpackItemChangeCount; i++)
            {
                ChangeItem _Item = new ChangeItem();
                _Item.BackpackId = ms.ReadInt();
                _Item.ChangeType = (byte)ms.ReadByte();
                _Item.GoodsType = (byte)ms.ReadByte();
                _Item.GoodsId = ms.ReadInt();
                _Item.GoodsCount = ms.ReadInt();
                _Item.GoodsServerId = ms.ReadInt();
                proto.ItemList.Add(_Item);
            }
        }
        return proto;
    }
}