using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace vorpinventory_sv
{
    public class vorpinventory_sv : BaseScript
    {
        public vorpinventory_sv()
        {
            EventHandlers["vorpinventory:getItemsTable"] += new Action<Player>(getItemsTable);
            EventHandlers["vorpinventory:getInventory"] += new Action<Player>(getInventory);
            EventHandlers["vorpinventory:serverGiveItem"] += new Action<Player, string, int, int>(serverGiveItem);
            EventHandlers["vorpinventory:serverGiveWeapon"] += new Action<Player, int, int>(serverGiveWeapon);
            EventHandlers["vorpinventory:serverDropItem"] += new Action<Player, string, int>(serverDropItem);
            EventHandlers["vorpinventory:serverDropWeapon"] += new Action<Player, int>(serverDropWeapon);
            EventHandlers["vorpinventory:sharePickupServer"] += new Action<string, int, int, Vector3, int>(sharePickupServer);
            EventHandlers["vorpinventory:onPickup"] += new Action<Player, int>(onPickup);
            EventHandlers["vorpinventory:setUsedWeapon"] += new Action<Player, int, bool>(usedWeapon);
            EventHandlers["vorpinventory:setWeaponBullets"] += new Action<Player, int, string, int>(setWeaponBullets);
            EventHandlers["playerDropped"] += new Action<Player, string>(SaveInventoryItems);
            //EventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);
        }
        public static Dictionary<int, Dictionary<string, dynamic>> Pickups = new Dictionary<int, Dictionary<string, dynamic>>();

        private void setWeaponBullets([FromSource] Player player, int weaponId, string type, int bullet)
        {
            if (ItemDatabase.userWeapons.ContainsKey(weaponId))
            {
                ItemDatabase.userWeapons[weaponId].setAmmo(bullet, type);
            }
        }

        // private void OnResourceStop(string resource)
        // {
        //     PlayerList pl = new PlayerList();
        //     foreach (Player p in pl)
        //     {
        //         string identifier = "steam:" + p.Identifiers["steam"];
        //         Dictionary<string,int> items = new Dictionary<string, int>();
        //         if (ItemDatabase.usersInventory.ContainsKey(identifier))
        //         {
        //             foreach (var item in ItemDatabase.usersInventory[identifier])
        //             {
        //                 items.Add(item.Key,item.Value.getCount());
        //             }
        //             if (items.Count > 0) 
        //             {
        //                 string json = Newtonsoft.Json.JsonConvert.SerializeObject(items);
        //                 Exports["ghmattimysql"].execute($"UPDATE characters SET inventory = '{json}' WHERE identifier=?", new[] {identifier});
        //             }   
        //         }
        //     }
        //
        //     foreach (KeyValuePair<int,WeaponClass> weap in ItemDatabase.userWeapons)
        //     {//SET ammo = '{Newtonsoft.Json.JsonConvert.SerializeObject(ItemDatabase.userWeapons[weaponId].getAllAmmo())}'
        //         Exports["ghmattimysql"]
        //             .execute(
        //                 $"UPDATE loadout SET identifier = '{weap.Value.getPropietary()}',SET ammo = " +
        //                 $"'{Newtonsoft.Json.JsonConvert.SerializeObject(weap.Value.getAllAmmo())}'," +
        //                 $"SET components = '{Newtonsoft.Json.JsonConvert.SerializeObject(weap.Value.getAllComponents())}' WHERE id=?",
        //                 new[] {weap.Value.getId()});
        //     }
        // }
        private void SaveInventoryItems([FromSource] Player p, string something)
        {
            string identifier = "steam:" + p.Identifiers["steam"];
            Dictionary<string, int> items = new Dictionary<string, int>();
            if (ItemDatabase.usersInventory.ContainsKey(identifier))
            {
                foreach (var item in ItemDatabase.usersInventory[identifier])
                {
                    items.Add(item.Key, item.Value.getCount());
                }
                if (items.Count > 0)
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(items);
                    Exports["ghmattimysql"].execute($"UPDATE characters SET inventory = '{json}' WHERE identifier=?", new[] { identifier });
                }
            }
        }
        private void usedWeapon([FromSource]Player source, int id, bool used)
        {
            int Used = used ? 1 : 0;
            Exports["ghmattimysql"]
                .execute(
                    $"UPDATE loadout SET used = '{Used}' WHERE id=?",
                    new[] { id });
        }
        //Sub items for other scripts
        private void subItem(int player, string name, int cuantity)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[player];
            string identifier = "steam:" + p.Identifiers["steam"];
            if (ItemDatabase.usersInventory.ContainsKey(identifier))
            {
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
        }

        //For other scripts add items
        private void addItem(int player, string name, int cuantity)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[player];
            string identifier = "steam:" + p.Identifiers["steam"];
            if (ItemDatabase.usersInventory.ContainsKey(identifier))
            {
                if (ItemDatabase.usersInventory[identifier].ContainsKey(name))
                {
                    if (cuantity > 0)
                    {
                        ItemDatabase.usersInventory[identifier][name].addCount(cuantity);
                    }
                }
                else
                {
                    if (ItemDatabase.svItems.ContainsKey(name))
                    {
                        ItemDatabase.usersInventory[identifier].Add(name, new ItemClass(cuantity, ItemDatabase.svItems[name].getLimit(),
                            ItemDatabase.svItems[name].getLabel(), name, "item_inventory", true, ItemDatabase.svItems[name].getCanRemove()));
                    }
                }
            }
            else
            {
                Dictionary<string, ItemClass> userinv = new Dictionary<string, ItemClass>();
                ItemDatabase.usersInventory.Add(identifier, userinv);
                if (ItemDatabase.svItems.ContainsKey(name))
                {
                    ItemDatabase.usersInventory[identifier].Add(name, new ItemClass(cuantity, ItemDatabase.svItems[name].getLimit(),
                        ItemDatabase.svItems[name].getLabel(), name, "item_inventory", true, ItemDatabase.svItems[name].getCanRemove()));
                }
            }
        }

        private void addWeapon(int player, int weapId)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[player];
            string identifier = "steam:" + p.Identifiers["steam"];
            if (ItemDatabase.userWeapons.ContainsKey(weapId))
            {
                ItemDatabase.userWeapons[weapId].setPropietary(identifier);
                Exports["ghmattimysql"]
                    .execute(
                        $"UPDATE loadout SET identifier = '{ItemDatabase.userWeapons[weapId].getPropietary()}' WHERE id=?",
                        new[] { weapId });
            }
        }

        private void subWeapon(int player, int weapId)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[player];
            string identifier = "steam:" + p.Identifiers["steam"];
            if (ItemDatabase.userWeapons.ContainsKey(weapId))
            {
                ItemDatabase.userWeapons[weapId].setPropietary("");
                Exports["ghmattimysql"]
                    .execute(
                        $"UPDATE loadout SET identifier = '{ItemDatabase.userWeapons[weapId].getPropietary()}' WHERE id=?",
                        new[] { weapId });
            }
        }

        private void onPickup([FromSource]Player player, int obj)
        {
            string identifier = "steam:" + player.Identifiers["steam"];
            int source = int.Parse(player.Handle);
            if (Pickups.ContainsKey(obj))
            {
                if (Pickups[obj]["weaponid"] == 1)
                {
                    if (ItemDatabase.usersInventory.ContainsKey(identifier))
                    {
                        addItem(source, Pickups[obj]["name"], Pickups[obj]["amount"]);
                        Debug.WriteLine($"añado {Pickups[obj]["amount"]}");
                        TriggerClientEvent("vorpInventory:sharePickupClient", Pickups[obj]["name"], Pickups[obj]["obj"],
                            Pickups[obj]["amount"], Pickups[obj]["coords"], 2, Pickups[obj]["weaponid"]);
                        TriggerClientEvent("vorpInventory:removePickupClient", Pickups[obj]["obj"]);
                        player.TriggerEvent("vorpinventory:receiveItem", Pickups[obj]["name"], Pickups[obj]["amount"]);
                        player.TriggerEvent("vorpInventory:playerAnim", obj);
                        Pickups.Remove(obj);
                    }
                }
                else
                {
                    int weaponId = Pickups[obj]["weaponid"];
                    addWeapon(source, Pickups[obj]["weaponid"]);
                    //Debug.WriteLine($"añado {ItemDatabase.userWeapons[Pickups[obj]["weaponid"].ToString()].getPropietary()}");
                    TriggerClientEvent("vorpInventory:sharePickupClient", Pickups[obj]["name"], Pickups[obj]["obj"],
                        Pickups[obj]["amount"], Pickups[obj]["coords"], 2, Pickups[obj]["weaponid"]);
                    TriggerClientEvent("vorpInventory:removePickupClient", Pickups[obj]["obj"]);
                    player.TriggerEvent("vorpinventory:receiveWeapon", weaponId, ItemDatabase.userWeapons[weaponId].getPropietary(),
                        ItemDatabase.userWeapons[weaponId].getName(), ItemDatabase.userWeapons[weaponId].getAllAmmo(), ItemDatabase.userWeapons[weaponId].getAllComponents());
                    player.TriggerEvent("vorpInventory:playerAnim", obj);
                    Pickups.Remove(obj);
                }
            }

        }

        private void sharePickupServer(string name, int obj, int amount, Vector3 position, int weaponId)
        {
            TriggerClientEvent("vorpInventory:sharePickupClient", name, obj, amount, position, 1, weaponId);
            Debug.WriteLine(obj.ToString());
            Pickups.Add(obj, new Dictionary<string, dynamic>
            {
                ["name"] = name,
                ["obj"] = obj,
                ["amount"] = amount,
                ["weaponid"] = weaponId,
                ["inRange"] = false,
                ["coords"] = position
            });
        }

        //Weapon methods
        private void serverDropWeapon([FromSource] Player source, int weaponId)
        {
            subWeapon(int.Parse(source.Handle), weaponId);
            source.TriggerEvent("vorpInventory:createPickup", ItemDatabase.userWeapons[weaponId].getName(), 1, weaponId);
        }

        //Items methods
        private void serverDropItem([FromSource] Player source, string itemname, int cuantity)
        {
            subItem(int.Parse(source.Handle), itemname, cuantity);
            source.TriggerEvent("vorpInventory:createPickup", itemname, cuantity, 1);
        }

        private void serverGiveWeapon([FromSource] Player source, int weaponId, int target)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[target];
            string identifier = "steam:" + source.Identifiers["steam"];

            if (ItemDatabase.userWeapons.ContainsKey(weaponId))
            {
                subWeapon(int.Parse(source.Handle), weaponId);
                addWeapon(int.Parse(p.Handle), weaponId);
                p.TriggerEvent("vorpinventory:receiveWeapon", weaponId, ItemDatabase.userWeapons[weaponId].getPropietary(),
                    ItemDatabase.userWeapons[weaponId].getName(), ItemDatabase.userWeapons[weaponId].getAllAmmo(), ItemDatabase.userWeapons[weaponId].getAllComponents());
            }
        }
        private void serverGiveItem([FromSource] Player source, string itemname, int amount, int target)
        {
            bool give = true;
            PlayerList pl = new PlayerList();
            Player p = pl[target];
            string identifier = "steam:" + source.Identifiers["steam"];
            string targetIdentifier = "steam:" + p.Identifiers["steam"];

            if (ItemDatabase.usersInventory[identifier][itemname].getCount() >= amount)
            {
                if (ItemDatabase.usersInventory[targetIdentifier].ContainsKey(itemname))
                {
                    if (ItemDatabase.usersInventory[targetIdentifier][itemname].getCount() + amount
                        >= ItemDatabase.usersInventory[targetIdentifier][itemname].getLimit())
                    {
                        give = false;
                    }
                }

                if (give)
                {
                    addItem(int.Parse(p.Handle), itemname, amount);
                    subItem(int.Parse(source.Handle), itemname, amount);
                    p.TriggerEvent("vorpinventory:receiveItem", itemname, amount);
                }
                else
                {
                    TriggerClientEvent(source, "vorp:Tip", Config.lang["fullInventoryGive"], 2000);
                    TriggerClientEvent(p, "vorp:Tip", Config.lang["fullInventory"], 2000);
                }

            }
        }

        private void getItemsTable([FromSource] Player source)
        {
            if (ItemDatabase.items.Count != 0)
            {
                source.TriggerEvent("vorpInventory:giveItemsTable", ItemDatabase.items);
            }
        }

        private void getInventory([FromSource]Player source)
        {
            string steamId = "steam:" + source.Identifiers["steam"];
            Debug.WriteLine(steamId);
            Exports["ghmattimysql"].execute("SELECT inventory FROM characters WHERE identifier = ?;", new[] { steamId }, new Action<dynamic>((result) =>
              {
                  if (result.Count == 0)
                  {
                      Debug.WriteLine($"{steamId} doesn`t have inventory yet.");
                      Dictionary<string, ItemClass> items = new Dictionary<string, ItemClass>();
                      ItemDatabase.usersInventory.Add(steamId, items); // Si no existe le metemos en la caché para tenerlo preparado para recibir cosas
                  }
                  else
                  {
                      //Debug.WriteLine(result[0].inventory);
                      source.TriggerEvent("vorpInventory:giveInventory", result[0].inventory);
                  }
              }));
            Exports["ghmattimysql"].execute("SELECT * FROM loadout WHERE identifier = ?;", new[] { steamId }, new Action<dynamic>((result) =>
               {
                   if (result.Count == 0)
                   {
                       Debug.WriteLine($"{steamId} doesn`t have loadout yet.");
                   }
                   else
                   {
                       source.TriggerEvent("vorpInventory:giveLoadout", result);
                   }
               }));
        }
    }
}
