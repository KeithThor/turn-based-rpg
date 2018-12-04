using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// UI component responsible for rendering out a combat log panel that displays what happened in combat.
    /// </summary>
    public class CombatLogPanel
    {
        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }

        public CombatLogPanel()
        {
            MaxHeight = 16;
            MaxWidth = 40;
            _combatLog = new List<string>();
            _dataChanged = true;
        }

        private List<string> _combatLog;
        private bool _dataChanged;
        private IReadOnlyList<string> _cachedRender;

        /// <summary>
        /// Renders out the combat log panel with the newest combat log messages that can fit within it's size.
        /// </summary>
        /// <returns>A list of string containing the render.</returns>
        public IReadOnlyList<string> Render()
        {
            var render = new List<string>();
            if (!_dataChanged) return _cachedRender;

            render.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            for (int i = 0; i < MaxHeight - 2; i++)
            {
                // Display the last few messages only, ignore the earliest messages in the case that there are more
                // lines in the combat log than can fit in the panel
                if (_combatLog.Count() > MaxHeight - 2)
                {
                    int index = _combatLog.Count() - MaxHeight + 2 + i;
                    render.Add("║ " + _combatLog[index] + new string(' ', MaxWidth - 4 - _combatLog[index].Length) + " ║");
                }
                else if (i < _combatLog.Count())
                {
                    render.Add("║ " + _combatLog[i] + new string(' ', MaxWidth - 4 - _combatLog[i].Length) + " ║");
                }
                else
                {
                    render.Add("║ " + new string(' ', MaxWidth - 4) + " ║");
                }
            }
            render.Add("╚" + new string('═', MaxWidth - 2) + "╝");

            _dataChanged = false;
            _cachedRender = render;
            return render;
        }

        /// <summary>
        /// Adds a message to the log, to be displayed in future renders.
        /// </summary>
        /// <param name="logMessage">The message to add to the log.</param>
        public void AddToLog(string logMessage)
        {
            var logList = logMessage.GetStringAsList(MaxWidth - 4);
            _combatLog.AddRange(logList);
            _dataChanged = true;
        }
    }
}
