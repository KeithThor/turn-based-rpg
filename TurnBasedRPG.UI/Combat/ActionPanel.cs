using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat
{
    public class ActionPanel
    {
        private int _maxPanelWidth = 0;
        public int MaxPanelWidth
        {
            get
            {
                if (_maxPanelWidth == 0)
                    MaxPanelWidth = 0;
                return _maxPanelWidth;
            }
            set
            {
                if (value < MaxActionNameLength * 2 + 7)
                    _maxPanelWidth = MaxActionNameLength * 2 + 7;
                else
                    _maxPanelWidth = value;
            }
        }
        public int MaxActionNameLength { get; set; } = 20;
        private int _maxHeight = 16;

        public int MaxHeight
        {
            get { return _maxHeight; }
            set { _maxHeight = value; }
        }

        public int MaxActionPanelItems
        {
            get { return _maxHeight - 2; }
        }
        public int MaxActionsPerLine { get; set; } = 2;

        public ActionPanel() { }

        // Renders an action panel along with action names
        public IReadOnlyList<string> Render(IReadOnlyList<string> actionNames, bool isactionPanelFocused, int focusNumber)
        {
            var actionPanel = new List<string>();
            
            actionPanel.Add(new string('═', MaxPanelWidth - 1) + "╗");
            actionPanel.AddRange(
                RenderAllActions(actionNames, isactionPanelFocused, focusNumber));
            actionPanel.Add(new string('═', MaxPanelWidth - 1) + "╝");
            return actionPanel;
        }

        // Renders the max  actions per line along with a focus triangle if an action is focused by the player
        private List<string> RenderAllActions(IReadOnlyList<string> ActionNames, bool isSubPanelFocused, int focusNumber)
        {
            var actions = new List<string>();
            string actionLine = "";
            for (int i = 0; i < MaxActionPanelItems; i++)
            {
                bool isFocus = isSubPanelFocused && focusNumber == i + 1;
                if (i < ActionNames.Count)
                    actionLine += RenderSubActionName(ActionNames[i], isFocus);
                else
                    actionLine += RenderSubActionName("", false);

                if (i % MaxActionsPerLine == MaxActionsPerLine - 1)
                {
                    actions.Add(actionLine + "  ║");
                    // Render a blank line beneath each  action line
                    actions.Add(new string(' ', MaxPanelWidth - 1) + "║");
                    actionLine = "";
                }
            }
            return actions;
        }

        // Renders a single  action name along with a focus triangle if it is focused by the player
        private string RenderSubActionName(string name, bool isFocus)
        {
            string focus = "  ";
            int spaces = MaxActionNameLength - name.Length;
            if (isFocus)
                focus = "► ";
            return focus + name + new string(' ', spaces);
        }
    }
}
