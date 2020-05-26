
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
    public class vorp_inventoryClient : BaseScript
    {
        public static Dictionary<string, Dictionary<string, dynamic>> citems =
            new Dictionary<string, Dictionary<string, dynamic>>();

        public static Dictionary<string, ItemClass> useritems = new Dictionary<string, ItemClass>();
        public static Dictionary<int, WeaponClass> userWeapons = new Dictionary<int, WeaponClass>();
        public static Dictionary<int, string> bulletsHash = new Dictionary<int, string>();

        public vorp_inventoryClient()
        {
            API.RegisterCommand("saveWeapon",new Action(() =>
            {
                uint weaponHash = 0;
                API.GetCurrentPedWeapon(API.PlayerPedId(), ref weaponHash, true, 0, true);
                Debug.WriteLine(Function.Call<string>((Hash)0x89CF5FF3D363311E,weaponHash));
                foreach (KeyValuePair<int,WeaponClass> weapon in userWeapons)
                {
                    if (weapon.Value.getName() == Function.Call<string>((Hash) 0x89CF5FF3D363311E, weaponHash) &&
                        weapon.Value.getUsed())
                    {
                        weapon.Value.setUsed(false);
                        Debug.WriteLine($"id de arma quitada {weapon.Key} nombre {weapon.Value.getName()}");
                        API.RemoveWeaponFromPed(API.PlayerPedId(),weaponHash,true,0);
                    }
                }
                Debug.WriteLine(Function.Call<bool>((Hash)0x8DECB02F88F428BC,API.PlayerPedId(),API.GetHashKey("WEAPON_RIFLE_VARMINT"),0,true).ToString());
            }),false);
            
            API.RegisterCommand("hasweapon",new Action(() =>
            {
                Debug.WriteLine(Function.Call<bool>((Hash)0x8DECB02F88F428BC,API.PlayerPedId(),API.GetHashKey("WEAPON_RIFLE_VARMINT"),0,true).ToString());
            }),false );
            EventHandlers["vorpInventory:giveItemsTable"] += new Action<dynamic>(processItems);
            EventHandlers["vorpInventory:giveInventory"] += new Action<string>(getInventory);
            EventHandlers["vorpInventory:giveLoadout"] += new Action<dynamic>(getLoadout);
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["vorpinventory:receiveItem"] += new Action<string, int>(receiveItem);
            EventHandlers["vorpinventory:receiveWeapon"] +=
                new Action<int, string, string, ExpandoObject, List<dynamic>>(receiveWeapon);
            //Tick += updateAmmoInWeapon;
        }

        // private async Task updateAmmoInWeapon()
        // {
        //     uint weaponHash = 0;
        //     if (API.GetCurrentPedWeapon(API.PlayerPedId(), ref weaponHash, true, 0, true))
        //     {
        //        int ammo = Function.Call<int>((Hash) 0x015A522136D7F951, API.PlayerPedId(), weaponHash);
        //        int type = API.GetPedAmmoTypeFromWeapon(API.PlayerPedId(), weaponHash);
        //        foreach (KeyValuePair<int,WeaponClass> weap in userWeapons)
        //        {
        //            if (API.GetHashKey(weap.Value.getName()) == weaponHash && weap.Value.getUsed())
        //            {
        //                if (weap.Value.getAmmo(Utils.Publicammo[(uint) type]) != ammo)
        //                {
        //                    weap.Value.addAmmo(ammo,Utils.Publicammo[(uint)type]);
        //                }
        //            }
        //        }
        //     }
        // }//Update weapon ammo

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

        private void receiveWeapon(int id,string propietary,string name ,ExpandoObject ammo ,List<dynamic> components)
        {
            Dictionary<string,int> ammoaux = new Dictionary<string, int>();
            foreach (KeyValuePair<string,object> amo in ammo)
            {
                ammoaux.Add(amo.Key,int.Parse(amo.Value.ToString()));
            }
            List<string> auxcomponents = new List<string>();
            foreach (var comp in components)
            {
                auxcomponents.Add(comp.ToString());
            }
            WeaponClass weapon = new WeaponClass(id,propietary,name,ammoaux,auxcomponents,false);
            if (!userWeapons.ContainsKey(weapon.getId()))
            {
                userWeapons.Add(weapon.getId(),weapon);
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

        private void getLoadout(dynamic loadout)
        {
            foreach (var row in loadout)
            {
                JArray componentes = Newtonsoft.Json.JsonConvert.DeserializeObject(row.components.ToString());
                JObject amunitions = Newtonsoft.Json.JsonConvert.DeserializeObject(row.ammo.ToString());
                List<string> components = new List<string>();
                Dictionary<string,int> ammos = new Dictionary<string, int>();
                foreach (JToken componente in componentes)
                {
                    components.Add(componente.ToString());
                }

                foreach (JProperty amunition in amunitions.Properties())
                {
                    ammos.Add(amunition.Name,int.Parse(amunition.Value.ToString()));
                }
                WeaponClass auxweapon = new WeaponClass(int.Parse(row.id.ToString()),row.identifier.ToString(),row.name.ToString(),ammos,components,false);
                userWeapons.Add(auxweapon.getId(),auxweapon);
            }
        }

        private void getInventory(string inventory)
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
        }
    }
}