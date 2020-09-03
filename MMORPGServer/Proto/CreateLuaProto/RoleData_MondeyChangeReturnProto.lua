--服务器返回元宝更新消息
RoleData_MondeyChangeReturnProto = { ProtoCode = 11004, OldMoney = 0, CurrMoney = 0, ChangeType = 0, AddType = 0, ReduceType = 0, GoodsType = 0, GoodsId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleData_MondeyChangeReturnProto.__index = RoleData_MondeyChangeReturnProto;

function RoleData_MondeyChangeReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleData_MondeyChangeReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleData_MondeyChangeReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.OldMoney);
    ms:WriteInt(proto.CurrMoney);
    ms:WriteByte(proto.ChangeType);
    ms:WriteByte(proto.AddType);
    ms:WriteByte(proto.ReduceType);
    ms:WriteByte(proto.GoodsType);
    ms:WriteInt(proto.GoodsId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleData_MondeyChangeReturnProto.GetProto(buffer)

    local proto = RoleData_MondeyChangeReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.OldMoney = ms:ReadInt();
    proto.CurrMoney = ms:ReadInt();
    proto.ChangeType = ms:ReadByte();
    proto.AddType = ms:ReadByte();
    proto.ReduceType = ms:ReadByte();
    proto.GoodsType = ms:ReadByte();
    proto.GoodsId = ms:ReadInt();

    ms:Dispose();
    return proto;
end