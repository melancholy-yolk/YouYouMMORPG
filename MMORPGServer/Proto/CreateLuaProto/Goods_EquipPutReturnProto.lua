--服务器返回穿戴消息
Goods_EquipPutReturnProto = { ProtoCode = 16013, IsSuccess = false, MsgCode = 0, Type = 0, GoodsId = 0, GoodsServerId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Goods_EquipPutReturnProto.__index = Goods_EquipPutReturnProto;

function Goods_EquipPutReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, Goods_EquipPutReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Goods_EquipPutReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteBool(proto.IsSuccess);
    if(proto.IsSuccess) then
        ms:WriteByte(Type);
        ms:WriteInt(GoodsId);
        ms:WriteInt(GoodsServerId);
        else
    end
    ms:WriteInt(proto.MsgCode);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Goods_EquipPutReturnProto.GetProto(buffer)

    local proto = Goods_EquipPutReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(proto.IsSuccess) then
        proto.Type = ms:ReadByte();
        proto.GoodsId = ms:ReadInt();
        proto.GoodsServerId = ms:ReadInt();
        else
    end
    proto.MsgCode = ms:ReadInt();

    ms:Dispose();
    return proto;
end