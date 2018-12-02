using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// UI component that is responsible for rendering a target's name and
    /// a health bar that shows a target's current health.
    /// </summary>
    public class TargetPanel
    {
        private int _maxWidth = 42;
        /// <summary>
        /// Gets or sets the max width of the component. MaxWidth will always be 2 units larger than
        /// the width of the healthbar.
        /// </summary>
        public int MaxWidth
        {
            get { return _maxWidth; }
            set {
                WidthOfHealthBar = value - 2;
                _maxWidth = value;
            }
        }
        private int _widthOfHealthBar;
        /// <summary>
        /// Gets or sets the max width of the healthbar. MaxWidth will always be 2 units smaller than
        /// the max width of the component.
        /// </summary>
        public int WidthOfHealthBar
        {
            get { return _widthOfHealthBar; }
            set
            {
                _maxWidth = value + 2;
                _widthOfHealthBar = value;
            }
        }
        public int MaxDetailsLength { get; set; } = 32;

        public TargetPanel()
        {
            _widthOfHealthBar = _maxWidth - 2;
        }

        private IReadOnlyList<string> _cachedRender;
        private CachedData _cachedData;
        private class CachedData
        {
            public int Id;
            public int CurrentHealth;
            public int MaxHealth;
        }

        /// <summary>
        /// Creates and returns a list of string that represents the targetUI component with a target's
        /// details and a healthbar.
        /// </summary>
        /// <param name="target">The character whose details should be displayed.</param>
        /// <returns>A read-only list of string that represents the targetUI component.</returns>
        public IReadOnlyList<string> RenderTargetDetails(IDisplayCharacter target)
        {
            if (IsCacheData(target)) return _cachedRender;
            else
            {
                _cachedData = new CachedData()
                {
                    Id = target.GetId(),
                    CurrentHealth = target.GetCurrenthealth(),
                    MaxHealth = target.GetMaxHealth()
                };
            }

            var targetDetails = new List<string>();
            // Calculate healthbar string
            int healthBars = target.GetCurrenthealth() * 100 / target.GetMaxHealth() * WidthOfHealthBar / 100;
            if (healthBars > WidthOfHealthBar) healthBars = WidthOfHealthBar;
            if (healthBars < 0) healthBars = 0;
            StringBuilder healthString = new StringBuilder("│");
            healthString.Append('█', healthBars);
            healthString.Append(' ', WidthOfHealthBar - healthBars);
            healthString.Append("│");

            // Calculate enemy name and health numbers
            StringBuilder enemyDetails = new StringBuilder("  ");
            enemyDetails.Append(target.GetName() + " (" + target.GetSymbol() + ")");
            enemyDetails.Append(' ', MaxDetailsLength - target.GetName().Length - 
                                        target.GetMaxHealth().ToString().Length - target.GetCurrenthealth().ToString().Length);
            enemyDetails.Append("HP " + target.GetCurrenthealth().ToString() + "/" + target.GetMaxHealth().ToString());
            
            targetDetails.Add(enemyDetails.ToString());
            targetDetails.Add("┌" + new string('─', WidthOfHealthBar) + "┐");
            targetDetails.Add(healthString.ToString());
            targetDetails.Add("└" + new string('─', WidthOfHealthBar) + "┘");

            _cachedRender = targetDetails;
            return targetDetails;
        }

        /// <summary>
        /// Checks whether or not the character passed in contains the same values as the cached data.
        /// </summary>
        /// <param name="character">The character whose values to check.</param>
        /// <returns>Returns whether or not the character contains the same values as the cached data.</returns>
        private bool IsCacheData(IDisplayCharacter character)
        {
            if (_cachedData == null) return false;
            if (character.GetId() != _cachedData.Id) return false;
            if (character.GetCurrenthealth() != _cachedData.CurrentHealth) return false;
            if (character.GetMaxHealth() != _cachedData.MaxHealth) return false;

            return true;
        }
    }
}
