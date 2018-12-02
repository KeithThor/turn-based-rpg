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
        public Attack()
        {
        }

        public IReadOnlyList<int> GetActionTargets()
        {
            return TargetPositions;
        }

        public int GetId() { return Id; }

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
