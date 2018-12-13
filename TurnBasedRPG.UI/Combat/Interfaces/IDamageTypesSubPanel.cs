using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Panel responsible for rendering an amount of elemental stats.
    /// </summary>
    public interface IDamageTypesSubPanel
    {
        /// <summary>
        /// The name of the panel.
        /// </summary>
        string PanelName { get; set; }
        bool IsActive { get; set; }
        int FocusNumber { get; set; }
        int MaxWidth { get; set; }
        int MaxHeight { get; set; }

        /// <summary>
        /// Renders the panel using the amount of elemental stats provided.
        /// </summary>
        /// <param name="damageTypes">The amount of elemental stats a character or item possesses.</param>
        /// <returns>A string containing the rendered panel.</returns>
        IReadOnlyList<string> Render(DamageTypes damageTypes);

        /// <summary>
        /// Handles key press events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnKeyPressed(object sender, KeyPressedEventArgs args);
    }
}
