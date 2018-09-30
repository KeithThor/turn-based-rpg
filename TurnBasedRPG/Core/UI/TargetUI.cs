using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core.Interfaces;

namespace TurnBasedRPG.Core.UI
{
    public class TargetUI
    {
        private int _maxWidth;
        public int MaxWidth
        {
            get { return _maxWidth; }
            set {
                WidthOfHealthBar = value - 2;
                _maxWidth = value;
            }
        }
        private int _widthOfHealthBar;
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

        public TargetUI(int maxWidth)
        {
            MaxWidth = maxWidth;
        }

        // Renders the current target's details and health bar
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
