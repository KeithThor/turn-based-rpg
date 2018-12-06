using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Shared
{
    /// <summary>
    /// Represents the UI layer representation of a game character.
    /// </summary>
    public class DisplayCharacter : IDisplayCharacter
    {
        public int Id { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public char Symbol { get; set; }
        public int Position { get; set; }
        public DamageTypes Armor { get; set; }
        public PrimaryStat CurrentStats { get; set; }

        public int HealthChange { get; set; }
        public DamageTypes DamageModifier { get; set; }
        public DamageTypes DamagePercentageModifier { get; set; }
        public int SpellDamageModifier { get; set; }
        public int SpellDamagePercentageModifier { get; set; }
        public DamageTypes ArmorPercentage { get; set; }
        public int ResistAll { get; set; }
        public int ResistAllPercentage { get; set; }
        public int CritChance { get; set; }
        public int CritMultiplier { get; set; }
        public int Threat { get; set; }
        public int ThreatMultiplier { get; set; }

        public int CurrentMana { get; set; }
        public int MaxMana { get; set; }
    }
}
