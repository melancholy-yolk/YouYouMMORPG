--客户端发送删除角色消息
RoleOperation_DeleteRoleProto = { ProtoCode = 10005, RoleId = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleOperation_DeleteRoleProto.__index = RoleOperation_DeleteRoleProto;

function RoleOperation_DeleteRoleProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleOperation_DeleteRoleProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleOperation_DeleteRoleProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.RoleId);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleOperation_DeleteRoleProto.GetProto(buffer)

    local proto = RoleOperation_DeleteRoleProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.RoleId = ms:ReadInt();

    ms:Dispose();
    return proto;
end