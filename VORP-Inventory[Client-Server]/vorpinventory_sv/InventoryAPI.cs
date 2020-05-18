using System;
using CitizenFX.Core;
namespace vorpinventory_sv
{
    public class InventoryAPI:BaseScript
    {
        public InventoryAPI()
        {
            EventHandlers["vorpInventory:subWeapon"] += new Action<Player, int>(subWeapons);
            EventHandlers["vorpInventory:addWeapon"] += new Action<Player,int>(addWeapons);
            EventHandlers["vorpInventory:createWeapon"] += new Action<int>(createWeapon);
        }

        public void subWeapons([FromSource] Player source, int weaponId)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[int.Parse(source.Handle)];
            string identifier = "steam:" + p.Identifiers["steam"];
            if (ItemDatabase.userWeapons.ContainsKey(weaponId) && ItemDatabase.userWeapons[weaponId].getPropietary() == identifier)
            {
                ItemDatabase.userWeapons[weaponId].setPropietary("");
                //p.TriggerEvent();
            }
            else
            {
                Debug.WriteLine("Someone tried to quit weapons to others"+identifier);
            }
        }

        public void addWeapons([FromSource] Player source, int weaponId)
        {
            
        }

        public void createWeapon(int player)
        {
            
        }
    }
}