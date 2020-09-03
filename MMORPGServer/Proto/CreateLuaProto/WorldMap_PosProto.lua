--客户端发送自身坐标
WorldMap_PosProto = { ProtoCode = 13003, x = 0, y = 0, z = 0, yAngle = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_PosProto.__index = WorldMap_PosProto;

function WorldMap_PosProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_PosProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_PosProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteFloat(proto.x);
    ms:WriteFloat(proto.y);
    ms:WriteFloat(proto.z);
    ms:WriteFloat(proto.yAngle);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_PosProto.GetProto(buffer)

    local proto = WorldMap_PosProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.x = ms:ReadFloat();
    proto.y = ms:ReadFloat();
    proto.z = ms:ReadFloat();
    proto.yAngle = ms:ReadFloat();

    ms:Dispose();
    return proto;
end