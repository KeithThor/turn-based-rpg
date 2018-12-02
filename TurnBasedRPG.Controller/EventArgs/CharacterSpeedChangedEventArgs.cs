using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.EventArgs
{
    public class CharacterSpeedChangedEventArgs : System.EventArgs
    {
        public int CharacterId { get; set; }
        public int PreSpeedChange { get; set; }
        public int SpeedChange { get; set; }
    }
}
