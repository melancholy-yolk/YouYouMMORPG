--客户端发送角色已经进入世界地图场景消息
WorldMap_RoleAlreadyEnterProto = { ProtoCode = 13004, TargetWorldMapSceneId = 0, RolePosX = 0, RolePosY = 0, RolePosZ = 0, RoleYAngle = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_RoleAlreadyEnterProto.__index = WorldMap_RoleAlreadyEnterProto;

function WorldMap_RoleAlreadyEnterProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_RoleAlreadyEnterProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_RoleAlreadyEnterProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.TargetWorldMapSceneId);
    ms:WriteFloat(proto.RolePosX);
    ms:WriteFloat(proto.RolePosY);
    ms:WriteFloat(proto.RolePosZ);
    ms:WriteFloat(proto.RoleYAngle);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_RoleAlreadyEnterProto.GetProto(buffer)

    local proto = WorldMap_RoleAlreadyEnterProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.TargetWorldMapSceneId = ms:ReadInt();
    proto.RolePosX = ms:ReadFloat();
    proto.RolePosY = ms:ReadFloat();
    proto.RolePosZ = ms:ReadFloat();
    proto.RoleYAngle = ms:ReadFloat();

    ms:Dispose();
    return proto;
end