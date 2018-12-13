using System;
using System.Collections.Generic;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Sub panel responsible for rendering the offensive stats of the CharacterPanel.
    /// </summary>
    public class OffensiveSubPanel : IOffensiveSubPanel
    {
        public bool IsActive { get; set; }
        public int FocusNumber { get; set; }
        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }

        public OffensiveSubPanel()
        {
            MaxWidth = 18;
        }

        /// <summary>
        /// Handles key press events if this sub panel is active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders a character's offensive stats.
        /// </summary>
        /// <param name="character">The character to render offensive stats for.</param>
        /// <returns>A list of string containing the rendered stats.</returns>
        public IReadOnlyList<string> Render(IDisplayCharacter character)
        {
            var render = new List<string>();

            render.Add(" _Offensive_" + new string(' ', MaxWidth - 12));
            render.Add(RenderStatPercentage("Crit Chance", character.CritChance));
            render.Add(RenderStatPercentage("Crit Damage", character.CritMultiplier));
            render.Add(RenderStat("Extra Threat", character.Threat, true));
            render.Add(RenderStatPercentage("Threat %", character.ThreatMultiplier, true));
            render.Add(RenderStat("Spell Damage", character.SpellDamageModifier, true));
            render.Add(RenderStatPercentage("Spell %", character.SpellDamagePercentageModifier, true));

            return render;
        }

        /// <summary>
        /// Renders a stat name along with its amount and an optional sign.
        /// </summary>
        /// <param name="statName">The name of the stat to render.</param>
        /// <param name="statAmount">The amount of the stat the character has.</param>
        /// <param name="includeSign">Whether or not to include a + sign in the case that the amount is positive.</param>
        /// <returns>A string containing the rendered stat and stat amount as well as the optional sign.</returns>
        private string RenderStat(string statName, int statAmount, bool includeSign = false)
        {
            string sign = "";
            if (includeSign)
            {
                if (statAmount > 0) sign = "+";
                else if (statAmount < 0) sign = "";
            }

            string render = $"{statName}: {sign}{statAmount}";
            render += new string(' ', MaxWidth - render.Length);

            return render;
        }

        /// <summary>
        /// Renders a stat name along with its amount as a percentage and an optional sign.
        /// </summary>
        /// <param name="statName">The name of the stat to render.</param>
        /// <param name="percentAmount">The amount of the stat in percentage that the character has.</param>
        /// <param name="includeSign">Whether or not to include a + sign in the case that the amount is positive.</param>
        /// <returns>A string containing the rendered stat and stat amount as a percentage as well as the optional sign.</returns>
        private string RenderStatPercentage(string statName, int percentAmount, bool includeSign = false)
        {
            string sign = "";
            if (includeSign)
            {
                if (percentAmount > 0) sign = "+";
                else if (percentAmount < 0) sign = "-";
            }

            string render = $"{statName}: {sign}{percentAmount}%";
            render += new string(' ', MaxWidth - render.Length);

            return render;
        }
    }
}
