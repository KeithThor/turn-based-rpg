using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.EventArgs
{
    public class CharactersDiedEventArgs : System.EventArgs
    {
        public List<Character> DyingCharacters { get; set; }
    }
}
