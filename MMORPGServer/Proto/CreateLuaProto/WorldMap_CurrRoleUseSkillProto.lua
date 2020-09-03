--客户端发送角色使用技能消息
WorldMap_CurrRoleUseSkillProto = { ProtoCode = 13010, SkillId = 0, SkillLevel = 0, RolePosX = 0, RolePosY = 0, RolePosZ = 0, RoleYAngle = 0, BeAttackCount = 0, ItemTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_CurrRoleUseSkillProto.__index = WorldMap_CurrRoleUseSkillProto;

function WorldMap_CurrRoleUseSkillProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_CurrRoleUseSkillProto); --将self的元表设定为Class
    return self;
end


--定义被攻击者
Item = { BeAttackRoleId = 0 }
Item.__index = Item;
function Item.New()
    local self = { };
    setmetatable(self, Item);
    return self;
end


--发送协议
function WorldMap_CurrRoleUseSkillProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.SkillId);
    ms:WriteInt(proto.SkillLevel);
    ms:WriteFloat(proto.RolePosX);
    ms:WriteFloat(proto.RolePosY);
    ms:WriteFloat(proto.RolePosZ);
    ms:WriteFloat(proto.RoleYAngle);
    ms:WriteInt(proto.BeAttackCount);
    for i = 1, proto.BeAttackCount, 1 do
        ms:WriteInt(ItemList[i].BeAttackRoleId);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_CurrRoleUseSkillProto.GetProto(buffer)

    local proto = WorldMap_CurrRoleUseSkillProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.SkillId = ms:ReadInt();
    proto.SkillLevel = ms:ReadInt();
    proto.RolePosX = ms:ReadFloat();
    proto.RolePosY = ms:ReadFloat();
    proto.RolePosZ = ms:ReadFloat();
    proto.RoleYAngle = ms:ReadFloat();
    proto.BeAttackCount = ms:ReadInt();
	proto.ItemTable = {};
    for i = 1, proto.BeAttackCount, 1 do
        local _Item = Item.New();
        _Item.BeAttackRoleId = ms:ReadInt();
        proto.ItemTable[#proto.ItemTable+1] = _Item;
    end

    ms:Dispose();
    return proto;
end