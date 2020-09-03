--客户端发送查询任务消息
Task_SearchTaskProto = { ProtoCode = 15001 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Task_SearchTaskProto.__index = Task_SearchTaskProto;

function Task_SearchTaskProto.New()
    local self = { }; --初始化self
    setmetatable(self, Task_SearchTaskProto); --将self的元表设定为Class
    return self;
end


--发送协议
function Task_SearchTaskProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);


    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Task_SearchTaskProto.GetProto(buffer)

    local proto = Task_SearchTaskProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);


    ms:Dispose();
    return proto;
end