using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace vorpinventory_sv
{
    public class ItemDatabase:BaseScript
    {
        //public static Dictionary<string,Dictionary<string,dynamic>> items = new Dictionary<string, Dictionary<string, dynamic>>();
        public static dynamic items;
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
                    // foreach (dynamic item in result)
                    // {
                    //     Dictionary<string, dynamic> it = new Dictionary<string, dynamic>();
                    //     it.Add("item",item.item.ToString());
                    //     it.Add("label",item.label.ToString());
                    //     it.Add("limit",item.limit);
                    //     it.Add("can_remove",item.can_remove);
                    //     Debug.WriteLine($"Item: {it["item"]} loaded in cache with label: {it["label"]} limit: {it["limit"].ToString()}, " +
                    //                      $"remove: {it["can_remove"].ToString()}");
                    //     items.Add(item.item,it);
                    // }
                }
            }));
        }
    }
}