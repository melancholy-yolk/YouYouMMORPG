//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 获取物品列表
/// </summary>
public struct Get_Item_ListProto : IProto
{
    public ushort ProtoCode { get { return 17003; } }

    public int ItemCount; //物品数量
    public List<ItemEntity> ItemList; //一个物品对象(自定义类型)

    /// <summary>
    /// 一个物品对象(自定义类型)
    /// </summary>
    public struct ItemEntity
    {
        public string ItemName; //自定义类型的一个字段
        public float ItemPrice; //自定义类型的一个字段
    }

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(ItemCount);
            for (int i = 0; i < ItemCount; i++)
            {
                ms.WriteUTF8String(ItemList[i].ItemName);
                ms.WriteFloat(ItemList[i].ItemPrice);
            }
            return ms.ToArray();
        }
    }

    public static Get_Item_ListProto GetProto(byte[] buffer)
    {
        Get_Item_ListProto proto = new Get_Item_ListProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.ItemCount = ms.ReadInt();
            proto.ItemList = new List<ItemEntity>();
            for (int i = 0; i < proto.ItemCount; i++)
            {
                ItemEntity _Item = new ItemEntity();
                _Item.ItemName = ms.ReadUTF8String();
                _Item.ItemPrice = ms.ReadFloat();
                proto.ItemList.Add(_Item);
            }
        }
        return proto;
    }
}