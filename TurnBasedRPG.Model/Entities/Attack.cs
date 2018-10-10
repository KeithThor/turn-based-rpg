using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Model.Entities
{
    public class Attack : ActionBase, IDisplayAction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<int> TargetPositions { get; set; } = new List<int>();
        public int CenterOfTargetsPosition { get; set; } = 5;
        public bool CanTargetThroughUnits { get; set; }
        public bool CanSwitchTargetPosition { get; set; }
        public int DamageModifier { get; set; }
        public int DamagePercentageModifier { get; set; }

        public Attack()
        {
        }

        public IReadOnlyList<int> GetActionTargets()
        {
            return TargetPositions;
        }

        public string GetDisplayName()
        {
            // return _name;
            return Name;
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
