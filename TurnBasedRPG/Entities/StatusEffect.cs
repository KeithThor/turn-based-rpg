using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Entities
{
    public class StatusEffect
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public char Symbol { get; set; }

        public int ModifySpeed { get; set; }
        public int ModifyMaxHealth { get; set; }
        public int ModifyCurrentHealth { get; set; }

        public int Duration { get; set; }
        public bool Stackable { get; set; }
        public bool IsDebuff { get; set; }
    }
}
