--服务器返回角色充值信息
RoleData_RechargeReturnProto = { ProtoCode = 11002, IsSuccess = false, RechargeProductId = 0, RechargeProductType = 0, Money = 0, RemainDay = 0, ErrorCode = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleData_RechargeReturnProto.__index = RoleData_RechargeReturnProto;

function RoleData_RechargeReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleData_RechargeReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleData_RechargeReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteBool(proto.IsSuccess);
    if(proto.IsSuccess) then
        ms:WriteInt(RechargeProductId);
        ms:WriteByte(RechargeProductType);
        ms:WriteInt(Money);
        ms:WriteInt(RemainDay);
        else
        ms:WriteInt(ErrorCode);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleData_RechargeReturnProto.GetProto(buffer)

    local proto = RoleData_RechargeReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(proto.IsSuccess) then
        proto.RechargeProductId = ms:ReadInt();
        proto.RechargeProductType = ms:ReadByte();
        proto.Money = ms:ReadInt();
        proto.RemainDay = ms:ReadInt();
        else
        proto.ErrorCode = ms:ReadInt();
    end

    ms:Dispose();
    return proto;
end