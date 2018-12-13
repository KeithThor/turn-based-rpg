using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Panel responsible for rendering the commands a character can perform.
    /// </summary>
    public interface ICommandPanel : IReceiveInputPanel
    {
        /// <summary>
        /// Event invoked whenever the command focus is changed.
        /// </summary>
        event EventHandler<CommandFocusChangedEventArgs> CommandFocusChanged;
    }
}
