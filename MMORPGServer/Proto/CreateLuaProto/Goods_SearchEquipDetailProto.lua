--客户端发送查询装备详情消息
Goods_SearchEquipDetailProto = { ProtoCode = 16006, GoodsServerId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Goods_SearchEquipDetailProto.__index = Goods_SearchEquipDetailProto;

function Goods_SearchEquipDetailProto.New()
    local self = { }; --初始化self
    setmetatable(self, Goods_SearchEquipDetailProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Goods_SearchEquipDetailProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.GoodsServerId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Goods_SearchEquipDetailProto.GetProto(buffer)

    local proto = Goods_SearchEquipDetailProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.GoodsServerId = ms:ReadInt();

    ms:Dispose();
    return proto;
end