using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpinventory_cl
{
    public class ItemClass
    {
        int count;
        int limit;
        string label;
        string name;
        string type;
        bool usable;
        bool canRemove;

        public ItemClass(int count, int limit, string label, string name, string type, bool usable, bool canRemove)
        {
            this.count = count;
            this.limit = limit;
            this.label = label;
            this.name = name;
            this.type = type;
            this.usable = usable;
            this.canRemove = canRemove;
        }
    }
}
