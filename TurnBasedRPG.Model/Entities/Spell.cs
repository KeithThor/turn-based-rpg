using System.Collections.Generic;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Model.Entities
{
    /// <summary>
    /// Represents a single in-game spell.
    /// </summary>
    public class Spell : ActionBase, ICategorizable, IDisplayAction
    {
        public string Description { get; set; }

        /// <summary>
        /// How many rounds does it take for the spell to take effect.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// Which positions does this spell target on the battlefield.
        /// </summary>
        public IReadOnlyList<int> TargetPositions { get; } = new List<int>();

        /// <summary>
        /// Which position should this spell be aimed from.
        /// </summary>
        public int CenterOfTargetsPosition { get; set; } = 5;

        public List<StatusEffect> BuffsToApply { get; set; } = new List<StatusEffect>();
        public Category SpellCategory { get; set; } = new Category();
        public string Category { get => SpellCategory.Name; set => SpellCategory.Name = value; }
        public string CategoryDescription { get => SpellCategory.Description; set => SpellCategory.Description = value; }

        /// <summary>
        /// Can the user bypass targets that are blocking targets in the back of the formation.
        /// </summary>
        public bool CanTargetThroughUnits { get; set; }
        public bool CanSwitchTargetPosition { get; set; }

        // Spell stats
        public int HealAmount { get; set; }

        /// <summary>
        /// How much of the target's total health is healed from this spell.
        /// </summary>
        public int HealAmountPercent { get; set; }
        public DamageTypes Damage { get; } = new DamageTypes();

        /// <summary>
        /// How much additional healing each point of intellect increases.
        /// </summary>
        public int HealIntellectModifier { get; set; }

        /// <summary>
        /// How many points of intellect increase healing by 1%.
        /// </summary>
        public int IntellectPerHealPercentage { get; set; }

        /// <summary>
        /// How much of each type of damage is increased per point of intellect.
        /// </summary>
        public DamageTypes DamageIntellectModifier { get; } = new DamageTypes();


        public Spell()
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
            return Description;
        }

        public int GetCenterOfTargetsPosition()
        {
            return CenterOfTargetsPosition;
        }
    }
}