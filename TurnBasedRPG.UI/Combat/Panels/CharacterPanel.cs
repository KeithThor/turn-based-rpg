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

        public bool IsActive { get; set; }
        public int FocusNumber { get; set; }

        private readonly IStatsSubPanel _statsSubPanel;
        private readonly IDamageTypesSubPanel _armorSubPanel;
        private readonly IDamageTypesSubPanel _damageSubPanel;
        private readonly IDamageTypesSubPanel _resistanceSubPanel;
        private readonly IDamageTypesSubPanel _damagePercentSubPanel;
        private readonly IOffensiveSubPanel _offensiveSubPanel;
        private readonly IUIStateTracker _defaultsHandler;
        private readonly IUICharacterManager _uiCharacterManager;

        public CharacterPanel(IStatsSubPanel statsSubPanel,
                              IDamageTypesSubPanel armorSubPanel,
                              IDamageTypesSubPanel damageSubPanel,
                              IDamageTypesSubPanel resistanceSubPanel,
                              IDamageTypesSubPanel damagePercentSubPanel,
                              IOffensiveSubPanel offensiveSubPanel,
                              IUIStateTracker defaultsHandler,
                              IUICharacterManager uiCharacterManager)
        {
            MaxHeight = 31;
            MaxWidth = 40;
            MinPrimaryBlockSize = 6;
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
            _defaultsHandler = defaultsHandler;
            _uiCharacterManager = uiCharacterManager;
            _offensiveSubPanel.MaxWidth--;
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
        /// Returns a details panel injected with the data from a character.
        /// </summary>
        /// <param name="character">The character whose data will be used to fill the details panel.</param>
        /// <returns>A read-only list of string that contains the details panel.</returns>
        public IReadOnlyList<string> Render()
        {
            IDisplayCharacter focusedTarget;
            IReadOnlyList<IDisplayCharacter> otherTargets = new List<IDisplayCharacter>();
            var targets = _defaultsHandler.CurrentTargetPositions;

            // If there are no characters within any of our target positions, return the current turn character
            if (!targets.Any(targetPosition =>
                             _uiCharacterManager.GetCharacterFromPosition(targetPosition) != null))
            {
                focusedTarget = _uiCharacterManager.GetCurrentTurnCharacter();
            }
            // If our main target position is occupied, display that target
            else if (_uiCharacterManager.CharacterInPositionExists(_defaultsHandler.CurrentTargetPosition)
                     && targets.Contains(_defaultsHandler.CurrentTargetPosition))
            {
                focusedTarget = _uiCharacterManager.GetCharacterFromPosition(_defaultsHandler.CurrentTargetPosition);
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
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private string RenderResource(string resourceName, int resourceAmount1, int resourceAmount2)
        {
            string health = $"║ {resourceName}: {resourceAmount1}/{resourceAmount2}";
            int spaces = MaxWidth - health.Length - 1;
            health += new string(' ', spaces) + "║";
            return health;
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

            // Render stats and offensive panels
            for (int i = 0; i < statsRender.Count(); i++)
            {
                subPanels.Add("║ " + statsRender[i] + " " + offensiveRender[i] + " ║");
            }

            subPanels.Add(RenderEmptyLine());

            // Render damage and armor panels
            for (int i = 0; i < statsRender.Count(); i++)
            {
                subPanels.Add("║ " + damageRender[i] + " " + armorRender[i] + " ║");
            }

            subPanels.Add(RenderEmptyLine());

            // Render damage percent and resistance panels
            for (int i = 0; i < offensiveRender.Count(); i++)
            {
                subPanels.Add("║ " + damagePercentRender[i] + " " + resistanceRender[i] + " ║");
            }
            
            return subPanels;
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
            throw new NotImplementedException();
        }
    }
}
