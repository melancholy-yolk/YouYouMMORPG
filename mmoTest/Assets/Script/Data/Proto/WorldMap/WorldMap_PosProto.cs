//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2018-02-25 22:40:38
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送自身坐标
/// </summary>
public struct WorldMap_PosProto : IProto
{
    public ushort ProtoCode { get { return 13003; } }

    public float x; //x
    public float y; //y
    public float z; //z
    public float yAngle; //y轴旋转

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteFloat(x);
            ms.WriteFloat(y);
            ms.WriteFloat(z);
            ms.WriteFloat(yAngle);
            return ms.ToArray();
        }
    }

    public static WorldMap_PosProto GetProto(byte[] buffer)
    {
        WorldMap_PosProto proto = new WorldMap_PosProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.x = ms.ReadFloat();
            proto.y = ms.ReadFloat();
            proto.z = ms.ReadFloat();
            proto.yAngle = ms.ReadFloat();
        }
        return proto;
    }
}