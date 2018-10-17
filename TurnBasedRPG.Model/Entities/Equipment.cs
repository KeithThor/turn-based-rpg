using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Enums;

namespace TurnBasedRPG.Model.Entities
{
    public class Equipment : Item
    {
        public EquipmentSlot Slot { get; set; }

        // Resource stats
        public int BonusHealth { get; set; }
        public int BonusMana { get; set; }

        // Primary Stats
        public PrimaryStat Stats { get; } = new PrimaryStat();

        // Damage modifiers
        public DamageTypes DamageModifier { get; } = new DamageTypes();
        public DamageTypes DamagePercentageModifier { get; } = new DamageTypes();
        public int CritChance { get; set; }
        public int CritMultiplier { get; set; }
        public int SpellDamageModifier { get; set; }
        public int SpellDamagePercentageModifier { get; set; }

        public int ThreatMultiplier { get; set; }
        public int Threat { get; set; }

        // Armor modifiers
        public DamageTypes Armor { get; } = new DamageTypes();
        public DamageTypes ArmorPercentage { get; } = new DamageTypes();
        public int ResistAll { get; set; }
        public int ResistAllPercentage { get; set; }

        public List<Spell> SpellList { get; } = new List<Spell>();
        public List<Skill> SkillList { get; } = new List<Skill>();
        public List<StatusEffect> Buffs { get; } = new List<StatusEffect>();
        public List<StatusEffect> Debuffs { get; } = new List<StatusEffect>();
    }
}
