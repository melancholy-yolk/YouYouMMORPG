--测试协议
TestProto = { ProtoCode = 17001, Id = 0, Name = "", price = 0, Type = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
TestProto.__index = TestProto;

function TestProto.New()
    local self = { }; --初始化self
    setmetatable(self, TestProto); --将self的元表设定为Class
    return self;
end


--发送协议
function TestProto.SendProto(proto)

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
function TestProto.GetProto(buffer)

    local proto = TestProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.Id = ms:ReadInt();
    proto.Name = ms:ReadUTF8String();
    proto.price = ms:ReadFloat();
    proto.Type = ms:ReadInt();

    ms:Dispose();
    return proto;
end