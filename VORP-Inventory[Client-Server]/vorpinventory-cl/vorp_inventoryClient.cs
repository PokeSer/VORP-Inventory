
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
        public static Dictionary<string,ItemClass> useritems = new Dictionary<string, ItemClass>();
        public vorp_inventoryClient()
        {
            EventHandlers["vorpInventory:giveItemsTable"] += new Action<dynamic>(processItems);
            EventHandlers["vorpInventory:giveInventory"] += new Action<dynamic,dynamic>(getInventory);
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["vorpinventory:receiveItem"] += new Action<string,int>(receiveItem);
        }

        private void receiveItem(string name, int count)
        {
            if (useritems.ContainsKey(name))
            {
                useritems[name].addCount(count);
            }
            else
            {
                useritems.Add(name,new ItemClass(count,citems[name]["limit"],citems[name]["label"],name,
                    "item_standard",true,citems[name]["can_remove"]));
            }
            
            NUIEvents.LoadInv();
        }

        private async void OnClientResourceStart(string eventName)
        {
            if (API.GetCurrentResourceName() != eventName) return;
            API.SetNuiFocus(false, false);
            API.SendNuiMessage("{\"action\": \"hide\"}");
            Debug.WriteLine("Cargando el inventario");
            TriggerServerEvent("vorpinventory:getItemsTable");
            await Delay(300);
            TriggerServerEvent("vorpinventory:getInventory");
        }
        private void processItems(dynamic items)
        {
            citems.Clear();
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
                useritems.Clear();
                dynamic deserializedInventory = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(inventory);
                foreach (KeyValuePair<string,Dictionary<string,dynamic>> fitems in citems)
                {
                    if (deserializedInventory[fitems.Key] != null)
                    {
                        int cuantity = int.Parse(deserializedInventory[fitems.Key].ToString());
                        int limit = int.Parse(fitems.Value["limit"].ToString());
                        string label = fitems.Value["label"].ToString();
                        bool can_remove = bool.Parse(fitems.Value["can_remove"].ToString());
                        ItemClass item = new ItemClass(cuantity,limit,label,fitems.Key,"item_standard",true,can_remove);
                        useritems.Add(fitems.Key,item);
                    }
                }
            }
        }
    }
}