using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.Interfaces
{
    /// <summary>
    /// Represents a readonly version of a character to be used in display.
    /// </summary>
    public interface IDisplayCharacter
    {
        int Id { get; }
        int CurrentHealth { get; }
        int MaxHealth { get; }
        int CurrentMana { get; }
        int MaxMana { get; }
        int Level { get; }
        string Name { get; }
        char Symbol { get; }
        int Position { get; }
        DamageTypes DamageModifier { get; }
        DamageTypes DamagePercentageModifier { get; }
        int SpellDamageModifier { get; }
        int SpellDamagePercentageModifier { get; }
        DamageTypes ArmorPercentage { get; }
        int ResistAll { get; }
        int ResistAllPercentage { get; }
        int CritChance { get; }
        int CritMultiplier { get; }
        int Threat { get; }
        int ThreatMultiplier { get; }

        PrimaryStat CurrentStats { get; }
        DamageTypes Armor { get; }
    }
}
