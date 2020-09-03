//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送战斗胜利消息
/// </summary>
public struct GameLevel_VictoryProto : IProto
{
    public ushort ProtoCode { get { return 12003; } }

    public int GameLevelId; //游戏关卡Id
    public byte Grade; //难度等级
    public byte Star; //获得星级
    public int Exp; //获得经验
    public int Gold; //获得金币
    public int KillTotalMonsterCount; //杀怪数量
    public List<MonsterItem> KillMonsterList; //杀怪列表
    public int GoodsTotalCount; //获得物品数量
    public List<GoodsItem> GetGoodsList; //获得物品

    /// <summary>
    /// 杀怪列表
    /// </summary>
    public struct MonsterItem
    {
        public int MonsterId; //怪Id
        public int MonsterCount; //怪数量
    }

    /// <summary>
    /// 获得物品
    /// </summary>
    public struct GoodsItem
    {
        public byte GoodsType; //物品类型
        public int GoodsId; //物品Id
        public int GoodsCount; //物品数量
    }

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(GameLevelId);
            ms.WriteByte(Grade);
            ms.WriteByte(Star);
            ms.WriteInt(Exp);
            ms.WriteInt(Gold);
            ms.WriteInt(KillTotalMonsterCount);
            for (int i = 0; i < KillTotalMonsterCount; i++)
            {
                ms.WriteInt(KillMonsterList[i].MonsterId);
                ms.WriteInt(KillMonsterList[i].MonsterCount);
            }
            ms.WriteInt(GoodsTotalCount);
            for (int i = 0; i < GoodsTotalCount; i++)
            {
                ms.WriteByte(GetGoodsList[i].GoodsType);
                ms.WriteInt(GetGoodsList[i].GoodsId);
                ms.WriteInt(GetGoodsList[i].GoodsCount);
            }
            return ms.ToArray();
        }
    }

    public static GameLevel_VictoryProto GetProto(byte[] buffer)
    {
        GameLevel_VictoryProto proto = new GameLevel_VictoryProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.GameLevelId = ms.ReadInt();
            proto.Grade = (byte)ms.ReadByte();
            proto.Star = (byte)ms.ReadByte();
            proto.Exp = ms.ReadInt();
            proto.Gold = ms.ReadInt();
            proto.KillTotalMonsterCount = ms.ReadInt();
            proto.KillMonsterList = new List<MonsterItem>();
            for (int i = 0; i < proto.KillTotalMonsterCount; i++)
            {
                MonsterItem _KillMonster = new MonsterItem();
                _KillMonster.MonsterId = ms.ReadInt();
                _KillMonster.MonsterCount = ms.ReadInt();
                proto.KillMonsterList.Add(_KillMonster);
            }
            proto.GoodsTotalCount = ms.ReadInt();
            proto.GetGoodsList = new List<GoodsItem>();
            for (int i = 0; i < proto.GoodsTotalCount; i++)
            {
                GoodsItem _GetGoods = new GoodsItem();
                _GetGoods.GoodsType = (byte)ms.ReadByte();
                _GetGoods.GoodsId = ms.ReadInt();
                _GetGoods.GoodsCount = ms.ReadInt();
                proto.GetGoodsList.Add(_GetGoods);
            }
        }
        return proto;
    }
}