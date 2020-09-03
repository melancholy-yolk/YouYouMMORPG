--服务器返回服务器时间
System_ServerTimeReturnProto = { ProtoCode = 14002, LocalTime = 0, ServerTime = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
System_ServerTimeReturnProto.__index = System_ServerTimeReturnProto;

function System_ServerTimeReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, System_ServerTimeReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function System_ServerTimeReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteFloat(proto.LocalTime);
    ms:WriteLong(proto.ServerTime);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function System_ServerTimeReturnProto.GetProto(buffer)

    local proto = System_ServerTimeReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.LocalTime = ms:ReadFloat();
    proto.ServerTime = ms:ReadLong();

    ms:Dispose();
    return proto;
end