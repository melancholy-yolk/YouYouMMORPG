--服务器返回角色信息
RoleOperation_SelectRoleInfoReturnProto = { ProtoCode = 10010, IsSuccess = false, MsgCode = 0, RoldId = 0, RoleNickName = "", JobId = 0, Level = 0, TotalRechargeMoney = 0, Money = 0, Gold = 0, Exp = 0, MaxHP = 0, MaxMP = 0, CurrHP = 0, CurrMP = 0, Attack = 0, Defense = 0, Hit = 0, Dodge = 0, Cri = 0, Res = 0, Fighting = 0, LastInWorldMapId = 0, LastInWorldMapPos = "", Equip_Weapon = 0, Equip_Pants = 0, Equip_Clothes = 0, Equip_Belt = 0, Equip_Cuff = 0, Equip_Necklace = 0, Equip_Shoe = 0, Equip_Ring = 0, Equip_WeaponTableId = 0, Equip_PantsTableId = 0, Equip_ClothesTableId = 0, Equip_BeltTableId = 0, Equip_CuffTableId = 0, Equip_NecklaceTableId = 0, Equip_ShoeTableId = 0, Equip_RingTableId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleOperation_SelectRoleInfoReturnProto.__index = RoleOperation_SelectRoleInfoReturnProto;

function RoleOperation_SelectRoleInfoReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleOperation_SelectRoleInfoReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleOperation_SelectRoleInfoReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteBool(proto.IsSuccess);
    if(proto.IsSuccess) then
        ms:WriteInt(RoldId);
        ms:WriteUTF8String(RoleNickName);
        ms:WriteByte(JobId);
        ms:WriteInt(Level);
        ms:WriteInt(TotalRechargeMoney);
        ms:WriteInt(Money);
        ms:WriteInt(Gold);
        ms:WriteInt(Exp);
        ms:WriteInt(MaxHP);
        ms:WriteInt(MaxMP);
        ms:WriteInt(CurrHP);
        ms:WriteInt(CurrMP);
        ms:WriteInt(Attack);
        ms:WriteInt(Defense);
        ms:WriteInt(Hit);
        ms:WriteInt(Dodge);
        ms:WriteInt(Cri);
        ms:WriteInt(Res);
        ms:WriteInt(Fighting);
        ms:WriteInt(LastInWorldMapId);
        ms:WriteUTF8String(LastInWorldMapPos);
        ms:WriteInt(Equip_Weapon);
        ms:WriteInt(Equip_Pants);
        ms:WriteInt(Equip_Clothes);
        ms:WriteInt(Equip_Belt);
        ms:WriteInt(Equip_Cuff);
        ms:WriteInt(Equip_Necklace);
        ms:WriteInt(Equip_Shoe);
        ms:WriteInt(Equip_Ring);
        ms:WriteInt(Equip_WeaponTableId);
        ms:WriteInt(Equip_PantsTableId);
        ms:WriteInt(Equip_ClothesTableId);
        ms:WriteInt(Equip_BeltTableId);
        ms:WriteInt(Equip_CuffTableId);
        ms:WriteInt(Equip_NecklaceTableId);
        ms:WriteInt(Equip_ShoeTableId);
        ms:WriteInt(Equip_RingTableId);
        else
        ms:WriteInt(MsgCode);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleOperation_SelectRoleInfoReturnProto.GetProto(buffer)

    local proto = RoleOperation_SelectRoleInfoReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(proto.IsSuccess) then
        proto.RoldId = ms:ReadInt();
        proto.RoleNickName = ms:ReadUTF8String();
        proto.JobId = ms:ReadByte();
        proto.Level = ms:ReadInt();
        proto.TotalRechargeMoney = ms:ReadInt();
        proto.Money = ms:ReadInt();
        proto.Gold = ms:ReadInt();
        proto.Exp = ms:ReadInt();
        proto.MaxHP = ms:ReadInt();
        proto.MaxMP = ms:ReadInt();
        proto.CurrHP = ms:ReadInt();
        proto.CurrMP = ms:ReadInt();
        proto.Attack = ms:ReadInt();
        proto.Defense = ms:ReadInt();
        proto.Hit = ms:ReadInt();
        proto.Dodge = ms:ReadInt();
        proto.Cri = ms:ReadInt();
        proto.Res = ms:ReadInt();
        proto.Fighting = ms:ReadInt();
        proto.LastInWorldMapId = ms:ReadInt();
        proto.LastInWorldMapPos = ms:ReadUTF8String();
        proto.Equip_Weapon = ms:ReadInt();
        proto.Equip_Pants = ms:ReadInt();
        proto.Equip_Clothes = ms:ReadInt();
        proto.Equip_Belt = ms:ReadInt();
        proto.Equip_Cuff = ms:ReadInt();
        proto.Equip_Necklace = ms:ReadInt();
        proto.Equip_Shoe = ms:ReadInt();
        proto.Equip_Ring = ms:ReadInt();
        proto.Equip_WeaponTableId = ms:ReadInt();
        proto.Equip_PantsTableId = ms:ReadInt();
        proto.Equip_ClothesTableId = ms:ReadInt();
        proto.Equip_BeltTableId = ms:ReadInt();
        proto.Equip_CuffTableId = ms:ReadInt();
        proto.Equip_NecklaceTableId = ms:ReadInt();
        proto.Equip_ShoeTableId = ms:ReadInt();
        proto.Equip_RingTableId = ms:ReadInt();
        else
        proto.MsgCode = ms:ReadInt();
    end

    ms:Dispose();
    return proto;
end