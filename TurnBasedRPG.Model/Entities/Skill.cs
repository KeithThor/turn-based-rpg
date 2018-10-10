using System.Collections.Generic;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Model.Entities
{
    public class Skill : ActionBase, ICategorizable, IDisplayAction
    {
        public string Description { get; set; }
        public int Delay { get; set; }
        public IReadOnlyList<int> TargetPositions { get; } = new List<int>();
        public int CenterOfTargetsPosition { get; set; } = 5;
        public IReadOnlyList<StatusEffect> BuffsToApply { get; } = new List<StatusEffect>();
        public Category SkillCategory { get; set; } = new Category();
        public string Category { get => SkillCategory.Name; set => SkillCategory.Name = value; }
        public string CategoryDescription { get => SkillCategory.Description; set => SkillCategory.Description = value; }
        public bool CanTargetThroughUnits { get; set; }
        public bool CanSwitchTargetPosition { get; set; }

        // Skill stats
        public int HealAmount { get; set; }
        public int HealAmountPercent { get; set; }
        public DamageTypes Damage { get; } = new DamageTypes();

        /// <summary>
        /// How much healing is increased per point of stat
        /// </summary>
        public PrimaryStat HealStatModifier { get; } = new PrimaryStat();

        /// <summary>
        /// How many stats it takes to increase 1% of healing.
        /// </summary>
        public PrimaryStat StatPerHealPercentage { get; } = new PrimaryStat();

        /// <summary>
        /// How much damage is increased per point of stat.
        /// </summary>
        public PrimaryStat DamageStatModifier { get; } = new PrimaryStat();

        public Skill()
        {
        }

        public string GetDisplayName()
        {
            return Name;
        }

        public IReadOnlyList<int> GetActionTargets()
        {
            return TargetPositions;
        }

        public string GetDescription()
        {
            return "Skill description goes here";
        }

        public int GetCenterOfTargetsPosition()
        {
            return CenterOfTargetsPosition;
        }
    }
}