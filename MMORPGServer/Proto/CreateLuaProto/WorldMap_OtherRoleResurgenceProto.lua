--服务器广播角色复活消息
WorldMap_OtherRoleResurgenceProto = { ProtoCode = 13016, RoleId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_OtherRoleResurgenceProto.__index = WorldMap_OtherRoleResurgenceProto;

function WorldMap_OtherRoleResurgenceProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_OtherRoleResurgenceProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_OtherRoleResurgenceProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RoleId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_OtherRoleResurgenceProto.GetProto(buffer)

    local proto = WorldMap_OtherRoleResurgenceProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RoleId = ms:ReadInt();

    ms:Dispose();
    return proto;
end