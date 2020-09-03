--服务器返回战斗胜利消息
GameLevel_VictoryReturnProto = { ProtoCode = 12004, IsSuccess = false, MsgCode = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
GameLevel_VictoryReturnProto.__index = GameLevel_VictoryReturnProto;

function GameLevel_VictoryReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, GameLevel_VictoryReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function GameLevel_VictoryReturnProto.SendProto(proto)

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
function GameLevel_VictoryReturnProto.GetProto(buffer)

    local proto = GameLevel_VictoryReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(not proto.IsSuccess) then
        proto.MsgCode = ms:ReadInt();
    end

    ms:Dispose();
    return proto;
end