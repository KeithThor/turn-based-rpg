using System;

namespace TurnBasedRPG.UI
{
    /// <summary>
    /// Class that wraps around the Console to provide screen buffering when writing to the Console.
    /// </summary>
    public class ScreenBuffer
    {
        private string[] screenBufferArray = new string[Console.WindowHeight - 1];
        private int line = 0;

        /// <summary>
        /// Adds text to the ScreenBuffer to be drawn next time DrawScreen is called.
        /// <para>The max height of the buffer is 1 less than the console window height.</para>
        /// </summary>
        /// <param name="text">The string to add to the screen buffer.</param>
        public void AddToBuffer(string text)
        {
            screenBufferArray[line] = text;
            line++;
            if (line == Console.WindowHeight - 1)
                line = 0;
        }

        /// <summary>
        /// Draws the buffer onto the Console screen and clears the current buffer.
        /// </summary>
        public void DrawScreen()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < screenBufferArray.Length; i++)
            {
                Console.WriteLine(screenBufferArray[i]);
            }
            screenBufferArray = new string[Console.WindowHeight - 1];
            line = 0;
        }
    }
}
