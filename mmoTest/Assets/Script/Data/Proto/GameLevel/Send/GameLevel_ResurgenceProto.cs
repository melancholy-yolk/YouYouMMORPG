//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2018-02-25 22:40:38
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送复活消息
/// </summary>
public struct GameLevel_ResurgenceProto : IProto
{
    public ushort ProtoCode { get { return 12007; } }

    public int GameLevelId; //游戏关卡Id
    public byte Grade; //难度等级
    public byte Type; //复活类型

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(GameLevelId);
            ms.WriteByte(Grade);
            ms.WriteByte(Type);
            return ms.ToArray();
        }
    }

    public static GameLevel_ResurgenceProto GetProto(byte[] buffer)
    {
        GameLevel_ResurgenceProto proto = new GameLevel_ResurgenceProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.GameLevelId = ms.ReadInt();
            proto.Grade = (byte)ms.ReadByte();
            proto.Type = (byte)ms.ReadByte();
        }
        return proto;
    }
}