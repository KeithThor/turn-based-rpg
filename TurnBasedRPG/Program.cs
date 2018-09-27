using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core;

namespace TurnBasedRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            bool shutdownTriggered = false;

            Game game = new Game();

            game.Start();
        }
    }
}
