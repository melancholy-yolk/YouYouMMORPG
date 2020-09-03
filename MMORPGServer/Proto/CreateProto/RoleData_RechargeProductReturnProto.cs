//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器返回充值产品信息
/// </summary>
public struct RoleData_RechargeProductReturnProto : IProto
{
    public ushort ProtoCode { get { return 11003; } }

    public int RechargeProductCount; //充值产品数量
    public List<RechargeProductItem> CurrItemList; //充值产品项

    /// <summary>
    /// 充值产品项
    /// </summary>
    public struct RechargeProductItem
    {
        public int RechargeProductId; //充值产品编号
        public string ProductDesc; //描述
        public byte CanBuy; //是否可以购买
        public int RemainDay; //剩余天数
        public byte DoubleFlag; //是否双倍
    }

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(RechargeProductCount);
            for (int i = 0; i < RechargeProductCount; i++)
            {
                ms.WriteInt(CurrItemList[i].RechargeProductId);
                ms.WriteUTF8String(CurrItemList[i].ProductDesc);
                ms.WriteByte(CurrItemList[i].CanBuy);
                ms.WriteInt(CurrItemList[i].RemainDay);
                ms.WriteByte(CurrItemList[i].DoubleFlag);
            }
            return ms.ToArray();
        }
    }

    public static RoleData_RechargeProductReturnProto GetProto(byte[] buffer)
    {
        RoleData_RechargeProductReturnProto proto = new RoleData_RechargeProductReturnProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.RechargeProductCount = ms.ReadInt();
            proto.CurrItemList = new List<RechargeProductItem>();
            for (int i = 0; i < proto.RechargeProductCount; i++)
            {
                RechargeProductItem _CurrItem = new RechargeProductItem();
                _CurrItem.RechargeProductId = ms.ReadInt();
                _CurrItem.ProductDesc = ms.ReadUTF8String();
                _CurrItem.CanBuy = (byte)ms.ReadByte();
                _CurrItem.RemainDay = ms.ReadInt();
                _CurrItem.DoubleFlag = (byte)ms.ReadByte();
                proto.CurrItemList.Add(_CurrItem);
            }
        }
        return proto;
    }
}