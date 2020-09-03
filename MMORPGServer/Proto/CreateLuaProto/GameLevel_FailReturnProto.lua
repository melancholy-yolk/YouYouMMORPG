--服务器返回战斗失败消息
GameLevel_FailReturnProto = { ProtoCode = 12006, IsSuccess = false, MsgCode = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
GameLevel_FailReturnProto.__index = GameLevel_FailReturnProto;

function GameLevel_FailReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, GameLevel_FailReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function GameLevel_FailReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteBool(proto.IsSuccess);
    if(not proto.IsSuccess) then
        ms:WriteInt(MsgCode);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function GameLevel_FailReturnProto.GetProto(buffer)

    local proto = GameLevel_FailReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(not proto.IsSuccess) then
        proto.MsgCode = ms:ReadInt();
    end

    ms:Dispose();
    return proto;
end