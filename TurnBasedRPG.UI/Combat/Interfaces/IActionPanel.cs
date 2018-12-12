using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    public interface IActionPanel : IReceiveInputPanel
    {
        event EventHandler<ActionChangedEventArgs> ActionChanged;
    }
}
