using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace vorpinventory_sv
{
    public class Config : BaseScript
    {
        public static JObject config = new JObject();
        public static string ConfigString;
        public static Dictionary<string, string> lang = new Dictionary<string, string>();
        public static string resourcePath = $"{API.GetResourcePath(API.GetCurrentResourceName())}";
        private static Dictionary<string, string> weaponBaseBullets = new Dictionary<string, string>
        {
            {"WEAPON_THROWN_TOMAHAWK","AMMO_TOMAHAWK"},
            {"WEAPON_THROWN_THROWING_KNIVES","AMMO_THROWING_KNIVES"},
            {"WEAPON_BOW","AMMO_ARROW"},
            {"WEAPON_PISTOL","AMMO_PISTOL"},
            {"WEAPON_REVOLVER","AMMO_REVOLVER"},
            {"WEAPON_RIFLE_VARMINT","AMMO_RIFLE_VARMINT"},
            {"WEAPON_REPEATER","AMMO_REPEATER"},
            {"WEAPON_SNIPERRIFLE","AMMO_RIFLE"},
            {"WEAPON_RIFLE","AMMO_RIFLE"},
            {"WEAPON_SHOTGUN","AMMO_SHOTGUN"}

        };

        public Config()
        {
            EventHandlers["vorp:firstSpawn"] += new Action<int>(itemsConfig);

            if (File.Exists($"{resourcePath}/Config.json"))
            {
                ConfigString = File.ReadAllText($"{resourcePath}/Config.json", Encoding.UTF8);
                config = JObject.Parse(ConfigString);
                if (File.Exists($"{resourcePath}/languages/{config["defaultlang"]}.json"))
                {
                    string langstring = File.ReadAllText($"{resourcePath}/languages/{config["defaultlang"]}.json",
                        Encoding.UTF8);
                    lang = JsonConvert.DeserializeObject<Dictionary<string, string>>(langstring);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{API.GetCurrentResourceName()}: Language {config["defaultlang"]}.json loaded!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Debug.WriteLine($"{API.GetCurrentResourceName()}: {config["defaultlang"]}.json Not Found");
                }
            }
        }

        private void itemsConfig(int player)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[player];
            string identifier = "steam:" + p.Identifiers["steam"];
            foreach (JObject items in config["startItems"][0].Children<JObject>())
            {
                foreach (JProperty item in items.Properties())
                {
                    TriggerEvent("vorpCore:addItem", player, item.Name, item.Value.ToObject<int>());
                }
            }

            foreach (JObject weapons in config["startItems"][1].Children<JObject>())
            {
                foreach (JProperty weapon in weapons.Properties())
                {
                    TriggerEvent("vorpCore:registerWeapon", player, weapon.Name);
                    Delay(300);
                    foreach (KeyValuePair<int, WeaponClass> weap in ItemDatabase.userWeapons)
                    {
                        if (weap.Value.getPropietary() == identifier)
                        {
                            if (weap.Value.getName() == "WEAPON_THROWN_TOMAHAWK")
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets[weap.Value.getName()]);
                            }
                            if (weap.Value.getName() == "WEAPON_THROWN_THROWING_KNIVES")
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets[weap.Value.getName()]);
                            }
                            if (weap.Value.getName() == "WEAPON_BOW")
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets[weap.Value.getName()]);
                            }
                            if (weap.Value.getName().Contains("WEAPON_PISTOL"))
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets["WEAPON_PISTOL"]);
                            }
                            if (weap.Value.getName().Contains("WEAPON_REVOLVER"))
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets["WEAPON_REVOLVER"]);
                            }
                            if (weap.Value.getName().Contains("WEAPON_RIFLE_VARMINT"))
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets["WEAPON_RIFLE_VARMINT"]);
                            }
                            if (weap.Value.getName().Contains("WEAPON_REPEATER"))
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets["WEAPON_REPEATER"]);
                            }
                            if (weap.Value.getName().Contains("WEAPON_SNIPERRIFLE"))
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets["WEAPON_SNIPERRIFLE"]);
                            }
                            if (weap.Value.getName().Contains("WEAPON_RIFLE"))
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets["WEAPON_RIFLE"]);
                            }
                            if (weap.Value.getName().Contains("WEAPON_SHOTGUN"))
                            {
                                weap.Value.addAmmo(weapon.Value.ToObject<int>(), weaponBaseBullets["WEAPON_SHOTGUN"]);
                            }
                        }
                    }
                }
            }
        }
    }
}