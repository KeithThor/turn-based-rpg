using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.Shared.Viewmodel;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// A UI component responsible for rendering more information about a selected action, character, or category.
    /// </summary>
    public class InformationPanelUI
    {
        private int _maxWidth;
        public int MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }
        private int _maxHeight;

        public int MaxHeight
        {
            get { return _maxHeight; }
            set { _maxHeight = value; }
        }

        public InformationPanelUI()
        {
            MaxWidth = 55;
            MaxHeight = 16;
        }

        /// <summary>
        /// Returns an information panel injected with the data from a character.
        /// </summary>
        /// <param name="character">The character whose data will be used to fill the information panel.</param>
        /// <returns>A read-only list of string that contains the information panel.</returns>
        public IReadOnlyList<string> RenderCharacterDetails(IDisplayCharacter character)
        {
            if (character == null) return RenderBlankPanel();

            var informationPanel = new List<string>();
            informationPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            int headerLength = character.GetName().Count() + 7;
            informationPanel.Add("║ " + character.GetName() + " (" + character.GetSymbol() + ")" + new string(' ', MaxWidth - headerLength) + "║");
            informationPanel.Add("║" + new string('─', MaxWidth - 2) + "║");
            informationPanel.Add(GetStatDisplay("Health", 
                                                character.GetCurrenthealth().ToString(), 
                                                character.GetMaxHealth().ToString()));

            // Fill empty spaces 
            for (int i = 0; i < MaxHeight - 5; i++)
            {
                informationPanel.Add("║" + new string(' ', MaxWidth - 2) + "║");
            }
            
            informationPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return informationPanel;
        }

        /// <summary>
        /// Returns an information panel injected with a category name and it's description.
        /// </summary>
        /// <param name="category">A string array of 2 indeces, containing the category name and description.</param>
        /// <returns>A read-only list of string containing the information panel.</returns>
        public IReadOnlyList<string> RenderCategoryDetails(string[] category)
        {
            if (category == null) return RenderBlankPanel();

            var informationPanel = new List<string>();
            int maxLineWidth = MaxWidth - 3;
            

            informationPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            informationPanel.Add("║ " + category[0] + new string(' ', maxLineWidth - category[0].Count()) + "║");
            informationPanel.Add("║" + new string('─', MaxWidth - 2) + "║");

            informationPanel.AddRange(RenderCategoryDescription(category));
            
            informationPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return informationPanel;
        }

        /// <summary>
        /// Returns a read-only list containing the information panel injected with data from the a
        /// provided action.
        /// </summary>
        /// <param name="action">The action to display in the information panel.</param>
        /// <param name="data">The data of the action to display.</param>
        /// <returns>A read-only list containing the information panel.</returns>
        public IReadOnlyList<string> RenderActionDetails(IDisplayAction action, SubActionData data)
        {
            if (action == null) return RenderBlankPanel();

            var informationPanel = new List<string>();
            informationPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            int spaces = MaxWidth - 2 - action.GetDisplayName().Count();
            informationPanel.Add("║ " + action.GetDisplayName() + new string(' ', spaces - 1) + "║");
            informationPanel.Add("║" + new string('─', MaxWidth - 2) + "║");
            var actionTargetBoxes = RenderActionTargets(action);
            var actionDescription = RenderActionDescription(action, data);
            for (int i = 0; i < actionTargetBoxes.Count(); i++)
            {
                informationPanel.Add(actionDescription.ElementAt(i) + actionTargetBoxes.ElementAt(i));
            }
            int size = informationPanel.Count();
            for (int i = 0; i < MaxHeight - size - 1; i++)
            {
                informationPanel.Add("║" + new string(' ', MaxWidth - 2) + "║");
            }
            informationPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return informationPanel;
        }
        
        /// <summary>
        /// In case of null objects, renders a panel with no data.
        /// </summary>
        /// <returns>A panel with no data.</returns>
        private List<string> RenderBlankPanel()
        {
            var informationPanel = new List<string>();
            informationPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            for (int i = 0; i < MaxHeight - 2; i++)
            {
                informationPanel.Add("║ " + new string(' ', MaxWidth - 3) + "║");
            }
            informationPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return informationPanel;
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
        /// Returns a list of string containing the description of an action's category.
        /// </summary>
        /// <param name="category">A string array containing the category name and the category description.</param>
        /// <returns>Contains the description of an action's category.</returns>
        private List<string> RenderCategoryDescription(string[] category)
        {
            List<string> descriptionFull = new List<string>();
            int maxLineWidth = MaxWidth - 3;
            var descriptionAsArr = category[1].GetStringAsList(maxLineWidth);
            for (int i = 0; i < MaxHeight - 4; i++)
            {
                if(i < descriptionAsArr.Count())
                {
                    string description = descriptionAsArr.ElementAt(i);
                    description = description + new string(' ', maxLineWidth - description.Count());
                    descriptionFull.Add("║ " + description + "║");
                }
                else
                {
                    descriptionFull.Add("║" + new string(' ', MaxWidth - 2) + "║");
                }
            }
            return descriptionFull;
        }

        /// <summary>
        /// Returns a string containing the display information for a single stat.
        /// </summary>
        /// <param name="statName">The name of the stat to display.</param>
        /// <param name="statAmount1">The amount of the stat.</param>
        /// <param name="statAmount2">If the stat is modified, this is the modified stat amount.</param>
        /// <returns>Contains the display information for a single stat.</returns>
        private string GetStatDisplay(string statName, string statAmount1, string statAmount2 = "")
        {
            int length = statName.Count() + statAmount1.Count() + statAmount2.Count() + 6;
            if(statAmount2.Count() == 0)
                return "║ " + statName + ": " + statAmount1 + new string(' ', MaxWidth - length + 1) + "║";
            else
                return "║ " + statName + ": " + statAmount1 + "/" + statAmount2 + new string(' ', MaxWidth - length) + "║";
        }

        /// <summary>
        /// Renders the combat stats and description of an action.
        /// </summary>
        /// <param name="action">The action to display.</param>
        /// <param name="data">The data corresponding to the action to display.</param>
        /// <returns>A list of string that contains the action description.</returns>
        private List<string> RenderActionDescription(IDisplayAction action, SubActionData data)
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
