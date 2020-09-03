--服务器返回查询背包项消息
Backpack_SearchReturnProto = { ProtoCode = 16005, BackpackItemCount = 0, ItemTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Backpack_SearchReturnProto.__index = Backpack_SearchReturnProto;

function Backpack_SearchReturnProto.New()
    local self = { }; --初始化self
    setmetatable(self, Backpack_SearchReturnProto); --将self的元表设定为Class
    return self;
end


--定义背包项
Item = { BackpackItemId = 0, GoodsType = 0, GoodsId = 0, GoodsServerId = 0, GoodsOverlayCount = 0 }
Item.__index = Item;
function Item.New()
    local self = { };
    setmetatable(self, Item);
    return self;
end


--发送协议
function Backpack_SearchReturnProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.BackpackItemCount);
    for i = 1, proto.BackpackItemCount, 1 do
        ms:WriteInt(ItemList[i].BackpackItemId);
        ms:WriteByte(ItemList[i].GoodsType);
        ms:WriteInt(ItemList[i].GoodsId);
        ms:WriteInt(ItemList[i].GoodsServerId);
        ms:WriteInt(ItemList[i].GoodsOverlayCount);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Backpack_SearchReturnProto.GetProto(buffer)

    local proto = Backpack_SearchReturnProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.BackpackItemCount = ms:ReadInt();
	proto.ItemTable = {};
    for i = 1, proto.BackpackItemCount, 1 do
        local _Item = Item.New();
        _Item.BackpackItemId = ms:ReadInt();
        _Item.GoodsType = ms:ReadByte();
        _Item.GoodsId = ms:ReadInt();
        _Item.GoodsServerId = ms:ReadInt();
        _Item.GoodsOverlayCount = ms:ReadInt();
        proto.ItemTable[#proto.ItemTable+1] = _Item;
    end

    ms:Dispose();
    return proto;
end