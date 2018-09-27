using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core.UI;
using TurnBasedRPG.Entities;

namespace TurnBasedRPG.Core
{
    public class GameUIController
    {
        public const int ScreenWidth = 120;
        public const int ScreenHeight = 51;
        
        public GameUIController()
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.OutputEncoding = System.Text.Encoding.Unicode;
        }
    }
}
