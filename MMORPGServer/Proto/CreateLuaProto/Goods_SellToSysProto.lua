--客户端发送出售物品给系统消息
Goods_SellToSysProto = { ProtoCode = 16008, roleBackpackId = 0, GoodsType = 0, GoodsId = 0, GoodsServerId = 0, SellCount = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Goods_SellToSysProto.__index = Goods_SellToSysProto;

function Goods_SellToSysProto.New()
    local self = { }; --初始化self
    setmetatable(self, Goods_SellToSysProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Goods_SellToSysProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.roleBackpackId);
    ms:WriteByte(proto.GoodsType);
    ms:WriteInt(proto.GoodsId);
    ms:WriteInt(proto.GoodsServerId);
    ms:WriteInt(proto.SellCount);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Goods_SellToSysProto.GetProto(buffer)

    local proto = Goods_SellToSysProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.roleBackpackId = ms:ReadInt();
    proto.GoodsType = ms:ReadByte();
    proto.GoodsId = ms:ReadInt();
    proto.GoodsServerId = ms:ReadInt();
    proto.SellCount = ms:ReadInt();

    ms:Dispose();
    return proto;
end