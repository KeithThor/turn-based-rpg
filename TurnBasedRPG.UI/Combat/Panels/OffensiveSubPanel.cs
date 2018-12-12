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
    public class OffensiveSubPanel
    {
        public bool IsActive { get; set; }
        public int FocusNumber { get; set; }
        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }

        public OffensiveSubPanel()
        {
            MaxWidth = 18;
        }

        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            throw new NotImplementedException();
        }

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

        private string RenderStat(string statName, int statAmount, bool includeSign = false)
        {
            string sign = "";
            if (includeSign)
            {
                if (statAmount > 0) sign = "+";
                else if (statAmount < 0) sign = "-";
            }

            string render = $"{statName}: {sign}{statAmount}";
            render += new string(' ', MaxWidth - render.Length);

            return render;
        }

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
