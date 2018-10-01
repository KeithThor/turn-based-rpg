using System.Collections.Generic;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Model.Entities
{
    public class Skill : ActionBase, ICategorizable, IDisplayAction
    {
        private string _category;
        public string Category { get => _category; set => _category = value; }
        private string _categoryDescription;
        public string CategoryDescription { get => _categoryDescription; set => _categoryDescription = value; }
        public List<int> TargetPositions { get; set; }
        private bool _canTargetThroughUnits;
        private bool _canSwitchTargetPosition;

        public Skill(bool canTargetThroughUnits, bool canSwitchTargetPosition)
        {
            _canTargetThroughUnits = canTargetThroughUnits;
            _canSwitchTargetPosition = canSwitchTargetPosition;
        }

        public string GetDisplayName()
        {
            return Name;
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

        public string GetDescription()
        {
            return "Skill description goes here";
        }
    }
}