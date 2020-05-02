using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinventory_cl
{
    public class Pickups:BaseScript
    {
        public Pickups()
        {
            EventHandlers["vorpInventory:createPickup"] += new Action<string,int,int>(createPickup);
        }

        private async void createPickup(string name, int amoun, int hash)
        {
            int ped = API.PlayerPedId();
            Vector3 coords = Function.Call<Vector3>((Hash) 0xA86D5F069399F44D, ped, true, true);
            Vector3 forward = Function.Call<Vector3>((Hash) 0x2412D9C05BB09B97, ped);
            Vector3 position = (coords + forward * 1.6F);
            if(!Function.Call<bool>((Hash)0x1283B8B89DD5D1B6,(uint)API.GetHashKey("P_COTTONBOX01X")))
            {
                Function.Call((Hash)0xFA28FE3A6246FC30,(uint)API.GetHashKey("P_COTTONBOX01X"));
            }

            while (!Function.Call<bool>((Hash)0x1283B8B89DD5D1B6,(uint)API.GetHashKey("P_COTTONBOX01X")))
            {
                await Delay(1);
            }

            int obj = Function.Call<int>((Hash) 0x509D5878EB39E842, (uint) API.GetHashKey("P_COTTONBOX01X"), position.X
                , position.Y, position.Z, true, true, true);
            Function.Call((Hash)0x58A850EAEE20FAA3,obj);
            Function.Call((Hash)0xDC19C288082E586E,obj,true,false);
            Function.Call((Hash)0x7D9EFB7AD6B19754,obj,true);
            //Evento que manda al servidor el item creado
            Function.Call((Hash)0x67C540AA08E4A6F5,"show_info", "Study_Sounds", true, 0);
        }
    }
}