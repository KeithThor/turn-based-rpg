using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Combat;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    // Handles the rendering of player and enemy formations in battle
    public class FormationPanel : IReceiveInputPanel
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

        public bool IsActive
        {
            get { return _defaultsHandler.IsInFormationPanel; }
            set { _defaultsHandler.IsInFormationPanel = value; }
        }
        public int FocusNumber { get; set; }

        /// <summary>
        /// The minimum possible height of the formation panel.
        /// </summary>
        public const int Min_Height = 18;
        private readonly DefaultsHandler _defaultsHandler;
        private readonly UICharacterManager _uiCharacterManager;

        public event EventHandler<ActivePanelChangedEventArgs> ActivePanelChanged;

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

        public FormationPanel(DefaultsHandler defaultsHandler,
                              UICharacterManager uiCharacterManager)
        {
            TargetPositions = new List<int>();
            _characters = new List<IDisplayCharacter>();
            MaxWidth = 119;
            MaxHeight = 31;
            _defaultsHandler = defaultsHandler;
            _uiCharacterManager = uiCharacterManager;
        }

        public IReadOnlyList<string> Render()
        {
            var characters = _uiCharacterManager.GetAllCharacters();
            int activeCharacterId = _defaultsHandler.ActiveCharacterId;
            var targets = _defaultsHandler.CurrentTargetPositions;

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
                int id = character.Id;
                if (!_cachedData.CachedCharacters.ContainsKey(character.Id)) return false;
                if (_cachedData.CachedCharacters[id].CurrentHealth != character.CurrentHealth) return false;
                if (_cachedData.CachedCharacters[id].MaxHealth != character.MaxHealth) return false;
                if (_cachedData.CachedCharacters[id].Position != character.Position) return false;
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
                int id = character.Id;
                cache[id] = new CachedCharacter()
                {
                    Position = character.Position,
                    CurrentHealth = character.CurrentHealth,
                    MaxHealth = character.MaxHealth
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
                    if (character.Position == j + (offset * _numberInRow))
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
                    if (_characters[k].Position == j + 9 + (offset * _numberInRow))
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
                if (charactersToRender[i] != null && charactersToRender[i].Id == activeCharacterID)
                {
                    bottomPanelSB.Append("╚═╝");
                }
                else if (charactersToRender[i] != null && charactersToRender[i].CurrentHealth <= 0)
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
                    int healthPercentage = charactersToRender[i].CurrentHealth * 100 / charactersToRender[i].MaxHealth;
                    int totalHealthBars = healthPercentage * 5 / 100;
                    if (totalHealthBars == 0 && charactersToRender[i].CurrentHealth != 0)
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
                if (charactersToRender[i] != null && charactersToRender[i].Id == activeCharacterId)
                {
                    middlePanelSB.Append("║" + charactersToRender[i].Symbol + "║");
                }
                else
                {
                    if (charactersToRender[i] == null)
                        middlePanelSB.Append("│ │");
                    else
                    {
                        string sidebar = charactersToRender[i].CurrentHealth <= 0 ? " " : "│";
                        middlePanelSB.Append(sidebar + charactersToRender[i].Symbol + sidebar);
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
                if (charactersToRender[i] != null && charactersToRender[i].Id == activeCharacterId)
                {
                    topPanelSB.Append("╔═╗");
                }
                else if (charactersToRender[i] != null && charactersToRender[i].CurrentHealth <= 0)
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
                    int namePaddingLeft = (maxNameLength - charactersToRender[i].Name.Length) / 2;
                    nameSB.Append(' ', namePaddingLeft);
                    nameSB.Append(charactersToRender[i].Name);
                    nameSB.Append(' ', maxNameLength - charactersToRender[i].Name.Length - namePaddingLeft + 1);
                }
            }
            return nameSB.ToString() + new string(' ', MaxWidth - nameSB.Length);
        }

        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (IsActive)
            {
                switch (args.PressedKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        OnUpArrowPressed();
                        break;
                    case ConsoleKey.DownArrow:
                        OnDownArrowPressed();
                        break;
                    case ConsoleKey.LeftArrow:
                        OnLeftArrowPressed();
                        break;
                    case ConsoleKey.RightArrow:
                        OnRightArrowPressed();
                        break;
                    case ConsoleKey.Escape:
                        ActivePanelChanged?.Invoke(this, new ActivePanelChangedEventArgs()
                        {
                            InActionPanel = true,
                            InCategoryPanel = false,
                            InCommandPanel = false,
                            InFormationPanel = false
                        });
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnUpArrowPressed()
        {
            bool isBlocked = IsUpArrowBlocked();

            // If the player is in the formation panel and not in the top-most rows of the formations and upwards movement is not blocked
            if (!isBlocked
                && ((_defaultsHandler.CurrentTargetPosition > 3 && _defaultsHandler.CurrentTargetPosition <= 9)
                   || _defaultsHandler.CurrentTargetPosition > 12))
            {
                ChangeFormationPanelFocus(-3);
            }
        }

        private void OnDownArrowPressed()
        {
            // If the player is in the formation panel, check to see if downwards movement is blocked
            bool isBlocked = IsDownArrowBlocked();
            if (!isBlocked
                && ((_defaultsHandler.CurrentTargetPosition <= 6 && _defaultsHandler.CurrentTargetPosition > 0)
                    || (_defaultsHandler.CurrentTargetPosition <= 15 && _defaultsHandler.CurrentTargetPosition > 9)))
            {
                ChangeFormationPanelFocus(3);
            }
        }

        private void OnRightArrowPressed()
        {
            bool isBlocked = IsRightArrowBlocked();

            if (!isBlocked
                && ((_defaultsHandler.CurrentTargetPosition % 3 != 0 && _defaultsHandler.CurrentTargetPosition >= 10)
                    || _defaultsHandler.CurrentTargetPosition < 10))
            {
                // If the player is moving from the player's formation to the enemy's formation
                if (_defaultsHandler.CurrentTargetPosition % 3 == 0 && _defaultsHandler.CurrentTargetPosition < 10)
                    ChangeFormationPanelFocus(7);
                else
                    ChangeFormationPanelFocus(1);
            }
        }

        private void OnLeftArrowPressed()
        {
            // If the player is in the formation panel, but not at the left-most column in the player's formation
            if ((_defaultsHandler.CurrentTargetPosition % 3 != 1
                    && _defaultsHandler.CurrentTargetPosition < 10)
                    || _defaultsHandler.CurrentTargetPosition >= 10)
            {
                // If the player moves from the enemy's formation to the player's formation
                if (_defaultsHandler.CurrentTargetPosition % 3 == 1 && _defaultsHandler.CurrentTargetPosition >= 10)
                    ChangeFormationPanelFocus(-7);
                else
                    ChangeFormationPanelFocus(-1);
            }
        }
        
        /// <summary>
        /// Checks to see if movement upward in the formation panel should be blocked based on the current position of the
        /// player's target.
        /// </summary>
        /// <returns>Returns true if movement should be blocked.</returns>
        private bool IsUpArrowBlocked()
        {
            bool isBlocked = !_defaultsHandler.ActiveAction.CanTargetThroughUnits;
            // Action can only be blocked when not on the player's side of the field and not in the enemy's first column
            isBlocked = isBlocked && _defaultsHandler.CurrentTargetPosition % 3 != 1 && _defaultsHandler.CurrentTargetPosition >= 10;
            // If the target position is in the 3rd column, check if the 1st or 2nd column of the row is occupied, if so action is blocked
            if (_defaultsHandler.CurrentTargetPosition % 3 == 0 && isBlocked)
            {
                if (_uiCharacterManager.CharacterInPositionExists(_defaultsHandler.CurrentTargetPosition - 4, false))
                    return true;
                if (_uiCharacterManager.CharacterInPositionExists(_defaultsHandler.CurrentTargetPosition - 5, false))
                    return true;
            }
            else
            {
                // Action can only be blocked if there is a character in front of the spot the player is trying to reach
                isBlocked = isBlocked
                            && _uiCharacterManager.CharacterInPositionExists(_defaultsHandler.CurrentTargetPosition - 4, false);
            }
            return isBlocked;
        }

        /// <summary>
        /// Returns true if an action cannot bypass a character blocking it's path downwards.
        /// </summary>
        /// <returns></returns>
        private bool IsDownArrowBlocked()
        {
            bool isBlocked = !_defaultsHandler.ActiveAction.CanTargetThroughUnits;
            // Action can only be blocked when not on the player's side of the field and not in the enemy's first column
            isBlocked = isBlocked && _defaultsHandler.CurrentTargetPosition % 3 != 1 && _defaultsHandler.CurrentTargetPosition >= 10;
            // If target position is on the last column, check for
            if (_defaultsHandler.CurrentTargetPosition % 3 == 0 && isBlocked)
            {
                if (_uiCharacterManager.CharacterInPositionExists(_defaultsHandler.CurrentTargetPosition + 2, false))
                    isBlocked = false;

                if (!isBlocked)
                {
                    if (_uiCharacterManager.CharacterInPositionExists(_defaultsHandler.CurrentTargetPosition + 1, false))
                        isBlocked = false;
                }
            }
            else
            {
                var character = _uiCharacterManager.GetCharacterFromPosition(_defaultsHandler.CurrentTargetPosition + 2);
                // Action can only be blocked if there is a character in front of the spot the player is trying to reach
                isBlocked = isBlocked && !(character == null || character.CurrentHealth == 0);
            }

            return isBlocked;
        }

        /// <summary>
        /// Returns true if an action cannot bypass a character blocking its path to the right.
        /// </summary>
        /// <returns></returns>
        private bool IsRightArrowBlocked()
        {
            if (_defaultsHandler.ActiveAction.CanTargetThroughUnits) return false;

            // Don't block any actions from the player's field
            if (_defaultsHandler.CurrentTargetPosition <= 9) return false;

            var character = _uiCharacterManager.GetCharacterFromPosition(_defaultsHandler.CurrentTargetPosition);
            // If there is no character or a dead character in that position, the target is not blocked
            if (character == null || character.CurrentHealth == 0) return false;

            return true;
        }

        /// <summary>
        /// Changes the focus target in the formation panel.
        /// </summary>
        /// <param name="changeAmount">The amount to change the focus target by.</param>
        private void ChangeFormationPanelFocus(int changeAmount)
        {
            if (_defaultsHandler.IsInFormationPanel && changeAmount != 0)
            {
                _defaultsHandler.CurrentTargetPosition += changeAmount;
                if (_defaultsHandler.CurrentTargetPosition < 1) _defaultsHandler.CurrentTargetPosition = 1;
                if (_defaultsHandler.CurrentTargetPosition > 18) _defaultsHandler.CurrentTargetPosition = 18;

                var targetPositions = CombatTargeter.GetTranslatedTargetPositions(_defaultsHandler.ActiveAction.TargetPositions,
                                                                                _defaultsHandler.ActiveAction.CenterOfTargets,
                                                                                _defaultsHandler.ActiveAction.CanSwitchTargetPosition
                                                                                    || RenderFocus,
                                                                                _defaultsHandler.CurrentTargetPosition);

                _defaultsHandler.CurrentTargetPositions = targetPositions;
            }
        }
    }
}
