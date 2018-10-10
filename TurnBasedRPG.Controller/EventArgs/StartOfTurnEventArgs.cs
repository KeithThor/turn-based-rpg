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
        public Character StartOfTurnCharacter { get; set; }
    }
}
