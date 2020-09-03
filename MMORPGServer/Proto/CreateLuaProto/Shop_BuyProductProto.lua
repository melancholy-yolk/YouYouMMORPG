--客户端发送购买商城物品消息
Shop_BuyProductProto = { ProtoCode = 16001, ProductId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Shop_BuyProductProto.__index = Shop_BuyProductProto;

function Shop_BuyProductProto.New()
    local self = { }; --初始化self
    setmetatable(self, Shop_BuyProductProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Shop_BuyProductProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.ProductId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Shop_BuyProductProto.GetProto(buffer)

    local proto = Shop_BuyProductProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.ProductId = ms:ReadInt();

    ms:Dispose();
    return proto;
end