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

                            if (userInventory.loadout != null)
                            {
                                List<WeaponClass> uweapons = new List<WeaponClass>();
                                string name = "";
                                Dictionary<string, int> ammos;
                                //WeaponClass weapon;
                                JArray weapons = JArray.Parse(userInventory.loadout);
                                //Debug.WriteLine(weapons.Count.ToString());
                                foreach (JObject weapon in weapons)
                                {
                                    foreach (JProperty x in weapon.Children<JObject>().Properties())
                                    {
                                        Debug.WriteLine(x.Value.ToString());
                                    }
                                }
                            }

                            //     foreach (JObject content in weapons.Children<JObject>())
                            //     {
                            //         foreach (JProperty prop in content.Properties())
                            //         {
                            //             if (prop.Name == "name")
                            //             {
                            //                 //Debug.WriteLine(prop.Value.ToString());
                            //                 name = prop.Value.ToString();
                            //             }
                            //             ammos = new Dictionary<string, int>();
                            //             foreach (JProperty ammo in prop.Children<JObject>().Properties())
                            //             {
                            //                 ammos.Add(ammo.Name,int.Parse(ammo.Value.ToString()));
                            //                 //Debug.WriteLine(ammo.Name);
                            //                 //Debug.WriteLine(ammo.Value.ToString());
                            //             }
                            //             //Debug.WriteLine(name);
                            //             weapon = new WeaponClass(name,ammos);
                            //             uweapons.Add(weapon);
                            //         }
                            //     }
                            //     Debug.WriteLine(uweapons.Count.ToString());
                            // }
                        }
                    }));
                }
            }));
        }
    }
}