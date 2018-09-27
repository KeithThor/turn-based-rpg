using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core.UI.Enums;

namespace TurnBasedRPG.Core.UI
{
    public class ActionPanelUI
    {
        private const int _maxNumOfActions = 6;
        private const int _maxItemsInSubPanel = 12;
        private const int _maxSizeOfActionPanel = 18;
        private const int _maxActionNameLength = 12;

        // Renders the action panel with the names of the actions as well as a focus triangle if an action is focused by the player
        public List<string> Render(int focusNumber)
        {
            return RenderActionPanel(focusNumber);
        }

        private List<string> RenderActionPanel(int focusNumber)
        {
            var actionPanel = new List<string>();
            string actionName = "";
            
            actionPanel.Add("╔" + new string('═', _maxSizeOfActionPanel - 2));
            for(int i = 1; i <= _maxNumOfActions; i++)
            {
                string focus = focusNumber == i ? "► " : "  ";
                // Determines the name of the action to display
                switch ((Actions)i)
                {
                    case Actions.Attack:
                        actionName = "Attack";
                        break;
                    case Actions.Spells:
                        actionName = "Spells";
                        break;
                    case Actions.Skills:
                        actionName = "Skills";
                        break;
                    case Actions.Items:
                        actionName = "Items";
                        break;
                    case Actions.Pass:
                        actionName = "Pass";
                        break;
                    case Actions.Run:
                        actionName = "Run";
                        break;
                    default:
                        actionName = "";
                        break;
                }
                int spaces = _maxActionNameLength - actionName.Length;
                actionPanel.Add("║ " + focus + actionName + new string(' ', spaces) + "│");
                actionPanel.Add("║ " + new string(' ', _maxSizeOfActionPanel - 4) + "│");
            }
            actionPanel.Add("╚" + new string('═', _maxSizeOfActionPanel - 2));
            return actionPanel;
        }
    }
}
