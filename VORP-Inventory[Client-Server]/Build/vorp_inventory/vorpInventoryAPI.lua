
function inventoryApi()
    local self = {}
    self.subWeapon = function(source,weaponid)
        TriggerEvent("vorpCore:subWeapon",source,tonumber(weaponid))
    end

    self.addWeapon = function(source,weaponName)
        TriggerEvent("vorpCore:registerWeapon",source,tostring(weaponName))
    end

    self.addItem = function(source,itemName,cuantity)
        TriggerEvent("vorpCore:addItem",source,tostring(itemName),tonumber(cuantity))
    end

    self.subItem = function(source,itemName,cuantity)
        TriggerEvent("vorpCore:subItem",source,tostring(itemName),tonumber(cuantity))
    end

    self.getItemCuantity = function(source,item)
        local count = 0
        TriggerEvent("vorpCore:getItemCount",source,function(itemcount)
            count = itemcount
        end,tostring(item))
        return count
    end
    return self
end