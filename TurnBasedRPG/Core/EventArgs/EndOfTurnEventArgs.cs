using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Entities;

namespace TurnBasedRPG.Core.EventArgs
{
    public class EndOfTurnEventArgs : System.EventArgs
    {
        public Character EndOfTurnCharacter { get; set; }
        public int CurrentTurnNumber { get; set; }
    }
}
