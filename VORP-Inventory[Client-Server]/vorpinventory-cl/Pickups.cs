using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinventory_cl
{
    public class Pickups:BaseScript
    {
        public Pickups()
        {
            EventHandlers["vorpInventory:createPickup"] += new Action<string,int,int>();
        }

        private void createPickup(string name, int amoun, int hash)
        {
            int ped = API.PlayerPedId();
            Vector3 coords = Function.Call<Vector3>((Hash) 0xA86D5F069399F44D, ped, true, true);
            Vector3 forward = Function.Call<Vector3>((Hash) 0x2412D9C05BB09B97, ped);
            Vector3 position = (coords + forward * 1.6F);
            if(Function.Call<bool>((Hash)0x1283B8B89DD5D1B6,"P_COTTONBOX01X"))
        }
    }
}