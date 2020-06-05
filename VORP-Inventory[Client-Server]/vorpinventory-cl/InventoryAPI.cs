using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinventory_cl
{
    public class InventoryAPI:BaseScript
    {
        //TODO HACER QUE CUANDO LA TENGAS COGIDA SI TE DAN O TE QUITAN MUNICION TE LA QUITE Y TE LA AÑADA TAMBIEN AL PJ
        public InventoryAPI()
        {
            EventHandlers["vorpCoreClient:addItem"] += new Action<int,int,string,string,string,bool,bool>(addItem);
            EventHandlers["vorpCoreClient:subItem"] += new Action<string,int>(subItem);
            EventHandlers["vorpCoreClient:subWeapon"] += new Action<int>(subWeapon);
            EventHandlers["vorpCoreClient:addBullets"] += new Action<int,string,int>(addWeaponBullets);
            EventHandlers["vorpCoreClient:subBullets"]+= new Action<int,string,int>(subWeaponBullets);
            EventHandlers["vorpCoreClient:addComponent"] += new Action<int,string>(addComponent);
        }

        private void addComponent(int weaponId, string component)
        {
            if (vorp_inventoryClient.userWeapons.ContainsKey(weaponId))
            {
                if (!vorp_inventoryClient.userWeapons[weaponId].getAllComponents().Contains(component))
                {
                    vorp_inventoryClient.userWeapons[weaponId].setComponent(component);
                }
            }
        }
        
        private void subWeaponBullets(int weaponId, string bulletType, int cuantity)
        {
            if (vorp_inventoryClient.userWeapons.ContainsKey(weaponId))
            {
                vorp_inventoryClient.userWeapons[weaponId].subAmmo(cuantity,bulletType);
            }
            NUIEvents.LoadInv();
        }
        
        private void addWeaponBullets(int weaponId, string bulletType, int cuantity)
        {
            if (vorp_inventoryClient.userWeapons.ContainsKey(weaponId))
            {
                vorp_inventoryClient.userWeapons[weaponId].addAmmo(cuantity,bulletType);
            }
            NUIEvents.LoadInv();
        }
        
        private void subWeapon(int weaponId)
        {
            if (vorp_inventoryClient.userWeapons.ContainsKey(weaponId))
            {
                if (vorp_inventoryClient.userWeapons[weaponId].getUsed())
                {
                    API.RemoveWeaponFromPed(API.PlayerPedId(),
                        (uint)API.GetHashKey(vorp_inventoryClient.userWeapons[weaponId].getName()),
                        true,0); //Falta revisar que pasa con esto
                }
                vorp_inventoryClient.userWeapons.Remove(weaponId);
            }
            NUIEvents.LoadInv();
        }
        
        private void subItem(string name, int cuantity)
        {
            Debug.WriteLine($"Me llaman con cantidad a poner = {cuantity}");
            if (vorp_inventoryClient.useritems.ContainsKey(name))
            {
                vorp_inventoryClient.useritems[name].setCount(cuantity);
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