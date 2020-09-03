--客户端发送穿戴消息
Goods_EquipPutProto = { ProtoCode = 16012, Type = 0, GoodsId = 0, GoodsServerId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Goods_EquipPutProto.__index = Goods_EquipPutProto;

function Goods_EquipPutProto.New()
    local self = { }; --初始化self
    setmetatable(self, Goods_EquipPutProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Goods_EquipPutProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteByte(proto.Type);
    ms:WriteInt(proto.GoodsId);
    ms:WriteInt(proto.GoodsServerId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Goods_EquipPutProto.GetProto(buffer)

    local proto = Goods_EquipPutProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.Type = ms:ReadByte();
    proto.GoodsId = ms:ReadInt();
    proto.GoodsServerId = ms:ReadInt();

    ms:Dispose();
    return proto;
end