--服务器返回任务列表消息
Task_SearchTaskReturnProto = { ProtoCode = 15002, TaskCount = 0, CurrTaskItemTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Task_SearchTaskReturnProto.__index = Task_SearchTaskReturnProto;

function Task_SearchTaskReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, Task_SearchTaskReturnProto); --将self的元表设定为Class
    return self;
end


--定义任务项
CurrTaskItem = { Id = 0, Name = "", Status = 0, Content = "" }
CurrTaskItem.__index = CurrTaskItem;
function CurrTaskItem.New()
    local self = { };
    setmetatable(self, CurrTaskItem);
    return self;
end


--发送协议
function Task_SearchTaskReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.TaskCount);
    for i = 1, proto.TaskCount, 1 do
        ms:WriteInt(CurrTaskItemList[i].Id);
        ms:WriteUTF8String(CurrTaskItemList[i].Name);
        ms:WriteInt(CurrTaskItemList[i].Status);
        ms:WriteUTF8String(CurrTaskItemList[i].Content);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Task_SearchTaskReturnProto.GetProto(buffer)

    local proto = Task_SearchTaskReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.TaskCount = ms:ReadInt();
	proto.CurrTaskItemTable = {};
    for i = 1, proto.TaskCount, 1 do
        local _CurrTaskItem = CurrTaskItem.New();
        _CurrTaskItem.Id = ms:ReadInt();
        _CurrTaskItem.Name = ms:ReadUTF8String();
        _CurrTaskItem.Status = ms:ReadInt();
        _CurrTaskItem.Content = ms:ReadUTF8String();
        proto.CurrTaskItemTable[#proto.CurrTaskItemTable+1] = _CurrTaskItem;
    end

    ms:Dispose();
    return proto;
end