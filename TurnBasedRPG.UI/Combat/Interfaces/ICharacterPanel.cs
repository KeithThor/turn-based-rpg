using System;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Panel responsible for rendering a character's stats and extra information.
    /// </summary>
    public interface ICharacterPanel : IReceiveInputPanel
    {
        /// <summary>
        /// Event called whenever the player presses a key.
        /// </summary>
        event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        /// Event called whenever the Focus for this panel is changed.
        /// </summary>
        event EventHandler<FocusChangedEventArgs> FocusChanged;

        /// <summary>
        /// Event called whenever the Focus for one of this panel's subpanels is changed.
        /// </summary>
        event EventHandler<FocusChangedEventArgs> SubPanelFocusChanged;

        /// <summary>
        /// Event called whenever a subpanel has it's IsActive property toggled.
        /// </summary>
        event EventHandler<ActivenessChangedEventArgs> SubPanelActivenessChanged;
    }
}
