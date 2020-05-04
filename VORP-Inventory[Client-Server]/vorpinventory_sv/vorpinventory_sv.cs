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
            
        }
        public static Dictionary<int,Dictionary<string,dynamic>> Pickups = new Dictionary<int, Dictionary<string, dynamic>>();



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
                // if(ItemDatabase.items[name)
                // ItemDatabase.usersInventory[identifier].Add(name,new ItemClass(cuantity,ItemDatabase.items[name]["limit"]
                //     ,ItemDatabase.items[name]["label"],name,ItemDatabase.items[name]["type"],ItemDatabase.items[name]["cam_remove"]));
            }
        }
        
        private void onPickup([FromSource]Player player,int obj)
        {
            string identifier = "steam:" + player.Identifiers["steam"];
            int source = int.Parse(player.Handle);
            if (ItemDatabase.usersInventory.ContainsKey(identifier))
            {
                addItem(source,Pickups[obj]["name"],Pickups[obj]["amount"]);
                TriggerClientEvent("vorpInventory:sharePickupClient",Pickups[obj]["name"],Pickups[obj]["obj"],
                    Pickups[obj]["amount"],Pickups[obj]["coords"],2,Pickups[obj]["hash"]);
                TriggerClientEvent("vorpInventory:removePickupClient",Pickups[obj]["obj"]);
                Pickups.Remove(obj);
                player.TriggerEvent("vorpInventory:playerAnim",obj);
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
            Debug.WriteLine($"Me ha llegado {name}");
            Debug.WriteLine(Pickups[obj]["name"].ToString());
        }
        private void serverDropItem([FromSource] Player source, string itemname, int cuantity)
        {
            string identifier = "steam:" + source.Identifiers["steam"];
            if (ItemDatabase.usersInventory[identifier][itemname].getCount() >= cuantity)
            {
                ItemDatabase.usersInventory[identifier][itemname].quitCount(cuantity);
                Debug.WriteLine(ItemDatabase.usersInventory[identifier][itemname].getCount().ToString());
            }

            if (ItemDatabase.usersInventory[identifier][itemname].getCount() == 0)
            {
                ItemDatabase.usersInventory[identifier].Remove(itemname);
            }
            source.TriggerEvent("vorpInventory:createPickup",itemname,cuantity,1);
        }

        private void serverGiveItem([FromSource] Player source, string itemname, int amount, int target)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[target];
            string targetIdentifier = "steam:"+p.Identifiers["steam"];
            string identifier = "steam:" + source.Identifiers["steam"];
            if (ItemDatabase.usersInventory[identifier][itemname].getCount() >= amount)
            {
                if (ItemDatabase.usersInventory[targetIdentifier].ContainsKey(itemname))
                {
                    ItemDatabase.usersInventory[targetIdentifier][itemname].addCount(amount);
                    ItemDatabase.usersInventory[identifier][itemname].quitCount(amount);
                    Debug.WriteLine($"{ItemDatabase.usersInventory[targetIdentifier][itemname].getCount()}");
                }
                else
                {
                    int limit = Utils.getItemCharacteristics(itemname)["limit"];
                    string label = Utils.getItemCharacteristics(itemname)["label"];
                    bool can_remove = Utils.getItemCharacteristics(itemname)["can_remove"];
                    ItemDatabase.usersInventory[targetIdentifier].Add(itemname,new ItemClass(amount,limit,label,
                        itemname,"item_inventory", true,can_remove));
                    ItemDatabase.usersInventory[identifier][itemname].quitCount(amount);
                }
                p.TriggerEvent("vorpinventory:receiveItem",itemname,amount);
                if (ItemDatabase.usersInventory[identifier][itemname].getCount() == 0)
                {
                    ItemDatabase.usersInventory[identifier].Remove(itemname);
                }
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
                    source.TriggerEvent("vorpInventory:giveInventory",result[0].inventory,result[0].loadout);
                }
            }));
        }
    }
}
