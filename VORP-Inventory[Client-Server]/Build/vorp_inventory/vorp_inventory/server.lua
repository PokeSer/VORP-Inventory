RegisterServerEvent("player:getItems")
RegisterServerEvent("item:giveItem")
local invTable = {}
data = {}
local inventory = {}
data = inventory

AddEventHandler('vorpinventory:getInv', function(cb)
    cb(data)
end)

AddEventHandler("player:getItems", function()
    local _source = source
    local check = false
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        --print(identifier)
        print("doing stuff")
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                TriggerClientEvent("gui:getItems", _source, k.inventory)
				TriggerClientEvent("item:LoadPickups", _source, Pickups)
                TriggerClientEvent("player:loadWeapons", _source)
                check = true
                print("LOAD OLD")
                break

            end
        end
        if check == false then
            print("LOAD NEW")
            exports.ghmattimysql:execute('SELECT * FROM characters WHERE `identifier`=@identifier;', { ['identifier'] = tostring(identifier)}, function(inventory)
                if inventory[1] ~= nil then
                    local inv = json.decode(inventory[1].inventory)
                    table.insert(invTable, {id = identifier, inventory = inv})
                    TriggerClientEvent("gui:getItems", _source, inv)
			TriggerClientEvent("item:LoadPickups", _source, Pickups)
                    TriggerClientEvent("player:loadWeapons", _source)
                else
                    local test = {
                        ["water"] = 3,
                        ["bread"] = 3,
                    }
                    
                    table.insert(invTable, {id = identifier, inventory = test})
                    TriggerClientEvent("gui:getItems", _source, test)
			TriggerClientEvent("item:LoadPickups", _source, Pickups)
                end
            end)
        end
    end)
end)


RegisterServerEvent("weapon:saveAmmo")
AddEventHandler("weapon:saveAmmo", function(data)
local _data = data
local _source = source
 TriggerEvent("player:savInvSv", _source, _data)
end)

RegisterServerEvent("player:savInvSv")
AddEventHandler('player:savInvSv', function(source, data)
    local _source = source
	local _data = data
	local eq
 TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        --print(identifier)
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                for name,value in pairs(k.inventory) do
					eq = k.inventory
						if value == 0 then
							eq[name] = nil
						end
                end
				if _data ~= nil then
					eq = _data					
				end
                exports.ghmattimysql:execute('UPDATE characters SET inventory = @items WHERE identifier = @identifier', {
                    ['@identifier']  = identifier,
                    ['@items'] = json.encode(eq)
                }, function (rowsChanged)
                    if rowsChanged == 0 then
                        print(('characters: Something went wrong saving %s!'):format(identifier))
                    else
                        print("saved")
                    end
                end)

                break 
				
				end
        end

    end)
end)


AddEventHandler("item:add", function(source, arg, identifier)
    local _source = source
    for i,k in pairs(invTable) do
        if k.id == identifier  then
            local name = tostring(arg[1])
            local amount = arg[2]
            local hash = arg[3]
            print(name)
            print(hash)
            if hash == 1 then
                if k.inventory[name] ~= nil then
                    local val = invTable[i]["inventory"][name]
                    newVal = val + amount
                    print(val)
                    print(newVal)
                    invTable[i]["inventory"][name]= tonumber(newVal)
                else
                    invTable[i]["inventory"][name]= tonumber(amount)
                end
            else
                invTable[i]["inventory"][name]= {tonumber(amount)  , hash}
                TriggerClientEvent("player:giveWeapon", _source, tonumber(amount) , hash )
            end
            print("send shit")
            TriggerClientEvent("gui:getItems", _source, k.inventory)
            TriggerEvent("player:savInvSv", _source)
            break
        end
    end

end)

AddEventHandler("item:delete", function(source, arg, identifier)
    local _source = source
    for i,k in pairs(invTable) do
        if k.id == identifier  then
            local name = tostring(arg[1])
            local amount = tonumber(arg[2])
            if tonumber(invTable[i]["inventory"][name]) ~= nil then
                local val = invTable[i]["inventory"][name]
                newVal = val - amount
                invTable[i]["inventory"][name]= tonumber(newVal)
            else
                invTable[i]["inventory"][name]= nil
                TriggerClientEvent("player:removeWeapon", _source, tonumber(amount) , hash )
            end
            TriggerClientEvent("gui:getItems", _source, k.inventory)
            TriggerEvent("player:savInvSv", _source)
            TriggerClientEvent('gui:ReloadMenu', _source)
            break end
    end
end)


RegisterServerEvent("item:onpickup")
AddEventHandler("item:onpickup", function(id)
    local _source = source
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                local pickup  = Pickups[id]
                TriggerEvent("item:add", _source ,{pickup.name, pickup.amount ,pickup.hash}, identifier)
                TriggerClientEvent("item:Sharepickup", -1, pickup.name, pickup.obj , pickup.amount, x, y, z, 2)
                TriggerClientEvent('item:removePickup', -1, pickup.obj)
                Pickups[id] = nil
		TriggerClientEvent('player:anim', _source)
                TriggerClientEvent('gui:ReloadMenu', _source)
                break
            end
        end
    end)
end)



RegisterCommand('giveitem', function(source, args)
    local _source = source
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                local item = args[1]
                local amount = args[2]
                local test = 1
                TriggerEvent("item:add", _source, {item, amount, test}, identifier)
                print("add")
                TriggerClientEvent('gui:ReloadMenu', _source)
                break
            end
        end
    end)
