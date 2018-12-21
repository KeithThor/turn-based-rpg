using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.EventArgs
{
    public class KeyPressedEventArgs: System.EventArgs
    {
        public ConsoleKeyInfo PressedKey { get; set; }
        public bool Handled { get; set; } = false;
    }
}
