using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Controller.Combat.Interfaces;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// Contains properties used to access information about previously used actions and positions
    /// of the player for each character.
    /// </summary>
    public class UIStateTracker : IUIStateTracker
    {
        private readonly IDisplayCombatState _combatStateHandler;

        public UIStateTracker(IDisplayCombatState combatStateHandler)
        {
            _combatStateHandler = combatStateHandler;
            var playerCharacterIds = _combatStateHandler.GetPlayerCharacterIds();
            ActiveCharacterId = _combatStateHandler.GetNextActivePlayerId();
            _characterDefaults = new Dictionary<int, PlayerCharacterDefaults>();
            foreach (var characterId in playerCharacterIds)
            {
                _characterDefaults.Add(characterId, new PlayerCharacterDefaults());
            }
            ActiveAction = new ActionStore { TargetPositions = new List<int>() };
            ActionPanelList = new List<IDisplayAction>();
        }

        /// <summary>
        /// Contains default values for each player character, allowing the game to remember each action
        /// and target positions the player previously selected per character.
        /// </summary>
        private class PlayerCharacterDefaults
        {
            public int CommandFocusNumber;
            public class SubPanelDefaults
            {
                public int CategoryFocusNumber;
                public int CategoryItemCount;
                public int CategoryLineOffset;
                public int ActionFocusNumber;
                public int ActionPanelItemCount;
                public int ActionPanelLineOffset;
            }
            public int CurrentTargetPosition;
            public IReadOnlyList<int> CurrentTargetPositions;
            public IReadOnlyList<string[]> ActiveCategories;
            public IReadOnlyList<IDisplayAction> ActionPanelList;
            private Dictionary<int, SubPanelDefaults> _subPanelDefaults;
            public PlayerCharacterDefaults()
            {
                CommandFocusNumber = 1;
                CurrentTargetPosition = 13;
                CurrentTargetPositions = new List<int>();
                ActiveCategories = new List<string[]>();
                _subPanelDefaults = new Dictionary<int, SubPanelDefaults>();
            }
            public SubPanelDefaults GetSubPanelDefaults()
            {
                if (!_subPanelDefaults.ContainsKey(CommandFocusNumber))
                {
                    _subPanelDefaults.Add(CommandFocusNumber, new SubPanelDefaults()
                    {
                        CategoryFocusNumber = 1,
                        CategoryItemCount = 0,
                        CategoryLineOffset = 0,
                        ActionFocusNumber = 1,
                        ActionPanelItemCount = 0,
                        ActionPanelLineOffset = 0
                    });
                }
                return _subPanelDefaults[CommandFocusNumber];
            }
        }

        // Keeps default settings for each individual character player so that on consecutive turns, a character retains its last used action
        private Dictionary<int, PlayerCharacterDefaults> _characterDefaults;

        public bool IsInCommandPanel { get; set; }

        /// <summary>
        /// Gets or sets whether the player is in the action panel.
        /// </summary>
        public bool IsInActionPanel { get; set; }

        /// <summary>
        /// Gets or sets whether the player is in the category panel.
        /// </summary>
        public bool IsInCategoryPanel { get; set; }

        /// <summary>
        /// Gets or sets whether the player is in the status command.
        /// </summary>
        public bool IsInStatusCommand { get; set; }

        /// <summary>
        /// Gets or sets whether the player is in the character panel.
        /// </summary>
        public bool IsInCharacterPanel { get; set; }

        /// <summary>
        /// Gets or sets whether the player is in the formation panel.
        /// </summary>
        public bool IsInFormationPanel { get; set; }

        /// <summary>
        /// Gets or sets the Id of the currently active character.
        /// </summary>
        public int ActiveCharacterId { get; set; }

        /// <summary>
        /// Gets the active action.
        /// </summary>
        public ActionStore ActiveAction { get; set; }

        /// <summary>
        /// Gets or sets the focus number for the action menu of the currently active character.
        /// </summary>
        public int CommandFocusNumber
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return 1;
                return _characterDefaults[ActiveCharacterId].CommandFocusNumber;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                    _characterDefaults[ActiveCharacterId].CommandFocusNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the focus number for categories for the currently active character.
        /// </summary>
        public int CategoryFocusNumber
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return 1;
                return _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().CategoryFocusNumber;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                {
                    _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().CategoryFocusNumber = value;
                    ActionFocusNumber = 1;
                }
            }
        }

        /// <summary>
        /// Gets the number of categories that exist in the active action type for the currently active character.
        /// </summary>
        public int CategoryItemCount
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return 0;
                return _characterDefaults[ActiveCharacterId].ActiveCategories[0].Count();
            }
        }

        /// <summary>
        /// Gets or sets the number of lines being displaced from the top of a category list for the currently active character.
        /// </summary>
        public int CategoryLineOffset
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return 0;
                return _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().CategoryLineOffset;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                    _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().CategoryLineOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the focus number for the selected numbers in the subaction menu of the currently active character.
        /// </summary>
        public int ActionFocusNumber
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return 1;
                return _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().ActionFocusNumber;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                    _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().ActionFocusNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of actions that exists in the active category for the currently active character.
        /// </summary>
        public int ActionPanelItemCount
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return 0;
                return _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().ActionPanelItemCount;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                    _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().ActionPanelItemCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of lines being displaced from the top of the action menu of the active category
        /// of the currently active character.
        /// </summary>
        public int ActionPanelLineOffset
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return 0;
                return _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().ActionPanelLineOffset;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                    _characterDefaults[ActiveCharacterId].GetSubPanelDefaults().ActionPanelLineOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the previously used target position for the active character.
        /// </summary>
        public int CurrentTargetPosition
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return 1;
                return _characterDefaults[ActiveCharacterId].CurrentTargetPosition;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                    _characterDefaults[ActiveCharacterId].CurrentTargetPosition = value;
            }
        }

        private IReadOnlyList<int> aiTargetPositions;

        /// <summary>
        /// Gets or sets the previously used target positions for the active character.
        /// </summary>
        public IReadOnlyList<int> CurrentTargetPositions
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId))
                {
                    if (aiTargetPositions == null) return new List<int>();
                    else return aiTargetPositions;
                }
                return _characterDefaults[ActiveCharacterId].CurrentTargetPositions;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                    _characterDefaults[ActiveCharacterId].CurrentTargetPositions = value;
                else
                    aiTargetPositions = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of category names for the currently active command type.
        /// </summary>
        public IReadOnlyList<string[]> ActionCategories
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return new List<string[]>();
                return _characterDefaults[ActiveCharacterId].ActiveCategories;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                {
                    _characterDefaults[ActiveCharacterId].ActiveCategories = value;
                }
            }
        }

        /// <summary>
        /// Gets the currently selected category from the active character.
        /// </summary>
        public string ActiveCategory
        {
            get
            {
                if (ActionCategories.Count() == 0) return "";
                return ActionCategories[CategoryFocusNumber - 1][0];
            }
        }

        /// <summary>
        /// Gets or sets the list of actions associated with the currently active Command and Category.
        /// </summary>
        public IReadOnlyList<IDisplayAction> ActionPanelList
        {
            get
            {
                if (!_characterDefaults.ContainsKey(ActiveCharacterId)) return new List<IDisplayAction>();
                return _characterDefaults[ActiveCharacterId].ActionPanelList;
            }
            set
            {
                if (_characterDefaults.ContainsKey(ActiveCharacterId))
                {
                    _characterDefaults[ActiveCharacterId].ActionPanelList = value;
                    ActionPanelItemCount = value.Count();
                }
            }
        }

        /// <summary>
        /// Gets whether or not it is the player's turn.
        /// </summary>
        public bool IsPlayerTurn { get { return _combatStateHandler.IsPlayerTurn(); } }
    }
}
