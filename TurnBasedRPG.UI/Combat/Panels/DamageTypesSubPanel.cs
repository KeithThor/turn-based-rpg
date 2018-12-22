using System;
using System.Collections.Generic;
using TurnBasedRPG.Shared;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for rendering the amount of armor a character has of all types.
    /// </summary>
    public class DamageTypesSubPanel : IDamageTypesSubPanel
    {
        public bool IsActive { get; set; }
        public int FocusNumber { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public string PanelName { get; set; }
        private const int MaxDamageTypes = 6;

        public DamageTypesSubPanel()
        {
            MaxWidth = 18;
            MaxHeight = 7;
            FocusNumber = 1;
        }

        /// <summary>
        /// Event called whenever the focus changes on this panel.
        /// </summary>
        public event EventHandler<FocusChangedEventArgs> FocusChanged;

        public virtual void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (IsActive && !args.Handled)
            {
                switch (args.PressedKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        OnUpArrowPressed();
                        args.Handled = true;
                        break;
                    case ConsoleKey.DownArrow:
                        OnDownArrowPressed();
                        args.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Renders a panel containing the damage type values of a character.
        /// </summary>
        /// <param name="armor">The object containing the damage type values of a character.</param>
        /// <param name="armorPercentage"></param>
        /// <returns>A list of string that contains the panel containing the damage type values of a character.</returns>
        public IReadOnlyList<string> Render(DamageTypes damageTypes)
        {
            var render = new List<string>();

            render.Add($" _{PanelName}_" + new string(' ', MaxWidth - PanelName.Length - 3));
            render.Add(RenderArmor("Physical", damageTypes.Physical.ToString(), 1));
            render.Add(RenderArmor("Fire", damageTypes.Fire.ToString(), 2));
            render.Add(RenderArmor("Frost", damageTypes.Frost.ToString(), 3));
            render.Add(RenderArmor("Lightning", damageTypes.Lightning.ToString(), 4));
            render.Add(RenderArmor("Shadow", damageTypes.Shadow.ToString(), 5));
            render.Add(RenderArmor("Light", damageTypes.Light.ToString(), 6));

            return render;
        }

        /// <summary>
        /// Renders a string containing the amount of a type of damage type a character has.
        /// </summary>
        /// <param name="typeName">The type of damage type to render.</param>
        /// <param name="amount">The amount of damage type a character has of a type.</param>
        /// <returns>A string containing the amount of a type of damage type a character has.</returns>
        private string RenderArmor(string typeName, string amount, int index)
        {
            string focus = "";
            if (IsActive && FocusNumber == index) focus = "►";
            int length = typeName.Length + amount.Length + focus.Length + 2;

            return focus + typeName + $": {amount}" + new string(' ', MaxWidth - length);
        }

        private void OnUpArrowPressed()
        {
            FocusNumber--;
            if (FocusNumber < 1)
                FocusNumber = MaxDamageTypes;

            FocusChanged?.Invoke(this, new FocusChangedEventArgs() { NewFocus = FocusNumber });
        }

        private void OnDownArrowPressed()
        {
            FocusNumber++;
            if (FocusNumber > MaxDamageTypes)
                FocusNumber = 1;

            FocusChanged?.Invoke(this, new FocusChangedEventArgs() { NewFocus = FocusNumber });
        }
    }
}
