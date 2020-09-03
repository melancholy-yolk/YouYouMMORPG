--服务器返回出售物品给系统消息
Goods_SellToSysReturnProto = { ProtoCode = 16009, IsSuccess = false, MsgCode = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Goods_SellToSysReturnProto.__index = Goods_SellToSysReturnProto;

function Goods_SellToSysReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, Goods_SellToSysReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Goods_SellToSysReturnProto.SendProto(proto)

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
function Goods_SellToSysReturnProto.GetProto(buffer)

    local proto = Goods_SellToSysReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(not proto.IsSuccess) then
    end
    proto.MsgCode = ms:ReadInt();

    ms:Dispose();
    return proto;
end