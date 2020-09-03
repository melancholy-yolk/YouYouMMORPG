--服务器返回金币更新消息
RoleData_GoldChangeReturnProto = { ProtoCode = 11005, OldGold = 0, CurrGold = 0, ChangeType = 0, AddType = 0, ReduceType = 0, GoodsType = 0, GoodsId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleData_GoldChangeReturnProto.__index = RoleData_GoldChangeReturnProto;

function RoleData_GoldChangeReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleData_GoldChangeReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleData_GoldChangeReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.OldGold);
    ms:WriteInt(proto.CurrGold);
    ms:WriteByte(proto.ChangeType);
    ms:WriteByte(proto.AddType);
    ms:WriteByte(proto.ReduceType);
    ms:WriteByte(proto.GoodsType);
    ms:WriteInt(proto.GoodsId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleData_GoldChangeReturnProto.GetProto(buffer)

    local proto = RoleData_GoldChangeReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.OldGold = ms:ReadInt();
    proto.CurrGold = ms:ReadInt();
    proto.ChangeType = ms:ReadByte();
    proto.AddType = ms:ReadByte();
    proto.ReduceType = ms:ReadByte();
    proto.GoodsType = ms:ReadByte();
    proto.GoodsId = ms:ReadInt();

    ms:Dispose();
    return proto;
end