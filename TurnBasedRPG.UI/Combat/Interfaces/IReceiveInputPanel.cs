using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Represents any UI panels that can receive input.
    /// </summary>
    public interface IReceiveInputPanel : IPanel
    {
        /// <summary>
        /// Whether this panel can receive input.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Keeps track of where the player is within the input panel.
        /// </summary>
        int FocusNumber { get; set; }

        /// <summary>
        /// Handles key press events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnKeyPressed(object sender, KeyPressedEventArgs args);
    }
}