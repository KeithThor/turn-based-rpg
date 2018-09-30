using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core.Interfaces;

namespace TurnBasedRPG.Entities
{
    public class Character : IDisplayCharacter
    {
        public int Id { get; set; }
        private int _maxHealth;
        // The amount of max health this character has before applying any bonuses from buffs and items
        public int MaxHealth
        {
            get
            {
                return _maxHealth;
            }
            set
            {
                _maxHealth = value;
                CalculateMaxHealth();
            }
        }
        private int _currentMaxHealth;
        // The amount of modified max health this character has
        public int CurrentMaxHealth
        {
            get
            {
                CalculateMaxHealth();
                return _currentMaxHealth;
            }
        }
        private int _currentHealth;
        public int CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
            set
            {
                if (value > CurrentMaxHealth) _currentHealth = CurrentMaxHealth;
                else if (value < 0) _currentHealth = 0;
                else _currentHealth = value;
            }
        }
        public int Speed { get; set; }
        private int _currentSpeed;

        private void CalculateCurrentSpeed()
        {
            int bonus = 0;
            foreach (var buff in Buffs)
            {
                bonus += buff.ModifySpeed;
            }
            foreach (var debuff in Debuffs)
            {
                bonus += debuff.ModifySpeed;
            }
            _currentSpeed = Speed + bonus;
        }
        private void CalculateMaxHealth()
        {
            int bonus = 0;
            foreach (var buff in Buffs)
            {
                bonus += buff.ModifyMaxHealth;
            }
            foreach (var debuff in Debuffs)
            {
                bonus += debuff.ModifyMaxHealth;
            }
            _currentMaxHealth = _maxHealth + bonus;
        }
        public int CurrentSpeed
        {
            get
            {
                CalculateCurrentSpeed();
                return _currentSpeed;
            }
            set
            {
                CalculateCurrentSpeed();
                _currentSpeed += value;
            }
        }

        public string Name { get; set; }
        // The character that represents this unit on the battlefield
        public char Symbol { get; set; }
        // Which position this character is on the battlefield. Player characters are on position 1-9, enemies on 10-18.
        // 1  2  3
        // 4  5  6
        // 7  8  9
        public int Position { get; set; }

        public List<Attack> Attacks { get; set; }

        public bool IsAttackAffectedByFormation()
        {
            return false;
        }

        public List<Spell> SpellList;
        public List<Skill> SkillList;
        public List<StatusEffect> Buffs;
        public List<StatusEffect> Debuffs;

        public Character()
        {
            SpellList = new List<Spell>();
            SkillList = new List<Skill>();
            Buffs = new List<StatusEffect>();
            Debuffs = new List<StatusEffect>();
        }

        public int GetCurrenthealth() => CurrentHealth;

        public int GetMaxHealth() => MaxHealth;

        public string GetName() => Name;

        public char GetSymbol() => Symbol;

        public int GetPosition() => Position;

        public int GetId() => Id;
    }
}
