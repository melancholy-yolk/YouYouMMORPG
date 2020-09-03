--客户端发送登录区服消息
RoleOperation_LogOnGameServerProto = { ProtoCode = 10001, AccountId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleOperation_LogOnGameServerProto.__index = RoleOperation_LogOnGameServerProto;

function RoleOperation_LogOnGameServerProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleOperation_LogOnGameServerProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleOperation_LogOnGameServerProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.AccountId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleOperation_LogOnGameServerProto.GetProto(buffer)

    local proto = RoleOperation_LogOnGameServerProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.AccountId = ms:ReadInt();

    ms:Dispose();
    return proto;
end