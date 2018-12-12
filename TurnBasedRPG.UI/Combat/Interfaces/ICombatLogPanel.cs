using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    public interface ICombatLogPanel : IPanel
    {
        void AddToLog(string logMessage);
    }
}
