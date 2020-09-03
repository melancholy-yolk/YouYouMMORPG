--服务器返回使用道具消息
Goods_UseItemReturnProto = { ProtoCode = 16011, IsSuccess = false, MsgCode = 0, GoodsId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Goods_UseItemReturnProto.__index = Goods_UseItemReturnProto;

function Goods_UseItemReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, Goods_UseItemReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Goods_UseItemReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteBool(proto.IsSuccess);
    if(proto.IsSuccess) then
        ms:WriteInt(GoodsId);
        else
        ms:WriteInt(MsgCode);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Goods_UseItemReturnProto.GetProto(buffer)

    local proto = Goods_UseItemReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(proto.IsSuccess) then
        proto.GoodsId = ms:ReadInt();
        else
        proto.MsgCode = ms:ReadInt();
    end

    ms:Dispose();
    return proto;
end