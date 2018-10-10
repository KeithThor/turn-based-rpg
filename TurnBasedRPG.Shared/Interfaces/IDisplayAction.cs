using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.Interfaces
{
    public interface IDisplayAction
    {
        string GetDisplayName();
        string GetDescription();
        IReadOnlyList<int> GetActionTargets();
        int GetCenterOfTargetsPosition();
        bool CanTargetThroughUnits { get; set; }
        bool CanSwitchTargetPosition { get; set; }
    }
}
