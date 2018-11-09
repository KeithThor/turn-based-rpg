﻿using System;
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
    public class TargetUI
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

        public TargetUI()
        {
            _widthOfHealthBar = _maxWidth - 2;
        }

        /// <summary>
        /// Creates and returns a list of string that represents the targetUI component with a target's
        /// details and a healthbar.
        /// </summary>
        /// <param name="target">The character whose details should be displayed.</param>
        /// <returns>A read-only list of string that represents the targetUI component.</returns>
        public IReadOnlyList<string> RenderTargetDetails(IDisplayCharacter target)
        {
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
            return targetDetails;
        }
    }
}