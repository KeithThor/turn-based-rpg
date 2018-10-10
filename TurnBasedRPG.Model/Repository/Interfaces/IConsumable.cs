using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Model.Repository.Interfaces
{
    public interface IConsumable
    {
        Category ConsumableCategory { get; set; }
        int Charges { get; set; }
        bool AreEffectsPermanent { get; set; }
        Spell ItemSpell { get; set; }
    }
}
