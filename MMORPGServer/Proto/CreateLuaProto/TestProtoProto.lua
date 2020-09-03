--测试协议
TestProtoProto = { ProtoCode = 17001, Id = 0, Name = "", price = 0, Type = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
TestProtoProto.__index = TestProtoProto;

function TestProtoProto.New()
    local self = { }; --初始化self
    setmetatable(self, TestProtoProto); --将self的元表设定为Class
    return self;
end


--发送协议
function TestProtoProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.Id);
    ms:WriteUTF8String(proto.Name);
    ms:WriteFloat(proto.price);
    ms:WriteInt(proto.Type);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function TestProtoProto.GetProto(buffer)

    local proto = TestProtoProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.Id = ms:ReadInt();
    proto.Name = ms:ReadUTF8String();
    proto.price = ms:ReadFloat();
    proto.Type = ms:ReadInt();

    ms:Dispose();
    return proto;
end