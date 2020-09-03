//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送战斗失败消息
/// </summary>
public struct GameLevel_FailProto : IProto
{
    public ushort ProtoCode { get { return 12005; } }

    public int GameLevelId; //游戏关卡Id
    public byte Grade; //难度等级

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(GameLevelId);
            ms.WriteByte(Grade);
            return ms.ToArray();
        }
    }

    public static GameLevel_FailProto GetProto(byte[] buffer)
    {
        GameLevel_FailProto proto = new GameLevel_FailProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.GameLevelId = ms.ReadInt();
            proto.Grade = (byte)ms.ReadByte();
        }
        return proto;
    }
}