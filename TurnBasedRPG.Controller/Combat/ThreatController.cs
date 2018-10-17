using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Controller responsible for calculating and managing threat caused from a character's interactions with another.
    /// </summary>
    public class ThreatController
    {
        /// <summary>
        /// Applies threat caused from an action or status effect to a character. Threat is increased based on the amount of damage
        /// or healing was done as a percentage of a target's maximum health.
        /// </summary>
        /// <param name="character">The character gaining threat.</param>
        /// <param name="target">The target of the character gaining threat.</param>
        /// <param name="modifiedHealth">The amount of health modified from a threatening action.</param>
        /// <param name="actionThreat">The amount of threat caused by an action or status effect.</param>
        /// <param name="actionThreatModifier">The threat multiplier caused by an action or status effect.</param>
        public void ApplyThreat(Character character, Character target, int modifiedHealth, int actionThreat, int actionThreatModifier)
        {
            int flatThreat = modifiedHealth * 100 / target.CurrentMaxHealth + actionThreat;
            int totalMultiplier = actionThreatModifier + character.ThreatMultiplier + 100;

            foreach (var buff in character.Buffs)
            {
                flatThreat += buff.Threat;
                totalMultiplier += buff.ThreatMultiplier;
            }

            foreach (var debuff in character.Debuffs)
            {
                flatThreat += debuff.Threat;
                totalMultiplier += debuff.ThreatMultiplier;
            }

            character.Threat += flatThreat * totalMultiplier / 100;
        }
    }
}
