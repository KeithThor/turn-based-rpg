using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core.Interfaces;

namespace TurnBasedRPG.Entities
{
    public class Attack : ActionBase, IDisplayAction
    {
        private string _name;
        public List<int> TargetPositions { get; set; }
        private bool _canTargetThroughUnits;
        private bool _canSwitchTargetPosition;

        public Attack(bool canTargetThroughUnits, bool canSwitchTargetPosition)
        {
            _canTargetThroughUnits = canTargetThroughUnits;
            _canSwitchTargetPosition = canSwitchTargetPosition;
        }

        public List<int> GetActionTargets()
        {
            return TargetPositions;
        }

        public bool CanTargetThroughUnits()
        {
            return _canTargetThroughUnits;
        }

        public bool CanSwitchTargetPosition()
        {
            return _canSwitchTargetPosition;
        }

        public string GetDisplayName()
        {
            // return _name;
            return "Basic Attack";
        }

        public string GetDescription()
        {
            return "This is a basic attack";
        }
    }
}
