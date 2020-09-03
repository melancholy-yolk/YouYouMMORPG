--客户端发送创建角色消息
RoleOperation_CreateRoleProto = { ProtoCode = 10003, JobId = 0, RoleNickName = "" }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleOperation_CreateRoleProto.__index = RoleOperation_CreateRoleProto;

function RoleOperation_CreateRoleProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleOperation_CreateRoleProto); --将self的元表设定为Class
    return self;
end


--发送协议
function RoleOperation_CreateRoleProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteByte(proto.JobId);
    ms:WriteUTF8String(proto.RoleNickName);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleOperation_CreateRoleProto.GetProto(buffer)

    local proto = RoleOperation_CreateRoleProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.JobId = ms:ReadByte();
    proto.RoleNickName = ms:ReadUTF8String();

    ms:Dispose();
    return proto;
end