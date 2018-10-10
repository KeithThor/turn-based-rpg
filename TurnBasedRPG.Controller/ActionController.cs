using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Model.Repository.Interfaces;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Handles the activation and effects of a character action.
    /// </summary>
    public class ActionController
    {
        private CharacterController _characterController;
        private StatusController _statusController;
        private IRepository<Spell> _spellRepo;

        public event EventHandler<CharactersDiedEventArgs> CharactersDied;

        private void OnCharactersDying(object sender, CharactersDiedEventArgs args)
        {
            CharactersDied?.Invoke(sender, args);
        }

        /// <summary>
        /// A wrapper around the Spell class that represents a cast spell with delayed effects, keeping track
        /// of the spell's damage and healing at the time of casting.
        /// </summary>
        private class DelayedSpell
        {
            public Spell BaseSpell { get; set; }
            public DamageTypes TotalDamage { get; set; }
            public int HealAmount { get; set; }
            public int HealPercentage { get; set; }
            public int TurnsRemaining { get; set; }
            public IReadOnlyList<Character> Targets { get; set; }
        }

        private Dictionary<Character, List<DelayedSpell>> _delayedSpells = new Dictionary<Character, List<DelayedSpell>>();

        public ActionController(CharacterController characterController, 
                                StatusController statusController,
                                IRepository<Spell> spellRepo)
        {
            _characterController = characterController;
            _statusController = statusController;
            _statusController.CharactersDied += OnCharactersDying;
            _spellRepo = spellRepo;
        }

        /// <summary>
        /// Starts the effect of a spell, applying damage and then status efects.
        /// </summary>
        /// <param name="caster">The character casting the spell.</param>
        /// <param name="spellId">The id of the spell being cast.</param>
        /// <param name="targets">A list of character containing the targets of the spell.</param>
        public void StartSpell(Character caster, int spellId, IReadOnlyList<Character> targets)
        {
            var spell = _spellRepo.GetAll().First(sp => sp.Id == spellId);
            StartSpell(caster, spell, targets);
        }

        /// <summary>
        /// Starts the effect of a spell, applying damage and then status efects.
        /// </summary>
        /// <param name="caster">The character casting the spell.</param>
        /// <param name="spell">The spell being cast.</param>
        /// <param name="targets">A list of character containing the targets of the spell.</param>
        public void StartSpell(Character caster, Spell spell, IReadOnlyList<Character> targets)
        {
            if (spell.Delay > 0)
            {
                CreateDelayedSpells(caster, spell, targets);
                ApplyStatusEffects(caster, spell, targets);
            }
            else
            {
                ApplySpellDamage(caster, spell, targets);
                ApplyStatusEffects(caster, spell, targets);
            }
            CheckForDeadTargets(targets);
        }

        /// <summary>
        /// Creates a DelayedSpell instance, which keeps track of a spell with a delay and its targets.
        /// </summary>
        /// <param name="caster">The character casting the spell.</param>
        /// <param name="spell">The spell being cast.</param>
        /// <param name="targets">The targets the spell is being cast on.</param>
        private void CreateDelayedSpells(Character caster, Spell spell, IReadOnlyList<Character> targets)
        {
            if (!_delayedSpells.ContainsKey(caster)) _delayedSpells[caster] = new List<DelayedSpell>();
            _delayedSpells[caster].Add(new DelayedSpell()
            {
                BaseSpell = spell,
                TotalDamage = GetSpellDamage(caster, spell),
                HealAmount = GetSpellHealing(caster, spell),
                HealPercentage = GetSpellHealingPercentage(caster, spell),
                TurnsRemaining = spell.Delay,
                Targets = targets
            });
        }


        /// <summary>
        /// Starts an attack action, applying damage and then status effects.
        /// </summary>
        /// <param name="attacker">The character performing the attack.</param>
        /// <param name="attack">The attack being performed.</param>
        /// <param name="targets">A list of characters the attack is targeting.</param>
        public void StartAttack(Character attacker, Attack attack, IReadOnlyList<Character> targets)
        {
            ApplyAttackDamage(attacker, attack, targets);
        }

        /// <summary>
        /// Called at the start of a character's turn to check for and start the effect of delayed spells.
        /// </summary>
        /// <param name="character">The character whose turn is starting.</param>
        public void StartTurn(Character character)
        {
            _statusController.StartTurn(character);
            if (_delayedSpells.ContainsKey(character))
            {
                var removeSpells = new List<DelayedSpell>();
                foreach (var spell in _delayedSpells[character])
                {
                    spell.TurnsRemaining--;
                    if (spell.TurnsRemaining <= 0)
                    {
                        removeSpells.Add(spell);
                        ApplySpellDamage(spell);
                        CheckForDeadTargets(spell.Targets);
                    }
                }
            }
        }

        private void CheckForDeadTargets(IReadOnlyList<Character> targets)
        {
            var deadCharacters = targets.Where(target => target.CurrentHealth <= 0);
            if (deadCharacters.Count() > 0)
                CharactersDied?.Invoke(this, new CharactersDiedEventArgs() { DyingCharacters = deadCharacters.ToList() });
        }

        /// <summary>
        /// Applies attack damage to each target.
        /// </summary>
        /// <param name="attacker">The character performing the attack.</param>
        /// <param name="attack">The attack being performed.</param>
        /// <param name="targets">A list of characters being targeted by the attack.</param>
        private void ApplyAttackDamage(Character attacker, Attack attack, IReadOnlyList<Character> targets)
        {
            var damageTypes = GetAttackDamage(attacker, attack);
            int[] damage = damageTypes.GetDamageTypesAsArray();
            for (int i = 0; i < targets.Count(); i++)
            {
                int totalDamage = GetTotalDamage(damageTypes, targets[i]);
                _characterController.ModifyCurrentHealth(targets[i], totalDamage);
            }
        }

        /// <summary>
        /// Starts the application of damage and healing from a spell. In the case of spells with damage and healing,
        /// healing is applied first, then damage.
        /// </summary>
        /// <param name="caster">The character casting the spell.</param>
        /// <param name="spell">The spell being cast.</param>
        /// <param name="targets">The targets of the spell being cast.</param>
        private void ApplySpellDamage(Character caster, Spell spell, IReadOnlyList<Character> targets)
        {
            var damageTypes = GetSpellDamage(caster, spell);
            int[] damage = damageTypes.GetDamageTypesAsArray();

            for (int i = 0; i < targets.Count(); i++)
            {
                // Calculate and apply healing
                int percentHealAmount = targets[i].CurrentMaxHealth * GetSpellHealingPercentage(caster, spell);
                int modifiedHealAmount = GetSpellHealing(caster, spell);
                
                _characterController.ModifyCurrentHealth(targets[i], percentHealAmount);
                _characterController.ModifyCurrentHealth(targets[i], modifiedHealAmount);

                int totalDamage = GetTotalDamage(damageTypes, targets[i]);
                _characterController.ModifyCurrentHealth(targets[i], totalDamage);
            }
        }

        /// <summary>
        /// Applies healing and then damage to targets given an instance of a DelayedSpell.
        /// </summary>
        /// <param name="spell">The DelayedSpell that be used to apply damage and healing.</param>
        private void ApplySpellDamage(DelayedSpell spell)
        {
            for (int i = 0; i < spell.Targets.Count(); i++)
            {
                int totalDamage = GetTotalDamage(spell.TotalDamage, spell.Targets[i]);
                int percentHealAmount = spell.Targets[i].CurrentMaxHealth * spell.HealPercentage;

                _characterController.ModifyCurrentHealth(spell.Targets[i], percentHealAmount);
                _characterController.ModifyCurrentHealth(spell.Targets[i], spell.HealAmount);

                _characterController.ModifyCurrentHealth(spell.Targets[i], totalDamage);
            }
        }

        /// <summary>
        /// Calculates the maximum damage an attacker can deal with an attack.
        /// </summary>
        /// <param name="attacker">The character performing the attack.</param>
        /// <param name="attack">The attack being performed.</param>
        /// <returns>A wrapper around an array of damage types and values.</returns>
        public DamageTypes GetAttackDamage(Character attacker, Attack attack)
        {
            int[] damage = new int[6];
            for (int i = 0; i < damage.Count(); i++)
            {
                damage[i] = 0;
                if (attacker.DamageModifier.GetDamageTypesAsArray()[i] > 0)
                {
                    // Calculates an attack's damage based on modifiers and strength
                    damage[i] = (attacker.DamageModifier.GetDamageTypesAsArray()[i] + attack.DamageModifier)
                                    * (attacker.DamagePercentageModifier.GetDamageTypesAsArray()[i] + 100) / 100
                                    * (attack.DamagePercentageModifier + 100) / 100;
                }
            }
            return new DamageTypes(damage);
        }

        /// <summary>
        /// Calculates the maximum damage a caster can deal with a spell.
        /// </summary>
        /// <param name="caster">The character that is casting the spell.</param>
        /// <param name="spell">The spell being casted.</param>
        /// <returns>A wrapper around an array of damage types and values.</returns>
        public DamageTypes GetSpellDamage(Character caster, Spell spell)
        {
            int[] damage = new int[6];
            for (int i = 0; i < spell.Damage.GetDamageTypesAsArray().Count(); i++)
            {
                damage[i] = 0;
                if (spell.Damage.GetDamageTypesAsArray()[i] > 0 || spell.DamageIntellectModifier.GetDamageTypesAsArray()[i] > 0)
                {
                    // Calculates a spell's damage based on a spell's base damage, it's intellect modifier and the caster's spell modifiers
                    damage[i] = (spell.Damage.GetDamageTypesAsArray()[i] + spell.DamageIntellectModifier.GetDamageTypesAsArray()[i]
                                    * caster.CurrentStats.Intellect + caster.DamageModifier.GetDamageTypesAsArray()[i])
                                    * (caster.DamagePercentageModifier.GetDamageTypesAsArray()[i] + 100) / 100
                                    * (caster.SpellDamagePercentageModifier + 100) / 100;
                }
            }
            return new DamageTypes(damage);
        }

        /// <summary>
        /// Calculates and returns the total flat health a spell will heal.
        /// </summary>
        /// <param name="caster">The character casting the spell.</param>
        /// <param name="spell">The spell to calculate healing for.</param>
        /// <returns>The flat amount of health the spell will heal.</returns>
        private int GetSpellHealing(Character caster, Spell spell)
        {
            int totalhealing = (spell.HealAmount + spell.HealIntellectModifier * caster.CurrentStats.Intellect + caster.SpellDamageModifier) *
                                            (caster.SpellDamagePercentageModifier + 100) / 100;
            return totalhealing;
        }

        /// <summary>
        /// Calculates and returns the total percentage of max health a spell will heal.
        /// </summary>
        /// <param name="caster">The character casting the spell.</param>
        /// <param name="spell">The spell to calculate healing for.</param>
        /// <returns>The percentage out of 100 that the spell will heal from a target's max health.</returns>
        private int GetSpellHealingPercentage(Character caster, Spell spell)
        {
            return (spell.HealAmountPercent + spell.IntellectPerHealPercentage * caster.CurrentStats.Intellect) / 100;
        }

        /// <summary>
        /// Returns the amount of damage that will be deal to a character after considering the
        /// character's stats.
        /// </summary>
        /// <param name="damage">An array of damage</param>
        /// <param name="damagedCharacter"></param>
        /// <returns></returns>
        private int GetTotalDamage(DamageTypes damageTypes, Character damagedCharacter)
        {
            int[] damage = damageTypes.GetDamageTypesAsArray();
            int totalDamage = 0;
            for (int j = 0; j < damage.Count(); j++)
            {
                // If a specific type of armor is over 100% (meaning heals instead of damages) then target heals for any percentage of the damage
                // over 100%
                if (damagedCharacter.ArmorPercentage.GetDamageTypesAsArray()[j] > 100 && damage[j] > 0)
                    totalDamage += damage[j] * (100 - damagedCharacter.ArmorPercentage.GetDamageTypesAsArray()[j]) / 100;
                // If resist all percentage is over 100%, target heals for any percentage of damage over 100%
                else if (damagedCharacter.ResistAllPercentage > 100 && damage[j] > 0)
                    totalDamage += damage[j] * (100 - damagedCharacter.ResistAllPercentage) / 100;
                // If damage is greater than the target's armor, calculate total damage by deducting target's armor values from damage
                else if (damage[j] > damagedCharacter.Armor.GetDamageTypesAsArray()[j])
                    totalDamage -= (damage[j] - damagedCharacter.Armor.GetDamageTypesAsArray()[j])
                                    * (100 - damagedCharacter.ArmorPercentage.GetDamageTypesAsArray()[j]) / 100
                                    * (100 - damagedCharacter.ResistAllPercentage) / 100;
            }
            return totalDamage;
        }

        /// <summary>
        /// Applies status effects to target characters. If the spell has a delay, will apply the status effects
        /// after the delay.
        /// </summary>
        /// <param name="caster">The character applying the status effects.</param>
        /// <param name="spell">The spell which the status effects originate from.</param>
        /// <param name="targets">The targets to apply the status effects to.</param>
        private void ApplyStatusEffects(Character caster, Spell spell, IReadOnlyList<Character> targets)
        {
            var livingTargets = new List<Character>(targets);
            livingTargets.RemoveAll(character => character.CurrentHealth <= 0);

            if (spell.Delay > 0)
            {
                foreach (var status in spell.BuffsToApply)
                {
                    _statusController.CreateDelayedStatus(caster, status, livingTargets, spell.Delay);
                }
            }
            else
            {
                foreach (var status in spell.BuffsToApply)
                {
                    _statusController.ApplyStatus(caster, status, livingTargets);
                }
            }
        }
    }
}
