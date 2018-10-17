using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Model.Entities
{
    public class Character : IDisplayCharacter
    {
        public int Id { get; set; }
        public int BaseId { get; set; }
        public string Name { get; set; }
        // The character that represents this unit on the battlefield
        public char Symbol { get; set; }

        // Resource stats
        public int MaxHealth { get; set; }
        // Temporary max health
        public int CurrentMaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxMana { get; set; }
        public int CurrentMaxMana { get; set; }
        public int CurrentMana { get; set; }

        public int CritChance { get; set; }
        public int CritMultiplier { get; set; }

        // Primary Stats
        public PrimaryStat Stats { get; set; }
        public PrimaryStat CurrentStats { get; set; }

        // Damage modifiers
        public DamageTypes DamageModifier { get; set; }
        public DamageTypes DamagePercentageModifier { get; set; }
        public int SpellDamageModifier { get; set; }
        public int SpellDamagePercentageModifier { get; set; }

        // Armor modifiers
        public DamageTypes Armor { get; set; }
        public DamageTypes ArmorPercentage { get; set; }
        public int ResistAll { get; set; }
        public int ResistAllPercentage { get; set; }

        // Which position this character is on the battlefield. Player characters are on position 1-9, enemies on 10-18.
        // 1  2  3
        // 4  5  6
        // 7  8  9
        public int Position { get; set; }

        public List<Attack> Attacks { get; set; } = new List<Attack>();
        public List<Spell> SpellList { get; set; } = new List<Spell>();
        public List<Skill> SkillList { get; set; } = new List<Skill>();
        public Equipment[] EquippedItems;
        public List<Item> Inventory { get; set; }
        public List<StatusEffect> Buffs;
        public List<StatusEffect> Debuffs;

        public Character()
        {
            SpellList = new List<Spell>();
            SkillList = new List<Skill>();
            Buffs = new List<StatusEffect>();
            Debuffs = new List<StatusEffect>();
        }

        // Interface definitions
        public int GetCurrenthealth() => CurrentHealth;
        public int GetMaxHealth() => CurrentMaxHealth;
        public string GetName() => Name;
        public char GetSymbol() => Symbol;
        public int GetPosition() => Position;
        public int GetId() => Id;
    }
}
