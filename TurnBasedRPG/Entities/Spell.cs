using System.Collections.Generic;
using TurnBasedRPG.Core.Interfaces;

namespace TurnBasedRPG.Entities
{
    public class Spell : ActionBase, IDisplayable, ICategorizable
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

        public string GetDisplayName()
        {
            return Name;
        }
    }
}