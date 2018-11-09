using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Controller.EventArgs
{
    public class AIChoseTargetEventArgs : System.EventArgs
    {
        public IDisplayCharacter AICharacter;
        public int CenterOfTarget;
        public IReadOnlyList<int> TargetPositions;
    }
}
