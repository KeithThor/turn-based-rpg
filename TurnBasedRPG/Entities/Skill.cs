using System.Collections.Generic;
using TurnBasedRPG.Core.Interfaces;

namespace TurnBasedRPG.Entities
{
    public class Skill : ActionBase, IDisplayable, ICategorizable
    {
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