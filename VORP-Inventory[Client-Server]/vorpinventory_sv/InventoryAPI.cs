using System;
using CitizenFX.Core;
namespace vorpinventory_sv
{
    public class InventoryAPI:BaseScript
    {
        public InventoryAPI()
        {
            EventHandlers["vorpCore:subWeapon"] += new Action<int, int>(subWeapon);
            EventHandlers["vorpCore:giveWeapon"] += new Action<int,int,int>(giveWeapon);
            //EventHandlers["vorpCore:registerWeapon"] += new Action<int(registerWeapon);
            EventHandlers["vorpCore:addItem"] += new Action<int, string, int>(addItem);
            EventHandlers["vorpCore:subItem"] += new Action<int,string,int>(subItem);
        }
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
                if (ItemDatabase.svItems.ContainsKey(name))
                {
                    ItemDatabase.usersInventory[identifier].Add(name,new ItemClass(cuantity,ItemDatabase.svItems[name].getLimit(), 
                        ItemDatabase.svItems[name].getLabel(),name,"item_inventory",true,ItemDatabase.svItems[name].getCanRemove()));
                }
            }

            if (ItemDatabase.usersInventory[identifier].ContainsKey(name))
            {
                int count = ItemDatabase.usersInventory[identifier][name].getCount();
                int limit = ItemDatabase.usersInventory[identifier][name].getLimit();
                string label = ItemDatabase.usersInventory[identifier][name].getLabel();
                string type = ItemDatabase.usersInventory[identifier][name].getType();
                bool usable = ItemDatabase.usersInventory[identifier][name].getUsable();
                bool canRemove = ItemDatabase.usersInventory[identifier][name].getCanRemove();
                p.TriggerEvent("vorpCoreClient:addItem",count,limit,label,name,type,usable,canRemove);//Pass item to client
            }
        }
        
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
                p.TriggerEvent("vorpCoreClient:subItem",name,cuantity);
            }
        }

        // private void registerWeapon(int target,)
        // {
        //     
        // }
        private void giveWeapon(int player, int weapId, int target)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[player];
            Player ptarget = null;
            bool targetIsPlayer = false;
            foreach (Player pla in pl)
            {
                if (int.Parse(pla.Handle) == target)
                {
                    targetIsPlayer = true;
                }
            }

            if (targetIsPlayer)
            {
                ptarget = pl[target];
            }
            string identifier = "steam:" + p.Identifiers["steam"];
            
            if (ItemDatabase.userWeapons.ContainsKey(weapId))
            {
                ItemDatabase.userWeapons[weapId].setPropietary(identifier);
                Exports["ghmattimysql"]
                    .execute(
                        $"UPDATE loadout SET identifier = '{ItemDatabase.userWeapons[weapId].getPropietary()}' WHERE id=?",
                        new[] {weapId});
                p.TriggerEvent("vorpinventory:receiveWeapon",weapId,ItemDatabase.userWeapons[weapId].getPropietary(),
                    ItemDatabase.userWeapons[weapId].getName(),ItemDatabase.userWeapons[weapId].getAllAmmo(),ItemDatabase.userWeapons[weapId].getAllComponents());
                if (targetIsPlayer && ptarget != null)
                {
                    ptarget.TriggerEvent("vorpCoreClient:subWeapon",weapId);
                }
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
                        new[] {weapId});
            }
            p.TriggerEvent("vorpCoreClient:subWeapon",weapId);
        }
    }
}