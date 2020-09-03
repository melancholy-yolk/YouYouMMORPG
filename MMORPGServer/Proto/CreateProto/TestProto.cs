//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 测试协议
/// </summary>
public struct TestProto : IProto
{
    public ushort ProtoCode { get { return 17001; } }

    public int Id; //
    public string Name; //
    public float price; //
    public int Type; //

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(Id);
            ms.WriteUTF8String(Name);
            ms.WriteFloat(price);
            ms.WriteInt(Type);
            return ms.ToArray();
        }
    }

    public static TestProto GetProto(byte[] buffer)
    {
        TestProto proto = new TestProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.Id = ms.ReadInt();
            proto.Name = ms.ReadUTF8String();
            proto.price = ms.ReadFloat();
            proto.Type = ms.ReadInt();
        }
        return proto;
    }
}