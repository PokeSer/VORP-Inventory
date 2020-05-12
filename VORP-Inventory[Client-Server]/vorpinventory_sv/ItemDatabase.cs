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
                    Exports["ghmattimysql"].execute("SELECT identifier,inventory FROM characters", new Action<dynamic>((uinvento) =>
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
                        }
                    }));
                    Exports["ghmattimysql"].execute("SELECT * FROM loadout", new Action<dynamic>((weaponsinvento) =>
                    {
                        WeaponClass wp;
                        foreach (var row in weaponsinvento)
                        {
                            
                            JObject ammo = Newtonsoft.Json.JsonConvert.DeserializeObject(row.ammo.ToString());
                            JArray comp = Newtonsoft.Json.JsonConvert.DeserializeObject(row.components.ToString());
                            Dictionary<string,int> amunition = new Dictionary<string, int>();
                            List<string>components = new List<string>();
                            foreach (JProperty ammos in ammo.Properties())
                            {
                                //Debug.WriteLine(ammos.Name);
                                amunition.Add(ammos.Name,int.Parse(ammos.Value.ToString()));
                            }
                            foreach (JToken x in comp)
                            { 
                                components.Add(x.ToString());
                            }
                            wp = new WeaponClass(int.Parse(row.id.ToString()),row.name.ToString(),amunition,components);
                            if (usersWeapons.ContainsKey(row.identifier.ToString()))
                            {
                                usersWeapons[row.identifier.ToString()].Add(wp);
                            }
                            else
                            {
                                List<WeaponClass> weap = new List<WeaponClass>();
                                weap.Add(wp);
                                usersWeapons.Add(row.identifier.ToString(),weap);
                            }
                        }
                    }));
                }
            }));
        }
    }
}