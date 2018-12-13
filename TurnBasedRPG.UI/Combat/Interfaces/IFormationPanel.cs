using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Panel responsible for displaying the combat formation of the player and the enemy to the player.
    /// </summary>
    public interface IFormationPanel : IReceiveInputPanel
    {
        /// <summary>
        /// Forces the panel to render focus triangles.
        /// </summary>
        bool RenderFocus { get; set; }
    }
}
