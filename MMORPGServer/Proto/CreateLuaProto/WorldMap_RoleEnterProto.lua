--客户端发送进入世界地图场景消息
WorldMap_RoleEnterProto = { ProtoCode = 13001, WorldMapSceneId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_RoleEnterProto.__index = WorldMap_RoleEnterProto;

function WorldMap_RoleEnterProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_RoleEnterProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_RoleEnterProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.WorldMapSceneId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_RoleEnterProto.GetProto(buffer)

    local proto = WorldMap_RoleEnterProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.WorldMapSceneId = ms:ReadInt();

    ms:Dispose();
    return proto;
end