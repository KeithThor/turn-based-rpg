using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI
{
    public class GameUIConstants
    {
        /// <summary>
        /// Controls the width of the game console window.
        /// </summary>
        public readonly int ScreenWidth = (120 > Console.LargestWindowWidth) ? Console.LargestWindowWidth : 120;

        /// <summary>
        /// Controls the height of the game console window.
        /// </summary>
        public readonly int ScreenHeight = (53 > Console.LargestWindowHeight) ? Console.LargestWindowHeight : 53;

        /// <summary>
        /// Controls how many characters can exist in one row of the formation.
        /// </summary>
        public readonly int CharactersPerFormationRow = 3;

        /// <summary>
        /// Controls how many actions can exist in one row of the action panel.
        /// </summary>
        public readonly int ItemsPerActionPanelRow = 2;

        /// <summary>
        /// Controls how many categories can exist in one row of the action panel.
        /// </summary>
        public readonly int ItemsPerCategoryPanelRow = 2;

        public GameUIConstants()
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;
            Console.Title = "Turn Based RPG";
        }
    }
}
