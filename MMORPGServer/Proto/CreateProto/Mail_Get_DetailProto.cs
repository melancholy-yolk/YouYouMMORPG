//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 获取邮件详情
/// </summary>
public struct Mail_Get_DetailProto : IProto
{
    public ushort ProtoCode { get { return 17002; } }

    public bool IsSuccess; //是否成功
    public string MailTitle; //邮件标题
    public string MailContent; //邮件内容
    public string ErrorMsg; //错误信息

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteBool(IsSuccess);
            if(IsSuccess)
            {
                ms.WriteUTF8String(MailTitle);
                ms.WriteUTF8String(MailContent);
            }
            else
            {
                ms.WriteUTF8String(ErrorMsg);
            }
            return ms.ToArray();
        }
    }

    public static Mail_Get_DetailProto GetProto(byte[] buffer)
    {
        Mail_Get_DetailProto proto = new Mail_Get_DetailProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.IsSuccess = ms.ReadBool();
            if(proto.IsSuccess)
            {
                proto.MailTitle = ms.ReadUTF8String();
                proto.MailContent = ms.ReadUTF8String();
            }
            else
            {
                proto.ErrorMsg = ms.ReadUTF8String();
            }
        }
        return proto;
    }
}