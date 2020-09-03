--服务器返回复活消息
GameLevel_ResurgenceReturnProto = { ProtoCode = 12008, IsSuccess = false, MsgCode = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
GameLevel_ResurgenceReturnProto.__index = GameLevel_ResurgenceReturnProto;

function GameLevel_ResurgenceReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, GameLevel_ResurgenceReturnProto); --将self的元表设定为Class
    return self;
end


--发送协议
function GameLevel_ResurgenceReturnProto.SendProto(proto)

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
function GameLevel_ResurgenceReturnProto.GetProto(buffer)

    local proto = GameLevel_ResurgenceReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.IsSuccess = ms:ReadBool();
    if(not proto.IsSuccess) then
        proto.MsgCode = ms:ReadInt();
    end

    ms:Dispose();
    return proto;
end