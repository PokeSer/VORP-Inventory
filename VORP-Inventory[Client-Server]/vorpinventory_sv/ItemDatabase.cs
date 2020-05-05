using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace vorpinventory_sv
{
    public class ItemDatabase:BaseScript
    {
        //Lista de items con sus labels para que el cliente conozca el label de cada item
        public static dynamic items;
        //Lista de itemclass con el nombre de su dueño para poder hacer todo el tema de añadir y quitar cuando se robe y demas
        public static Dictionary<string,Dictionary<string,ItemClass>> usersInventory = new Dictionary<string, Dictionary<string,ItemClass>>();
        public static Dictionary<string,List<WeaponClass>> usersWeapons = new Dictionary<string, List<WeaponClass>>();
        public static Dictionary<string,Items> svItems = new Dictionary<string, Items>();
        public ItemDatabase()
        {
            // WeaponClass wp = new WeaponClass("WEAPON_REVOLVER_LEMAT",new Dictionary<string, int>
            // {
            //     ["AMMO_PISTOL"] = 10,
            //     ["AMMO_PISTOL_EXPRESS"] = 20
            // });
            // List<Dictionary<string,dynamic>> allweapons = new List<Dictionary<string, dynamic>>();
            // Dictionary<string,dynamic> weapon = new Dictionary<string, dynamic>
            // {
            //     ["name"] = wp.getName(),
            //     ["ammo"] = wp.getAllAmmo()
            // };
            // allweapons.Add(weapon);
            // string js = Newtonsoft.Json.JsonConvert.SerializeObject(allweapons);
            Exports["ghmattimysql"].execute("SELECT * FROM items",new Action<dynamic>((result) =>
            {
                if (result.Count == 0)
                {
                    Debug.WriteLine("There`re no items in database");
                }
                else
                {
                    items = result;
                    foreach (dynamic item in items)
                    {
                        svItems.Add(item.item.ToString(),new Items(item.item,item.label,int.Parse(item.limit.ToString()),item.can_remove));
                    }
                    Exports["ghmattimysql"].execute("SELECT identifier,inventory,loadout FROM characters", new Action<dynamic>((uinvento) =>
                    {
                        foreach (var userInventory in uinvento)
                        {
                            //Carga del inventario
                            string user = userInventory.identifier.ToString();
                            Dictionary<string,ItemClass> userinv = new Dictionary<string, ItemClass>();
                            List<WeaponClass> userwep = new List<WeaponClass>();
                            if (userInventory.inventory != null)
                            {
                                dynamic thing = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(userInventory.inventory);
                                foreach (dynamic itemname in items)
                                {
                                    if (thing[itemname.item.ToString()] != null)
                                    {
                                        ItemClass item = new ItemClass(int.Parse(thing[itemname.item.ToString()].ToString()),int.Parse(itemname.limit.ToString()),
                                            itemname.label,itemname.item,itemname.type,itemname.usable,itemname.can_remove);
                                        userinv.Add(itemname.item.ToString(),item);
                                    }
                                }
                                usersInventory.Add(user,userinv);
                            }
                            else
                            {
                                usersInventory.Add(user,userinv);
                            }
                            List<WeaponClass> uweapons = new List<WeaponClass>();
                            if (userInventory.loadout != null)
                            {
                                JArray weapons = JArray.Parse(userInventory.loadout);
                                string weaponName = "";
                                foreach (JObject x in weapons.Children())
                                {
                                    weaponName = x.First.First.ToString();
                                    Dictionary<string,int> ammos = new Dictionary<string, int>();
                                    foreach (JProperty ammo in x.Properties())
                                    {
                                        foreach (JProperty type in ammo.First)
                                        {
                                            ammos.Add(type.Name,int.Parse(type.Value.ToString()));
                                        }
                                    }
                                    WeaponClass weapon = new WeaponClass(weaponName,ammos);
                                    uweapons.Add(weapon);
                                }
                                usersWeapons.Add(user,uweapons);
                            }
                            else
                            {
                                usersWeapons.Add(user,uweapons);
                            }
                        }
                    }));
                }
            }));
        }
    }
}