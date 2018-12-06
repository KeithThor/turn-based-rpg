using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Enums;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Responsible for rendering the command panel for the UI, containing the commands a player can use such as
    /// Attack, Flee, and Spells.
    /// </summary>
    public class CommandPanel
    {
        private int _maxNumOfActions = 7;
        private int _maxWidth = 17;
        public int MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }
        private int _maxHeight = 16;
        public int MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                _maxHeight = value;
                _maxNumOfActions = (value - 2) / 2;
            }
        }
        private int _maxActionNameLength = 12;
        public int MaxActionNameLength
        {
            get { return _maxActionNameLength; }
            set
            {
                _maxActionNameLength = value;
                if (MaxWidth < value + 5) MaxWidth += 5;
            }
        }

        private int _cachedFocus;
        private IReadOnlyList<string> _cachedRender;

        // Renders the action panel with the names of the actions as well as a focus triangle if an action is focused by the player
        public IReadOnlyList<string> Render(int focusNumber)
        {
            if (_cachedFocus == focusNumber) return _cachedRender;
            else _cachedFocus = focusNumber;

            _cachedRender = RenderActionPanel(focusNumber);
            return _cachedRender;
        }

        private List<string> RenderActionPanel(int focusNumber)
        {
            var actionPanel = new List<string>();
            string actionName = "";
            
            actionPanel.Add("╔" + new string('═', MaxWidth - 1));
            for(int i = 1; i <= _maxNumOfActions; i++)
            {
                string focus = focusNumber == i ? "► " : "  ";
                // Determines the name of the action to display
                switch ((Commands)i)
                {
                    case Commands.Attack:
                        actionName = "Attack";
                        break;
                    case Commands.Spells:
                        actionName = "Spells";
                        break;
                    case Commands.Skills:
                        actionName = "Skills";
                        break;
                    case Commands.Items:
                        actionName = "Items";
                        break;
                    case Commands.Status:
                        actionName = "Status";
                        break;
                    case Commands.Wait:
                        actionName = "Wait";
                        break;
                    case Commands.Run:
                        actionName = "Run";
                        break;
                    default:
                        actionName = "";
                        break;
                }
                int spaces = MaxWidth - actionName.Length - 5;
                actionPanel.Add("║ " + focus + actionName + new string(' ', spaces) + "│");
                actionPanel.Add("║ " + new string(' ', MaxWidth - 3) + "│");
            }
            actionPanel.Add("╚" + new string('═', MaxWidth - 1));
            return actionPanel;
        }
    }
}
