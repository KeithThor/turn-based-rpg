using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Enums;

namespace TurnBasedRPG.UI.Combat.EventArgs
{
    public class ActionStartedEventArgs : System.EventArgs
    {
        public Commands ActionType { get; set; }
        public string CategoryName { get; set; }
        public int ActionIndex { get; set; }
        public IReadOnlyList<int> TargetPositions { get; set; }
    }
}
