--服务器广播其他角色死亡消息
WorldMap_OtherRoleDieProto = { ProtoCode = 13012, AttackRoleId = 0, DieCount = 0, RoleIdTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
WorldMap_OtherRoleDieProto.__index = WorldMap_OtherRoleDieProto;

function WorldMap_OtherRoleDieProto.New()
    local self = { }; --初始化self
    setmetatable(self, WorldMap_OtherRoleDieProto); --将self的元表设定为Class
    return self;
end


--发送协议
function WorldMap_OtherRoleDieProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.AttackRoleId);
    ms:WriteInt(proto.DieCount);
    for i = 1, proto.DieCount, 1 do
        ms:WriteInt(RoleIdList[i]);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function WorldMap_OtherRoleDieProto.GetProto(buffer)

    local proto = WorldMap_OtherRoleDieProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.AttackRoleId = ms:ReadInt();
    proto.DieCount = ms:ReadInt();
	proto.RoleIdTable = {};
    for i = 1, proto.DieCount, 1 do
        local _RoleId = ms:ReadInt();  --角色编号
        proto.RoleIdTable[#proto.RoleIdTable+1] = _RoleId;
    end

    ms:Dispose();
    return proto;
end