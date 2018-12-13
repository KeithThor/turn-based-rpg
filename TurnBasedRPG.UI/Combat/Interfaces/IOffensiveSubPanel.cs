using System.Collections.Generic;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Sub panel responsible for rendering offensive stats given a character.
    /// </summary>
    public interface IOffensiveSubPanel
    {
        int FocusNumber { get; set; }
        bool IsActive { get; set; }
        int MaxHeight { get; set; }
        int MaxWidth { get; set; }

        /// <summary>
        /// Handles key pressed events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnKeyPressed(object sender, KeyPressedEventArgs args);

        /// <summary>
        /// Renders offensive stats from a character.
        /// </summary>
        /// <param name="character">The character to render offensive stats of.</param>
        /// <returns>A list of string containing the render.</returns>
        IReadOnlyList<string> Render(IDisplayCharacter character);
    }
}