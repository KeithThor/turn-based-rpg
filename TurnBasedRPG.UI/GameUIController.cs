using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI
{
    public class GameUIController
    {
        private readonly int _screenWidth = (120 > Console.LargestWindowWidth) ? Console.LargestWindowWidth : 120;
        private readonly int _screenHeight = (53 > Console.LargestWindowHeight) ? Console.LargestWindowHeight : 53;

        public GameUIController()
        {
            Console.SetWindowSize(_screenWidth, _screenHeight);
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;
            Console.Title = "Turn Based RPG";
        }
    }
}
