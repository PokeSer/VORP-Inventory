using System;
using CitizenFX.Core;

namespace vorpinventory_cl
{
    public class InventoryAPI:BaseScript
    {
        public InventoryAPI()
        {
            EventHandlers["vorpCoreClient:addItem"] += new Action<int,int,string,string,string,bool,bool>(addItem);
            EventHandlers["vorpCoreClient:subItem"] += new Action<string,int>(subItem);
            EventHandlers["vorpCoreClient:subWeapon"] += new Action<int>(subWeapon);
        }

        private void subWeapon(int weaponId)
        {
            if (vorp_inventoryClient.userWeapons.ContainsKey(weaponId))
            {
                vorp_inventoryClient.userWeapons.Remove(weaponId);
            }
            NUIEvents.LoadInv();
        }
        
        private void subItem(string name, int cuantity)
        {
            if (vorp_inventoryClient.useritems.ContainsKey(name))
            {
                vorp_inventoryClient.useritems[name].quitCount(cuantity);
                if (vorp_inventoryClient.useritems[name].getCount() == 0)
                {
                    vorp_inventoryClient.useritems.Remove(name);
                }
            }

            NUIEvents.LoadInv();
        }

        private void addItem(int count, int limit, string label, string name, string type, bool usable, bool canRemove)
        {
            if (vorp_inventoryClient.useritems.ContainsKey(name))
            {
                vorp_inventoryClient.useritems[name].addCount(count);
            }
            else
            {
                ItemClass auxitem = new ItemClass(count,limit,label,name,type,usable,canRemove);
                vorp_inventoryClient.useritems.Add(name,auxitem);
            }
            NUIEvents.LoadInv();
        }
    }
}