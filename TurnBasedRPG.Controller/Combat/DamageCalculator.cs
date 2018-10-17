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
    /// A static class that handles all the damage calculation for actions and status effects.
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// Given an actor and an action, calculates the maximum non-critical damage that action can deal.
        /// </summary>
        /// <param name="actor">The actor that will perform the action.</param>
        /// <param name="action">The action to check damage for.</param>
        /// <returns>A DamageTypes instance containing the damage the action will do.</returns>
        public static DamageTypes GetDamage(Character actor, ActionBase action)
        {
            int[] damage = new int[6];
            var actionDamage = action.Damage.AsArray();
            // For each different damage type
            for (int i = 0; i < actionDamage.Count(); i++)
            {
                damage[i] = 0;
                bool doAnyStatsModifyDamage = false;
                // Check the action to see if it modifies damage based on a stat
                foreach (var stat in action.DamageStatModifier.ToArray())
                {
                    if (!doAnyStatsModifyDamage && stat.AsArray()[i] > 0) doAnyStatsModifyDamage = true;
                }
                // If the action does more than 0 damage or if the action deals damage based on the actor's stats
                if (action.Damage.AsArray()[i] > 0 || doAnyStatsModifyDamage)
                {
                    damage[i] = action.Damage.AsArray()[i] + actor.DamageModifier.AsArray()[i];
                    // For each damage type bonus provided per stat, multiply it with the actor's current stat
                    for (int j = 0; j < actor.CurrentStats.GetStatTypesAsArray().Count(); j++)
                    {
                        damage[i] += action.DamageStatModifier.ToArray()[j].AsArray()[i]
                                        * actor.CurrentStats.GetStatTypesAsArray()[j];
                    }

                    damage[i] = damage[i] * (actor.DamagePercentageModifier.AsArray()[i] + 100) / 100;
                    damage[i] = damage[i] * (action.DamageMultiplier + 100) / 100;
                    if (action is Spell)
                        damage[i] *= (actor.SpellDamagePercentageModifier + 100) / 100;
                }
            }
            return new DamageTypes(damage);
        }

        /// <summary>
        /// Returns the amount of damage caused by a status effect, modified by a character's stats.
        /// </summary>
        /// <param name="applicator">The character applying the status effect.</param>
        /// <param name="status">The status effect to get the damage of.</param>
        /// <returns></returns>
        public static DamageTypes GetDamage(Character applicator, StatusEffect status)
        {
            int[] damage = new int[6];
            var statusDamage = status.Damage.AsArray();
            // For each different damage type
            for (int i = 0; i < statusDamage.Count(); i++)
            {
                damage[i] = 0;
                bool doAnyStatsModifyDamage = false;
                // Check the status to see if it modifies damage based on a stat
                foreach (var stat in status.DamageStatModifier.ToArray())
                {
                    if (!doAnyStatsModifyDamage && stat.AsArray()[i] > 0) doAnyStatsModifyDamage = true;
                }
                // If the status does more than 0 damage or if the status deals damage based on the applicator's stats
                if (status.Damage.AsArray()[i] > 0 || doAnyStatsModifyDamage)
                {
                    damage[i] = status.Damage.AsArray()[i] + applicator.DamageModifier.AsArray()[i];
                    // For each damage type bonus provided per stat, multiply it with the applicator's current stat
                    for (int j = 0; j < applicator.CurrentStats.GetStatTypesAsArray().Count(); j++)
                    {
                        damage[i] += status.DamageStatModifier.ToArray()[j].AsArray()[i]
                                        * applicator.CurrentStats.GetStatTypesAsArray()[j];
                    }

                    damage[i] *= (applicator.DamagePercentageModifier.AsArray()[i] + 100) / 100;
                    if (status.IsMagical) damage[i] *= (applicator.SpellDamagePercentageModifier + 100) / 100;
                }
            }
            return new DamageTypes(damage);
        }

        /// <summary>
        /// Given an actor and an action, returns the amount of healing that action will do.
        /// </summary>
        /// <param name="actor">The character that will perform the action.</param>
        /// <param name="action">The action to get the healing amount from.</param>
        /// <returns>An int containing the heal amount.</returns>
        public static int GetHealing(Character actor, ActionBase action)
        {
            int totalHealing = action.HealAmount;

            for (int i = 0; i < actor.CurrentStats.GetStatTypesAsArray().Count(); i++)
            {
                totalHealing += action.HealStatModifier.GetStatTypesAsArray()[i]
                                * actor.CurrentStats.GetStatTypesAsArray()[i];
            }

            if (action is Spell)
                totalHealing = (totalHealing + actor.SpellDamageModifier) * (actor.SpellDamagePercentageModifier + 100) / 100;

            return totalHealing;
        }

        /// <summary>
        /// Gets the amount of healing caused by a status effect, modified by a character's stats.
        /// </summary>
        /// <param name="applicator">The character applying the buff.</param>
        /// <param name="status">The status effect to get the healing from.</param>
        /// <returns>The amount of healing from the buff.</returns>
        public static int GetHealing(Character applicator, StatusEffect status)
        {
            int totalHealing = status.HealAmount;

            for (int i = 0; i < applicator.CurrentStats.GetStatTypesAsArray().Count(); i++)
            {
                totalHealing += status.HealStatModifier.GetStatTypesAsArray()[i]
                                * applicator.CurrentStats.GetStatTypesAsArray()[i];
            }

            if (status.IsMagical)
                totalHealing = (totalHealing + applicator.SpellDamageModifier) * (applicator.SpellDamagePercentageModifier + 100) / 100;

            return totalHealing;
        }

        /// <summary>
        /// Given an actor and an action, returns the percentage of heal that action will heal.
        /// </summary>
        /// <param name="actor">The character that will perform the action.</param>
        /// <param name="action">The action to get the healing amount from.</param>
        /// <returns>An int containing the percentage of maximum health an action will heal.</returns>
        public static int GetHealingPercentage(Character actor, ActionBase action)
        {
            int healingPercentage = action.HealAmountPercent;
            for (int i = 0; i < actor.CurrentStats.GetStatTypesAsArray().Count(); i++)
            {
                if (action.StatPerHealPercentage.GetStatTypesAsArray()[i] > 0)
                    healingPercentage += actor.CurrentStats.GetStatTypesAsArray()[i] /
                                action.StatPerHealPercentage.GetStatTypesAsArray()[i];
            }
            return healingPercentage;
        }

        /// <summary>
        /// Gets the percentage of max health a status effect heals, modified by a character's stats.
        /// </summary>
        /// <param name="applicator">The character applying the status effect.</param>
        /// <param name="status">The status effect to get the healing from.</param>
        /// <returns>The percentage of max health this status effect heals.</returns>
        public static int GetHealingPercentage(Character applicator, StatusEffect status)
        {
            var stats = status.StatsPerHealPercentage.GetStatTypesAsArray();
            var charStats = applicator.CurrentStats.GetStatTypesAsArray();
            int totalPercentage = status.HealPercentage;
            for (int i = 0; i < stats.Count(); i++)
            {
                if (stats[i] > 0) totalPercentage += charStats[i] / stats[i];
            }
            return totalPercentage;
        }

        /// <summary>
        /// Gets the total amount of damage a given damage amount will do to a character modified by
        /// the character's stats.
        /// </summary>
        /// <param name="damage">The unmodified amount of damage.</param>
        /// <param name="character">The character whose stats are being calculated against.</param>
        /// <returns>An integer representing the total damage that will be dealt to the character.</returns>
        public static int GetTotalDamage(DamageTypes damageType, Character character)
        {
            int totalDamage = 0;
            int[] damage = damageType.AsArray();
            for (int j = 0; j < damage.Count(); j++)
            {
                // If a specific type of armor is over 100% (meaning heals instead of damages) then target heals for any percentage of the damage
                // over 100%
                if (character.ArmorPercentage.AsArray()[j] > 100 && damage[j] > 0)
                    totalDamage += damage[j] * (100 - character.ArmorPercentage.AsArray()[j]) / 100;
                // If resist all percentage is over 100%, target heals for any percentage of damage over 100%
                else if (character.ResistAllPercentage > 100 && damage[j] > 0)
                    totalDamage += damage[j] * (100 - character.ResistAllPercentage) / 100;
                // If damage is greater than the target's armor, calculate total damage by deducting target's armor values from damage
                else if (damage[j] > character.Armor.AsArray()[j])
                    totalDamage -= (damage[j] - character.Armor.AsArray()[j])
                                    * (100 + character.ArmorPercentage.AsArray()[j]) / 100
                                    * (100 + character.ResistAllPercentage) / 100;
            }
            return totalDamage;
        }
    }
}
