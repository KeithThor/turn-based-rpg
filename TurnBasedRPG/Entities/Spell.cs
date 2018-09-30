using System.Collections.Generic;
using TurnBasedRPG.Core.Interfaces;

namespace TurnBasedRPG.Entities
{
    public class Spell : ActionBase, ICategorizable, IDisplayAction
    {
        public int SpellDelay { get; set; }
        public int ModifySpeed { get; set; }
        public int ModifyHealthMax { get; set; }
        public int ModifyHealthCurrent { get; set; }
        public List<StatusEffect> BuffsToApply;
        private string _category;
        public string Category { get => _category; set => _category = value; }
        private string _categoryDescription;
        public string CategoryDescription { get => _categoryDescription; set => _categoryDescription = value; }
        public List<int> TargetPositions { get; set; }
        private bool _canTargetThroughUnits;
        private bool _canSwitchTargetPosition;

        public Spell(bool canTargetThroughUnits, bool canSwitchTargetPosition)
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
            return "Spell description goes here.Spell description goes here.";
        }
    }
}