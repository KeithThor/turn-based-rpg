using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared
{
    public class DamageStatModifier
    {
        public DamageTypes Agility { get; set; } = new DamageTypes();
        public DamageTypes Strength { get; set; } = new DamageTypes();
        public DamageTypes Intellect { get; set; } = new DamageTypes();
        public DamageTypes Speed { get; set; } = new DamageTypes();
        public DamageTypes Stamina { get; set; } = new DamageTypes();

        public DamageTypes[] ToArray()
        {
            return new DamageTypes[] { Agility, Intellect, Speed, Stamina, Strength };
        }
    }
}
