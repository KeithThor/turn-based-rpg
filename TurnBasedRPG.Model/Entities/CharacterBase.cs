using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.Model.Entities
{
    /// <summary>
    /// Contains data for the template of a character. Used by other classes to create
    /// instances of a character for use by the game.
    /// </summary>
    public class CharacterBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // The character that represents this unit on the battlefield
        public char Symbol { get; set; }

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

        // Armor modifiers
        public DamageTypes Armor { get; } = new DamageTypes();
        public DamageTypes ArmorPercentage { get; } = new DamageTypes();
        public int ResistAll { get; set; }
        public int ResistAllPercentage { get; set; }

        // Skills, spells and items
        public List<int> SpellIdList { get; } = new List<int>();
        public List<int> EquipmentIdList { get; } = new List<int>();
        public List<int> ItemIdList { get; } = new List<int>();
        public List<int> SkillIdList { get; } = new List<int>();
    }
}
