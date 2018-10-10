using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Model.Repository.Interfaces;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Handles equipping and unequipping a character.
    /// </summary>
    public class EquipmentController
    {
        private IRepository<Attack> _attackRepo;
        public EquipmentController(IRepository<Attack> attackRepo)
        {
            _attackRepo = attackRepo;
        }

        /// <summary>
        /// Called to add a piece of equipment to a character. If the character has an item currently equipped
        /// in the slot that the new equipment occupies, the currently equipped item is unequipped first.
        /// </summary>
        /// <param name="character">The character to equip.</param>
        /// <param name="equipment">The item to equip on the character.</param>
        public void EquipCharacter(Character character, Equipment equipment)
        {
            var itemInEquipmentSlot = character.EquippedItems[(int)equipment.Slot];
            // Character currently has another item equipped in the same spot as the new equipment
            if (itemInEquipmentSlot != null)
            {
                character.Inventory.Add(itemInEquipmentSlot);
                RemoveEquipmentEffects(character, itemInEquipmentSlot);
            }
            itemInEquipmentSlot = equipment;
            character.Inventory.Remove(equipment);
            AddEquipmentEffects(character, equipment);
        }

        /// <summary>
        /// Removes all equipment effects from a character, effectively nullifying the bonuses and detriments
        /// the item gives to the character.
        /// </summary>
        /// <param name="character">The character to remove the equipment from.</param>
        /// <param name="equipment">The item to remove from the character</param>
        private void RemoveEquipmentEffects(Character character, Equipment equipment)
        {
            // Subtracts all equipment stats from the character
            character.CurrentMaxHealth -= equipment.BonusHealth + StatDefinition.GetHealthFromStamina(equipment.Stats.Stamina);
            character.CurrentHealth -= equipment.BonusHealth + StatDefinition.GetHealthFromStamina(equipment.Stats.Stamina);
            character.CurrentMaxMana -= equipment.BonusMana + StatDefinition.GetManaFromIntellect(equipment.Stats.Intellect);
            character.CurrentMana -= equipment.BonusMana + StatDefinition.GetManaFromIntellect(equipment.Stats.Intellect);
            character.CurrentStats -= equipment.Stats;
            character.Armor -= equipment.Armor;
            character.ArmorPercentage -= equipment.ArmorPercentage;
            character.DamageModifier -= equipment.DamageModifier;
            character.DamagePercentageModifier -= equipment.DamagePercentageModifier;
            character.SpellDamageModifier -= equipment.SpellDamageModifier;
            character.SpellDamagePercentageModifier -= equipment.SpellDamagePercentageModifier;
            character.ResistAll -= equipment.ResistAll;
            character.ResistAllPercentage -= equipment.ResistAllPercentage;

            // Remove any skills, spells, and status effects given from the equipment from the character
            foreach (var spell in equipment.SpellList)
            {
                character.SpellList.Remove(spell);
            }
            foreach (var skill in equipment.SkillList)
            {
                character.SkillList.Remove(skill);
            }
            foreach (var buff in equipment.Buffs)
            {
                character.Buffs.Remove(buff);
            }
            foreach (var debuff in equipment.Debuffs)
            {
                character.Debuffs.Remove(debuff);
            }
        }

        /// <summary>
        /// Adds all equipment effects from a piece of equipment to a character.
        /// </summary>
        /// <param name="character">The character to add the equipment effects to.</param>
        /// <param name="equipment">The item which is being added.</param>
        private void AddEquipmentEffects(Character character, Equipment equipment)
        {
            // Subtracts all equipment stats from the character
            character.CurrentMaxHealth += equipment.BonusHealth + StatDefinition.GetHealthFromStamina(equipment.Stats.Stamina);
            character.CurrentHealth += equipment.BonusHealth + StatDefinition.GetHealthFromStamina(equipment.Stats.Stamina);
            character.CurrentMaxMana += equipment.BonusMana + StatDefinition.GetManaFromIntellect(equipment.Stats.Intellect);
            character.CurrentMana += equipment.BonusMana + StatDefinition.GetManaFromIntellect(equipment.Stats.Intellect);
            character.CurrentStats += equipment.Stats;
            character.Armor += equipment.Armor;
            character.ArmorPercentage += equipment.ArmorPercentage;
            character.DamageModifier += equipment.DamageModifier;
            character.DamagePercentageModifier += equipment.DamagePercentageModifier;
            character.SpellDamageModifier += equipment.SpellDamageModifier;
            character.SpellDamagePercentageModifier += equipment.SpellDamagePercentageModifier;
            character.ResistAll += equipment.ResistAll;
            character.ResistAllPercentage += equipment.ResistAllPercentage;

            // Adds any skills, spells, and status effects from the equipment to the character
            foreach (var spell in equipment.SpellList)
            {
                character.SpellList.Add(spell);
            }
            foreach (var skill in equipment.SkillList)
            {
                character.SkillList.Add(skill);
            }
            foreach (var buff in equipment.Buffs)
            {
                character.Buffs.Add(buff);
            }
            foreach (var debuff in equipment.Debuffs)
            {
                character.Debuffs.Add(debuff);
            }
            AddAttacks(character, equipment as Weapon);
        }

        private void AddAttacks(Character character, Weapon weapon)
        {
            if (weapon == null)
                character.Attacks = new List<Attack>() { _attackRepo.GetAll().First(attack => attack.Id == 1) };
            else
                character.Attacks = weapon.AttackList;
        }
    }
}
