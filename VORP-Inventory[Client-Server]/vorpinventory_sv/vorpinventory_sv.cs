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
            EventHandlers["vorpinventory:getInventory"] += new Action<Player>(getInventory);
        }

        private void getInventory([FromSource]Player source)
        {
            TriggerEvent("vorp:getCharacter", source, new Action<dynamic>((user) =>
            {
                
            }));
        }
    }
}
