
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using vorpinventory_sv;

namespace vorpinventory_cl
{
    public class vorp_inventoryClient:BaseScript
    {    
        public static Dictionary<string,Dictionary<string,dynamic>> citems = new Dictionary<string, Dictionary<string, dynamic>>();
        public static Dictionary<string,ItemClass> useritems = new Dictionary<string, ItemClass>();
        public static List<WeaponClass> userWeapons = new List<WeaponClass>();
        public vorp_inventoryClient()
        {
            EventHandlers["vorpInventory:giveItemsTable"] += new Action<dynamic>(processItems);
            EventHandlers["vorpInventory:giveInventory"] += new Action<string,string>(getInventory);
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
                    ["can_remove"] = item.can_remove,
                    ["type"] = item.type,
                    ["usable"] = item.usable
                });
            }
        }

        private void getInventory(string inventory,string loadout)
        {
            if (inventory != null)
            {
                useritems.Clear();
                dynamic items = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(inventory);
                Debug.WriteLine(items.ToString());
                foreach (KeyValuePair<string, Dictionary<string, dynamic>> fitems in citems)
                {
                    if (items[fitems.Key] != null)
                    {
                        Debug.WriteLine(fitems.Key);
                        int cuantity = int.Parse(items[fitems.Key].ToString());
                        int limit = int.Parse(fitems.Value["limit"].ToString());
                        string label = fitems.Value["label"].ToString();
                        bool can_remove = bool.Parse(fitems.Value["can_remove"].ToString());
                        string type = fitems.Value["type"].ToString();
                        bool usable = bool.Parse(fitems.Value["usable"].ToString());
                        ItemClass item = new ItemClass(cuantity, limit, label, fitems.Key, type, usable, can_remove);
                        useritems.Add(fitems.Key, item);
                    }
                }
            }

            if (loadout != null)
            {
                userWeapons.Clear();
                string weaponName = "";
                JArray weapons = JArray.Parse(loadout);
                foreach (JObject x in weapons.Children())
                {
                    Dictionary<string,int> ammos = new Dictionary<string, int>();
                    List<string> components = new List<string>();
                    foreach (JProperty aux in x.Properties())
                    {
                        switch (aux.Name)
                        {
                            case "name":
                                weaponName = aux.Value.ToString();
                                Debug.WriteLine(aux.Value.ToString());
                                break;
                            case "ammo":
                                foreach (JObject ammo in aux.Children<JObject>())
                                {
                                    foreach (JProperty ammo2 in ammo.Properties())
                                    {
                                        ammos.Add(ammo2.Name,int.Parse(ammo2.Value.ToString()));
                                    }
                                }
                                break;
                            case "components":
                                foreach (var component in aux.Value)
                                {
                                    components.Add(component.ToString());
                                }
                                break;
                        }
                    }
                    WeaponClass weapon = new WeaponClass(weaponName,ammos,components);
                    Debug.WriteLine(weapon.getName());
                    userWeapons.Add(weapon);
                }
            }
        }
    }
}