--客户端发送当前角色移动消息
WorldMap_CurrRoleMoveProto = { ProtoCode = 13008, TargetPosX = 0, TargetPosY = 0, TargetPosZ = 0, ServerTime = 0, NeedTime = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_CurrRoleMoveProto.__index = WorldMap_CurrRoleMoveProto;

function WorldMap_CurrRoleMoveProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_CurrRoleMoveProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_CurrRoleMoveProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteFloat(proto.TargetPosX);
    ms:WriteFloat(proto.TargetPosY);
    ms:WriteFloat(proto.TargetPosZ);
    ms:WriteLong(proto.ServerTime);
    ms:WriteInt(proto.NeedTime);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_CurrRoleMoveProto.GetProto(buffer)

    local proto = WorldMap_CurrRoleMoveProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.TargetPosX = ms:ReadFloat();
    proto.TargetPosY = ms:ReadFloat();
    proto.TargetPosZ = ms:ReadFloat();
    proto.ServerTime = ms:ReadLong();
    proto.NeedTime = ms:ReadInt();

    ms:Dispose();
    return proto;
end