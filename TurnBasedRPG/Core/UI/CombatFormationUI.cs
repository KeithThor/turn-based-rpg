using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core.Interfaces;

namespace TurnBasedRPG.Core.UI
{
    // Handles the rendering of player and enemy formations in battle
    public class CombatFormationUI
    {
        private const int _paddingLeft = 12;
        private const int _paddingMiddle = 18;
        private const int _numberInRow = 3;
        private List<IDisplayCharacter> _characters;
        public bool IsInFormationPanel = false;
        public List<int> TargetPositions;
        public CombatFormationUI()
        {
            TargetPositions = new List<int>();
            _characters = new List<IDisplayCharacter>();
        }

        public List<string> Render(List<IDisplayCharacter> characters, int activeCharacterId)
        {
            _characters = characters;
            var completeCombatFormations = new List<string>();
            Console.WriteLine("\n\n\n");
            // For each row on the battlefield
            for (int i = 0; i < 3; i++)
            {
                var charInLine = FindCharactersInLine(i);
                completeCombatFormations.Add(RenderFormationNames(charInLine));
                completeCombatFormations.AddRange(RenderFormations(charInLine, activeCharacterId));
                completeCombatFormations.Add(RenderFormationTargets(i));
                completeCombatFormations.Add("\n");
            }
            return completeCombatFormations;
        }

        private List<IDisplayCharacter> FindCharactersInLine(int offset)
        {
            // Contains all the characters in one horizontal line, including up to 3 player characters and up to 3 enemy characters
            var charInLine = new List<IDisplayCharacter>();
            // For each column on the player's side of the row
            for (int j = 1; j <= 3; j++)
            {
                bool foundChar = false;
                // Find the player character that occupies this formation slot
                foreach (var character in _characters)
                {
                    if (character.GetPosition() == j + (offset * _numberInRow))
                    {
                        charInLine.Add(character);
                        foundChar = true;
                    }
                }
                // If no characters occupy this slot
                if (!foundChar)
                    charInLine.Add(null);
            }
            // For each column on the enemy's side of the row
            for (int j = 1; j <= 3; j++)
            {
                bool foundChar = false;
                int k = 0;
                while (!foundChar && k < _characters.Count)
                {
                    if (_characters[k].GetPosition() == j + 9 + (offset * _numberInRow))
                    {
                        charInLine.Add(_characters[k]);
                        foundChar = true;
                    }
                    k++;
                }
                if (!foundChar)
                    charInLine.Add(null);
            }
            return charInLine;
        }

        // Render one row of formation boxes
        private List<string> RenderFormations(List<IDisplayCharacter> charactersToRender, int activeCharacterId)
        {
            var rowOfFormationBoxes = new List<string>
            {
                RenderHealthBars(charactersToRender),
                RenderTopPanel(charactersToRender, activeCharacterId),
                RenderMiddlePanel(charactersToRender, activeCharacterId),
                RenderBottomPanel(charactersToRender, activeCharacterId)
            };
            return rowOfFormationBoxes;
        }

