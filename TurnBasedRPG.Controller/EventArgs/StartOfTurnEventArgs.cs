using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.EventArgs
{
    public class StartOfTurnEventArgs : System.EventArgs
    {
        public int CharacterId { get; set; }
        public bool IsPlayerTurn { get; set; }
        public IReadOnlyList<int> CurrentRoundOrderIds { get; set; }
        public IReadOnlyList<int> NextRoundOrderIds { get; set; }
    }
}
