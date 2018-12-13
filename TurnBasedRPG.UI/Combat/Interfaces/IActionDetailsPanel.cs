using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Panel responsible for rendering details for an action.
    /// </summary>
    public interface IActionDetailsPanel : IPanel
    {
        /// <summary>
        /// Gets whether or not the panel is active.
        /// </summary>
        bool IsActive { get; }
    }
}
