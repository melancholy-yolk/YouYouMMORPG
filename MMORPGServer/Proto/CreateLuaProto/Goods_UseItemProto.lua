--客户端发送使用道具消息
Goods_UseItemProto = { ProtoCode = 16010, BackpackItemId = 0, GoodsId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Goods_UseItemProto.__index = Goods_UseItemProto;

function Goods_UseItemProto.New()
    local self = { }; --初始化self
    setmetatable(self, Goods_UseItemProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Goods_UseItemProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.BackpackItemId);
    ms:WriteInt(proto.GoodsId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Goods_UseItemProto.GetProto(buffer)

    local proto = Goods_UseItemProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.BackpackItemId = ms:ReadInt();
    proto.GoodsId = ms:ReadInt();

    ms:Dispose();
    return proto;
end