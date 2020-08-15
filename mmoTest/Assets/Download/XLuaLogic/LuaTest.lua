

function TestNumber(x)
	return x*10;
end

function TestString(str)
--	return string.len(str);--单行注释
	return string.lower(str);
	--[[多行注释--]]
end

print(TestString("ABC"))