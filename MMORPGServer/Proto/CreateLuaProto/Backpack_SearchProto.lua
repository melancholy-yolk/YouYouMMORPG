--客户端发送查询背包项消息
Backpack_SearchProto = { ProtoCode = 16004 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Backpack_SearchProto.__index = Backpack_SearchProto;

function Backpack_SearchProto.New()
    local self = { }; --初始化self
    setmetatable(self, Backpack_SearchProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Backpack_SearchProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);


    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Backpack_SearchProto.GetProto(buffer)

    local proto = Backpack_SearchProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);


    ms:Dispose();
    return proto;
end