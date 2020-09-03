--服务器返回配置列表
System_GameServerConfigReturnProto = { ProtoCode = 14003, ConfigCount = 0, ServerConfigTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
System_GameServerConfigReturnProto.__index = System_GameServerConfigReturnProto;

function System_GameServerConfigReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, System_GameServerConfigReturnProto); --将self的元表设定为Class
    return self;
end


--定义配置项
ServerConfig = { ConfigCode = "", IsOpen = false, Param = "" }
ServerConfig.__index = ServerConfig;
function ServerConfig.New()
    local self = { };
    setmetatable(self, ServerConfig);
    return self;
end


--发送协议
function System_GameServerConfigReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.ConfigCount);
    for i = 1, proto.ConfigCount, 1 do
        ms:WriteUTF8String(ServerConfigList[i].ConfigCode);
        ms:WriteBool(ServerConfigList[i].IsOpen);
        ms:WriteUTF8String(ServerConfigList[i].Param);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function System_GameServerConfigReturnProto.GetProto(buffer)

    local proto = System_GameServerConfigReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.ConfigCount = ms:ReadInt();
	proto.ServerConfigTable = {};
    for i = 1, proto.ConfigCount, 1 do
        local _ServerConfig = ServerConfig.New();
        _ServerConfig.ConfigCode = ms:ReadUTF8String();
        _ServerConfig.IsOpen = ms:ReadBool();
        _ServerConfig.Param = ms:ReadUTF8String();
        proto.ServerConfigTable[#proto.ServerConfigTable+1] = _ServerConfig;
    end

    ms:Dispose();
    return proto;
end