using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.UI.Combat
{
    public class CombatLogPanel
    {
        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }

        public CombatLogPanel()
        {
            MaxHeight = 16;
            MaxWidth = 40;
            _combatLog = new List<string>();
        }

        private List<string> _combatLog;

        public IReadOnlyList<string> Render()
        {
            var render = new List<string>();

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
            return render;
        }

        public void AddToLog(string logMessage)
        {
            var logList = logMessage.GetStringAsList(MaxWidth - 4);
            _combatLog.AddRange(logList);
        }
    }
}
