
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
        private static bool firstspawn = false;
        Dictionary<string,ItemClass> useritems = new Dictionary<string, ItemClass>();
        public vorp_inventoryClient()
        {
            EventHandlers["playerSpawned"] += new Action<object>(IsPlayerSpawned);
            EventHandlers["vorpInventory:giveItemsTable"] += new Action<dynamic>(processItems);
            EventHandlers["vorpInventory:giveInventory"] += new Action<dynamic,dynamic>(getInventory);
        }

        private async void IsPlayerSpawned(object spawninfo)
        {
            if (!firstspawn)
            {
                await Delay(1000);
                TriggerServerEvent("vorpinventory:getItemsTable");
                await Delay(2000);
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

        private void getInventory(dynamic inventory,dynamic loadout)
        {
            
            if (inventory != null)
            {
                dynamic deserializedInventory = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(inventory);
                foreach (KeyValuePair<string,Dictionary<string,dynamic>> fitems in citems)
                {
                    if (deserializedInventory[fitems.Key] != null)
                    {
                        int cuantity = int.Parse(deserializedInventory[fitems.Key].ToString());
                        int limit = int.Parse(fitems.Value["limit"].ToString());
                        string label = fitems.Value["label"].ToString();
                        bool can_remove = bool.Parse(fitems.Value["can_remove"].ToString());
                        ItemClass item = new ItemClass(cuantity,limit,label,fitems.Key,"item_inventory",true,can_remove);
                        useritems.Add(fitems.Key,item);
                    }
                }
            }
        }
    }
}