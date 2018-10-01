using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI
{
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

        // Called to display the character details panel inside the information panel
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

        // Called whenever the information panel should display categories and category details
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

        // Called whenever the information panel should display an action and it's details
        public IReadOnlyList<string> RenderActionDetails(IDisplayAction action)
        {
            if (action == null) return RenderBlankPanel();

            var informationPanel = new List<string>();
            informationPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            int spaces = MaxWidth - 2 - action.GetDisplayName().Count();
            informationPanel.Add("║ " + action.GetDisplayName() + new string(' ', spaces - 1) + "║");
            informationPanel.Add("║" + new string('─', MaxWidth - 2) + "║");
            var actionTargetBoxes = RenderActionTargets(action);
            var actionDescription = RenderActionDescription(action);
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
        // In case of a null object, renders a blank panel
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

        // Renders a small diagram that shows which positions an action targets
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
                    if (j <= 3 && action.GetActionTargets().Contains(j + i / 2 * 3) && !action.CanSwitchTargetPosition())
                    {
                        if (i % 2 == 1) line += "╚╝";
                        else line += "╔╗";
                    }
                    // If the action can't switch targets and one of its targets is in one of the enemy's positions, then render a focused square.
                    // Or if the action can switch targets, render a focused square if this is one of the target positions
                    else if ((j > 3 && action.GetActionTargets().Contains(j + 6 + i / 2 * 3) && !action.CanSwitchTargetPosition()) ||
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
            return actionTargets;
        }

        // Renders the category description under the category name
        private List<string> RenderCategoryDescription(string[] category)
        {
            List<string> descriptionFull = new List<string>();
            int maxLineWidth = MaxWidth - 3;
            var descriptionAsArr = GetReducedLengthString(category[1], maxLineWidth);
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

        // Gets the display string for a single stat
        private string GetStatDisplay(string statName, string statAmount1, string statAmount2 = "")
        {
            int length = statName.Count() + statAmount1.Count() + statAmount2.Count() + 6;
            if(statAmount2.Count() == 0)
                return "║ " + statName + ": " + statAmount1 + new string(' ', MaxWidth - length + 1) + "║";
            else
                return "║ " + statName + ": " + statAmount1 + "/" + statAmount2 + new string(' ', MaxWidth - length) + "║";
        }

        // Returns an IEnumerable of string split from another string along spaces and periods
        private List<string> GetReducedLengthString(string str, int maxLength)
        {
            var reducedLengthList = new List<string>();
            int iterations = 0;
            int startIndex = 0;
            for (int i = 0; i <= str.Count() / maxLength; i++)
            {
                string reducedStr = "";
                // If the unrendered parts of the category description has more characters than fits in the next line
                if (str.Count() > (i + 1) * maxLength)
                {
                    reducedStr = str.Substring(startIndex, maxLength);
                    int tempIndex = 0;
                    // If the line ends in a letter, find the space or . character closest to the end of the array and make it
                    // the start index for the next iteration and the end point for this line
                    if (reducedStr.Last() == '.' || reducedStr.Last() == ' '
                        || str[startIndex + maxLength] == '.' || str[startIndex + maxLength] == ' ')
                    {
                        tempIndex = maxLength + startIndex;
                    }
                    else
                    {
                        tempIndex = reducedStr.LastIndexOfAny(new char[] { '.', ' ' }) + startIndex + 1;
                    }
                    reducedStr = str.Substring(startIndex, tempIndex - startIndex);
                    startIndex = tempIndex;
                }
                // Unrendered parts of the category description less than the maximum width of 1 line
                else
                {
                    reducedStr = str.Substring(startIndex);
                }
                // Remove spaces at the beginning of the new line
                if (reducedStr.Count() > 0 && reducedStr[0] == ' ') reducedStr = reducedStr.Remove(0, 1);
                // Add spaces to the end of the description line if there are extra spaces left
                reducedLengthList.Add(reducedStr);
                iterations++;
            }
            return reducedLengthList;
        }

        private List<string> RenderActionDescription(IDisplayAction action)
        {
            int maxLength = 30;
            int maxHeight = 7;
            var description = new List<string>();
            var reducedStr = GetReducedLengthString(action.GetDescription(), maxLength);
            for (int i = 0; i < maxHeight; i++)
            {
                string str = "";
                if (reducedStr.Count() > i)
                {
                    str = reducedStr.ElementAt(i);
                }
                description.Add("║ " + str + new string(' ', maxLength - str.Count() - 2));
            }
            return description;
        }
    }
}
