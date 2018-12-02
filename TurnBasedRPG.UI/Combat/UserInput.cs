using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Combat;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// Class that handles user input for the user interface in combat.
    /// </summary>
    public class UserInput
    {
        private readonly DefaultsHandler _defaultsHandler;
        private readonly UIContainer _uiContainer;
        private readonly UICharacterManager _uiCharacterManager;
        private readonly GameUIConstants _gameUIConstants;

        public UserInput(DefaultsHandler defaultsHandler,
                         UIContainer uiContainer,
                         UICharacterManager uiCharacterManager,
                         GameUIConstants gameUIConstants)
        {
            _defaultsHandler = defaultsHandler;
            _uiContainer = uiContainer;
            _uiCharacterManager = uiCharacterManager;
            _gameUIConstants = gameUIConstants;
        }
        
        /// <summary>
        /// Event called whenever the player selects an action from an action list.
        /// </summary>
        public EventHandler<ActionSelectedEventArgs> ActionSelectedEvent;

        /// <summary>
        /// Event called whenever the UI should update the action list.
        /// </summary>
        public EventHandler<UpdateActionListEventArgs> UpdateActionListEvent;

        /// <summary>
        /// Event called whenever the UI should update the categories list.
        /// </summary>
        public EventHandler<UpdateCategoriesEventArgs> UpdateCategoriesEvent;

        /// <summary>
        /// Event called whenever the player has chosen an action with its targets.
        /// </summary>
        public EventHandler<ActionStartedEventArgs> ActionStartedEvent;

        /// <summary>
        /// Event called whenever the player presses any key.
        /// </summary>
        public EventHandler<KeyPressedEventArgs> KeyPressEvent;

        /// <summary>
        /// Starts an infinite loop to listen for key presses.
        /// </summary>
        public void ListenForKeyPress()
        {
            while (true)
            {
                if (Console.KeyAvailable && !_uiContainer.IsUIUpdating)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    KeyPressEvent?.Invoke(this, new KeyPressedEventArgs()
                    {
                        PressedKey = keyInfo
                    });

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                            UpArrowKeyPressed();
                            break;
                        case ConsoleKey.DownArrow:
                            DownArrowKeyPressed();
                            break;
                        case ConsoleKey.LeftArrow:
                            LeftArrowKeyPressed();
                            break;
                        case ConsoleKey.RightArrow:
                            RightArrowKeyPressed();
                            break;
                        case ConsoleKey.Enter:
                            EnterKeyPressed();
                            break;
                        case ConsoleKey.Escape:
                            EscapeKeyPressed();
                            break;
                        default:
                            break;
                    }
                    ClearInputBuffer();
                }
            }
        }

        /// <summary>
        /// Loops and listens for the player to skip UI updating. If the UI updating finishes
        /// without the user skipping, then exit the loop.
        /// </summary>
        internal void ListenForUISkip()
        {
            while (!_uiContainer.SkipUIUpdating && !_uiContainer.IsUIUpdatingFinished)
            {
                bool consumedInput = false;
                if (Console.KeyAvailable && !consumedInput)
                {
                    consumedInput = true;
                    ConsoleKeyInfo keyinfo = Console.ReadKey(false);
                    if (keyinfo.Key == ConsoleKey.Enter)
                    {
                        _uiContainer.SkipUIUpdating = true;
                    }
                }
            }
            ClearInputBuffer();
        }

        internal void ClearInputBuffer()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Handles any actions that occurs after the escape key is pressed.
        /// </summary>
        private void EscapeKeyPressed()
        {
            if (_defaultsHandler.IsInFormationPanel)
            {
                _defaultsHandler.IsInFormationPanel = false;
                _defaultsHandler.IsInActionPanel = true;
            }
            else if (_defaultsHandler.IsInActionPanel && (Commands)_defaultsHandler.CommandFocusNumber != Commands.Attack)
            {
                _defaultsHandler.IsInActionPanel = false;
                _defaultsHandler.IsInCategoryPanel = true;
            }
            else if (_defaultsHandler.IsInActionPanel && (Commands)_defaultsHandler.CommandFocusNumber == Commands.Attack)
            {
                _defaultsHandler.IsInActionPanel = false;
            }
            else
                _defaultsHandler.IsInCategoryPanel = false;
            _uiContainer.PrintUI();
        }

        /// <summary>
        /// Handles any actions that occurs after the enter key is pressed.
        /// </summary>
        private void EnterKeyPressed()
        {
            // If the player is in the command panel
            if (!_defaultsHandler.IsInActionPanel
                && !_defaultsHandler.IsInFormationPanel
                && !_defaultsHandler.IsInCategoryPanel)
            {
                switch ((Commands)_defaultsHandler.CommandFocusNumber)
                {
                    case Commands.Attack:
                        _defaultsHandler.IsInActionPanel = true;
                        UpdateActionListEvent?.Invoke(this, new UpdateActionListEventArgs()
                        {
                            CommandFocus = (Commands)_defaultsHandler.CommandFocusNumber,
                            CategoryName = _defaultsHandler.ActiveCategory
                        });
                        break;
                    case Commands.Spells:
                    case Commands.Skills:
                    case Commands.Items:
                        if (_defaultsHandler.CategoryItemCount > 0)
                            _defaultsHandler.IsInCategoryPanel = true;
                        break;
                    case Commands.Wait:
                        ActionStartedEvent?.Invoke(this, new ActionStartedEventArgs()
                        {
                            ActionType = Commands.Wait,
                            CategoryName = "",
                            ActionIndex = -1,
                            TargetPositions = null
                        });
                        break;
                }
            }
            // If the player is in the formation panel, start an action
            else if (_defaultsHandler.IsInFormationPanel)
            {

                var targets = CombatTargeter.GetTranslatedTargetPositions(_defaultsHandler.ActiveAction.TargetPositions,
                                                                          _defaultsHandler.ActiveAction.CenterOfTargets,
                                                                          _defaultsHandler.ActiveAction.CanSwitchTargetPosition
                                                                              || !_uiContainer.IsPlayerTurn,
                                                                          _defaultsHandler.CurrentTargetPosition);

                ActionStartedEvent?.Invoke(this, new ActionStartedEventArgs()
                {
                    ActionType = (Commands)_defaultsHandler.CommandFocusNumber,
                    CategoryName = _defaultsHandler.ActiveCategory,
                    ActionIndex = _defaultsHandler.ActionFocusNumber - 1,
                    TargetPositions = targets
                });
            }
            // If the player is in the action panel, switch to the formation panel
            else if (_defaultsHandler.IsInActionPanel)
            {
                switch ((Commands)_defaultsHandler.CommandFocusNumber)
                {
                    case Commands.Attack:
                    case Commands.Spells:
                    case Commands.Skills:
                    case Commands.Items:
                        _defaultsHandler.IsInFormationPanel = true;
                        ActionSelectedEvent?.Invoke(this, new ActionSelectedEventArgs()
                        {
                            CommandFocus = (Commands)_defaultsHandler.CommandFocusNumber,
                            CategoryName = _defaultsHandler.ActiveCategory,
                            ActionFocusNumber = _defaultsHandler.ActionFocusNumber
                        });
                        break;
                    default:
                        break;
                }
            }
            else if (_defaultsHandler.IsInCategoryPanel)
            {
                // Get all spells or skills in category
                UpdateActionListEvent?.Invoke(this, new UpdateActionListEventArgs()
                {
                    CommandFocus = (Commands)_defaultsHandler.CommandFocusNumber,
                    CategoryName = _defaultsHandler.ActiveCategory
                });

                _defaultsHandler.IsInActionPanel = true;
                _defaultsHandler.IsInCategoryPanel = false;
            }

            _uiContainer.PrintUI();
        }

        /// <summary>
        /// Handles actions when the down key is pressed.
        /// </summary>
        private void DownArrowKeyPressed()
        {
            int itemsPerFormationRow = _gameUIConstants.CharactersPerFormationRow;
            int itemsPerActionRow = _gameUIConstants.ItemsPerActionPanelRow;

            // If the player is in the formation panel, check to see if downwards movement is blocked
            bool isBlocked = _defaultsHandler.IsInFormationPanel && IsDownArrowBlocked();
            if (_defaultsHandler.IsInFormationPanel 
                && !isBlocked 
                && ((_defaultsHandler.CurrentTargetPosition <= 6 && _defaultsHandler.CurrentTargetPosition > 0) 
                    || (_defaultsHandler.CurrentTargetPosition <= 15 && _defaultsHandler.CurrentTargetPosition > 9)))
            {
                ChangeFormationPanelFocus(itemsPerFormationRow);
            }
            // If the player is in the action panel and not at the bottom-most row of the action panel
            else if (!_defaultsHandler.IsInFormationPanel 
                     && _defaultsHandler.IsInActionPanel 
                     && _defaultsHandler.ActionFocusNumber + 2 <= _defaultsHandler.ActionPanelItemCount)
            {
                ChangeActionPanelFocus(itemsPerActionRow, true);
            }
            // If the player is in the category panel and not at the bottom-most row of the category panel
            else if (_defaultsHandler.IsInCategoryPanel 
                     && _defaultsHandler.CategoryFocusNumber + 2 <= _defaultsHandler.CategoryItemCount)
            {
                ChangeCategoryFocus(itemsPerActionRow, true);
            }
            // If the player is in the command panel
            else if (!_defaultsHandler.IsInActionPanel 
                     && !_defaultsHandler.IsInFormationPanel 
                     && !_defaultsHandler.IsInCategoryPanel)
            {
                ChangeCommandFocus(1);
            }
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
                isBlocked = isBlocked && !(character == null || character.GetCurrenthealth() == 0);
            }
                
            return isBlocked;
        }

        /// <summary>
        /// Handles actions whenever the left arrow key is pressed by the player.
        /// </summary>
        private void LeftArrowKeyPressed()
        {
            // If the player is in the formation panel, but not at the left-most column in the player's formation
            if (_defaultsHandler.IsInFormationPanel 
                && ((_defaultsHandler.CurrentTargetPosition % 3 != 1 
                    && _defaultsHandler.CurrentTargetPosition < 10) 
                    || _defaultsHandler.CurrentTargetPosition >= 10))
            {
                // If the player moves from the enemy's formation to the player's formation
                if (_defaultsHandler.CurrentTargetPosition % 3 == 1 && _defaultsHandler.CurrentTargetPosition >= 10)
                    ChangeFormationPanelFocus(-7);
                else
                    ChangeFormationPanelFocus(-1);
            }
            // If the player is in the action panel and on the right column
            else if (!_defaultsHandler.IsInFormationPanel 
                     && _defaultsHandler.IsInActionPanel 
                     && _defaultsHandler.ActionFocusNumber % 2 == 0)
            {
                ChangeActionPanelFocus(-1, false);
            }
            // If the player is in the category panel and on the right column
            else if (_defaultsHandler.IsInCategoryPanel && _defaultsHandler.CategoryFocusNumber % 2 == 0)
            {
                ChangeCategoryFocus(-1, false);
            }
        }

        /// <summary>
        /// Handles actions whenever the right arrow key is pressed by the player.
        /// </summary>
        private void RightArrowKeyPressed()
        {
            // How many spaces to move from one formation to the other, keeping the row the same
            // Equal to moving 2 whole rows + 1 
            int formationMoveOffset = _gameUIConstants.CharactersPerFormationRow * 2 + 1;
            bool isBlocked = IsRightArrowBlocked();

            // If the player is in the formation panel and not on the right-most column of the enemy's formation,
            // and not blocked by any enemy units if the current action is blockable
            if (_defaultsHandler.IsInFormationPanel 
                                && !isBlocked 
                                && ((_defaultsHandler.CurrentTargetPosition % 3 != 0 && _defaultsHandler.CurrentTargetPosition >= 10) 
                                || _defaultsHandler.CurrentTargetPosition < 10))
            {
                // If the player is moving from the player's formation to the enemy's formation
                if (_defaultsHandler.CurrentTargetPosition % 3 == 0 && _defaultsHandler.CurrentTargetPosition < 10)
                    ChangeFormationPanelFocus(7);
                else
                    ChangeFormationPanelFocus(1);
            }
            // If the player is in the action panel and on the left-most column
            else if (!_defaultsHandler.IsInFormationPanel 
                     && _defaultsHandler.IsInActionPanel 
                     && _defaultsHandler.ActionFocusNumber % 2 == 1 
                     && _defaultsHandler.ActionFocusNumber + 1 <= _defaultsHandler.ActionPanelItemCount)
            {
                ChangeActionPanelFocus(1, false);
            }
            // If the player is in the category panel and on the left-most column
            else if (_defaultsHandler.IsInCategoryPanel 
                     && _defaultsHandler.CategoryFocusNumber % 2 == 1 
                     && _defaultsHandler.CategoryFocusNumber + 1 <= _defaultsHandler.CategoryItemCount)
                ChangeCategoryFocus(1, false);
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
            if (character == null || character.GetCurrenthealth() == 0) return false;

            return true;
        }

        /// <summary>
        /// Handles actions whenever the right arrow key is pressed by the player.
        /// </summary>
        private void UpArrowKeyPressed()
        {
            int itemsPerFormationRow = _gameUIConstants.CharactersPerFormationRow;
            int itemsPerActionRow = _gameUIConstants.ItemsPerActionPanelRow;

            bool isBlocked = IsUpArrowBlocked();
            // If the player is in the formation panel and not in the top-most rows of the formations and upwards movement is not blocked
            if (_defaultsHandler.IsInFormationPanel 
                && !isBlocked 
                && ((_defaultsHandler.CurrentTargetPosition > 3 && _defaultsHandler.CurrentTargetPosition <= 9) 
                    || _defaultsHandler.CurrentTargetPosition > 12))
            {
                ChangeFormationPanelFocus(-itemsPerFormationRow);
            }
            // If the player is in the action panel and not in the top-most row
            else if (!_defaultsHandler.IsInFormationPanel 
                     && _defaultsHandler.IsInActionPanel 
                     && _defaultsHandler.ActionFocusNumber > 2)
            {
                ChangeActionPanelFocus(-itemsPerActionRow, true);
            }
            // If the player is in the category panel and not in the top-most row
            else if (_defaultsHandler.IsInCategoryPanel 
                     && _defaultsHandler.CategoryFocusNumber > 2)
            {
                ChangeCategoryFocus(-itemsPerActionRow, true);
            }
            // If the player is in the command panel
            else if (!_defaultsHandler.IsInActionPanel 
                     && !_defaultsHandler.IsInFormationPanel 
                     && !_defaultsHandler.IsInCategoryPanel)
                ChangeCommandFocus(-1);
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
                                                                                    || !_uiContainer.IsPlayerTurn,
                                                                                _defaultsHandler.CurrentTargetPosition);

                _defaultsHandler.CurrentTargetPositions = targetPositions;
                _uiContainer.PrintUI();
            }
        }

        /// <summary>
        /// Changes the category focus if change amount is not 0. If mayChangeOffset is true, also checks to see if movement excedes the
        /// maximum number of items in a panel.
        /// </summary>
        /// <param name="changeAmount">The amount to change the focus by.</param>
        /// <param name="mayChangeOffset">Whether or not the change in focus may go pass the maximum items in the category panel.</param>
        private void ChangeCategoryFocus(int changeAmount, bool mayChangeOffset)
        {
            int itemsPerActionRow = _gameUIConstants.ItemsPerActionPanelRow;

            if (_defaultsHandler.IsInCategoryPanel && changeAmount != 0)
            {
                _defaultsHandler.CategoryFocusNumber += changeAmount;
                // Check if the category focus number went out of bounds
                if (_defaultsHandler.CategoryFocusNumber > _defaultsHandler.ActionCategories.Count())
                {
                    _defaultsHandler.CategoryFocusNumber = _defaultsHandler.ActionCategories.Count();
                }
                if (_defaultsHandler.CategoryFocusNumber < 1)
                {
                    _defaultsHandler.CategoryFocusNumber = 1;
                }
                // If down key was pressed and the current position is the bottom-most row
                if (mayChangeOffset && 
                    _defaultsHandler.CategoryFocusNumber > _defaultsHandler.ActionCategories[0].Count() 
                        - itemsPerActionRow + _defaultsHandler.CategoryLineOffset * itemsPerActionRow)
                {
                    _defaultsHandler.CategoryLineOffset++;
                    // Potentially useless code
                    if (_defaultsHandler.CategoryLineOffset * itemsPerActionRow + _defaultsHandler.ActionCategories[0].Count() 
                        >= _defaultsHandler.CategoryFocusNumber)
                    {
                        _defaultsHandler.CategoryLineOffset--;
                    }
                }
                // If up key was pressed and the current position is in the 2nd from the top row
                if (_defaultsHandler.CategoryFocusNumber <= 4 + _defaultsHandler.CategoryLineOffset * 2 
                        && _defaultsHandler.CategoryLineOffset >= 1 
                        && mayChangeOffset) 
                {
                    _defaultsHandler.CategoryLineOffset--;
                }
                _defaultsHandler.ActionFocusNumber = 1;

                _uiContainer.PrintUI();
            }
        }

        /// <summary>
        /// Changes the subpanel focus if change amount is not 0. If mayChangeOffset is true, also checks to see if movement excedes the
        /// maximum number of items in a panel.
        /// </summary>
        /// <param name="changeAmount">The amount to change the focus by.</param>
        /// <param name="mayChangeOffset">Whether or not the change in focus may go pass the maximum items in the subpanel.</param>
        private void ChangeActionPanelFocus(int changeAmount, bool mayChangeOffset)
        {
            int itemsPerActionPanelRow = _gameUIConstants.ItemsPerActionPanelRow;
            if (_defaultsHandler.IsInActionPanel && changeAmount != 0)
            {
                _defaultsHandler.ActionFocusNumber += changeAmount;
                // Prevent the focus number from increasing pass the number of items in the action panel
                if (_defaultsHandler.ActionFocusNumber > _defaultsHandler.ActionPanelList.Count())
                {
                    _defaultsHandler.ActionFocusNumber = _defaultsHandler.ActionPanelList.Count();
                }
                if (_defaultsHandler.ActionFocusNumber < 1) _defaultsHandler.ActionFocusNumber = 1;

                // If down key was pressed and the current position is the bottom-most row and if offset might change
                if (_defaultsHandler.ActionFocusNumber
                    > _defaultsHandler.ActionPanelItemCount - itemsPerActionPanelRow 
                    + _defaultsHandler.ActionPanelLineOffset * itemsPerActionPanelRow 
                    && mayChangeOffset)
                {
                    _defaultsHandler.ActionPanelLineOffset++;
                    // redundant code?
                    if (_defaultsHandler.ActionPanelLineOffset * 2 + _defaultsHandler.ActionPanelItemCount >= _defaultsHandler.ActionFocusNumber)
                        _defaultsHandler.ActionPanelLineOffset--;
                }
                // If up key was pressed and the current position is in the 2nd from the top row
                if (_defaultsHandler.ActionFocusNumber <= 2 * itemsPerActionPanelRow 
                      + _defaultsHandler.ActionPanelLineOffset * itemsPerActionPanelRow 
                        && _defaultsHandler.ActionPanelLineOffset >= 1 
                        && mayChangeOffset)
                {
                    _defaultsHandler.ActionPanelLineOffset--;
                }

                _uiContainer.PrintUI();
            }
        }

        /// <summary>
        /// Changes the focus of the command panel by an amount.
        /// </summary>
        /// <param name="changeAmount">The amount to change the focus by.</param>
        private void ChangeCommandFocus(int changeAmount)
        {
            // If the player is in the command panel
            if (!_defaultsHandler.IsInFormationPanel 
                && !_defaultsHandler.IsInActionPanel 
                && changeAmount != 0)
            {
                int maxFocusNumber = Enum.GetValues(typeof(Commands)).Cast<int>().Max();
                _defaultsHandler.CommandFocusNumber += changeAmount;

                // Going to the past the bottom of the command panel takes you back to the top, vice versa
                if (_defaultsHandler.CommandFocusNumber <= 0) _defaultsHandler.CommandFocusNumber = maxFocusNumber;
                if (_defaultsHandler.CommandFocusNumber > maxFocusNumber) _defaultsHandler.CommandFocusNumber = 1;

                UpdateCategoriesEvent?.Invoke(this, new UpdateCategoriesEventArgs()
                {
                    CommandFocus = (Commands)_defaultsHandler.CommandFocusNumber
                });

                _defaultsHandler.CategoryItemCount = _defaultsHandler.ActionCategories.Count();
                _uiContainer.PrintUI();
            }
        }
    }
}
