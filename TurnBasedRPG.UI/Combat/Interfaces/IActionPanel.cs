using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Panel responsible for rendering actions for the player to interact with.
    /// </summary>
    public interface IActionPanel : IReceiveInputPanel
    {
    }
}
