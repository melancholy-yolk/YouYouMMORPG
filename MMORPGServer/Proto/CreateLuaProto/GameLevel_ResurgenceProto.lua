--客户端发送复活消息
GameLevel_ResurgenceProto = { ProtoCode = 12007, GameLevelId = 0, Grade = 0, Type = 0 }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
GameLevel_ResurgenceProto.__index = GameLevel_ResurgenceProto;

function GameLevel_ResurgenceProto.New()
    local self = { }; --初始化self
    setmetatable(self, GameLevel_ResurgenceProto); --将self的元表设定为Class
    return self;
end


--发送协议
function GameLevel_ResurgenceProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.GameLevelId);
    ms:WriteByte(proto.Grade);
    ms:WriteByte(proto.Type);

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function GameLevel_ResurgenceProto.GetProto(buffer)

    local proto = GameLevel_ResurgenceProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.GameLevelId = ms:ReadInt();
    proto.Grade = ms:ReadByte();
    proto.Type = ms:ReadByte();

    ms:Dispose();
    return proto;
end