using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinventory_cl
{
    public class Utils:BaseScript
    {
        public Utils()
        {
        }
        
        public static void addItems(string name, int cuantity)
        {
            if (vorp_inventoryClient.useritems.ContainsKey(name))
            {
                vorp_inventoryClient.useritems[name].addCount(cuantity);
            }
            else
            {
                vorp_inventoryClient.useritems.Add(name,new ItemClass(cuantity,vorp_inventoryClient.citems[name]["limit"],
                    vorp_inventoryClient.citems[name]["label"],name,
                    "item_standard",true,vorp_inventoryClient.citems[name]["can_remove"]));
            }
        }

        public static async Task DrawText3D(Vector3 position,string text)
        {
            float _x = 0.0F;
            float _y = 0.0F;
            //Debug.WriteLine(position.X.ToString());
            API.GetScreenCoordFromWorldCoord(position.X, position.Y, position.Z, ref _x, ref _y);
            API.SetTextScale(0.35F,0.35F);
            API.SetTextFontForCurrentCommand(1);
            API.SetTextColor(255,255,255,215);
            long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", text);
            Function.Call((Hash)0xBE5261939FBECB8C,1);
            Function.Call((Hash)0xD79334A4BB99BAD1,str,_x,_y);
            float factor = text.Length / 150.0F;
            Function.Call((Hash) 0xC9884ECADE94CB34, "generic_textures", "hud_menu_4a", _x, _y + 0.0125F, 0.015F + factor,
                0.03F, 0.1F, 100, 1, 1, 190, 0);
        }
        
        public static Dictionary<string,dynamic> expandoProcessing(dynamic objet)
        {
            Dictionary<string,dynamic> aux = new Dictionary<string, dynamic>();
            foreach (var o in objet)
            {
                aux.Add(o.Key,o.Value);
            }
            return aux;
        }

        public static List<int> getNearestPlayers()
        {
            float closestDistance = 5.0F;
            int localPed = API.PlayerPedId();
            Vector3 coords = API.GetEntityCoords(localPed,true,true);
            List<int> closestPlayers = new List<int>();
            List<int> players = new List<int>();
            foreach (var player in API.GetActivePlayers())
            {
                players.Add(player);
            }
            
            foreach (var player in players)
            {
                int target = API.GetPlayerPed(player);
                if (target != localPed)
                {
                    Vector3 targetCoords = API.GetEntityCoords(target, true, true);
                    float distance = API.GetDistanceBetweenCoords(targetCoords.X, targetCoords.Y, targetCoords.Z,
                        coords.X, coords.Y, coords.Z,false);
            
                    if (closestDistance > distance)
                    {
                        closestPlayers.Add(player);
                    }
                }
            }

            return closestPlayers;
        }
    }
}