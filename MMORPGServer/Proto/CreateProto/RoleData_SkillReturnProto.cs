//===================================================
//作    者：崔炜斌
//创建时间：2019-12-13 19:29:52
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 服务器返回角色学会的技能
/// </summary>
public struct RoleData_SkillReturnProto : IProto
{
    public ushort ProtoCode { get { return 11001; } }

    public byte SkillCount; //学会的技能数量
    public List<SkillData> CurrSkillDataList; //当前学会的技能

    /// <summary>
    /// 当前学会的技能
    /// </summary>
    public struct SkillData
    {
        public int SkillId; //技能编号
        public int SkillLevel; //技能等级
        public byte SlotsNo; //技能槽编号
    }

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteByte(SkillCount);
            for (int i = 0; i < SkillCount; i++)
            {
                ms.WriteInt(CurrSkillDataList[i].SkillId);
                ms.WriteInt(CurrSkillDataList[i].SkillLevel);
                ms.WriteByte(CurrSkillDataList[i].SlotsNo);
            }
            return ms.ToArray();
        }
    }

    public static RoleData_SkillReturnProto GetProto(byte[] buffer)
    {
        RoleData_SkillReturnProto proto = new RoleData_SkillReturnProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.SkillCount = (byte)ms.ReadByte();
            proto.CurrSkillDataList = new List<SkillData>();
            for (int i = 0; i < proto.SkillCount; i++)
            {
                SkillData _CurrSkillData = new SkillData();
                _CurrSkillData.SkillId = ms.ReadInt();
                _CurrSkillData.SkillLevel = ms.ReadInt();
                _CurrSkillData.SlotsNo = (byte)ms.ReadByte();
                proto.CurrSkillDataList.Add(_CurrSkillData);
            }
        }
        return proto;
    }
}