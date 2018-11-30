using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.UI;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.UI.Combat;

namespace TurnBasedRPG.Core
{
    public class Game
    {
        public bool ShutdownTriggered { get; set; }
        private GameUIConstants UIInstance { get; set; }
        private CombatUI _combatUI;

        private List<Character> PlayerCharacters { get; set; }
        private List<Character> EnemyCharacters { get; set; }

        public Game(GameUIConstants uiInstance, CombatUI combatUI)
        {
            ShutdownTriggered = false;
            UIInstance = uiInstance;
            _combatUI = combatUI;
            // For testing purposes; delete afterwards
            //PlayerCharacters = new List<Character>()
            //{
                //    new Character()
                //    {
                //        Id = 1,
                //        Name = "Syla",
                //        Symbol = 'S',
                //        Position = 1,
                //        Speed = 6,
                //        MaxHealth = 232,
                //        CurrentHealth = 141

                //    },
                //    new Character()
                //    {
                //        Id = 2,
                //        Name = "Derrik",
                //        Symbol = 'D',
                //        Position = 3,
                //        Speed = 3,
                //        MaxHealth = 270,
                //        CurrentHealth = 53
                //    },
                //    new Character()
                //    {
                //        Id = 4,
                //        Name = "Marcus",
                //        Symbol = 'M',
                //        Position = 6,
                //        Speed = 1,
                //        MaxHealth = 232,
                //        CurrentHealth = 232,
                //        SpellList = new List<Spell>()
                //        {
                //            new Spell(true, true)
                //            {
                //                Id = 1,
                //                Name = "Frostbolt",
                //                TargetPositions = new List<int>(){ 2, 4, 5, 6, 8 },
                //                Category = "Frost Spells",
                //                CategoryDescription = "Frost spells fall under the category of the Ice god. This message repeats on for 5 more times.Frost spells fall under the category of the Ice god. This message repeats on for 5 more times.Frost spells fall under the category of the Ice god. This message repeats on for 5 more times.Frost spells fall under the category of the Ice god. This message repeats on for 5 more times.Frost spells fall under the category of the Ice god. This message repeats on for 5 more times."
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 2,
                //                Name = "Fireball",
                //                TargetPositions = new List<int>(){ 1, 3, 7, 9 },
                //                Category = "Fire Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 3,
                //                Name = "Shadow Nova",
                //                TargetPositions = new List<int>(){ 4, 5, 6 },
                //                Category = "Shadow Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, false)
                //            {
                //                Id = 4,
                //                Name = "Lightning Bolt",
                //                TargetPositions = new List<int>(){ 5, 10, 11, 12, 13, 14, 15, 16, 17, 18 },
                //                Category = "Lightning Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 1,
                //                Name = "Frostbolt",
                //                TargetPositions = new List<int>(){ 2, 4, 5, 6, 8 },
                //                Category = "Frost Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 2,
                //                Name = "Fireball",
                //                TargetPositions = new List<int>(){ 1, 3, 7, 9 },
                //                Category = "Fire Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 3,
                //                Name = "Shadow Nova",
                //                TargetPositions = new List<int>(){ 4, 5, 6 },
                //                Category = "Shadow Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, false)
                //            {
                //                Id = 4,
                //                Name = "Lightning Bolt",
                //                TargetPositions = new List<int>(){ 5, 10, 11, 12, 13, 14, 15, 16, 17, 18 },
                //                Category = "Lightning Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 1,
                //                Name = "Boulder Lob",
                //                TargetPositions = new List<int>(){ 2, 4, 5, 6, 8 },
                //                Category = "Earth Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 2,
                //                Name = "Shriek",
                //                TargetPositions = new List<int>(){ 1, 3, 7, 9 },
                //                Category = "Shadow Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 3,
                //                Name = "Shadow Nova",
                //                TargetPositions = new List<int>(){ 4, 5, 6 },
                //                Category = "Shadow Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, false)
                //            {
                //                Id = 4,
                //                Name = "Lightning Bolt",
                //                TargetPositions = new List<int>(){ 5, 10, 11, 12, 13, 14, 15, 16, 17, 18 },
                //                Category = "Lightning Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 1,
                //                Name = "Frostbolt",
                //                TargetPositions = new List<int>(){ 2, 4, 5, 6, 8 },
                //                Category = "Frost Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 2,
                //                Name = "Lightning Slash",
                //                TargetPositions = new List<int>(){ 1, 3, 7, 9 },
                //                Category = "Lightning Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, true)
                //            {
                //                Id = 3,
                //                Name = "Death Nova",
                //                TargetPositions = new List<int>(){ 4, 5, 6 },
                //                Category = "Shadow Spells",
                //                CategoryDescription = ""
                //            },
                //            new Spell(true, false)
                //            {
                //                Id = 4,
                //                Name = "Prisma Bolt",
                //                TargetPositions = new List<int>(){ 5, 10, 11, 12, 13, 14, 15, 16, 17, 18 },
                //                Category = "Arcane Spells",
                //                CategoryDescription = ""
                //            }
                //        },
                //        SkillList = new List<Skill>()
                //        {
                //            new Skill(true, true)
                //            {
                //                Id = 1,
                //                Name = "Roundhouse Kick",
                //                Category = "Martial Arts",
                //                CategoryDescription = ""
                //            },
                //            new Skill(true, true)
                //            {
                //                Id = 2,
                //                Name = "Quick Stab",
                //                Category = "Martial Arts",
                //                CategoryDescription = ""
                //            },
                //            new Skill(true, true)
                //            {
                //                Id = 3,
                //                Name = "Mark for Death",
                //                Category = "Assassination",
                //                CategoryDescription = ""
                //            }
                //        }

                //    },
                //    new Character()
                //    {
                //        Id = 6,
                //        Name = "Dana",
                //        Symbol = 'A',
                //        Position = 9,
                //        Speed = 9,
                //        MaxHealth = 432,
                //        CurrentHealth = 90
                //    },
                //    new Character()
                //    {
                //        Id = 9,
                //        Name = "Echo",
                //        Symbol = 'E',
                //        Position = 2,
                //        Speed = 4,
                //        MaxHealth = 232,
                //        CurrentHealth = 0
                //    }
                //};
                //EnemyCharacters = new List<Character>()
                //{
                //    new Character()
                //    {
                //        Id = 10,
                //        Name = "Ghoul",
                //        Symbol = 'G',
                //        Position = 10,
                //        Speed = 6,
                //        MaxHealth = 800,
                //        CurrentHealth = 432
                //    },
                //    new Character()
                //    {
                //        Id = 11,
                //        Name = "Maverick",
                //        Symbol = 'M',
                //        Position = 12,
                //        Speed = 11,
                //        MaxHealth = 632,
                //        CurrentHealth = 230
                //    },
                //    new Character()
                //    {
                //        Id = 12,
                //        Name = "Bat",
                //        Symbol = 'B',
                //        Position = 18,
                //        Speed = 2,
                //        MaxHealth = 432,
                //        CurrentHealth = 123
                //    },
                //    //new Character()
                //    //{
                //    //    Id = 13,
                //    //    Name = "Spearman",
                //    //    Symbol = 'S',
                //    //    Position = 14,
                //    //    Speed = 4,
                //    //    MaxHealth = 565,
                //    //    CurrentHealth = 432
                //    //},
                //    new Character()
                //    {
                //        Id = 14,
                //        Name = "Axeman",
                //        Symbol = 'A',
                //        Position = 16,
                //        Speed = 8,
                //        MaxHealth = 422,
                //        CurrentHealth = 233
                //    }
                //};
            //}
            }

        public void Start()
        {
            StartCombat();
        }

        public void StartCombat()
        {
            _combatUI.StartCombat();
        }
    }
}
