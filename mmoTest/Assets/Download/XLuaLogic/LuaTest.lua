

function TestNumber(x)
	return x*10;
end

function TestString(str)
--	return string.len(str);--����ע��
	return string.lower(str);
	--[[����ע��--]]
end

print(TestString("ABC"))