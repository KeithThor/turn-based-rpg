using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI.Combat
{
    // Handles the rendering of player and enemy formations in battle
    public class FormationPanel
    {
        private const int _paddingLeft = 12;
        private const int _paddingMiddle = 18;
        private const int _numberInRow = 3;
        private IReadOnlyList<IDisplayCharacter> _characters;
        public bool RenderFocus { get; set; }
        private IReadOnlyList<int> TargetPositions;
        private CachedData _cachedData;
        private IReadOnlyList<string> _cachedRender;

        private bool AddRowSpaces { get { return MaxHeight >= Min_Height + 3; } }
        private bool AddHealthBarSpaces { get { return MaxHeight >= Min_Height + 6; } }
        private int VerticalPadding { get { return (MaxHeight - (Min_Height + 6)) / 2; } }
        private int ExtraPadding { get { return (MaxHeight - (Min_Height + 6)) % 2; } }

        /// <summary>
        /// Gets or sets the maximum width of the formation panel.
        /// </summary>
        public int MaxWidth { get; set; }

        private int _maxHeight;
        /// <summary>
        /// Gets or sets the max height of the formation panel.
        /// </summary>
        public int MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                if (value < Min_Height) _maxHeight = Min_Height;
                else _maxHeight = value;
            }
        }

        /// <summary>
        /// The minimum possible height of the formation panel.
        /// </summary>
        public const int Min_Height = 18;

        private class CachedData
        {
            public int ActiveCharacterId;
            public IReadOnlyList<int> Targets;
            public Dictionary<int, CachedCharacter> CachedCharacters;
            public bool RenderFocus;
        }

        private class CachedCharacter
        {
            public int Position;
            public int CurrentHealth;
            public int MaxHealth;
        }

        public FormationPanel()
        {
            TargetPositions = new List<int>();
            _characters = new List<IDisplayCharacter>();
            MaxWidth = 119;
            MaxHeight = 31;
        }

        public IReadOnlyList<string> Render(IReadOnlyList<IDisplayCharacter> characters, 
                                            int activeCharacterId,
                                            IReadOnlyList<int> targets)
        {
            if (IsCachedData(characters, activeCharacterId, targets)) return _cachedRender;
            else
            {
                _cachedData = new CachedData()
                {
                    ActiveCharacterId = activeCharacterId,
                    Targets = targets,
                    CachedCharacters = CacheCharacters(characters),
                    RenderFocus = this.RenderFocus
                };
            }

            TargetPositions = targets;
            _characters = characters;
            var completeCombatFormations = new List<string>();
            
            // Construct top padding
            for (int i = 0; i < VerticalPadding + ExtraPadding; i++)
            {
                completeCombatFormations.Add(new string(' ', MaxWidth));
            }

            // For each row on the battlefield
            for (int i = 0; i < 3; i++)
            {
                var charInLine = FindCharactersInLine(i);
                completeCombatFormations.Add(RenderFormationNames(charInLine));
                completeCombatFormations.AddRange(RenderFormations(charInLine, activeCharacterId));
                completeCombatFormations.Add(RenderFormationTargets(i));
                if (AddRowSpaces) completeCombatFormations.Add(new string(' ', MaxWidth));
            }

            // Construct bottom padding, plus extra in case of odd numbers
            for (int i = 0; i < VerticalPadding; i++)
            {
                completeCombatFormations.Add(new string(' ', MaxWidth));
            }

            _cachedRender = completeCombatFormations;
            return completeCombatFormations;
        }

        /// <summary>
        /// Checks incoming data against the cached data to see if both represent the same data.
        /// </summary>
        /// <param name="characters">The characters to display in the formation.</param>
        /// <param name="activeCharacterId">The Id of the currently active character.</param>
        /// <param name="targets">The targets to focus on in the formation.</param>
        /// <returns>Returns whether or not the incoming data is the same as the cached data.</returns>
        private bool IsCachedData(IReadOnlyList<IDisplayCharacter> characters,
                                  int activeCharacterId,
                                  IReadOnlyList<int> targets)
        {
            if (_cachedData == null) return false;
            if (activeCharacterId != _cachedData.ActiveCharacterId) return false;
            if (_cachedData.RenderFocus != RenderFocus) return false;
            if (characters.Count() != _cachedData.CachedCharacters.Count()) return false;
            if (!targets.SequenceEqual(_cachedData.Targets)) return false;
            foreach (var character in characters)
            {
                int id = character.GetId();
                if (!_cachedData.CachedCharacters.ContainsKey(character.GetId())) return false;
                if (_cachedData.CachedCharacters[id].CurrentHealth != character.GetCurrenthealth()) return false;
                if (_cachedData.CachedCharacters[id].MaxHealth != character.GetMaxHealth()) return false;
                if (_cachedData.CachedCharacters[id].Position != character.GetPosition()) return false;
            }
            return true;
        }

        /// <summary>
        /// Constructs a character cache from a list of IDisplayCharacters.
        /// </summary>
        /// <param name="characters">The list of IDisplayCharacters to construct a cache from.</param>
        /// <returns>A dictionary with the Id of each characters as the key and the cache data as the value.</returns>
        private Dictionary<int, CachedCharacter> CacheCharacters(IReadOnlyList<IDisplayCharacter> characters)
        {
            var cache = new Dictionary<int, CachedCharacter>();
            foreach (var character in characters)
            {
                int id = character.GetId();
                cache[id] = new CachedCharacter()
                {
                    Position = character.GetPosition(),
                    CurrentHealth = character.GetCurrenthealth(),
                    MaxHealth = character.GetMaxHealth()
                };
            }
            return cache;
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
        private List<string> RenderFormations(IReadOnlyList<IDisplayCharacter> charactersToRender, int activeCharacterId)
        {
            if (AddHealthBarSpaces)
            {
                var rowOfFormationBoxes = new List<string>
                {
                    RenderHealthBars(charactersToRender),
                    new string(' ', MaxWidth),
                    RenderTopPanel(charactersToRender, activeCharacterId),
                    RenderMiddlePanel(charactersToRender, activeCharacterId),
                    RenderBottomPanel(charactersToRender, activeCharacterId)
                };
                return rowOfFormationBoxes;
            }
            else
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
        }

        // Renders one row of formation target triangles below the formation boxes
        private string RenderFormationTargets(int iterations)
        {
            if (TargetPositions.Count == 0)
            {
                return new string(' ', MaxWidth);
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
                    if(i < 4 && TargetPositions.Contains(i + iterations * _numberInRow) && RenderFocus)
                    {
                        sb.Append("▲");
                    }
                    else if(i >= 4 && TargetPositions.Contains(i + 3 + (iterations + 1) * _numberInRow) && RenderFocus)
                    {
                        sb.Append("▲");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
                return sb.ToString() + new string(' ', MaxWidth - sb.Length);
            }
        }

        // Render bottom part of formation boxes
        private string RenderBottomPanel(IReadOnlyList<IDisplayCharacter> charactersToRender, int activeCharacterID)
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
            return bottomPanelSB.ToString() + new string(' ', MaxWidth - bottomPanelSB.Length);
        }

        // Render healthbars under characters depending on their current health percentage
        private string RenderHealthBars(IReadOnlyList<IDisplayCharacter> charactersToRender)
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
            return healthBarSB + new string(' ', MaxWidth - healthBarSB.Length);
        }

        // Render middle part of formation boxes along with the symbols for each character
        private string RenderMiddlePanel(IReadOnlyList<IDisplayCharacter> charactersToRender, int activeCharacterId)
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
            return middlePanelSB.ToString() + new string(' ', MaxWidth - middlePanelSB.Length);
        }

        // Render top part of formation boxes
        private string RenderTopPanel(IReadOnlyList<IDisplayCharacter> charactersToRender, int activeCharacterId)
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
            return topPanelSB.ToString() + new string(' ', MaxWidth - topPanelSB.Length);
        }
        
        // Render the names of up to 6 characters on one line in the formation, 3 from player's side and 3 from enemy's side
        private string RenderFormationNames(IReadOnlyList<IDisplayCharacter> charactersToRender)
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
            return nameSB.ToString() + new string(' ', MaxWidth - nameSB.Length);
        }
    }
}
