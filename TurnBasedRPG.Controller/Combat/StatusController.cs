using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Controller.EventArgs;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Responsible for keeping track of and maintaining status effects.
    /// </summary>
    public class StatusController
    {
        /// <summary>
        /// A wrapper class around a status effect that tracks the amount of turns remaining on a status effect
        /// and the amount of modified damage and healing the status does every turn.
        /// </summary>
        private class AppliedStatus
        {
            public StatusEffect BaseStatus { get; set; }
            public DamageTypes TotalDamage { get; set; }
            public int HealAmount { get; set; }
            public int HealPercentage { get; set; }
            public int CritChance { get; set; }
            public int CritMultiplier { get; set; }
            public int StackCount { get; set; }
            public int TurnsRemaining { get; set; }
        }

        /// <summary>
        /// A wrapper around a status effect that tracks the targets and damage that a status effect should apply
        /// as well as the amount of turns left until the status effect should be applied.
        /// </summary>
        private class DelayedStatus
        {
            public IReadOnlyList<int> Targets { get; set; }
            public StatusEffect BaseStatus { get; set; }
            public DamageTypes TotalDamage { get; set; }
            public int CritChance { get; set; }
            public int CritMultiplier { get; set; }
            public int HealAmount { get; set; }
            public int HealPercentage { get; set; }
            public int SpellDelay { get; set; }
        }

        private Dictionary<Character, List<AppliedStatus>> _appliedStatuses = new Dictionary<Character, List<AppliedStatus>>();
        private Dictionary<Character, List<DelayedStatus>> _delayedStatuses = new Dictionary<Character, List<DelayedStatus>>();
        private Random _random = new Random();
        public IReadOnlyList<Character> AllCharacters { get; set; }

        public event EventHandler<CharactersDiedEventArgs> CharactersDied;

        /// <summary>
        /// Applies a status effect on a character.
        /// </summary>
        /// <param name="applicator">The character that is applying the status effect.</param>
        /// <param name="status">The status effect being applied.</param>
        /// <param name="character">The character the status effect is being applied on.</param>
        public void ApplyStatus(Character applicator, StatusEffect status, Character character)
        {
            if (!_appliedStatuses.ContainsKey(character)) _appliedStatuses[character] = new List<AppliedStatus>();
            // If the the same type of status is already on a character
            if (_appliedStatuses[character].Any(applied => applied.BaseStatus == status))
            {
                var matchingStatus = _appliedStatuses[character].First(applied => applied.BaseStatus == status);
                // If the status is stackable, refresh the duration and apply another layer of effects
                if (status.Stackable && matchingStatus.StackCount < status.StackSize)
                {
                    matchingStatus.TurnsRemaining = status.Duration;
                    matchingStatus.TotalDamage += GetModifiedDamage(applicator, status);
                    matchingStatus.HealAmount += GetHealing(applicator, status);
                    matchingStatus.HealPercentage += GetHealingPercentage(applicator, status);
                    matchingStatus.CritChance = status.CritChance + character.CritChance;
                    matchingStatus.CritMultiplier = status.CritMultiplier + character.CritMultiplier;
                    ApplyStatusEffects(status, character);
                    matchingStatus.StackCount++;
                }
                // If the status is stackable but has reached its stack limit, refresh the duration only
                else if (status.Stackable)
                {
                    matchingStatus.TurnsRemaining = status.Duration;
                }
                // If the status isn't stackable, refresh the duration and reset the damage
                else
                {
                    matchingStatus.TurnsRemaining = status.Duration;
                    matchingStatus.TotalDamage = GetModifiedDamage(applicator, status);
                    matchingStatus.HealAmount = GetHealing(applicator, status);
                    matchingStatus.HealPercentage = GetHealingPercentage(applicator, status);
                    matchingStatus.CritChance = status.CritChance + character.CritChance;
                    matchingStatus.CritMultiplier = status.CritMultiplier + character.CritMultiplier;
                }
            }
            // Create and apply a new status effect on a character
            else
            {
                ApplyStatusEffects(status, character);
                _appliedStatuses[character].Add(CreateAppliedStatus(applicator, status));
                if (status.IsDebuff)
                    character.Debuffs.Add(status);
                else
                    character.Buffs.Add(status);
            }
        }

        /// <summary>
        /// Applies a status effect to a group of characters.
        /// </summary>
        /// <param name="applicator">The character applying the status effect.</param>
        /// <param name="status">The status effect being applied.</param>
        /// <param name="characters">The characters the status effect is being applied to.</param>
        public void ApplyStatus(Character applicator, StatusEffect status, IEnumerable<Character> characters)
        {
            foreach (var character in characters)
            {
                ApplyStatus(applicator, status, character);
            }
        }

        /// <summary>
        /// Applys a delayed status effect to a group of characters.
        /// </summary>
        /// <param name="status">The delayed status to apply.</param>
        private void ApplyStatus(DelayedStatus status)
        {
            var livingTargets = new List<Character>(
                                                    AllCharacters.Where(
                                                    character => status.Targets.Contains(character.Position)));
            livingTargets.RemoveAll(target => target.CurrentHealth <= 0);

            foreach (var target in livingTargets)
            {
                if (!_appliedStatuses.ContainsKey(target)) _appliedStatuses[target] = new List<AppliedStatus>();
                // If the the same type of status is already on a character
                if (_appliedStatuses[target].Any(applied => applied.BaseStatus == status.BaseStatus))
                {
                    var matchingStatus = _appliedStatuses[target].First(applied => applied.BaseStatus == status.BaseStatus);
                    // If the status is stackable, refresh the duration and apply another layer of effects
                    if (status.BaseStatus.Stackable && matchingStatus.StackCount < status.BaseStatus.StackSize)
                    {
                        matchingStatus.TurnsRemaining = status.BaseStatus.Duration;
                        matchingStatus.TotalDamage += status.TotalDamage;
                        matchingStatus.HealAmount += status.HealAmount;
                        matchingStatus.HealPercentage += status.HealPercentage;
                        ApplyStatusEffects(status.BaseStatus, target);
                        matchingStatus.StackCount++;
                    }
                    // If the status is stackable but has reached its stack limit, refresh the duration only
                    else if (status.BaseStatus.Stackable)
                    {
                        matchingStatus.TurnsRemaining = status.BaseStatus.Duration;
                    }
                    // If the status isn't stackable, refresh the duration and reset the damage
                    else
                    {
                        matchingStatus.TurnsRemaining = status.BaseStatus.Duration;
                        matchingStatus.TotalDamage = status.TotalDamage;
                        matchingStatus.HealAmount = status.HealAmount;
                        matchingStatus.HealPercentage = status.HealPercentage;
                    }
                }
                // Create and apply a new status effect on a character
                else
                {
                    ApplyStatusEffects(status.BaseStatus, target);
                    _appliedStatuses[target].Add(CreateAppliedStatus(status));
                    if (status.BaseStatus.IsDebuff)
                        target.Debuffs.Add(status.BaseStatus);
                    else
                        target.Buffs.Add(status.BaseStatus);
                }
            }
        }

        /// <summary>
        /// At the beginning of a character's round, apply healing then damage and decrement durations of buffs
        /// and debuffs. If the duration is 0, remove the buff or debuff from the character. Will invoke the
        /// character died event if a character dies as a result of damage from a status effect.
        /// </summary>
        /// <param name="character">The character whose turn is starting.</param>
        public void StartTurn(Character character)
        {
            if (_appliedStatuses.ContainsKey(character))
            {
                StartOfTurnEffects(character);
            }
            if (_delayedStatuses.ContainsKey(character))
            {
                HandleDelayedStatuses(character);
            }
        }

        /// <summary>
        /// Creates a delayed status instance, which will apply a status effect after a set amount of turns has passed.
        /// </summary>
        /// <param name="applicator">The character from which the spell originated.</param>
        /// <param name="statusBase">The status effect to use as the base for the delayed status.</param>
        /// <param name="target">A list of characters to apply to status effects to.</param>
        /// <param name="spellDelay">How many rounds until the buffs activate.</param>
        public void CreateDelayedStatus(Character applicator, StatusEffect statusBase, IReadOnlyList<int> targets, int spellDelay)
        {
            var delayedStatus = new DelayedStatus()
            {
                BaseStatus = statusBase,
                TotalDamage = GetModifiedDamage(applicator, statusBase),
                HealAmount = GetHealing(applicator, statusBase),
                HealPercentage = GetHealingPercentage(applicator, statusBase),
                SpellDelay = spellDelay,
                CritChance = applicator.CritChance + statusBase.CritChance,
                CritMultiplier = applicator.CritMultiplier + statusBase.CritMultiplier,
                Targets = targets
            };

            if (!_delayedStatuses.ContainsKey(applicator)) _delayedStatuses[applicator] = new List<DelayedStatus>();
            _delayedStatuses[applicator].Add(delayedStatus);
        }

        /// <summary>
        /// Handles the delayed statuses from spells cast by a character, decrementing the spell delay. Once
        /// the spell delay is 0, applies the status to all target characters.
        /// </summary>
        /// <param name="character">The character from which the status originated.</param>
        private void HandleDelayedStatuses(Character character)
        {
            if (_delayedStatuses.ContainsKey(character))
            {
                var removeStatuses = new List<DelayedStatus>();
                foreach (var status in _delayedStatuses[character])
                {
                    status.SpellDelay--;
                    if (status.SpellDelay <= 0)
                    {
                        ApplyStatus(status);
                        removeStatuses.Add(status);
                    }
                }

                _delayedStatuses[character].RemoveAll(status => removeStatuses.Contains(status));
            }
        }

        /// <summary>
        /// Applies damage and healing from status effects affecting a character at the start of its turn.
        /// If there are no turns remaining on the status, the status is removed. If the character dies from
        /// status effect damage, the character died event is invoked.
        /// </summary>
        /// <param name="character">The character starting its turn.</param>
        private void StartOfTurnEffects(Character character)
        {
            var removeStatuses = new List<AppliedStatus>();
            int totalDamage = 0;
            // Calculate damage and apply healing from each status effect
            foreach (var status in _appliedStatuses[character])
            {
                totalDamage += CalculateTotalDamage(status.TotalDamage, character);
                int healAmount = status.HealAmount;

                if (_random.Next(1, 101) <= status.CritChance)
                {
                    totalDamage = totalDamage * status.CritMultiplier / 100;
                    healAmount = healAmount * status.CritMultiplier / 100;
                }

                character.CurrentHealth += healAmount;
                character.CurrentHealth += character.CurrentMaxHealth * status.HealPercentage / 100;
                if (character.CurrentHealth > character.CurrentMaxHealth) character.CurrentHealth = character.CurrentMaxHealth;
                status.TurnsRemaining--;
                if (status.TurnsRemaining == 0)
                    removeStatuses.Add(status);
            }
            character.CurrentHealth += totalDamage;

            // If a status is queued for removal, remove from the character's buff and debuff lists
            foreach (var status in removeStatuses)
            {
                if (status.BaseStatus.IsDebuff)
                    character.Debuffs.Remove(status.BaseStatus);
                else
                    character.Buffs.Remove(status.BaseStatus);
                RemoveStatusEffects(status, character);
            }
            _appliedStatuses[character].RemoveAll(status => removeStatuses.Contains(status));

            // Invoke characters dying event if a character died as a result of this status effect.
            if (character.CurrentHealth <= 0)
            {
                character.CurrentHealth = 0;
                RemoveAllStatuses(character);
                CharactersDied?.Invoke(this, new CharactersDiedEventArgs() { DyingCharacters = new List<Character>() { character } });
            }
        }

        /// <summary>
        /// Removes all status effects from a character.
        /// </summary>
        /// <param name="character">The character to remove all status effects from.</param>
        public void RemoveAllStatuses(Character character, bool removePermanent = false)
        {
            foreach (var status in _appliedStatuses[character])
            {
                // Remove all status effects from dead character
                if (!status.BaseStatus.IsPermanent || removePermanent)
                {
                    RemoveStatusEffects(status, character, removePermanent);
                    if (status.BaseStatus.IsDebuff) character.Debuffs.Remove(status.BaseStatus);
                    else character.Buffs.Remove(status.BaseStatus);
                }
            }
        }

        /// <summary>
        /// Creates and returns an instance of an AppliedStatus.
        /// </summary>
        /// <param name="applicator">The character from which the status originated.</param>
        /// <param name="statusBase">The status which the AppliedStatus is based off.</param>
        /// <returns>A wrapper around a status effect, containing modified values and a turn counter.</returns>
        private AppliedStatus CreateAppliedStatus(Character applicator, StatusEffect statusBase)
        {
            return new AppliedStatus()
            {
                BaseStatus = statusBase,
                TotalDamage = GetModifiedDamage(applicator, statusBase),
                HealAmount = GetHealing(applicator, statusBase),
                HealPercentage = GetHealingPercentage(applicator, statusBase),
                TurnsRemaining = statusBase.Duration,
                CritChance = applicator.CritChance + statusBase.CritChance,
                CritMultiplier = applicator.CritMultiplier + statusBase.CritMultiplier,
                StackCount = 1
            };
        }

        /// <summary>
        /// Given an instance of a DelayedStatus, creates and returns an instance of an AppliedStatus.
        /// </summary>
        /// <param name="status">The DelayedStatus the AppliedStatus should use as a base.</param>
        /// <returns>A wrapper around a status effect, containing modified values and a turn counter.</returns>
        private AppliedStatus CreateAppliedStatus(DelayedStatus status)
        {
            return new AppliedStatus()
            {
                BaseStatus = status.BaseStatus,
                TotalDamage = status.TotalDamage,
                HealAmount = status.HealAmount,
                HealPercentage = status.HealPercentage,
                TurnsRemaining = status.BaseStatus.Duration,
                CritChance = status.CritChance,
                CritMultiplier = status.CritMultiplier,
                StackCount = 1
            };
        }
        
        /// <summary>
        /// Returns the amount of damage caused by a status effect, modified by a character's stats.
        /// </summary>
        /// <param name="applicator">The character applying the status effect.</param>
        /// <param name="status">The status effect to get the damage of.</param>
        /// <returns></returns>
        private DamageTypes GetModifiedDamage(Character applicator, StatusEffect status)
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
        /// Gets the amount of healing caused by a status effect, modified by a character's stats.
        /// </summary>
        /// <param name="applicator">The character applying the buff.</param>
        /// <param name="status">The status effect to get the healing from.</param>
        /// <returns>The amount of healing from the buff.</returns>
        private int GetHealing(Character applicator, StatusEffect status)
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
        /// Gets the percentage of max health a status effect heals, modified by a character's stats.
        /// </summary>
        /// <param name="applicator">The character applying the status effect.</param>
        /// <param name="status">The status effect to get the healing from.</param>
        /// <returns>The percentage of max health this status effect heals.</returns>
        private int GetHealingPercentage(Character applicator, StatusEffect status)
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
        /// Applies the modifications from a status effect to a character.
        /// </summary>
        /// <param name="status">The status effect to apply the modifications from.</param>
        /// <param name="character">The character to apply the modifications to.</param>
        private void ApplyStatusEffects(StatusEffect status, Character character)
        {
            character.Armor += status.Armor;
            character.ArmorPercentage += status.ArmorPercentage;
            character.CurrentMaxHealth += status.ModifyMaxHealth;
            character.CurrentMaxMana += status.ModifyMaxMana;
            character.CurrentStats += status.ModifyStats;
            character.DamageModifier += status.DamageModifier;
            character.DamagePercentageModifier += status.DamagePercentageModifier;
            character.CritChance += status.CritChance;
            character.CritMultiplier += status.CritMultiplier;
            character.ResistAll += status.ResistAll;
            character.ResistAllPercentage += status.ResistAllPercentage;
            character.SpellDamageModifier += status.SpellDamageModifier;
            character.SpellDamagePercentageModifier += status.SpellDamagePercentageModifier;
        }

        /// <summary>
        /// Removes modifications from a status effect from a character. Takes into account how many stacks
        /// have been previously applied to a character.
        /// </summary>
        /// <param name="status">The status effect to remove modifications.</param>
        /// <param name="character">The character to remove modifications from.</param>
        private void RemoveStatusEffects(AppliedStatus status, Character character, bool removePermanent = false)
        {
            if (status.BaseStatus.IsPermanent && !removePermanent) return;

            var baseStatus = status.BaseStatus;
            character.Armor -= baseStatus.Armor * status.StackCount;
            character.ArmorPercentage -= baseStatus.ArmorPercentage * status.StackCount;
            character.CurrentMaxHealth -= baseStatus.ModifyMaxHealth * status.StackCount;
            character.CurrentMaxMana -= baseStatus.ModifyMaxMana * status.StackCount;
            character.CurrentStats -= baseStatus.ModifyStats * status.StackCount;
            character.DamageModifier -= baseStatus.DamageModifier * status.StackCount;
            character.DamagePercentageModifier -= baseStatus.DamagePercentageModifier * status.StackCount;
            character.CritChance -= baseStatus.CritChance * status.StackCount;
            character.CritMultiplier -= baseStatus.CritMultiplier * status.StackCount;
            character.ResistAll -= baseStatus.ResistAll * status.StackCount;
            character.ResistAllPercentage -= baseStatus.ResistAllPercentage * status.StackCount;
            character.SpellDamageModifier -= baseStatus.SpellDamageModifier * status.StackCount;
            character.SpellDamagePercentageModifier -= baseStatus.SpellDamagePercentageModifier * status.StackCount;
        }

        /// <summary>
        /// Calculates the total amount of damage a status effect will do to a character modified by
        /// the character's stats.
        /// </summary>
        /// <param name="statusDamage">The amount of damage a status effect will do.</param>
        /// <param name="character">The character whose stats are being calculated against.</param>
        /// <returns>An integer representing the total damage that will be dealt to the character.</returns>
        private int CalculateTotalDamage(DamageTypes statusDamage, Character character)
        {
            int totalDamage = 0;
            int[] damage = statusDamage.AsArray();
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
