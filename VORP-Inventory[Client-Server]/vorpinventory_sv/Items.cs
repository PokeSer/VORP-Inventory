namespace vorpinventory_sv
{
    public class Items
    {
        private string item;
        private string label;
        private int limit;
        private bool can_remove;

        public Items(string item, string label, int limit, bool can_remove)
        {
            this.item = item;
            this.limit = limit;
            this.can_remove = can_remove;
            this.label = label;
        }

        public string getName()
        {
            return this.item;
        }

        public string getLabel()
        {
            return this.label;
        }

        public int getLimit()
        {
            return this.limit;
        }

        public bool getCanRemove()
        {
            return this.can_remove;
        }
    }
}