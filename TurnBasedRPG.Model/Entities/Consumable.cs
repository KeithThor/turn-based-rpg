using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Repository.Interfaces;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Model.Entities
{
    public class Consumable : Item, IConsumable, IDisplayAction
    {
        public Category ConsumableCategory { get; set; }
        public int Charges { get; set; }
        public bool AreEffectsPermanent { get; set; }
        public Spell ItemSpell { get; set; }

        public bool CanTargetThroughUnits
        {
            get { return ItemSpell.CanTargetThroughUnits; }
            set { }
        }
        public bool CanSwitchTargetPosition
        {
            get { return ItemSpell.CanSwitchTargetPosition; }
            set { }
        }

        public IReadOnlyList<int> GetActionTargets()
        {
            return ItemSpell.TargetPositions;
        }

        public int GetCenterOfTargetsPosition()
        {
            return ItemSpell.CenterOfTargetsPosition;
        }

        public string GetDescription()
        {
            return Description;
        }

        public string GetDisplayName()
        {
            return Name;
        }
    }
}
