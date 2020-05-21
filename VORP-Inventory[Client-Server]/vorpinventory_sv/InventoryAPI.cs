using System;
using System.Collections.Generic;
using System.Dynamic;
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

        private void registerWeapon(int target,string name,ExpandoObject ammo ,List<dynamic> components)//Needs dirt level
        {
            PlayerList pl = new PlayerList();
            Player p = null;
            bool targetIsPlayer = false;
            foreach (Player pla in pl)
            {
                if (int.Parse(pla.Handle) == target)
                {
                    p = pl[target];
                    targetIsPlayer = true;
                }
            }

            string identifier;
            if (targetIsPlayer)
            { 
                identifier = "steam:" + p.Identifiers["steam"];
            }
            else
            {
                identifier = target.ToString();
            }
            
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
            string componentsJ = Newtonsoft.Json.JsonConvert.SerializeObject(auxcomponents);
            string ammosJ = Newtonsoft.Json.JsonConvert.SerializeObject(ammoaux);
            int weaponId = -1;
            Exports["ghmattimysql"].execute("SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'vorp' AND TABLE_NAME   = 'loadout';",
                new Action<dynamic>((id) =>
            {
                weaponId = int.Parse(id[0].AUTO_INCREMENT.ToString());
            }));
            Exports["ghmattimysql"]
                .execute(
                    $"INSERT INTO loadout (identifier,name,ammo,components) VALUES ({identifier},{name},{ammosJ},{componentsJ}");
            
            WeaponClass auxWeapon = new WeaponClass(weaponId,identifier,name,ammoaux,auxcomponents);
            ItemDatabase.userWeapons.Add(weaponId,auxWeapon);
            if (targetIsPlayer)
            {
                p.TriggerEvent("vorpinventory:receiveWeapon",weaponId,ItemDatabase.userWeapons[weaponId].getPropietary(),
                    ItemDatabase.userWeapons[weaponId].getName(),ItemDatabase.userWeapons[weaponId].getAllAmmo(),ItemDatabase.userWeapons[weaponId].getAllComponents());
            }

        }
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