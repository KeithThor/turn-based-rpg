using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Core.Interfaces
{
    public interface IDisplayAction
    {
        string GetDisplayName();
        string GetDescription();
        List<int> GetActionTargets();
        bool CanTargetThroughUnits();
        bool CanSwitchTargetPosition();
    }
}
