--客户端查询角色信息
RoleOperation_SelectRoleInfoProto = { ProtoCode = 10009, RoldId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleOperation_SelectRoleInfoProto.__index = RoleOperation_SelectRoleInfoProto;

function RoleOperation_SelectRoleInfoProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleOperation_SelectRoleInfoProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleOperation_SelectRoleInfoProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RoldId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleOperation_SelectRoleInfoProto.GetProto(buffer)

    local proto = RoleOperation_SelectRoleInfoProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RoldId = ms:ReadInt();

    ms:Dispose();
    return proto;
end