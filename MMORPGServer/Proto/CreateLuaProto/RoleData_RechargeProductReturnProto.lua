--服务器返回充值产品信息
RoleData_RechargeProductReturnProto = { ProtoCode = 11003, RechargeProductCount = 0, CurrItemTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleData_RechargeProductReturnProto.__index = RoleData_RechargeProductReturnProto;

function RoleData_RechargeProductReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleData_RechargeProductReturnProto); --将self的元表设定为Class
    return self;
end


--定义充值产品项
CurrItem = { RechargeProductId = 0, ProductDesc = "", CanBuy = 0, RemainDay = 0, DoubleFlag = 0 }
CurrItem.__index = CurrItem;
function CurrItem.New()
    local self = { };
    setmetatable(self, CurrItem);
    return self;
end


--发送协议
function RoleData_RechargeProductReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RechargeProductCount);
    for i = 1, proto.RechargeProductCount, 1 do
        ms:WriteInt(CurrItemList[i].RechargeProductId);
        ms:WriteUTF8String(CurrItemList[i].ProductDesc);
        ms:WriteByte(CurrItemList[i].CanBuy);
        ms:WriteInt(CurrItemList[i].RemainDay);
        ms:WriteByte(CurrItemList[i].DoubleFlag);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleData_RechargeProductReturnProto.GetProto(buffer)

    local proto = RoleData_RechargeProductReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RechargeProductCount = ms:ReadInt();
	proto.CurrItemTable = {};
    for i = 1, proto.RechargeProductCount, 1 do
        local _CurrItem = CurrItem.New();
        _CurrItem.RechargeProductId = ms:ReadInt();
        _CurrItem.ProductDesc = ms:ReadUTF8String();
        _CurrItem.CanBuy = ms:ReadByte();
        _CurrItem.RemainDay = ms:ReadInt();
        _CurrItem.DoubleFlag = ms:ReadByte();
        proto.CurrItemTable[#proto.CurrItemTable+1] = _CurrItem;
    end

    ms:Dispose();
    return proto;
end