--请求邮件列表
Mail_Request_ListProto = { ProtoCode = 17004 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Mail_Request_ListProto.__index = Mail_Request_ListProto;

function Mail_Request_ListProto.New()
    local self = { }; --初始化self
    setmetatable(self, Mail_Request_ListProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Mail_Request_ListProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);


    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Mail_Request_ListProto.GetProto(buffer)

    local proto = Mail_Request_ListProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);


    ms:Dispose();
    return proto;
end