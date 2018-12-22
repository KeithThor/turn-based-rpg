using System;
using System.Collections.Generic;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Sub panel responsible for rendering character stats.
    /// </summary>
    public interface IStatsSubPanel
    {
        int FocusNumber { get; set; }
        bool IsActive { get; set; }
        int MaxHeight { get; set; }
        int MaxWidth { get; set; }

        /// <summary>
        /// Event called whenever the focus changes on this panel.
        /// </summary>
        event EventHandler<FocusChangedEventArgs> FocusChanged;

        /// <summary>
        /// Handles key press events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnKeyPressed(object sender, KeyPressedEventArgs args);

        /// <summary>
        /// Renders a character's stats.
        /// </summary>
        /// <param name="character">The character to render stats of.</param>
        /// <returns>A list of string containing the render.</returns>
        IReadOnlyList<string> Render(IDisplayCharacter character);
    }
}