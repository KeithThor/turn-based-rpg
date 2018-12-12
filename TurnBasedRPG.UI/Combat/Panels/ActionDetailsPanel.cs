using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.Shared.Viewmodel;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    public class ActionDetailsPanel : IPanel
    {
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public bool IsActive
        {
            get { return _defaultsHandler.IsInActionPanel; }
        }

        public ActionDetailsPanel(ViewModelController viewModelController,
                                  DisplayManager displayManager,
                                  DefaultsHandler defaultsHandler)
        {
            MaxWidth = 55;
            MaxHeight = 16;
            _viewModelController = viewModelController;
            _displayManager = displayManager;
            _defaultsHandler = defaultsHandler;
        }

        private int _cachedActionId = 0;
        private ActionData _cachedActionData;
        private IReadOnlyList<string> _cachedRender;
        private readonly ViewModelController _viewModelController;
        private readonly DisplayManager _displayManager;
        private readonly DefaultsHandler _defaultsHandler;

        /// <summary>
        /// Returns a read-only list containing the information panel injected with data from the a
        /// provided action.
        /// </summary>
        /// <param name="action">The action to display in the information panel.</param>
        /// <param name="data">The data of the action to display.</param>
        /// <returns>A read-only list containing the information panel.</returns>
        public IReadOnlyList<string> Render()
        {
            var action = _displayManager.GetActionFromCategory((Commands)_defaultsHandler.CommandFocusNumber,
                                                               _defaultsHandler.ActiveCategory,
                                                               _defaultsHandler.ActionFocusNumber - 1);

            var data = _viewModelController.GetActionViewData((Commands)_defaultsHandler.CommandFocusNumber,
                                                              _defaultsHandler.ActiveCategory,
                                                              _defaultsHandler.ActionFocusNumber - 1);

            if (action == null) return RenderBlankPanel();

            // If data is the same as previous render, use cached render instead of rerendering
            if (action.GetId() == _cachedActionId && data == _cachedActionData)
            {
                return _cachedRender;
            }
            else
            {
                _cachedActionId = action.GetId();
                _cachedActionData = data;
            }

            string navTriangle = "";
            if (data.StatusEffects.Any() && IsActive) navTriangle = "Tab ► ";

            var actionPanel = new List<string>();
            actionPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            int spaces = MaxWidth - 2 - action.GetDisplayName().Count() - navTriangle.Length;
            actionPanel.Add("║ " + action.GetDisplayName() + new string(' ', spaces - 1)+ navTriangle + "║");
            actionPanel.Add("║" + new string('─', MaxWidth - 2) + "║");
            var actionTargetBoxes = RenderActionTargets(action);
            var actionDescription = RenderActionDescription(action, data);
            for (int i = 0; i < actionTargetBoxes.Count(); i++)
            {
                actionPanel.Add(actionDescription.ElementAt(i) + actionTargetBoxes.ElementAt(i));
            }
            int size = actionPanel.Count();
            for (int i = 0; i < MaxHeight - size - 1; i++)
            {
                actionPanel.Add("║" + new string(' ', MaxWidth - 2) + "║");
            }
            actionPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");

            _cachedRender = actionPanel;
            return actionPanel;
        }

        /// <summary>
        /// In case of null objects, renders a panel with no data.
        /// </summary>
        /// <returns>A panel with no data.</returns>
        private List<string> RenderBlankPanel()
        {
            var actionPanel = new List<string>();
            actionPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            for (int i = 0; i < MaxHeight - 2; i++)
            {
                actionPanel.Add("║ " + new string(' ', MaxWidth - 3) + "║");
            }
            actionPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return actionPanel;
        }

        /// <summary>
        /// Renders a small diagram detailing the targets a currently selected action can hit.
        /// </summary>
        /// <param name="action">The action to detail the targets for.</param>
        /// <returns>A list of string containing the action targets panel.</returns>
        private List<string> RenderActionTargets(IDisplayAction action)
        {
            int maxWidth = 25;
            int maxRowsInFormation = 3;
            var actionTargets = new List<string>();
            actionTargets.Add("│" + new string(' ', (maxWidth - 9) / 2) + "Targets" + new string(' ', (maxWidth - 9) / 2) + "║");
            for (int i = 0; i < maxRowsInFormation * 2; i++)
            {
                string line = "";
                for (int j = 1; j <= 6; j++)
                {
                    // If the action can't switch targets and one of its targets is in one of the player's positions, then render a focused square.
                    if (j <= 3 && action.GetActionTargets().Contains(j + i / 2 * 3) && !action.CanSwitchTargetPosition)
                    {
                        if (i % 2 == 1) line += "╚╝";
                        else line += "╔╗";
                    }
                    // If the action can't switch targets and one of its targets is in one of the enemy's positions, then render a focused square.
                    // Or if the action can switch targets, render a focused square if this is one of the target positions
                    else if ((j > 3 && action.GetActionTargets().Contains(j + 6 + i / 2 * 3) && !action.CanSwitchTargetPosition) ||
                        (j > 3 && action.GetActionTargets().Contains(j - 3 + i / 2 * 3)))
                    {
                        if (i % 2 == 1) line += "╚╝";
                        else line += "╔╗";
                    }
                    else
                    {
                        if (i % 2 == 1) line += "└┘";
                        else line += "┌┐";
                    }
                    if (j == 3)
                        line += " ";
                }
                int spaces = (maxWidth - line.Count()) / 2 + (maxWidth - line.Count()) % 2 - 1;
                line = "│ " + new string(' ', spaces - 2) + line + new string(' ', spaces);
                line += line.Count() == maxWidth - 1 ? "║" : " ║";
                actionTargets.Add(line);
            }
            actionTargets.Add("│" + new string(' ', maxWidth - 2) + "║");
            string str1 = "";
            string str2 = "";
            if (action.CanSwitchTargetPosition) str1 = " - Can switch target position";
            else str1 = " - Can't switch target position";
            if (action.CanTargetThroughUnits) str2 = " - Position not affected by formation";
            else str2 = " - Position is affected by formation";
            var arr1 = str1.GetStringAsList(maxWidth - 4);
            var arr2 = str2.GetStringAsList(maxWidth - 4);
            foreach (var item in arr1)
            {
                actionTargets.Add("│ " + item + new string(' ', maxWidth - item.Count() - 4) + " ║");
            }
            foreach (var item in arr2)
            {
                actionTargets.Add("│ " + item + new string(' ', maxWidth - item.Count() - 4) + " ║");
            }
            return actionTargets;
        }

        /// <summary>
        /// Renders the combat stats and description of an action.
        /// </summary>
        /// <param name="action">The action to display.</param>
        /// <param name="data">The data corresponding to the action to display.</param>
        /// <returns>A list of string that contains the action description.</returns>
        private List<string> RenderActionDescription(IDisplayAction action, ActionData data)
        {
            int maxLength = 30;
            int maxHeight = 12;
            var description = new List<string>();
            var reducedStr = action.GetDescription().GetStringAsList(maxLength - 2);

            for (int i = 0; i < reducedStr.Count(); i++)
            {
                string st = "";
                st = reducedStr.ElementAt(i);
                description.Add("║ " + st + new string(' ', maxLength - st.Count() - 2));
            }
            description.Add("║ " + new string(' ', maxLength - 2));
            string str = "";
            // Display all values that are not 0 or null in the view model
            foreach (var item in data.GetDisplayableValues())
            {
                if (item.Value != null)
                {
                    if (item.Value is IEnumerable<string>)
                    {
                        foreach (var stringVal in item.Value as IEnumerable<string>)
                        {
                            str = $"Applies {stringVal}";
                            description.Add("║ " + str + new string(' ', maxLength - str.Count() - 2));
                        }
                    }
                    else
                    {
                        str = $"{item.Key} : {item.Value}";
                        description.Add("║ " + str + new string(' ', maxLength - str.Count() - 2));
                    }
                }
            }
            int currentCount = description.Count();
            // Adds empty lines if the description count is less than the max height.
            for (int i = 0; i < maxHeight - currentCount; i++)
            {
                description.Add("║ " + new string(' ', maxLength - 2));
            }
            return description;
        }
    }
}
