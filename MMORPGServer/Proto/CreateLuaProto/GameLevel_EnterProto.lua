--客户端发送进入游戏关卡消息
GameLevel_EnterProto = { ProtoCode = 12001, GameLevelId = 0, Grade = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
GameLevel_EnterProto.__index = GameLevel_EnterProto;

function GameLevel_EnterProto.New()
    local self = { }; --初始化self
    setmetatable(self, GameLevel_EnterProto); --将self的元表设定为Class
    return self;
end


--发送协议
function GameLevel_EnterProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.GameLevelId);
    ms:WriteByte(proto.Grade);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function GameLevel_EnterProto.GetProto(buffer)

    local proto = GameLevel_EnterProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.GameLevelId = ms:ReadInt();
    proto.Grade = ms:ReadByte();

    ms:Dispose();
    return proto;
end