        // Renders one row of formation target triangles below the formation boxes
        private string RenderFormationTargets(int iterations)
        {
            if(TargetPositions.Count == 0)
            {
                return "";
            }
            else
            {
                int padding = _paddingLeft + 2;
                var sb = new StringBuilder();
                for (int i = 1; i <= 6; i++)
                {
                    if(i == 4)
                        sb.Append(' ', _paddingMiddle);
                    sb.Append(' ', i == 1 ? padding - 1 : padding);
                    if(i < 4 && TargetPositions.Contains(i + iterations * _numberInRow) && IsInFormationPanel)
                    {
                        sb.Append("▲");
                    }
                    else if(i >= 4 && TargetPositions.Contains(i + 3 + (iterations + 1) * _numberInRow) && IsInFormationPanel)
                    {
                        sb.Append("▲");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
                return sb.ToString();
            }
        }

        // Render bottom part of formation boxes
        private string RenderBottomPanel(List<IDisplayCharacter> charactersToRender, int activeCharacterID)
        {
            var bottomPanelSB = new StringBuilder();
            
            for (int i = 0; i < 6; i++)
            {
                bottomPanelSB.Append(' ', _paddingLeft);
                if (i == 3)
                {
                    bottomPanelSB.Append(' ', _paddingMiddle);
                }
                if (charactersToRender[i] != null && charactersToRender[i].GetId() == activeCharacterID)
                {
                    bottomPanelSB.Append("╚═╝");
                }
                else if (charactersToRender[i] != null && charactersToRender[i].GetCurrenthealth() <= 0)
                {
                    bottomPanelSB.Append("/ \\");
                }
                else
                {
                    bottomPanelSB.Append("└─┘");
                }
            }
            return bottomPanelSB.ToString();
        }

        // Render healthbars under characters depending on their current health percentage
        private string RenderHealthBars(List<IDisplayCharacter> charactersToRender)
        {
            var healthBarSB = new StringBuilder();
            
            healthBarSB.Append(' ', 10);
            for (int i = 0; i < 6; i++)
            {
                if (i == 3)
                {
                    healthBarSB.Append(' ', 26);
                }
                else if (i != 0)
                {
                    healthBarSB.Append(' ', 8);
                }
                if (charactersToRender[i] != null)
                {
                    int healthPercentage = charactersToRender[i].GetCurrenthealth() * 100 / charactersToRender[i].GetMaxHealth();
                    int totalHealthBars = healthPercentage * 5 / 100;
                    if (totalHealthBars == 0 && charactersToRender[i].GetCurrenthealth() != 0)
                        totalHealthBars = 1;
                    healthBarSB.Append(charactersToRender[i] != null ? "│" : " ");
                    for (int j = 0; j < 5; j++)
                    {
                        if (j < totalHealthBars)
                            healthBarSB.Append("■");
                        else
                            healthBarSB.Append(" ");
                    }
                    healthBarSB.Append(charactersToRender[i] != null ? "│" : " ");
                }
                else if (charactersToRender[i] == null)
                {
                    healthBarSB.Append(' ', 7);
                }
            }
            return healthBarSB + "\n";
        }

        // Render middle part of formation boxes along with the symbols for each character
        private string RenderMiddlePanel(List<IDisplayCharacter> charactersToRender, int activeCharacterId)
        {
            var middlePanelSB = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                middlePanelSB.Append(' ', _paddingLeft);
                if (i == 3)
                {
                    middlePanelSB.Append(' ', _paddingMiddle);
                }
                if (charactersToRender[i] != null && charactersToRender[i].GetId() == activeCharacterId)
                {
                    middlePanelSB.Append("║" + charactersToRender[i].GetSymbol() + "║");
                }
                else
                {
                    if (charactersToRender[i] == null)
                        middlePanelSB.Append("│ │");
                    else
                    {
                        string sidebar = charactersToRender[i].GetCurrenthealth() <= 0 ? " " : "│";
                        middlePanelSB.Append(sidebar + charactersToRender[i].GetSymbol() + sidebar);
                    }
                }
            }
            return middlePanelSB.ToString();
        }

        // Render top part of formation boxes
        private string RenderTopPanel(List<IDisplayCharacter> charactersToRender, int activeCharacterId)
        {
            var topPanelSB = new StringBuilder();
            
            for (int i = 0; i < 6; i++)
            {
                topPanelSB.Append(' ', _paddingLeft);
                if (i == 3)
                {
                    topPanelSB.Append(' ', _paddingMiddle);
                }
                if (charactersToRender[i] != null && charactersToRender[i].GetId() == activeCharacterId)
                {
                    topPanelSB.Append("╔═╗");
                }
                else if (charactersToRender[i] != null && charactersToRender[i].GetCurrenthealth() <= 0)
                {
                    topPanelSB.Append("\\ /");
                }
                else
                {
                    topPanelSB.Append("┌─┐");
                }
            }
            return topPanelSB.ToString();
        }
        
        // Render the names of up to 6 characters on one line in the formation, 3 from player's side and 3 from enemy's side
        private string RenderFormationNames(List<IDisplayCharacter> charactersToRender)
        {
            int maxNameLength = 14;
            int paddingLeft = 6;
            int paddingMiddle = 18;

            var nameSB = new StringBuilder();
            nameSB.Append(' ', paddingLeft + 1);
            for (int i = 0; i < 6; i++)
            {
                if (i == 3)
                    nameSB.Append(' ', paddingMiddle);
                if (charactersToRender[i] == null)
                    nameSB.Append(' ', maxNameLength + 1);
                else
                {
                    int namePaddingLeft = (maxNameLength - charactersToRender[i].GetName().Length) / 2;
                    nameSB.Append(' ', namePaddingLeft);
                    nameSB.Append(charactersToRender[i].GetName());
                    nameSB.Append(' ', maxNameLength - charactersToRender[i].GetName().Length - namePaddingLeft + 1);
                }
            }
            return nameSB.ToString();
        }
    }
}
