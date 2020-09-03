--服务器广播其他角色进入场景消息
WorldMap_OtherRoleEnterProto = { ProtoCode = 13005, RoleId = 0, RoleNickName = "", RoleLevel = 0, RoleJobId = 0, RoleCurrMP = 0, RoleMaxMP = 0, RoleCurrHP = 0, RoleMaxHP = 0, RolePosX = 0, RolePosY = 0, RolePosZ = 0, RoleYAngle = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_OtherRoleEnterProto.__index = WorldMap_OtherRoleEnterProto;

function WorldMap_OtherRoleEnterProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_OtherRoleEnterProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_OtherRoleEnterProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RoleId);
    ms:WriteUTF8String(proto.RoleNickName);
    ms:WriteInt(proto.RoleLevel);
    ms:WriteInt(proto.RoleJobId);
    ms:WriteInt(proto.RoleCurrMP);
    ms:WriteInt(proto.RoleMaxMP);
    ms:WriteInt(proto.RoleCurrHP);
    ms:WriteInt(proto.RoleMaxHP);
    ms:WriteFloat(proto.RolePosX);
    ms:WriteFloat(proto.RolePosY);
    ms:WriteFloat(proto.RolePosZ);
    ms:WriteFloat(proto.RoleYAngle);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_OtherRoleEnterProto.GetProto(buffer)

    local proto = WorldMap_OtherRoleEnterProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RoleId = ms:ReadInt();
    proto.RoleNickName = ms:ReadUTF8String();
    proto.RoleLevel = ms:ReadInt();
    proto.RoleJobId = ms:ReadInt();
    proto.RoleCurrMP = ms:ReadInt();
    proto.RoleMaxMP = ms:ReadInt();
    proto.RoleCurrHP = ms:ReadInt();
    proto.RoleMaxHP = ms:ReadInt();
    proto.RolePosX = ms:ReadFloat();
    proto.RolePosY = ms:ReadFloat();
    proto.RolePosZ = ms:ReadFloat();
    proto.RoleYAngle = ms:ReadFloat();

    ms:Dispose();
    return proto;
end