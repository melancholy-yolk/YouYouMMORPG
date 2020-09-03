--客户端发送角色复活消息
WorldMap_CurrRoleResurgenceProto = { ProtoCode = 13015, Type = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_CurrRoleResurgenceProto.__index = WorldMap_CurrRoleResurgenceProto;

function WorldMap_CurrRoleResurgenceProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_CurrRoleResurgenceProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_CurrRoleResurgenceProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.Type);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_CurrRoleResurgenceProto.GetProto(buffer)

    local proto = WorldMap_CurrRoleResurgenceProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.Type = ms:ReadInt();

    ms:Dispose();
    return proto;
end