using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for rendering the amount of armor a character has of all types.
    /// </summary>
    public class DamageTypesSubPanel : IReceiveInputPanel
    {
        public bool IsActive { get; set; }
        public int FocusNumber { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public string PanelName { get; set; }

        public DamageTypesSubPanel()
        {
            MaxWidth = 18;
            MaxHeight = 7;
        }

        public virtual void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders a panel containing the damage type values of a character.
        /// </summary>
        /// <param name="armor">The object containing the damage type values of a character.</param>
        /// <param name="armorPercentage"></param>
        /// <returns>A list of string that contains the panel containing the damage type values of a character.</returns>
        public virtual IReadOnlyList<string> Render(DamageTypes damageTypes)
        {
            var render = new List<string>();

            render.Add($" _{PanelName}_" + new string(' ', MaxWidth - PanelName.Length - 3));
            render.Add(RenderArmor("Physical", damageTypes.Physical.ToString()));
            render.Add(RenderArmor("Fire", damageTypes.Fire.ToString()));
            render.Add(RenderArmor("Frost", damageTypes.Frost.ToString()));
            render.Add(RenderArmor("Lightning", damageTypes.Lightning.ToString()));
            render.Add(RenderArmor("Shadow", damageTypes.Shadow.ToString()));
            render.Add(RenderArmor("Light", damageTypes.Light.ToString()));

            return render;
        }

        /// <summary>
        /// Renders a string containing the amount of a type of damage type a character has.
        /// </summary>
        /// <param name="typeName">The type of damage type to render.</param>
        /// <param name="amount">The amount of damage type a character has of a type.</param>
        /// <returns>A string containing the amount of a type of damage type a character has.</returns>
        protected virtual string RenderArmor(string typeName, string amount)
        {
            string focus = "";
            if (IsActive) focus = "►";
            int length = typeName.Length + amount.Length + focus.Length + 2;

            return focus + typeName + $": {amount}" + new string(' ', MaxWidth - length);
        }
    }
}
