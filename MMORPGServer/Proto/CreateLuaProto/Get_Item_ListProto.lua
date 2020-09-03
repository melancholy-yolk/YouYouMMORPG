--获取物品列表
Get_Item_ListProto = { ProtoCode = 17003, ItemCount = 0, ItemTable = { } }

--这句是重定义元表的索引，就是说有了这句，这个才是一个类
Get_Item_ListProto.__index = Get_Item_ListProto;

function Get_Item_ListProto.New()
    local self = { }; --初始化self
    setmetatable(self, Get_Item_ListProto); --将self的元表设定为Class
    return self;
end


--定义一个物品对象(自定义类型)
Item = { ItemName = "", ItemPrice = 0 }
Item.__index = Item;
function Item.New()
    local self = { };
    setmetatable(self, Item);
    return self;
end


--发送协议
function Get_Item_ListProto.SendProto(proto)

    local ms = CS.LuaHelper.Instance:CreateMemoryStream();
    ms:WriteUShort(proto.ProtoCode);

    ms:WriteInt(proto.ItemCount);
    for i = 1, proto.ItemCount, 1 do
        ms:WriteUTF8String(ItemList[i].ItemName);
        ms:WriteFloat(ItemList[i].ItemPrice);
    end

    CS.LuaHelper.Instance:SendProto(ms:ToArray());
    ms:Dispose();
end


--解析协议
function Get_Item_ListProto.GetProto(buffer)

    local proto = Get_Item_ListProto.New(); --实例化一个协议对象
    local ms = CS.LuaHelper.Instance:CreateMemoryStream(buffer);

    proto.ItemCount = ms:ReadInt();
	proto.ItemTable = {};
    for i = 1, proto.ItemCount, 1 do
        local _Item = Item.New();
        _Item.ItemName = ms:ReadUTF8String();
        _Item.ItemPrice = ms:ReadFloat();
        proto.ItemTable[#proto.ItemTable+1] = _Item;
    end

    ms:Dispose();
    return proto;
end