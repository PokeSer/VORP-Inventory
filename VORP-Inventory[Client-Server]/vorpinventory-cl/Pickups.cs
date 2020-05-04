using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinventory_cl
{
    public class Pickups:BaseScript
    {
        public Pickups()
        {
            EventHandlers["vorpInventory:createPickup"] += new Action<string,int,int>(createPickup);
            EventHandlers["vorpInventory:sharePickupClient"] += new Action<string,int,int,Vector3,int,int>(sharePickupClient);
            EventHandlers["vorpInventory:removePickupClient"] += new Action<int>(removePickupClient);
            EventHandlers["vorpInventory:playerAnim"] += new Action<int>(playerAnim);
            Tick += principalFunctionPickups;
        }

        public static int PickPrompt;
        public static Dictionary<int,Dictionary<string,dynamic>> pickups = new Dictionary<int, Dictionary<string, dynamic>>();
        private static int wait = 0;
        private static bool active = false;

        private async Task principalFunctionPickups()
        {
            await SetupPickPrompt();
            await Delay(wait);

            int playerPed = API.PlayerPedId();
            Vector3 coords = Function.Call<Vector3>((Hash) 0xA86D5F069399F44D, playerPed);

            if (pickups.Count == 0)
            {
                await Delay(500);
            }

            foreach (var pick in pickups)
            {
                float distance = Function.Call<float>((Hash) 0x0BE7F4E3CDBAFB28, coords.X, coords.Y, coords.Z,
                    pick.Value["coords"].X,
                    pick.Value["coords"].Y, pick.Value["coords"].Z, true);

                if (distance >= 15.0F)
                {
                    wait = 2000;
                }
                else
                {
                    wait = 0;
                }

                if (distance <= 15.0F)
                {
                    if (pick.Value["hash"] == 1)
                    {
                        string name = pick.Value["name"];
                        if (vorp_inventoryClient.citems.ContainsKey(name))
                        {
                            name = vorp_inventoryClient.citems[name]["label"];
                        }
                        Utils.DrawText3D(pick.Value["coords"],name+" "+$"{pick.Value["amount"].ToString()}");
                    }
                }

                if (distance <= 0.7F && !pick.Value["inRange"])
                {
                    Function.Call((Hash)0x69F4BE8C8CC4796C,playerPed,pick.Value["obj"],3000,2048,3);
                    if (active == false)
                    {
                        Function.Call((Hash)0x8A0FB4D03A630D21,PickPrompt,true);
                        Function.Call((Hash)0x71215ACCFDE075EE,PickPrompt,true);
                    }

                    if (Function.Call<bool>((Hash) 0xE0F65F0640EF0617, PickPrompt))
                    {
                        TriggerServerEvent("vorpInventory:onPickup",pick.Value["obj"]);
                        pick.Value["inRange"] = true;
                    }
                }
                else
                {
                    if (active == true)
                    {
                        Function.Call((Hash)0x8A0FB4D03A630D21,PickPrompt,false);
                        Function.Call((Hash)0x71215ACCFDE075EE,PickPrompt,false);
                        active = false;
                    }
                }
            }
        }
        private async void playerAnim(int obj)
        {
            string dict = "amb_work@world_human_box_pickup@1@male_a@stand_exit_withprop";
            Function.Call((Hash)0xA862A2AD321F94B4,dict);

            while (!Function.Call<bool>((Hash)0x27FF6FE8009B40CA,dict))
            {
                await Delay(10);
            }
            Function.Call((Hash)0xEA47FE3719165B94,API.PlayerPedId(),dict,"exit_front", 1.0, 8.0, -1, 1, 0, false, false, false);
            await Delay(1200);
            Function.Call((Hash)0x67C540AA08E4A6F5,"CHECKPOINT_PERFECT", "HUD_MINI_GAME_SOUNDSET", true, 1);
            await Delay(1000);
            Function.Call((Hash)0xE1EF3C1216AFF2CD,API.PlayerPedId());
        }

        private async void removePickupClient(int obj)
        {
            Function.Call((Hash)0xDC19C288082E586E,obj,false,true);
            API.NetworkRequestControlOfEntity(obj);
            int timeout = 0;
            while (!API.NetworkHasControlOfEntity(obj) && timeout <5000)
            {
                timeout += 100;
                if (timeout == 5000)
                {
                    Debug.WriteLine("No se ha obtenido el control de la entidad");
                }

                await Delay(100);
            }
            Function.Call((Hash)0x4CD38C78BD19A497,obj);
            Function.Call((Hash)0x7D9EFB7AD6B19754,obj,false);
            
        }

        private void sharePickupClient(string name, int obj, int amount, Vector3 position, int value, int hash)
        {
            if (value == 1)
            {
                pickups.Add(obj,new Dictionary<string, dynamic>
                {
                    ["name"] = name,
                    ["obj"] = obj,
                    ["amount"] = amount,
                    ["hash"] = hash,
                    ["inRange"] = false,
                    ["coords"] = position
                });
            }
            else
            {
                pickups.Remove(obj);
            }   
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
            TriggerServerEvent("vorpInventory:sharePickupServer",name,obj,amoun,position,hash);
            Function.Call((Hash)0x67C540AA08E4A6F5,"show_info", "Study_Sounds", true, 0);
        }

        public static Task SetupPickPrompt()
        {
            PickPrompt = Function.Call<int>((Hash) 0x04F97DE45A519419);
            Function.Call((Hash)0xB5352B7494A08258,PickPrompt,0xF84FA74F);
            string var = Function.Call<string>((Hash) 0xFA925AC00EB830B9, 10, "LITERAL_STRING", "Coger");
            Function.Call((Hash) 0x5DD02A8318420DD7, PickPrompt, var);
            Function.Call((Hash) 0x8A0FB4D03A630D21,PickPrompt,false);
            Function.Call((Hash)0x71215ACCFDE075EE,false);
            Function.Call((Hash)0x94073D5CA3F16B7B,true);
            Function.Call((Hash)0xF7AA2696A22AD8B9);
        }
    }
}