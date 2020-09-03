--客户端发送本地时间
System_SendLocalTimeProto = { ProtoCode = 14001, LocalTime = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
System_SendLocalTimeProto.__index = System_SendLocalTimeProto;

function System_SendLocalTimeProto.New()
    local self = { }; --初始化self
    setmetatable(self, System_SendLocalTimeProto); --将self的元表设定为Class
    return self;
end


--发送协议
function System_SendLocalTimeProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteFloat(proto.LocalTime);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function System_SendLocalTimeProto.GetProto(buffer)

    local proto = System_SendLocalTimeProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.LocalTime = ms:ReadFloat();

    ms:Dispose();
    return proto;
end