using System.Collections.Generic;

namespace vorpinventory_sv
{
    public class WeaponClass
    {
        private string name;
        private Dictionary<string, int> ammo;
        public WeaponClass(string name, Dictionary<string,int>ammo)
        {
            this.name = name;
            this.ammo = ammo;
        }

        public string getName()
        {
            return this.name;
        }

        public Dictionary<string, int> getAllAmmo()
        {
            return this.ammo;
        }

        public int getAmmo(string type)
        {
            return this.ammo[type];
        }

        public void addAmmo(int ammo,string type)
        {
            if (this.ammo.ContainsKey(type))
            {
                this.ammo[type] += ammo;
            }
            else
            {
                this.ammo.Add(type,ammo);
            }
        }

        public void subAmmo(int ammo,string type)
        {
            if (this.ammo.ContainsKey(type))
            {
                this.ammo[type] -= ammo;
                if (this.ammo[type] == 0)
                {
                    this.ammo.Remove(type);
                }
            }
        }
        
    }
}