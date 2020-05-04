using System;
using System.Collections.Generic;
using System.Dynamic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinventory_cl
{
    public class Utils:BaseScript
    {
        public Utils()
        {
        }

        public static void DrawText3D(Vector3 position,string text)
        {
            float _x = 0.0F;
            float _y = 0.0F;
            API.GetScreenCoordFromWorldCoord(position.X, position.Y, position.Z, ref _x, ref _y);
            Vector3 camCoords = Function.Call<Vector3>((Hash) 0x595320200B98596E);
            Function.Call((Hash)0x4170B650590B3B00,0.35F,0.35F);
            API.SetTextFontForCurrentCommand(1);
            Function.Call((Hash)0x50A41AD966910F03,255,255,255,215);
            string str = Function.Call<string>((Hash) 0xFA925AC00EB830B9, 10, "LITERAL_STRING",text);
            Function.Call((Hash)0xBE5261939FBECB8C,1);
            Function.Call((Hash)0xD79334A4BB99BAD1,str,_x,_y);
            float factor = text.Length / 150.0F;
            Function.Call((Hash) 0xC9884ECADE94CB34, "generic_textures", "hud_menu_4a", _x, _y + 0.0125, 0.015 + factor,
                0.03, 0.1, 100, 1, 1, 190, 0);
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