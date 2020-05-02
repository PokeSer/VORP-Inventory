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
        }

        private void serverGiveItem([FromSource] Player source, string itemname, int amount, int target)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[target];
            string targetIdentifier = "steam:"+p.Identifiers["steam"];
            string identifier = "steam:" + source.Identifiers["steam"];
            if (ItemDatabase.usersInventory[targetIdentifier].ContainsKey(itemname))
            {
                ItemDatabase.usersInventory[targetIdentifier][itemname].addCount(amount);
                ItemDatabase.usersInventory[identifier][itemname].quitCount(amount);
                if (ItemDatabase.usersInventory[identifier][itemname].getCount() == 0)
                {
                    ItemDatabase.usersInventory[identifier].Remove(itemname);
                }
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
                if (ItemDatabase.usersInventory[identifier][itemname].getCount() == 0)
                {
                    ItemDatabase.usersInventory[identifier].Remove(itemname);
                }
            }
            p.TriggerEvent("vorpinventory:receiveItem",itemname,amount);

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
