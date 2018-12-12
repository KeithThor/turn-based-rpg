using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for rendering the amount of stats a character has for all stats.
    /// </summary>
    public class StatsSubPanel
    {
        public bool IsActive { get; set; }
        public int FocusNumber { get; set; }
        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }

        public StatsSubPanel()
        {
            MaxHeight = 7;
            MaxWidth = 18;
        }

        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders the stats panel, containing the amount of stats a character has for each stat.
        /// </summary>
        /// <param name="character">The character to use to retrieve the stat amounts.</param>
        /// <returns>A list of string containing the stats panel.</returns>
        public IReadOnlyList<string> Render(IDisplayCharacter character)
        {
            var render = new List<string>();
            
            render.Add(" _Stats_" + new string(' ', MaxWidth - 8));
            render.Add(GetStatDisplay("Strength", character.CurrentStats.Strength.ToString()));
            render.Add(GetStatDisplay("Stamina", character.CurrentStats.Stamina.ToString()));
            render.Add(GetStatDisplay("Intellect", character.CurrentStats.Intellect.ToString()));
            render.Add(GetStatDisplay("Agility", character.CurrentStats.Agility.ToString()));
            render.Add(GetStatDisplay("Speed", character.CurrentStats.Speed.ToString()));
            render.Add(new string(' ', MaxWidth));

            return render;
        }

        /// <summary>
        /// Returns a string containing the display details for a single stat.
        /// </summary>
        /// <param name="statName">The name of the stat to display.</param>
        /// <param name="statAmount">The amount of the stat.</param>
        /// <returns>Contains the display details for a single stat.</returns>
        private string GetStatDisplay(string statName, string statAmount)
        {
            string focus = "";
            if (IsActive) focus = "►";
            int length = statName.Length + statAmount.Length + focus.Length + 2;

            return focus + statName + ": " + statAmount + new string(' ', MaxWidth - length);
        }
    }
}
