using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Model.Entities
{
    public class Weapon : Equipment
    {
        public List<Attack> AttackList { get; set; } = new List<Attack>();
    }
}
