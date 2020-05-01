using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinventory_cl
{
    public class Utils:BaseScript
    {
        public Utils()
        {
            API.RegisterCommand("nearestPlayer",new Action(getNearestPlayers),false );
        }

        public static void getNearestPlayers()
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
            
            foreach (var VARIABLE in closestPlayers)
            {
                Debug.WriteLine(API.GetPlayerName(VARIABLE));
                Debug.WriteLine(API.GetPlayerServerId(VARIABLE).ToString());
            }
        }
    }
}