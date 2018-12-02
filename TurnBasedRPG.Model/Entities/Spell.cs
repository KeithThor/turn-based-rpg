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
        public Category SpellCategory { get; set; } = new Category();
        public string Category { get => SpellCategory.Name; set => SpellCategory.Name = value; }
        public string CategoryDescription { get => SpellCategory.Description; set => SpellCategory.Description = value; }


        public Spell()
        {
        }

        public int GetId() { return Id; }

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