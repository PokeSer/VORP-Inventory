using System.Collections.Generic;
using CitizenFX.Core;

namespace vorpinventory_sv
{
    public class Utils:BaseScript
    {
        public Utils()
        {
            
        }

        public static Dictionary<string, dynamic> getItemCharacteristics(string item)
        {
            Dictionary<string,dynamic> aux = new Dictionary<string, dynamic>();
            foreach (var it in ItemDatabase.items)
            {
                if (it.item.ToString() == item)
                {
                    aux.Add("name",it.item.ToString());
                    aux.Add("limit",int.Parse(it.limit.ToString()));
                    aux.Add("label",it.label.ToString());
                    aux.Add("can_remove",it.can_remove);
                }
            }
            return aux;
        }
    }
}