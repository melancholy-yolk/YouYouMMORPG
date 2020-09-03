--客户端发送战斗胜利消息
GameLevel_VictoryProto = { ProtoCode = 12003, GameLevelId = 0, Grade = 0, Star = 0, Exp = 0, Gold = 0, KillTotalMonsterCount = 0, KillMonsterTable = { }, GoodsTotalCount = 0, GetGoodsTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
GameLevel_VictoryProto.__index = GameLevel_VictoryProto;

function GameLevel_VictoryProto.New()
    local self = { }; --初始化self
    setmetatable(self, GameLevel_VictoryProto); --将self的元表设定为Class
    return self;
end


--定义杀怪列表
KillMonster = { MonsterId = 0, MonsterCount = 0 }
KillMonster.__index = KillMonster;
function KillMonster.New()
    local self = { };
    setmetatable(self, KillMonster);
    return self;
end


--定义获得物品
GetGoods = { GoodsType = 0, GoodsId = 0, GoodsCount = 0 }
GetGoods.__index = GetGoods;
function GetGoods.New()
    local self = { };
    setmetatable(self, GetGoods);
    return self;
end


--发送协议
function GameLevel_VictoryProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.GameLevelId);
    ms:WriteByte(proto.Grade);
    ms:WriteByte(proto.Star);
    ms:WriteInt(proto.Exp);
    ms:WriteInt(proto.Gold);
    ms:WriteInt(proto.KillTotalMonsterCount);
    for i = 1, proto.KillTotalMonsterCount, 1 do
        ms:WriteInt(KillMonsterList[i].MonsterId);
        ms:WriteInt(KillMonsterList[i].MonsterCount);
    end
    ms:WriteInt(proto.GoodsTotalCount);
    for i = 1, proto.GoodsTotalCount, 1 do
        ms:WriteByte(GetGoodsList[i].GoodsType);
        ms:WriteInt(GetGoodsList[i].GoodsId);
        ms:WriteInt(GetGoodsList[i].GoodsCount);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function GameLevel_VictoryProto.GetProto(buffer)

    local proto = GameLevel_VictoryProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.GameLevelId = ms:ReadInt();
    proto.Grade = ms:ReadByte();
    proto.Star = ms:ReadByte();
    proto.Exp = ms:ReadInt();
    proto.Gold = ms:ReadInt();
    proto.KillTotalMonsterCount = ms:ReadInt();
	proto.KillMonsterTable = {};
    for i = 1, proto.KillTotalMonsterCount, 1 do
        local _KillMonster = KillMonster.New();
        _KillMonster.MonsterId = ms:ReadInt();
        _KillMonster.MonsterCount = ms:ReadInt();
        proto.KillMonsterTable[#proto.KillMonsterTable+1] = _KillMonster;
    end
    proto.GoodsTotalCount = ms:ReadInt();
	proto.GetGoodsTable = {};
    for i = 1, proto.GoodsTotalCount, 1 do
        local _GetGoods = GetGoods.New();
        _GetGoods.GoodsType = ms:ReadByte();
        _GetGoods.GoodsId = ms:ReadInt();
        _GetGoods.GoodsCount = ms:ReadInt();
        proto.GetGoodsTable[#proto.GetGoodsTable+1] = _GetGoods;
    end

    ms:Dispose();
    return proto;
end