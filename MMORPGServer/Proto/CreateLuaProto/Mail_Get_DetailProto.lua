--获取邮件详情
Mail_Get_DetailProto = { ProtoCode = 17002, IsSuccess = false, MailTitle = "", MailContent = "", ErrorMsg = "" }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Mail_Get_DetailProto.__index = Mail_Get_DetailProto;

function Mail_Get_DetailProto.New()
    local self = { }; --初始化self
    setmetatable(self, Mail_Get_DetailProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Mail_Get_DetailProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteBool(proto.IsSuccess);
    if(proto.IsSuccess) then
        ms:WriteUTF8String(MailTitle);
        ms:WriteUTF8String(MailContent);
        else
        ms:WriteUTF8String(ErrorMsg);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Mail_Get_DetailProto.GetProto(buffer)

    local proto = Mail_Get_DetailProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(proto.IsSuccess) then
        proto.MailTitle = ms:ReadUTF8String();
        proto.MailContent = ms:ReadUTF8String();
        else
        proto.ErrorMsg = ms:ReadUTF8String();
    end

    ms:Dispose();
    return proto;
end