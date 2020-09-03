//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:24:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 请求邮件列表
/// </summary>
public struct Mail_RequestProto : IProto
{
    public ushort ProtoCode { get { return 17004; } }


    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            return ms.ToArray();
        }
    }

    public static Mail_RequestProto GetProto(byte[] buffer)
    {
        Mail_RequestProto proto = new Mail_RequestProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
        }
        return proto;
    }
}