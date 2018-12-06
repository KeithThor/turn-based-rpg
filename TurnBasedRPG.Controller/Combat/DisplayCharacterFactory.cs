using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Handles the creation of a DisplayCharacter.
    /// </summary>
    internal class DisplayCharacterFactory
    {
        /// <summary>
        /// Creates a DisplayCharacter using a character as a base.
        /// </summary>
        /// <param name="character">The base to use for the DisplayCharacter.</param>
        /// <returns>A DisplayCharacter representing the UI displayable version of a character.</returns>
        public DisplayCharacter Create(Character character)
        {
            return new DisplayCharacter()
            {
                Id = character.Id,
                Name = character.Name,
                Symbol = character.Symbol,
                CurrentHealth = character.CurrentHealth,
                MaxHealth = character.CurrentMaxHealth,
                Position = character.Position,
                HealthChange = 0,
                Armor = character.Armor,
                ArmorPercentage = character.ArmorPercentage,
                CritChance = character.CritChance,
                CritMultiplier = character.CritMultiplier,
                CurrentMana = character.CurrentMana,
                CurrentStats = character.CurrentStats,
                DamageModifier = character.DamageModifier,
                DamagePercentageModifier = character.DamagePercentageModifier,
                Level = character.Level,
                MaxMana = character.CurrentMaxMana,
                ResistAll = character.ResistAll,
                ResistAllPercentage = character.ResistAllPercentage,
                SpellDamageModifier = character.SpellDamageModifier,
                SpellDamagePercentageModifier = character.SpellDamagePercentageModifier,
                Threat = character.Threat,
                ThreatMultiplier = character.ThreatMultiplier
            };
        }
    }
}
