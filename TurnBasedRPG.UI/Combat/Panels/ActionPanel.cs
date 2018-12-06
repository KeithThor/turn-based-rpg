using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.Panels
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
        public int MaxHeight { get; set; } = 16;

        public int MaxActionPanelItems
        {
            get { return MaxHeight - 2; }
        }
        public int MaxActionsPerLine { get; set; } = 2;

        public ActionPanel() { }

        private IReadOnlyList<string> _cachedNames = new List<string>();
        private bool _cachedFocus;
        private int _cachedFocusNumber;
        private IReadOnlyList<string> _cachedRender;

        // Renders an action panel along with action names
        public IReadOnlyList<string> Render(IReadOnlyList<string> actionNames, bool isActionPanelFocused, int focusNumber)
        {
            // Return the cached render if the parameters are the same as the previous render
            if (isActionPanelFocused == _cachedFocus
                && focusNumber == _cachedFocusNumber
                && actionNames.SequenceEqual(_cachedNames))
            {
                return _cachedRender;
            }
            else
            {
                _cachedNames = actionNames;
                _cachedFocus = isActionPanelFocused;
                _cachedFocusNumber = focusNumber;
            }

            var actionPanel = new List<string>();
            
            actionPanel.Add(new string('═', MaxPanelWidth - 1) + "╗");
            actionPanel.AddRange(
                RenderAllActions(actionNames, isActionPanelFocused, focusNumber));
            actionPanel.Add(new string('═', MaxPanelWidth - 1) + "╝");

            _cachedRender = actionPanel;
            return actionPanel;
        }

        // Renders the max  actions per line along with a focus triangle if an action is focused by the player
        private List<string> RenderAllActions(IReadOnlyList<string> ActionNames, bool isActionPanelFocused, int focusNumber)
        {
            var actions = new List<string>();
            string actionLine = "";
            for (int i = 0; i < MaxActionPanelItems; i++)
            {
                bool isFocus = isActionPanelFocused && focusNumber == i + 1;
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
