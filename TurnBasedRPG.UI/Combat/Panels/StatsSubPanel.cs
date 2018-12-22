using System;
using System.Collections.Generic;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for rendering the amount of stats a character has for all stats.
    /// </summary>
    public class StatsSubPanel : IStatsSubPanel
    {
        public bool IsActive { get; set; }
        public int FocusNumber { get; set; }
        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }
        private const int MaxStatTypes = 5;

        public StatsSubPanel()
        {
            MaxHeight = 7;
            MaxWidth = 18;
            FocusNumber = 1;
        }

        /// <summary>
        /// Event called whenever the focus changes on this panel.
        /// </summary>
        public event EventHandler<FocusChangedEventArgs> FocusChanged;

        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (IsActive && !args.Handled)
            {
                switch (args.PressedKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        args.Handled = true;
                        OnUpArrowPressed();
                        break;
                    case ConsoleKey.DownArrow:
                        args.Handled = true;
                        OnDownArrowPressed();
                        break;
                    default:
                        break;
                }
            }
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
            render.Add(GetStatDisplay("Strength", character.CurrentStats.Strength.ToString(), 1));
            render.Add(GetStatDisplay("Stamina", character.CurrentStats.Stamina.ToString(), 2));
            render.Add(GetStatDisplay("Intellect", character.CurrentStats.Intellect.ToString(), 3));
            render.Add(GetStatDisplay("Agility", character.CurrentStats.Agility.ToString(), 4));
            render.Add(GetStatDisplay("Speed", character.CurrentStats.Speed.ToString(), 5));
            render.Add(new string(' ', MaxWidth));

            return render;
        }

        /// <summary>
        /// Returns a string containing the display details for a single stat.
        /// </summary>
        /// <param name="statName">The name of the stat to display.</param>
        /// <param name="statAmount">The amount of the stat.</param>
        /// <returns>Contains the display details for a single stat.</returns>
        private string GetStatDisplay(string statName, string statAmount, int index)
        {
            string focus = "";
            if (IsActive && index == FocusNumber) focus = "►";
            int length = statName.Length + statAmount.Length + focus.Length + 2;

            return focus + statName + ": " + statAmount + new string(' ', MaxWidth - length);
        }

        private void OnUpArrowPressed()
        {
            FocusNumber--;
            if (FocusNumber < 1)
                FocusNumber = MaxStatTypes;

            FocusChanged?.Invoke(this, new FocusChangedEventArgs() { NewFocus = FocusNumber });
        }

        private void OnDownArrowPressed()
        {
            FocusNumber++;
            if (FocusNumber > MaxStatTypes)
                FocusNumber = 1;

            FocusChanged?.Invoke(this, new FocusChangedEventArgs() { NewFocus = FocusNumber });
        }
    }
}
