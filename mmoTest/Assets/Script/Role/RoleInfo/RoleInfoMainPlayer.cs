using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleInfoMainPlayer : RoleInfoBase 
{
    public byte JobId; //ְҵ���
    public int Level; //�ȼ�
    public int TotalRechargeMoney; //�ܳ�ֵ���
    public int Money; //Ԫ��
    public int Gold; //���
    public int Exp;//����

    public RoleInfoMainPlayer() : base()
    {
        
    }

    public RoleInfoMainPlayer(RoleOperation_SelectRoleInfoReturnProto proto)
    {
        RoleId = proto.RoldId; //��ɫ���
        RoleNickName = proto.RoleNickName; //��ɫ�ǳ�
        JobId = proto.JobId; //ְҵ���
        Level = proto.Level; //�ȼ�
        TotalRechargeMoney = proto.TotalRechargeMoney; //�ܳ�ֵ���
        Money = proto.Money; //Ԫ��
        Gold = proto.Gold; //���
        Exp = proto.Exp; //����
        MaxHP = proto.MaxHP; //���HP
        MaxMP = proto.MaxMP; //���MP
        CurrHP = proto.CurrHP; //��ǰHP
        CurrMP = proto.CurrMP; //��ǰMP
        Attack = proto.Attack; //������
        Defense = proto.Defense; //����
        Hit = proto.Hit; //����
        Dodge = proto.Dodge; //����
        Cri = proto.Cri; //����
        Res = proto.Res; //����
        Fighting = proto.Fighting; //�ۺ�ս����

        SkillList = new List<RoleInfoSkill>();

    //public int LastInWorldMapId; //������������ͼ���
    //public string LastInWorldMapPos; //������������ͼ����
    //public int Equip_Weapon; //��������
    //public int Equip_Pants; //��������
    //public int Equip_Clothes; //�����·�
    //public int Equip_Belt; //��������
    //public int Equip_Cuff; //��������
    //public int Equip_Necklace; //��������
    //public int Equip_Shoe; //����Ь
    //public int Equip_Ring; //������ָ
    //public int Equip_WeaponTableId; //��������
    //public int Equip_PantsTableId; //��������
    //public int Equip_ClothesTableId; //�����·�
    //public int Equip_BeltTableId; //��������
    //public int Equip_CuffTableId; //��������
    //public int Equip_NecklaceTableId; //��������
    //public int Equip_ShoeTableId; //����Ь
    //public int Equip_RingTableId; //������ָ
    }

    public void LoadSkill(RoleData_SkillReturnProto proto)
    {
        SkillList.Clear();
        for (int i = 0; i < proto.SkillCount; i++)
        {
            SkillList.Add(new RoleInfoSkill() {
                SkillId = proto.CurrSkillDataList[i].SkillId,
                SkillLevel = proto.CurrSkillDataList[i].SkillLevel,
                SlotsNo = proto.CurrSkillDataList[i].SlotsNo
            });
        }
    }

    

}
