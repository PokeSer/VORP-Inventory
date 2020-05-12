using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;

namespace vorpinventory_sv
{
    public class vorpinventory_sv : BaseScript
    {
        public vorpinventory_sv()
        {
            EventHandlers["vorpinventory:getItemsTable"] += new Action<Player>(getItemsTable);
            EventHandlers["vorpinventory:getInventory"] += new Action<Player>(getInventory);
            EventHandlers["vorpinventory:serverGiveItem"] += new Action<Player,string,int,int>(serverGiveItem);
            EventHandlers["vorpinventory:serverDropItem"] += new Action<Player,string,int>(serverDropItem);
            EventHandlers["vorpInventory:sharePickupServer"] += new Action<string,int,int,Vector3,int>(sharePickupServer);
            EventHandlers["vorpInventory:onPickup"] += new Action<Player,int>(onPickup);
            EventHandlers["vorpInventory:addItem"] += new Action<int,string,int>(addItem);
            EventHandlers["vorpInventory:quitItem"] += new Action<int,string,int>(subItem);
            Tick += saveInventoryItems;
        }
        public static Dictionary<int,Dictionary<string,dynamic>> Pickups = new Dictionary<int, Dictionary<string, dynamic>>();

        [Tick]
        private async Task saveInventoryItems()
        {
            await Delay(10000);
            foreach (var uinventory in ItemDatabase.usersInventory)
            {
                await Delay(30);
                Dictionary<string,int> items = new Dictionary<string, int>();
                 foreach (var item in uinventory.Value)
                {
                    items.Add(item.Key,item.Value.getCount());
                }
                 
                if (items.Count > 0) 
                {
                     string json = Newtonsoft.Json.JsonConvert.SerializeObject(items);
                     Exports["ghmattimysql"].execute($"UPDATE characters SET inventory = '{json}' WHERE identifier=?", new[] {uinventory.Key});
                }
            }
            foreach (var uweapons in ItemDatabase.usersWeapons)
            {
                await Delay(30);
                List<Dictionary<string,dynamic>> userweapons = new List<Dictionary<string, dynamic>>();
                foreach (WeaponClass weapon in uweapons.Value)
                {
                    Dictionary<string,dynamic> userweapon = new Dictionary<string, dynamic>
                    {
                         ["name"] = weapon.getName(), 
                         ["ammo"] = weapon.getAllAmmo(),
                         ["components"] = weapon.getAllComponents()
                    };
                    userweapons.Add(userweapon);
                }
        
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(userweapons);
                if (userweapons.Count > 0)
                {
                    Exports["ghmattimysql"].execute($"UPDATE characters SET loadout = '{json}' WHERE identifier=?", new[] {uweapons.Key});
                }
            }
        }


        //Sub items for other scripts
        private void subItem(int player , string name, int cuantity)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[player];
            string identifier = "steam:" + p.Identifiers["steam"];
            if (ItemDatabase.usersInventory[identifier].ContainsKey(name))
            {
                if (cuantity <= ItemDatabase.usersInventory[identifier][name].getCount())
                {
                    ItemDatabase.usersInventory[identifier][name].quitCount(cuantity);
                }

                if (ItemDatabase.usersInventory[identifier][name].getCount() == 0)
                {
                    ItemDatabase.usersInventory[identifier].Remove(name);
                }
            }
        }

        //For other scripts add items
        private void addItem(int player, string name, int cuantity)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[player];
            string identifier = "steam:" + p.Identifiers["steam"];
            if (ItemDatabase.usersInventory[identifier].ContainsKey(name))
            {
                if (cuantity > 0)
                {
                    ItemDatabase.usersInventory[identifier][name].addCount(cuantity);
                }
            }
            else
            {
                if (!ItemDatabase.svItems.ContainsKey(name))
                {
                    ItemDatabase.usersInventory[identifier].Add(name,new ItemClass(cuantity,ItemDatabase.svItems[name].getLimit(),
                        ItemDatabase.svItems[name].getLabel(),name,"item_inventory",true,ItemDatabase.svItems[name].getCanRemove()));
                }
            }
        }
        
        private void onPickup([FromSource]Player player,int obj)
        {
            string identifier = "steam:" + player.Identifiers["steam"];
            int source = int.Parse(player.Handle);
            if (Pickups.ContainsKey(obj))
            {
                if (ItemDatabase.usersInventory.ContainsKey(identifier))
                {
                    addItem(source,Pickups[obj]["name"],Pickups[obj]["amount"]);
                    Debug.WriteLine($"añado {Pickups[obj]["amount"]}");
                    TriggerClientEvent("vorpInventory:sharePickupClient",Pickups[obj]["name"],Pickups[obj]["obj"],
                        Pickups[obj]["amount"],Pickups[obj]["coords"],2,Pickups[obj]["hash"]);
                    TriggerClientEvent("vorpInventory:removePickupClient",Pickups[obj]["obj"]);
                    player.TriggerEvent("vorpinventory:receiveItem",Pickups[obj]["name"],Pickups[obj]["amount"]);
                    Pickups.Remove(obj);
                    player.TriggerEvent("vorpInventory:playerAnim",obj);
                    
                }
            }
            
        }
        
        private void sharePickupServer(string name, int obj, int amount, Vector3 position, int hash)
        {
            TriggerClientEvent("vorpInventory:sharePickupClient",name,obj,amount,position,1,hash);
            Pickups.Add(obj,new Dictionary<string, dynamic>
            {
                ["name"] = name,
                ["obj"] = obj,
                ["amount"] = amount,
                ["hash"] = hash,
                ["inRange"] = false,
                ["coords"] = position
            });
        }
        private void serverDropItem([FromSource] Player source, string itemname, int cuantity)
        {
            subItem(int.Parse(source.Handle),itemname,cuantity);
            source.TriggerEvent("vorpInventory:createPickup",itemname,cuantity,1);
        }

        private void serverGiveItem([FromSource] Player source, string itemname, int amount, int target)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[target];
            string identifier = "steam:" + source.Identifiers["steam"];

            if (ItemDatabase.usersInventory[identifier][itemname].getCount() >= amount)
            {
                addItem(int.Parse(p.Handle),itemname,amount);
                subItem(int.Parse(source.Handle),itemname,amount);
                p.TriggerEvent("vorpinventory:receiveItem",itemname,amount);
            }
        }

        private void getItemsTable([FromSource] Player source)
        {
            if (ItemDatabase.items.Count != 0)
            {
                source.TriggerEvent("vorpInventory:giveItemsTable",ItemDatabase.items);
            }
        }

        private void getInventory([FromSource]Player source)
        {
            string steamId = "steam:"+source.Identifiers["steam"];
            Debug.WriteLine(steamId);
            Exports["ghmattimysql"].execute("SELECT inventory,loadout FROM characters WHERE identifier = ?;",new [] {steamId} ,new Action<dynamic>((result) =>
            {
                if (result.Count == 0)
                {
                    Debug.WriteLine($"{steamId} doesn`t have inventory yet.");
                }
                else
                {
                    Debug.WriteLine(result[0].inventory);
                    source.TriggerEvent("vorpInventory:giveInventory",result[0].inventory,result[0].loadout);
                }
            }));
        }
    }
}
