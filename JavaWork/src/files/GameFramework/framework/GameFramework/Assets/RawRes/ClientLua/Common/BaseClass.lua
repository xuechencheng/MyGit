ClassType = {
	class = 1,
	instance = 2
}

local _class = {}

function FindClass( tbl )
	-- body
	return _class[tbl]
end

local  function NewClassType( classname, ... )
	for i = 1, select('#', ...) do
		assert(select( i, ...), " " .. classname .. "has null parent")
	end
	local class_type = {}
	class_type.__init = false
	class_type.__delete = false
	class_type.__cname = classname
	class_type.__ctype = ClassType.class

	class_type.super = {...}
	local vtbl = {}
	_class[class_type] = vtbl

	class_type.new = function( ... )
		local obj = {}
		obj._class_type = class_type
		obj.__ctype = ClassType.instance
		setmetatable(obj,{
			__index = vtbl
		})
		return obj
	end

	setmetatable(class_type,{
		__newindex = function ( t, k , v)
			vtbl[k] = v
		end
		__index = vtbl
	})

	setmetatable(vtbl,{
		__index = function ( t, k)
			for _, su in pairs(class_type.super) do
				local ret = _class[su] and _class[su][k] or nil
				if ret then
					return ret
				end
			end
		end
	})
	return class_type
end 

function BaseClass( classname, ... )
	-- body
	local class_type = NewClassType( classname, ...)
	local vtbl = _class[class_type]
	local new_func = class_type.new
	class_type.new = function ( ... )
		-- body
		local obj = new_func(...)
		obj.Clear = function( self )
			CallFuncUp(self,"__clear")
		end
		obj.Destory = function( self )
			-- body
			CallFuncUp(self, "__delete")
			CallFuncUp(self,"__destory")
		end
		CallFuncDown(obj,"__init",...)
		return obj
	end
	return class_type
end

function CallFuncUp( tbl, name, ...)
	local func = nil
	func = function( tbl, clz, ... )
		if clz[name] then
			clz[name](tbl, ...)
		end
		for _, s in pairs(clz.super) do
			func(tbl, s, ...)
		end
	end
	func(tbl, tbl.__class_type, ...)
end

function CallFuncDown( tbl, name, ...)
	local func = nil
	func = function( tbl, clz, ... )
		
		for _, s in pairs(clz.super) do
			func(tbl, s, ...)
		end

		if clz[name] then
			clz[name](tbl, ...)
		end
	end
	func(tbl, tbl.__class_type, ...)
end

function Singleton( classname, ...)
	local class_type = NewClassType(classname, ...)
	local vtbl = _class[class_type]
	local new_func = class_type.new
	class_type.new = function ( ... )
		local instance = rawget(class_type, "__instance")
		if instance ~= nil then
			error(class_type.__cname .. "'s Instance is already exist !!!")
			return
		end
		local obj = new_func(...)
		CallFuncDown( obj, "__init", ...)
		return obj
	end

	class_type.GetInstance = function()
		local instance = rawget(class_type, "__instance")
		if instance == null then
			instance = class_type.new()
			rawset(class_type, "__instance", instance)
		end
		return instance
	end
	class_type.Destory = function()
		local instance = rawget(class_type, "__instance")
		if instance then
			CallFuncUp(instance,"__delete")
			CallFuncUp(instance,"__destory")
		end
		rawset(class_type,"__instance",nil)
	end
	local mt = getmetatable(vtbl)
	local mt_index_func = mt.__index
	mt.__index = function ( t, k )
		if k == "Instance" then
			return class_type.GetInstance()
		end
		return mt_index_func( t, k)
	end
	return class_type
end

function getcname( tbl )
	-- body
	if tbl then
		if tbl.__ctype == ClassType.instance then
			return tbl._class_type.__cname
		else
			return tbl.__cname
		end
	end
	return ""
end