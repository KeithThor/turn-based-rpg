using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Core.Interfaces
{
    public interface IAction
    {
        List<int> GetActionTargets();
        bool IsTargetAffectedByFormation();
        bool IsTargetAbsolutePosition();
    }
}
