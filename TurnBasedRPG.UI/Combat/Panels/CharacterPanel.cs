using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for displaying character data in the UI.
    /// </summary>
    public class CharacterPanel : ICharacterPanel
    {
        /// <summary>
        /// The minimum possible height of the formation panel.
        /// </summary>
        public const int Min_Height = 18;

        public int MaxWidth { get; set; }
        public int MinPrimaryBlockSize { get; set; }
        private int _maxHeight;
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
            get { return _uiStateTracker.IsInCharacterPanel; }
            set { _uiStateTracker.IsInCharacterPanel = value; }
        }

        public bool IsSubPanelActive { get; set; }
        public int FocusNumber { get; set; }

        private readonly IStatsSubPanel _statsSubPanel;
        private readonly IDamageTypesSubPanel _armorSubPanel;
        private readonly IDamageTypesSubPanel _damageSubPanel;
        private readonly IDamageTypesSubPanel _resistanceSubPanel;
        private readonly IDamageTypesSubPanel _damagePercentSubPanel;
        private readonly IOffensiveSubPanel _offensiveSubPanel;
        private readonly IUIStateTracker _uiStateTracker;
        private readonly IUICharacterManager _uiCharacterManager;

        public CharacterPanel(IStatsSubPanel statsSubPanel,
                              IDamageTypesSubPanel armorSubPanel,
                              IDamageTypesSubPanel damageSubPanel,
                              IDamageTypesSubPanel resistanceSubPanel,
                              IDamageTypesSubPanel damagePercentSubPanel,
                              IOffensiveSubPanel offensiveSubPanel,
                              IUIStateTracker uiStateTracker,
                              IUICharacterManager uiCharacterManager)
        {
            MaxHeight = 31;
            MaxWidth = 40;
            MinPrimaryBlockSize = 6;
            FocusNumber = 1;
            _statsSubPanel = statsSubPanel;
            _armorSubPanel = armorSubPanel;
            // All panels on the right side must have max size deducted by 1 to make room for space seperator
            _armorSubPanel.PanelName = "Armor";
            _armorSubPanel.MaxWidth--;
            _damageSubPanel = damageSubPanel;
            _damageSubPanel.PanelName = "Damage";
            _resistanceSubPanel = resistanceSubPanel;
            _resistanceSubPanel.PanelName = "Resistances";
            _resistanceSubPanel.MaxWidth--;
            _damagePercentSubPanel = damagePercentSubPanel;
            _damagePercentSubPanel.PanelName = "Bonus Damage";
            _offensiveSubPanel = offensiveSubPanel;
            _uiStateTracker = uiStateTracker;
            _uiCharacterManager = uiCharacterManager;
            _offensiveSubPanel.MaxWidth--;
            IsSubPanelActive = false;

            BindEvents();
        }

        private void BindEvents()
        {
            KeyPressed += _statsSubPanel.OnKeyPressed;
            KeyPressed += _offensiveSubPanel.OnKeyPressed;
            KeyPressed += _damageSubPanel.OnKeyPressed;
            KeyPressed += _armorSubPanel.OnKeyPressed;
            KeyPressed += _damagePercentSubPanel.OnKeyPressed;
            KeyPressed += _resistanceSubPanel.OnKeyPressed;
            _statsSubPanel.FocusChanged += OnSubPanelFocusChanged;
            _offensiveSubPanel.FocusChanged += OnSubPanelFocusChanged;
            _damageSubPanel.FocusChanged += OnSubPanelFocusChanged;
            _armorSubPanel.FocusChanged += OnSubPanelFocusChanged;
            _damagePercentSubPanel.FocusChanged += OnSubPanelFocusChanged;
            _resistanceSubPanel.FocusChanged += OnSubPanelFocusChanged;
        }

        private struct CachedCharacter
        {
            public int Id;
            public int CurrentHealth;
            public int MaxHealth;
        }

        private CachedCharacter _cachedCharacter;
        private IReadOnlyList<string> _cachedRender;

        /// <summary>
        /// Event called whenever a key is pressed by the player.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        /// Event called whenever the Focus for this panel is changed.
        /// </summary>
        public event EventHandler<FocusChangedEventArgs> FocusChanged;

        /// <summary>
        /// Event called whenever the Focus for one of this panel's subpanels is changed.
        /// </summary>
        public event EventHandler<FocusChangedEventArgs> SubPanelFocusChanged;

        /// <summary>
        /// Event called whenever a subpanel has it's IsActive property toggled.
        /// </summary>
        public event EventHandler<ActivenessChangedEventArgs> SubPanelActivenessChanged;

        private void OnSubPanelFocusChanged(object sender, FocusChangedEventArgs args)
        {
            SubPanelFocusChanged?.Invoke(sender, args);
        }

        /// <summary>
        /// Returns a details panel injected with the data from a character.
        /// </summary>
        /// <param name="character">The character whose data will be used to fill the details panel.</param>
        /// <returns>A read-only list of string that contains the details panel.</returns>
        public IReadOnlyList<string> Render()
        {
            IDisplayCharacter focusedTarget;
            IReadOnlyList<IDisplayCharacter> otherTargets = new List<IDisplayCharacter>();
            var targets = _uiStateTracker.CurrentTargetPositions;

            // If there are no characters within any of our target positions, return the current turn character
            if (!targets.Any(targetPosition =>
                             _uiCharacterManager.GetCharacterFromPosition(targetPosition) != null))
            {
                focusedTarget = _uiCharacterManager.GetCurrentTurnCharacter();
            }
            // If our main target position is occupied, display that target
            else if (_uiCharacterManager.CharacterInPositionExists(_uiStateTracker.CurrentTargetPosition)
                     && targets.Contains(_uiStateTracker.CurrentTargetPosition))
            {
                focusedTarget = _uiCharacterManager.GetCharacterFromPosition(_uiStateTracker.CurrentTargetPosition);
                otherTargets = _uiCharacterManager.GetCharactersFromPositions(targets);
            }
            // If our main target position isn't occupied, display any target that occupies a spot in our target list
            else
            {
                focusedTarget = _uiCharacterManager.Characters.First(
                                        character => targets.Contains(character.Position));
                otherTargets = _uiCharacterManager.GetCharactersFromPositions(targets);
            }

            if (otherTargets.Count() > 0)
            {
                return Render(otherTargets, focusedTarget);
            }

            if (focusedTarget == null) throw new NullReferenceException();
            if (IsCachedData(focusedTarget)) return _cachedRender;
            else
            {
                _cachedCharacter = new CachedCharacter()
                {
                    Id = focusedTarget.Id,
                    CurrentHealth = focusedTarget.CurrentHealth,
                    MaxHealth = focusedTarget.MaxHealth
                };
            }

            var characterDetails = new List<string>();
            characterDetails.AddRange(RenderCharacter(focusedTarget));
            
            _cachedRender = characterDetails;
            return characterDetails;
        }

        /// <summary>
        /// Renders a character panel with a group of characters and a focused character target that will show
        /// more stats.
        /// </summary>
        /// <param name="characters">The list of characters to render to the character panel.</param>
        /// <param name="focusedTarget">The character that is focused and will show more stats.</param>
        /// <returns>A list of string containing the rendered character panel.</returns>
        private IReadOnlyList<string> Render(IReadOnlyList<IDisplayCharacter> characters, IDisplayCharacter focusedTarget)
        {
            // Prevent target from being rendered twice
            var modifiedList = new List<IDisplayCharacter>(characters);
            modifiedList.Remove(focusedTarget);

            var render = new List<string>();
            var otherCharacters = RenderMany(modifiedList);
            var targetDetails = RenderCharacter(focusedTarget);

            // Remove extra details from the focusedTarget render depending on how much space there is left to fit the max height
            int removeIndex = MaxHeight - otherCharacters.Count();
            targetDetails.RemoveRange(removeIndex, targetDetails.Count() - removeIndex);

            render.AddRange(targetDetails);
            render.AddRange(otherCharacters);

            return render;
        }

        /// <summary>
        /// Renders many small character blocks inside the panel.
        /// </summary>
        /// <param name="characters">A list containing the characters to render inside the panel.</param>
        /// <returns>A list containing the render.</returns>
        private List<string> RenderMany(IReadOnlyList<IDisplayCharacter> characters)
        {
            // 1 to account for top border, 5 for size of each block
            int maxBlocks = (MaxHeight - 1 - MinPrimaryBlockSize) / 5;
            int totalBlocks = (characters.Count() > maxBlocks) ? maxBlocks : characters.Count();

            var characterBlock = new List<string>();
            for (int i = 0; i < totalBlocks; i++)
            {
                characterBlock.Add("║" + new string('═', MaxWidth - 2) + "║");
                characterBlock.AddRange(RenderCharacterHeader(characters[i]));

                characterBlock.Add(RenderResource("Health", characters[i].CurrentHealth, characters[i].MaxHealth));
                // Last iteration
                if (i == totalBlocks - 1)
                {
                    characterBlock.Add("╚" + new string('═', MaxWidth - 2) + "╝");
                }
                else
                {
                    characterBlock.Add("║" + new string('─', MaxWidth - 2) + "║");
                }
            }
            return characterBlock;
        }

        /// <summary>
        /// Renders a character's name and all it's stats wrapped in a panel.
        /// </summary>
        /// <param name="character">The character to render in the panel.</param>
        /// <returns>A list of strings containing the panel render.</returns>
        private List<string> RenderCharacter(IDisplayCharacter character)
        {
            var characterDetails = new List<string>();
            characterDetails.Add("╔" + new string('═', MaxWidth - 2) + "╗");

            characterDetails.AddRange(RenderCharacterHeader(character));

            characterDetails.Add(RenderResource("Health", character.CurrentHealth, character.MaxHealth));
            characterDetails.Add(RenderResource("Mana", character.CurrentMana, character.MaxMana));
            characterDetails.Add(RenderEmptyLine());

            characterDetails.AddRange(RenderSubPanels(character));

            int emptyLines = MaxHeight - 1 - characterDetails.Count();
            // Fill empty spaces 
            for (int i = 0; i < emptyLines; i++)
            {
                characterDetails.Add(RenderEmptyLine());
            }
            characterDetails.Add("╚" + new string('═', MaxWidth - 2) + "╝");

            return characterDetails;
        }

        /// <summary>
        /// Renders a resource name along with the resource's current amount and max amount.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="resourceAmount1">The current amount of the resource.</param>
        /// <param name="resourceAmount2">The maximum amount of the resource.</param>
        /// <returns>A string containing the resource name and the amount of that resource.</returns>
        private string RenderResource(string resourceName, int resourceAmount1, int resourceAmount2)
        {
            string resource = $"║ {resourceName}: {resourceAmount1}/{resourceAmount2}";
            int spaces = MaxWidth - resource.Length - 1;
            resource += new string(' ', spaces) + "║";
            return resource;
        }
        
        /// <summary>
        /// Given a character, constructs a header using the name of the character.
        /// </summary>
        /// <param name="character">The character to construct the header for.</param>
        /// <returns>A list of string containing the header.</returns>
        private IEnumerable<string> RenderCharacterHeader(IDisplayCharacter character)
        {
            var header = new List<string>();

            string characterLevel = $"Lv: {character.Level} ";
            int headerLength = character.Name.Count() + 7 + characterLevel.Length;
            header.Add("║ " + character.Name
                                + " (" + character.Symbol + ")"
                                + new string(' ', MaxWidth - headerLength)
                                + characterLevel + "║");
            header.Add("║" + new string('─', MaxWidth - 2) + "║");

            return header;
        }

        /// <summary>
        /// Constructs and returns the sub panels given a character to pull data from.
        /// </summary>
        /// <param name="character">The character whose data is used to construct the sub panels.</param>
        /// <returns>An enumerable of string that contains the rendered sub panels.</returns>
        private IEnumerable<string> RenderSubPanels(IDisplayCharacter character)
        {
            var subPanels = new List<string>();

            var offensiveRender = _offensiveSubPanel.Render(character);
            var statsRender = _statsSubPanel.Render(character);
            var armorRender = _armorSubPanel.Render(character.Armor);
            var resistanceRender = _resistanceSubPanel.Render(character.ArmorPercentage);
            var damageRender = _damageSubPanel.Render(character.DamageModifier);
            var damagePercentRender = _damagePercentSubPanel.Render(character.DamagePercentageModifier);


            int index = 1;
            // Render stats and offensive panels
            for (int i = 0; i < statsRender.Count(); i++)
            {
                if (i == 0 && IsActive)
                {
                    subPanels.Add("║" + GetFocus(index) + statsRender[i] + GetFocus(++index) + offensiveRender[i] + " ║");
                }
                else
                {
                    subPanels.Add("║ " + statsRender[i] + " " + offensiveRender[i] + " ║");
                }
            }

            subPanels.Add(RenderEmptyLine());

            // Render damage and armor panels
            for (int i = 0; i < statsRender.Count(); i++)
            {
                if (i == 0 && IsActive)
                {
                    subPanels.Add("║" + GetFocus(++index) + damageRender[i] + GetFocus(++index) + armorRender[i] + " ║");
                }
                else
                {
                    subPanels.Add("║ " + damageRender[i] + " " + armorRender[i] + " ║");
                }
            }

            subPanels.Add(RenderEmptyLine());

            // Render damage percent and resistance panels
            for (int i = 0; i < offensiveRender.Count(); i++)
            {
                if (i == 0 && IsActive)
                {
                    subPanels.Add("║" + GetFocus(++index) + damagePercentRender[i] + GetFocus(++index) + resistanceRender[i] + " ║");
                }
                else
                {
                    subPanels.Add("║ " + damagePercentRender[i] + " " + resistanceRender[i] + " ║");
                }
            }
            
            return subPanels;
        }

        /// <summary>
        /// Given an index, returns a focus triangle if the index matches the Focus Number, else returns an empty space.
        /// </summary>
        /// <param name="index">The index of the panel.</param>
        /// <returns>A string containing a focus triangle or a space.</returns>
        private string GetFocus(int index)
        {
            if (FocusNumber == index) return "►";
            return " ";
        }

        /// <summary>
        /// Constructs and returns an empty line.
        /// </summary>
        /// <returns>A string containing an empty line for the character panel.</returns>
        private string RenderEmptyLine()
        {
            return "║" + new string(' ', MaxWidth - 2) + "║";
        }

        /// <summary>
        /// Returns whether or not the stats of the character being passed in is the same as the cached data.
        /// </summary>
        /// <param name="character">The character to check with the cached data.</param>
        /// <returns>Returns whether or not the stats of the character being passed in is the same as the cached data.</returns>
        private bool IsCachedData(IDisplayCharacter character)
        {
            if (character.Id != _cachedCharacter.Id) return false;
            if (character.CurrentHealth != _cachedCharacter.CurrentHealth) return false;
            if (character.MaxHealth != _cachedCharacter.MaxHealth) return false;

            return true;
        }

        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            KeyPressed?.Invoke(sender, args);

            if (IsActive && !args.Handled)
            {
                args.Handled = true;
                switch(args.PressedKey.Key)
                {
                    case ConsoleKey.LeftArrow:
                        OnLeftArrowPressed();
                        break;
                    case ConsoleKey.RightArrow:
                        OnRightArrowPressed();
                        break;
                    case ConsoleKey.UpArrow:
                        OnUpArrowPressed();
                        break;
                    case ConsoleKey.DownArrow:
                        OnDownArrowPressed();
                        break;
                    case ConsoleKey.Enter:
                        OnEnterPressed();
                        break;
                    case ConsoleKey.Escape:
                        if (IsSubPanelActive)
                            DeactivateSubpanel();
                        else
                            args.Handled = false;
                        break;
                    default:
                        args.Handled = false;
                        break;
                }
            }
        }

        private void OnLeftArrowPressed()
        {
            if (FocusNumber % 2 == 0 && !IsSubPanelActive)
            {
                FocusNumber--;
                FocusChanged?.Invoke(this, new FocusChangedEventArgs() { NewFocus = FocusNumber });
            }
        }

        private void OnRightArrowPressed()
        {
            if (FocusNumber % 2 == 1 && !IsSubPanelActive)
            {
                FocusNumber++;
                FocusChanged?.Invoke(this, new FocusChangedEventArgs() { NewFocus = FocusNumber });
            }
        }

        private void OnUpArrowPressed()
        {
            if (FocusNumber > 2 && !IsSubPanelActive)
            {
                FocusNumber -= 2;
                FocusChanged?.Invoke(this, new FocusChangedEventArgs() { NewFocus = FocusNumber });
            }
        }

        private void OnDownArrowPressed()
        {
            if (FocusNumber < 5 && !IsSubPanelActive)
            {
                FocusNumber += 2;
                FocusChanged?.Invoke(this, new FocusChangedEventArgs() { NewFocus = FocusNumber });
            }
        }

        private void OnEnterPressed()
        {
            if (!IsSubPanelActive)
            {
                IsSubPanelActive = true;
                switch (FocusNumber)
                {
                    case 1:
                        _statsSubPanel.IsActive = true;
                        break;
                    case 2:
                        _offensiveSubPanel.IsActive = true;
                        break;
                    case 3:
                        _damageSubPanel.IsActive = true;
                        break;
                    case 4:
                        _armorSubPanel.IsActive = true;
                        break;
                    case 5:
                        _damagePercentSubPanel.IsActive = true;
                        break;
                    case 6:
                        _resistanceSubPanel.IsActive = true;
                        break;
                    default:
                        throw new Exception("Focus number not within accepted range in CharacterPanel!");
                }
                SubPanelActivenessChanged?.Invoke(this, new ActivenessChangedEventArgs() { IsActive = IsSubPanelActive });
            }
        }

        private void DeactivateSubpanel()
        {
            IsSubPanelActive = false;
            switch (FocusNumber)
            {
                case 1:
                    _statsSubPanel.IsActive = false;
                    break;
                case 2:
                    _offensiveSubPanel.IsActive = false;
                    break;
                case 3:
                    _damageSubPanel.IsActive = false;
                    break;
                case 4:
                    _armorSubPanel.IsActive = false;
                    break;
                case 5:
                    _damagePercentSubPanel.IsActive = false;
                    break;
                case 6:
                    _resistanceSubPanel.IsActive = false;
                    break;
                default:
                    throw new Exception("Focus number not within accepted range in CharacterPanel!");
            }
            SubPanelActivenessChanged?.Invoke(this, new ActivenessChangedEventArgs() { IsActive = IsSubPanelActive });
        }
    }
}
