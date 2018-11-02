using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.Model.Entities
{
    /// <summary>
    /// The base for all combat actions performable by a character.
    /// </summary>
    public class ActionBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AiWeight { get; set; }

        public bool CanTargetThroughUnits { get; set; }
        public bool CanSwitchTargetPosition { get; set; }
        public IReadOnlyList<int> TargetPositions { get; } = new List<int>();
        public int CenterOfTargetsPosition { get; set; } = 5;
        public string Description { get; set; }

        public int HealAmount { get; set; }
        public int HealAmountPercent { get; set; }
        public DamageTypes Damage { get; } = new DamageTypes();
        public int DamageMultiplier { get; set; }
        public int CritChance { get; set; }
        public int CritMultiplier { get; set; }
        public PrimaryStat HealStatModifier { get; } = new PrimaryStat();
        public PrimaryStat StatPerHealPercentage { get; } = new PrimaryStat();
        public DamageStatModifier DamageStatModifier { get; } = new DamageStatModifier();
        public int Cooldown { get; set; }
        public int Delay { get; set; }
        public int Threat { get; set; }
        public int ThreatMultiplier { get; set; }

        public List<StatusEffect> BuffsToApply { get; set; } = new List<StatusEffect>();
    }
}
