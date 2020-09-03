--服务器广播角色更新信息消息
WorldMap_OtherRoleUpdateInfoProto = { ProtoCode = 13014, RoldId = 0, RoleNickName = "" }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_OtherRoleUpdateInfoProto.__index = WorldMap_OtherRoleUpdateInfoProto;

function WorldMap_OtherRoleUpdateInfoProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_OtherRoleUpdateInfoProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_OtherRoleUpdateInfoProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RoldId);
    ms:WriteUTF8String(proto.RoleNickName);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_OtherRoleUpdateInfoProto.GetProto(buffer)

    local proto = WorldMap_OtherRoleUpdateInfoProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RoldId = ms:ReadInt();
    proto.RoleNickName = ms:ReadUTF8String();

    ms:Dispose();
    return proto;
end