--客户端发送进入游戏消息
RoleOperation_EnterGameProto = { ProtoCode = 10007, RoleId = 0, ChannelId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleOperation_EnterGameProto.__index = RoleOperation_EnterGameProto;

function RoleOperation_EnterGameProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleOperation_EnterGameProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleOperation_EnterGameProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RoleId);
    ms:WriteInt(proto.ChannelId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleOperation_EnterGameProto.GetProto(buffer)

    local proto = RoleOperation_EnterGameProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RoleId = ms:ReadInt();
    proto.ChannelId = ms:ReadInt();

    ms:Dispose();
    return proto;
end