end)

RegisterCommand('giveweapon', function(source, args)
    local _source = source
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                local name = args[1]
                local WeaponHash = tonumber(args[2])
                local Ammo = args[3]
                TriggerEvent("item:add", _source, {name ,Ammo, WeaponHash}, identifier)
                print("add")
                TriggerClientEvent('gui:ReloadMenu', _source)
                break
            end
        end
    end)
end)

RegisterServerEvent("item:use")
AddEventHandler("item:use", function(val)
    local _source = source
    local name = val
    local amount = 1
    print("poszlo")
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                TriggerEvent("RegisterUsableItem:"..name, _source)
                TriggerClientEvent("redemrp_notification:start", _source, "Item used: "..name, 3, "success")
                TriggerClientEvent('gui:ReloadMenu', _source)
                break
            end
        end
    end)
end)



RegisterServerEvent("item:drop")
AddEventHandler("item:drop", function(val, amount , hash)
    local _source = source
    local name
    local value
    local _hash = 1
    _hash = hash
    local name = val
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                value = invTable[i]["inventory"][name]
                print("tal".._hash)
                if _hash == 1 then
                    print(value)
                    print(amount)
                    local all = value-amount
                    print(all)
                    if all >= 0 then
                        TriggerClientEvent('item:pickup',_source, name, amount , 1)
                    end

                else
                    print("tal".._hash)
                    print("tal"..amount)
                    print("tal"..name)
                    TriggerClientEvent('item:pickup',_source, name, amount , _hash)
                end
                TriggerEvent("item:delete", _source, {name , amount}, identifier)
                TriggerClientEvent('gui:ReloadMenu', _source)
                break
            end
        end
    end)
end)

RegisterServerEvent("item:SharePickupServer")
AddEventHandler("item:SharePickupServer", function(name, obj , amount, x, y, z , hash)
    TriggerClientEvent("item:Sharepickup", -1, name, obj , amount, x, y, z, 1, hash)
    print("poszlo server")
    Pickups[obj] = {
        name = name,
        obj = obj,
        amount = amount,
        hash = hash,
        inRange = false,
        coords = {x = x, y = y, z = z}
    }
end)

RegisterServerEvent("test_lols")
AddEventHandler("test_lols", function(name, amount , target ,hash)
    local _target = target
    local _source = source
    local _name = name
    local _amount = amount
	local all
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                if hash == 1 then
                    local value = invTable[i]["inventory"][name]
                     all = value-amount
				else
					 all = amount
                end
                if all >= 0 then
                    TriggerEvent("item:delete",_source, { name , amount}, identifier)
                    TriggerEvent('test_lols222', _target , name , amount ,hash)
                    TriggerClientEvent('gui:ReloadMenu', _source)
                    TriggerClientEvent("redemrp_notification:start", _source, "You have given: [X"..tonumber(amount).."]"..name.. " to " ..GetPlayerName(_target), 3, "success")
                    TriggerClientEvent("redemrp_notification:start", _target, "You've received [X"..tonumber(amount).."]"..name.. " from " ..GetPlayerName(_source), 3, "success")
                end
                break
            end
        end
    end)
end)

RegisterServerEvent("test_lols222")
AddEventHandler("test_lols222", function(source, name, amount, hash)
    local _source = source
    local _name = name
    local _amount = amount
    local _hash = hash
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                TriggerEvent("item:add",_source, {name, amount, _hash}, identifier)
                if _hash ~= 1 then
                    TriggerClientEvent("player:giveWeapon", _source, tonumber(amount) , _hash)
                end
					TriggerClientEvent('gui:ReloadMenu', _source)
                break
            end
        end
    end)
end)
RegisterCommand('itemcheck', function(source, args)
    local value =  data.checkItem(source , args[1])
	print(value)
end)
function checkItem(_source, name)
 local value = 0
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
        for i,k in pairs(invTable) do
            if k.id == identifier  then
                value = invTable[i]["inventory"][name]
				
		   if tonumber(value) == nil then 
		       value = 0
			    
					end
                break
            end
        end
    end)
    return tonumber(value)
end

function inventory.checkItem(_source, name)
local  value = checkItem(_source, name)
  Wait(500)
    return tonumber(value)
end
function inventory.addItem(_source, name , amount ,hash)
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        
		if hash == nil or hash == 0 then
                        local test = 1
			TriggerEvent("item:add", _source ,{name, amount , test}, identifier)
		else
			TriggerEvent("item:add", _source ,{name, amount , hash}, identifier)
		end
    end)
end

function inventory.delItem(_source, name , amount)
    TriggerEvent('vorp:getCharacter', _source, function(user)
        local identifier = user.identifier
        

        TriggerEvent("item:delete", _source, {name , amount}, identifier)
    end)
end

RegisterServerEvent("vorpinventory:deleteInv")
AddEventHandler("vorpinventory:deleteInv", function(Callback) 
    local _source = source
    local id
    for k,v in ipairs(GetPlayerIdentifiers(_source))do
        if string.sub(v, 1, string.len("steam:")) == "steam:" then
            id = v
            break
        end
    end
	
  local Callback = callback

end)
--------EXAMPLE---------Register Usable item---------------EXAMPLE
RegisterServerEvent("RegisterUsableItem:wood")
AddEventHandler("RegisterUsableItem:wood", function(source)
    print("test")
end)
------------------------EXAMPLE----------------------------EXAMPLE

