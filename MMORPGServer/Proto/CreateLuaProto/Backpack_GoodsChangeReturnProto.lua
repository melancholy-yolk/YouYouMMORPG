--服务器返回背包物品更新消息
Backpack_GoodsChangeReturnProto = { ProtoCode = 16003, BackpackItemChangeCount = 0, ItemTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Backpack_GoodsChangeReturnProto.__index = Backpack_GoodsChangeReturnProto;

function Backpack_GoodsChangeReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, Backpack_GoodsChangeReturnProto); --将self的元表设定为Class
    return self;
end


--定义更改项
Item = { BackpackId = 0, ChangeType = 0, GoodsType = 0, GoodsId = 0, GoodsCount = 0, GoodsServerId = 0 }
Item.__index = Item;
function Item.New()
    local self = { };
    setmetatable(self, Item);
    return self;
end


--发送协议
function Backpack_GoodsChangeReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.BackpackItemChangeCount);
    for i = 1, proto.BackpackItemChangeCount, 1 do
        ms:WriteInt(ItemList[i].BackpackId);
        ms:WriteByte(ItemList[i].ChangeType);
        ms:WriteByte(ItemList[i].GoodsType);
        ms:WriteInt(ItemList[i].GoodsId);
        ms:WriteInt(ItemList[i].GoodsCount);
        ms:WriteInt(ItemList[i].GoodsServerId);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Backpack_GoodsChangeReturnProto.GetProto(buffer)

    local proto = Backpack_GoodsChangeReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.BackpackItemChangeCount = ms:ReadInt();
	proto.ItemTable = {};
    for i = 1, proto.BackpackItemChangeCount, 1 do
        local _Item = Item.New();
        _Item.BackpackId = ms:ReadInt();
        _Item.ChangeType = ms:ReadByte();
        _Item.GoodsType = ms:ReadByte();
        _Item.GoodsId = ms:ReadInt();
        _Item.GoodsCount = ms:ReadInt();
        _Item.GoodsServerId = ms:ReadInt();
        proto.ItemTable[#proto.ItemTable+1] = _Item;
    end

    ms:Dispose();
    return proto;
end