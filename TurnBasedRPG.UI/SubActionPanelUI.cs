using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI
{
    public class SubActionPanelUI
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

        public int MaxSubPanelItems
        {
            get { return _maxHeight - 2; }
        }
        public int MaxActionsPerLine { get; set; } = 2;

        public SubActionPanelUI() { }

        // Renders a sub action panel along with sub action names
        public IReadOnlyList<string> Render(IReadOnlyList<string> subActionNames, bool isSubPanelFocused, int focusNumber)
        {
            var subActionPanel = new List<string>();
            
            subActionPanel.Add(new string('═', MaxPanelWidth - 1) + "╗");
            subActionPanel.AddRange(
                RenderAllSubActions(subActionNames, isSubPanelFocused, focusNumber));
            subActionPanel.Add(new string('═', MaxPanelWidth - 1) + "╝");
            return subActionPanel;
        }

        // Renders the max sub actions per line along with a focus triangle if an action is focused by the player
        private List<string> RenderAllSubActions(IReadOnlyList<string> subActionNames, bool isSubPanelFocused, int focusNumber)
        {
            var subActions = new List<string>();
            string subActionLine = "";
            for (int i = 0; i < MaxSubPanelItems; i++)
            {
                bool isFocus = isSubPanelFocused && focusNumber == i + 1;
                if (i < subActionNames.Count)
                    subActionLine += RenderSubActionName(subActionNames[i], isFocus);
                else
                    subActionLine += RenderSubActionName("", false);

                if (i % MaxActionsPerLine == MaxActionsPerLine - 1)
                {
                    subActions.Add(subActionLine + "  ║");
                    // Render a blank line beneath each sub action line
                    subActions.Add(new string(' ', MaxPanelWidth - 1) + "║");
                    subActionLine = "";
                }
            }
            return subActions;
        }

        // Renders a single sub action name along with a focus triangle if it is focused by the player
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
