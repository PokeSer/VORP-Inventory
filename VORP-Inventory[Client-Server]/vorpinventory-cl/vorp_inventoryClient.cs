
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinventory_cl
{
    public class vorp_inventoryClient:BaseScript
    {    
        public static Dictionary<string,Dictionary<string,dynamic>> citems = new Dictionary<string, Dictionary<string, dynamic>>();
        List<ItemClass> items = new List<ItemClass>();
        private static bool firstspawn = false;
        public vorp_inventoryClient()
        {
            EventHandlers["playerSpawned"] += new Action<object>(IsPlayerSpawned);
            EventHandlers["vorpInventory:giveItemsTable"] += new Action<dynamic>(processItems);
            EventHandlers["vorpInventory:giveInventory"] += new Action<dynamic>(getInventory);
        }

        private async void IsPlayerSpawned(object spawninfo)
        {
            if (!firstspawn)
            {
                await Delay(1000);
                TriggerServerEvent("vorpinventory:getItemsTable");
                await Delay(3000);
                TriggerServerEvent("vorpinventory:getInventory");
                firstspawn = true;
            }
        }
        private void processItems(dynamic items)
        {
            foreach (dynamic item in items)
            {
                citems.Add(item.item,new Dictionary<string, dynamic>
                {
                    ["item"] = item.item,
                    ["label"] = item.label,
                    ["limit"] = item.limit,
                    ["can_remove"] = item.can_remove
                });
            }
        }

        private void getInventory(dynamic inventory)
        {
            
        }
    }
    
    
}