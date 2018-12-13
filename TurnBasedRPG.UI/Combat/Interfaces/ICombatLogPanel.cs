using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Panel responsible for logging important combat information to the player.
    /// </summary>
    public interface ICombatLogPanel : IPanel
    {
        /// <summary>
        /// Adds a message to the log panel.
        /// </summary>
        /// <param name="logMessage">The message to add.</param>
        void AddToLog(string logMessage);
    }
}
