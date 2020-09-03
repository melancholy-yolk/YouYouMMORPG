--服务器返回购买商城物品消息
Shop_BuyProductReturnProto = { ProtoCode = 16002, IsSuccess = false, MsgCode = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Shop_BuyProductReturnProto.__index = Shop_BuyProductReturnProto;

function Shop_BuyProductReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, Shop_BuyProductReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Shop_BuyProductReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteBool(proto.IsSuccess);
    if(not proto.IsSuccess) then
    end
    ms:WriteInt(proto.MsgCode);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Shop_BuyProductReturnProto.GetProto(buffer)

    local proto = Shop_BuyProductReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(not proto.IsSuccess) then
    end
    proto.MsgCode = ms:ReadInt();

    ms:Dispose();
    return proto;
end