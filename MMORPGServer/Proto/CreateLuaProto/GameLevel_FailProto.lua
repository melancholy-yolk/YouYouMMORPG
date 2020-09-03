--客户端发送战斗失败消息
GameLevel_FailProto = { ProtoCode = 12005, GameLevelId = 0, Grade = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
GameLevel_FailProto.__index = GameLevel_FailProto;

function GameLevel_FailProto.New()
    local self = { }; --初始化self
    setmetatable(self, GameLevel_FailProto); --将self的元表设定为Class
    return self;
end


--发送协议
function GameLevel_FailProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.GameLevelId);
    ms:WriteByte(proto.Grade);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function GameLevel_FailProto.GetProto(buffer)

    local proto = GameLevel_FailProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.GameLevelId = ms:ReadInt();
    proto.Grade = ms:ReadByte();

    ms:Dispose();
    return proto;
end