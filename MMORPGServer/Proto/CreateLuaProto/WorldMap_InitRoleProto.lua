--服务器广播当前场景角色
WorldMap_InitRoleProto = { ProtoCode = 13007, RoleCount = 0, ItemTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_InitRoleProto.__index = WorldMap_InitRoleProto;

function WorldMap_InitRoleProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_InitRoleProto); --将self的元表设定为Class
    return self;
end


--定义角色列表
Item = { RoleId = 0, RoleNickName = "", RoleLevel = 0, RoleMaxHP = 0, RoleCurrHP = 0, RoleMaxMP = 0, RoleCurrMP = 0, RoleJobId = 0, RolePosX = 0, RolePosY = 0, RolePosZ = 0, RoleYAngle = 0 }
Item.__index = Item;
function Item.New()
    local self = { };
    setmetatable(self, Item);
    return self;
end


--发送协议
function WorldMap_InitRoleProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RoleCount);
    for i = 1, proto.RoleCount, 1 do
        ms:WriteInt(ItemList[i].RoleId);
        ms:WriteUTF8String(ItemList[i].RoleNickName);
        ms:WriteInt(ItemList[i].RoleLevel);
        ms:WriteInt(ItemList[i].RoleMaxHP);
        ms:WriteInt(ItemList[i].RoleCurrHP);
        ms:WriteInt(ItemList[i].RoleMaxMP);
        ms:WriteInt(ItemList[i].RoleCurrMP);
        ms:WriteInt(ItemList[i].RoleJobId);
        ms:WriteFloat(ItemList[i].RolePosX);
        ms:WriteFloat(ItemList[i].RolePosY);
        ms:WriteFloat(ItemList[i].RolePosZ);
        ms:WriteFloat(ItemList[i].RoleYAngle);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_InitRoleProto.GetProto(buffer)

    local proto = WorldMap_InitRoleProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RoleCount = ms:ReadInt();
	proto.ItemTable = {};
    for i = 1, proto.RoleCount, 1 do
        local _Item = Item.New();
        _Item.RoleId = ms:ReadInt();
        _Item.RoleNickName = ms:ReadUTF8String();
        _Item.RoleLevel = ms:ReadInt();
        _Item.RoleMaxHP = ms:ReadInt();
        _Item.RoleCurrHP = ms:ReadInt();
        _Item.RoleMaxMP = ms:ReadInt();
        _Item.RoleCurrMP = ms:ReadInt();
        _Item.RoleJobId = ms:ReadInt();
        _Item.RolePosX = ms:ReadFloat();
        _Item.RolePosY = ms:ReadFloat();
        _Item.RolePosZ = ms:ReadFloat();
        _Item.RoleYAngle = ms:ReadFloat();
        proto.ItemTable[#proto.ItemTable+1] = _Item;
    end

    ms:Dispose();
    return proto;
end