--服务器返回角色学会的技能
RoleData_SkillReturnProto = { ProtoCode = 11001, SkillCount = 0, CurrSkillDataTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
RoleData_SkillReturnProto.__index = RoleData_SkillReturnProto;

function RoleData_SkillReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, RoleData_SkillReturnProto); --将self的元表设定为Class
    return self;
end


--定义当前学会的技能
CurrSkillData = { SkillId = 0, SkillLevel = 0, SlotsNo = 0 }
CurrSkillData.__index = CurrSkillData;
function CurrSkillData.New()
    local self = { };
    setmetatable(self, CurrSkillData);
    return self;
end


--发送协议
function RoleData_SkillReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteByte(proto.SkillCount);
    for i = 1, proto.SkillCount, 1 do
        ms:WriteInt(CurrSkillDataList[i].SkillId);
        ms:WriteInt(CurrSkillDataList[i].SkillLevel);
        ms:WriteByte(CurrSkillDataList[i].SlotsNo);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function RoleData_SkillReturnProto.GetProto(buffer)

    local proto = RoleData_SkillReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.SkillCount = ms:ReadByte();
	proto.CurrSkillDataTable = {};
    for i = 1, proto.SkillCount, 1 do
        local _CurrSkillData = CurrSkillData.New();
        _CurrSkillData.SkillId = ms:ReadInt();
        _CurrSkillData.SkillLevel = ms:ReadInt();
        _CurrSkillData.SlotsNo = ms:ReadByte();
        proto.CurrSkillDataTable[#proto.CurrSkillDataTable+1] = _CurrSkillData;
    end

    ms:Dispose();
    return proto;
end