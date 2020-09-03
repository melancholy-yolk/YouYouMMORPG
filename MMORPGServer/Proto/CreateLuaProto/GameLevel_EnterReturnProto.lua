--服务器返回进入关卡消息
GameLevel_EnterReturnProto = { ProtoCode = 12002, IsSuccess = false, MsgCode = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
GameLevel_EnterReturnProto.__index = GameLevel_EnterReturnProto;

function GameLevel_EnterReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, GameLevel_EnterReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function GameLevel_EnterReturnProto.SendProto(proto)

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
function GameLevel_EnterReturnProto.GetProto(buffer)

    local proto = GameLevel_EnterReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(not proto.IsSuccess) then
        proto.MsgCode = ms:ReadInt();
    end

    ms:Dispose();
    return proto;
end