--服务器广播其他角色移动消息
WorldMap_OtherRoleMoveProto = { ProtoCode = 13009, RoleId = 0, TargetPosX = 0, TargetPosY = 0, TargetPosZ = 0, ServerTime = 0, NeedTime = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_OtherRoleMoveProto.__index = WorldMap_OtherRoleMoveProto;

function WorldMap_OtherRoleMoveProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_OtherRoleMoveProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_OtherRoleMoveProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RoleId);
    ms:WriteFloat(proto.TargetPosX);
    ms:WriteFloat(proto.TargetPosY);
    ms:WriteFloat(proto.TargetPosZ);
    ms:WriteLong(proto.ServerTime);
    ms:WriteInt(proto.NeedTime);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_OtherRoleMoveProto.GetProto(buffer)

    local proto = WorldMap_OtherRoleMoveProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RoleId = ms:ReadInt();
    proto.TargetPosX = ms:ReadFloat();
    proto.TargetPosY = ms:ReadFloat();
    proto.TargetPosZ = ms:ReadFloat();
    proto.ServerTime = ms:ReadLong();
    proto.NeedTime = ms:ReadInt();

    ms:Dispose();
    return proto;
end