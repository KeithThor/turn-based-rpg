using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.EventArgs
{
    public class EndOfTurnEventArgs : System.EventArgs
    {
        public int EndOfTurnCharacterId { get; set; }
        public int CurrentTurnNumber { get; set; }
    }
}
