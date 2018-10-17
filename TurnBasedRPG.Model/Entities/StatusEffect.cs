using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.Model.Entities
{
    /// <summary>
    /// Represents an in-game buff or debuff.
    /// </summary>
    public class StatusEffect
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public char Symbol { get; set; }

        public int ModifyMaxHealth { get; }
        public int ModifyMaxMana { get; }
        public PrimaryStat ModifyStats { get; } = new PrimaryStat();
        public DamageTypes Damage { get; } = new DamageTypes();
        public DamageStatModifier DamageStatModifier { get; } = new DamageStatModifier();
        public DamageTypes DamageModifier { get; } = new DamageTypes();
        public DamageTypes DamagePercentageModifier { get; } = new DamageTypes();
        public int DamageCritChance { get; set; }
        public int DamageCritMultiplier { get; set; }
        public int CritChance { get; set; }
        public int CritMultiplier { get; set; }
        public int SpellDamageModifier { get; set; }
        public int SpellDamagePercentageModifier { get; set; }
        public int HealAmount { get; set; }
        public PrimaryStat HealStatModifier { get; set; } = new PrimaryStat();
        public int HealPercentage { get; set; }
        public PrimaryStat StatsPerHealPercentage { get; set; } = new PrimaryStat();

        public DamageTypes Armor { get; } = new DamageTypes();
        public DamageTypes ArmorPercentage { get; } = new DamageTypes();
        public int ResistAll { get; }
        public int ResistAllPercentage { get; }
        public int ThreatMultiplier { get; set; }
        public int Threat { get; set; }
        public int Duration { get; }
        public bool Stackable { get; }
        public int StackSize { get; }
        public bool IsDebuff { get; } = false;
        public bool IsPermanent { get; set; } = false;
        public bool IsMagical { get; set; }
    }
}
