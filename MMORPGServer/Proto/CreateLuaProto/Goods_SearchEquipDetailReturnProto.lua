--服务器返回查询装备详情消息
Goods_SearchEquipDetailReturnProto = { ProtoCode = 16007, EnchantLevel = 0, BaseAttr1Type = 0, BaseAttr1Value = 0, BaseAttr2Type = 0, BaseAttr2Value = 0, HP = 0, MP = 0, Attack = 0, Defense = 0, Hit = 0, Dodge = 0, Cri = 0, Res = 0, IsPutOn = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Goods_SearchEquipDetailReturnProto.__index = Goods_SearchEquipDetailReturnProto;

function Goods_SearchEquipDetailReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, Goods_SearchEquipDetailReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Goods_SearchEquipDetailReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.EnchantLevel);
    ms:WriteByte(proto.BaseAttr1Type);
    ms:WriteInt(proto.BaseAttr1Value);
    ms:WriteByte(proto.BaseAttr2Type);
    ms:WriteInt(proto.BaseAttr2Value);
    ms:WriteInt(proto.HP);
    ms:WriteInt(proto.MP);
    ms:WriteInt(proto.Attack);
    ms:WriteInt(proto.Defense);
    ms:WriteInt(proto.Hit);
    ms:WriteInt(proto.Dodge);
    ms:WriteInt(proto.Cri);
    ms:WriteInt(proto.Res);
    ms:WriteByte(proto.IsPutOn);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Goods_SearchEquipDetailReturnProto.GetProto(buffer)

    local proto = Goods_SearchEquipDetailReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.EnchantLevel = ms:ReadInt();
    proto.BaseAttr1Type = ms:ReadByte();
    proto.BaseAttr1Value = ms:ReadInt();
    proto.BaseAttr2Type = ms:ReadByte();
    proto.BaseAttr2Value = ms:ReadInt();
    proto.HP = ms:ReadInt();
    proto.MP = ms:ReadInt();
    proto.Attack = ms:ReadInt();
    proto.Defense = ms:ReadInt();
    proto.Hit = ms:ReadInt();
    proto.Dodge = ms:ReadInt();
    proto.Cri = ms:ReadInt();
    proto.Res = ms:ReadInt();
    proto.IsPutOn = ms:ReadByte();

    ms:Dispose();
    return proto;
end