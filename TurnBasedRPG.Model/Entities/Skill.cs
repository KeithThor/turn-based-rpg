using System.Collections.Generic;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Model.Entities
{
    public class Skill : ActionBase, ICategorizable, IDisplayAction
    {
        public Category SkillCategory { get; set; } = new Category();
        public string Category { get => SkillCategory.Name; set => SkillCategory.Name = value; }
        public string CategoryDescription { get => SkillCategory.Description; set => SkillCategory.Description = value; }
        
        public Skill()
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