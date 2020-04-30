using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpinventory_sv
{
    public class vorpinventory_sv : BaseScript
    {
        public vorpinventory_sv()
        {
            EventHandlers["vorpinventory:getItemsTable"] += new Action<Player>(getItemsTable);
            EventHandlers["vorpinventory:getInventory"] += new Action<Player>(getInventory);
        }

        private void getItemsTable([FromSource] Player source)
        {
            if (ItemDatabase.items.Count != 0)
            {
                source.TriggerEvent("vorpInventory:giveItemsTable",ItemDatabase.items);
            }
        }

        private void getInventory([FromSource]Player source)
        {
            string steamId = "steam:"+source.Identifiers["steam"];
            Exports["ghmattimysql"].execute("SELECT inventory,loadout FROM characters WHERE identifier = ?;",new [] {steamId} ,new Action<dynamic>((result) =>
            {
                if (result.Count == 0)
                {
                    Debug.WriteLine($"{steamId} doesn`t have inventory yet.");
                }
                else
                {
                    source.TriggerEvent("vorpInventory:giveInventory",result[0]);
                }
            }));
        }
    }
}
