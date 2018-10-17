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
    /// Responsible for instantiating new character objects from characterBase objects.
    /// </summary>
    public class CharacterFactory
    {
        private IRepository<CharacterBase> _characterRepo;
        private IRepository<Spell> _spellRepo;
        private IRepository<Item> _itemRepo;
        private IRepository<Attack> _attackRepo;
        private EquipmentController _equipmentController;
        private static int _currentId = 1;
        public CharacterFactory(IRepository<CharacterBase> characterRepo, 
                                IRepository<Spell> spellRepo, 
                                EquipmentController equipmentController,
                                IRepository<Item> itemRepo,
                                IRepository<Attack> attackRepo)
        {
            _characterRepo = characterRepo;
            _spellRepo = spellRepo;
            _equipmentController = equipmentController;
            _itemRepo = itemRepo;
            _attackRepo = attackRepo;
        }

        /// <summary>
        /// Creates a new character using the character template of the provided Id.
        /// </summary>
        /// <param name="characterId">Id of the character template to use to create this character.</param>
        /// <returns></returns>
        public Character Create(int characterId)
        {
            var character = new Character();
            var cBase = _characterRepo.GetAll().First(chr => chr.Id == characterId);

            character.Id = _currentId;
            _currentId++;
            character.BaseId = cBase.Id;
            character.Name = cBase.Name;
            character.Symbol = cBase.Symbol;
            character.MaxHealth = cBase.BonusHealth + StatDefinition.GetHealthFromStamina(cBase.Stats.Stamina);
            character.CurrentMaxHealth = character.MaxHealth;
            character.CurrentHealth = character.CurrentMaxHealth;
            character.MaxMana = cBase.BonusMana + StatDefinition.GetManaFromIntellect(cBase.Stats.Intellect);
            character.CurrentMaxMana = character.MaxMana;
            character.CurrentMana = character.CurrentMaxMana;
            character.Stats = new PrimaryStat(cBase.Stats);
            character.CurrentStats = new PrimaryStat(character.Stats);
            character.DamageModifier = new DamageTypes(cBase.DamageModifier);
            character.DamageModifier.Physical += StatDefinition.GetDamageFromStrength(character.CurrentStats.Strength);
            character.CritChance = cBase.CritChance;
            character.CritMultiplier = cBase.CritMultiplier;
            character.DamagePercentageModifier = new DamageTypes(cBase.DamagePercentageModifier);
            character.Armor = new DamageTypes(cBase.Armor);
            character.ArmorPercentage = new DamageTypes(cBase.ArmorPercentage);
            character.ThreatMultiplier = cBase.ThreatMultiplier;
            // Add spell references based on spell ids contained in the character base
            character.SpellList = _spellRepo.GetAll().Where(spell => cBase.SpellIdList.Contains(spell.Id)).ToList();
            character.Inventory = _itemRepo.GetAll().Where(item => cBase.ItemIdList.Contains(item.Id)).ToList();
            // Equip the character with each item in the equipment list
            foreach (var item in _itemRepo.GetAll().Where(i => cBase.EquipmentIdList.Contains(i.Id)))
            {
                character.Inventory.Add(item);
                _equipmentController.EquipCharacter(character, item as Equipment);
            }
            if (character.Attacks.Count == 0)
            {
                var basicAttack = _attackRepo.GetAll().First(attack => attack.Id == 1);
                character.Attacks = new List<Attack>() { basicAttack };
            }
                
            return character;
        }
    }
}
