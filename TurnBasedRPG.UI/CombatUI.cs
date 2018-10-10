using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.EventArgs;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Controller;

namespace TurnBasedRPG.UI
{
    /// <summary>
    /// UI controller responsible for handling UI rendering logic.
    /// </summary>
    public class CombatUI
    {
        /// <summary>
        /// Contains default values for each player character, allowing the game to remember each action
        /// and target positions the player previously selected per character.
        /// </summary>
        private class PlayerCharacterDefaults
        {
            public int FocusNumber;
            public class SubPanelDefaults
            {
                public int CategoryFocusNumber;
                public int CategoryItemCount;
                public int CategoryLineOffset;
                public int SubFocusNumber;
                public int SubPanelItemCount;
                public int SubPanelLineOffset;
            }
            public int DefaultTargetPosition;
            public IReadOnlyList<string[]> ActiveCategories;
            public IReadOnlyList<IDisplayAction> ActiveSubPanelList;
            private Dictionary<int, SubPanelDefaults> _subPanelDefaults;
            public PlayerCharacterDefaults()
            {
                FocusNumber = 1;
                DefaultTargetPosition = 13;
                ActiveCategories = new List<string[]>();
                _subPanelDefaults = new Dictionary<int, SubPanelDefaults>();
            }
            public SubPanelDefaults GetSubPanelDefaults()
            {
                if (!_subPanelDefaults.ContainsKey(FocusNumber))
                {
                    _subPanelDefaults.Add(FocusNumber, new SubPanelDefaults()
                    {
                        CategoryFocusNumber = 1,
                        CategoryItemCount = 0,
                        CategoryLineOffset = 0,
                        SubFocusNumber = 1,
                        SubPanelItemCount = 0,
                        SubPanelLineOffset = 0
                    });
                }
                return _subPanelDefaults[FocusNumber];
            }
        }
        // Keeps default settings for each individual character player so that on consecutive turns, a character retains its last used action
        private Dictionary<int, PlayerCharacterDefaults> _characterDefaults;
        private int _activeCharacterId;
        private int FocusNumber
        {
            get { return _characterDefaults[_activeCharacterId].FocusNumber; }
            set { _characterDefaults[_activeCharacterId].FocusNumber = value; }
        }
        private int CategoryFocusNumber
        {
            get { return _characterDefaults[_activeCharacterId].GetSubPanelDefaults().CategoryFocusNumber; }
            set { _characterDefaults[_activeCharacterId].GetSubPanelDefaults().CategoryFocusNumber = value; }
        }
        private int CategoryItemCount
        {
            get { return _characterDefaults[_activeCharacterId].GetSubPanelDefaults().CategoryItemCount; }
            set { _characterDefaults[_activeCharacterId].GetSubPanelDefaults().CategoryItemCount = value; }
        }
        private int CategoryLineOffset
        {
            get { return _characterDefaults[_activeCharacterId].GetSubPanelDefaults().CategoryLineOffset; }
            set { _characterDefaults[_activeCharacterId].GetSubPanelDefaults().CategoryLineOffset = value; }
        }
        private int SubFocusNumber
        {
            get { return _characterDefaults[_activeCharacterId].GetSubPanelDefaults().SubFocusNumber; }
            set { _characterDefaults[_activeCharacterId].GetSubPanelDefaults().SubFocusNumber = value; }
        }
        private int SubPanelItemCount
        {
            get { return _characterDefaults[_activeCharacterId].GetSubPanelDefaults().SubPanelItemCount; }
            set { _characterDefaults[_activeCharacterId].GetSubPanelDefaults().SubPanelItemCount = value; }
        }
        private int SubPanelLineOffset
        {
            get { return _characterDefaults[_activeCharacterId].GetSubPanelDefaults().SubPanelLineOffset; }
            set { _characterDefaults[_activeCharacterId].GetSubPanelDefaults().SubPanelLineOffset = value; }
        }
        private int DefaultTargetPosition
        {
            get { return _characterDefaults[_activeCharacterId].DefaultTargetPosition; }
            set { _characterDefaults[_activeCharacterId].DefaultTargetPosition = value; }
        }
        private IReadOnlyList<string[]> ActionCategories
        {
            get { return _characterDefaults[_activeCharacterId].ActiveCategories; }
            set { _characterDefaults[_activeCharacterId].ActiveCategories = value; }
        }
        private string ActiveCategory
        {
            get
            {
                if (ActionCategories.Count() == 0) return "";
                return ActionCategories[CategoryFocusNumber - 1][0];
            }
        }
        private IReadOnlyList<IDisplayAction> ActiveSubPanelList
        {
            get { return _characterDefaults[_activeCharacterId].ActiveSubPanelList; }
            set { _characterDefaults[_activeCharacterId].ActiveSubPanelList = value; }
        }
        private bool IsInSubPanel = false;
        private bool IsInCategory = false;
        private bool _isInFormationPanel;
        private bool IsInFormationPanel
        {
            get { return _isInFormationPanel; }
            set
            {
                _isInFormationPanel = value;
                if (_formationUI != null)
                    _formationUI.IsInFormationPanel = value;
            }
        }
        // Contains the default target positions for an action
        private IReadOnlyList<int> _tempTargetPositions;
        private int _centerOfTargetsPosition;
        private bool _canSwitchTargetPosition;
        private bool _canTargetThroughUnits;
        
        private CombatController _combatInstance;
        private CombatFormationUI _formationUI;
        private TargetUI _targetUI;
        private ActionPanelUI _actionPanel;
        private SubActionPanelUI _subActionPanel;
        private TurnOrderUI _turnOrder;
        private InformationPanelUI _informationPanel;

        public CombatUI(CombatController combatInstance,
                        CombatFormationUI formationUI,
                        TargetUI targetUI,
                        ActionPanelUI actionPanel,
                        SubActionPanelUI subActionPanelUI,
                        TurnOrderUI turnOrder,
                        InformationPanelUI informationPanel)
        {
            _combatInstance = combatInstance;
            _characterDefaults = new Dictionary<int, PlayerCharacterDefaults>();
            foreach (var characterId in _combatInstance.GetPlayerCharacterIds())
            {
                _characterDefaults.Add(characterId, new PlayerCharacterDefaults());
            }
            _activeCharacterId = _combatInstance.GetNextActivePlayerId();
            _combatInstance.EndOfTurn += EndOfTurnTriggered;
            _formationUI = formationUI;
            _targetUI = targetUI;
            _turnOrder = turnOrder;
            _actionPanel = actionPanel;
            _subActionPanel = subActionPanelUI;
            _informationPanel = informationPanel;
            GetActiveSubPanelList();
        }

        private void EndOfTurnTriggered(object sender, EndOfTurnEventArgs args)
        {
            _activeCharacterId = _combatInstance.GetNextActivePlayerId();
            IsInSubPanel = false;
            IsInFormationPanel = false;
            GetActiveSubPanelList();
            RefreshUI();
        }

        /// <summary>
        /// Begins the combat UI.
        /// </summary>
        public void StartRender()
        {
            // First render
            RefreshUI();

            ListenForKeyPress();
        }

        /// <summary>
        /// Called on updates to refresh the UI.
        /// </summary>
        private void RefreshUI()
        {
            Console.Clear();
            PrintTargetAndTurnOrder();
            PrintCombatFormations();
            PrintUserPanels();
        }

        /// <summary>
        /// Handles actions selected by the player depending on which panels are currently active.
        /// </summary>
        private void HandleAction()
        {
            if (!IsInSubPanel && !IsInFormationPanel && !IsInCategory)
            {
                switch ((Actions)FocusNumber)
                {
                    case Actions.Attack:
                        IsInSubPanel = true;
                        GetActiveSubPanelList();
                        break;
                    case Actions.Spells:
                    case Actions.Skills:
                    case Actions.Items:
                        if (CategoryItemCount > 0)
                            IsInCategory = true;
                        break;
                    case Actions.Pass:
                        _combatInstance.StartAction(Actions.Pass, "", -1 , null);
                        break;
                }
            }
            else if (IsInFormationPanel)
            {

                if (GetCorrectedTargets().Any(position => _combatInstance.IsPositionOccupied(position)))
                    _combatInstance.StartAction((Actions)FocusNumber, ActiveCategory, SubFocusNumber - 1, _formationUI.TargetPositions);
            }
            else if (IsInSubPanel)
            {
                switch ((Actions)FocusNumber)
                {
                    case Actions.Attack:
                    case Actions.Spells:
                    case Actions.Skills:
                    case Actions.Items:
                        SelectSubAction();
                        break;
                    default:
                        break;
                }
            }
            else if (IsInCategory)
            {
                // Get all spells or skills in category
                GetActiveSubPanelList();
                IsInSubPanel = true;
                IsInCategory = false;
            }

            RefreshUI();
        }

        /// <summary>
        /// Gets and handles the selection of a subaction from the subpanel menu.
        /// </summary>
        private void SelectSubAction()
        {
            IDisplayAction selectedAction;
            selectedAction = _combatInstance.GetSubActionFromCategory((Actions)FocusNumber, ActiveCategory, SubFocusNumber - 1);
            _tempTargetPositions = selectedAction.GetActionTargets();
            _centerOfTargetsPosition = selectedAction.GetCenterOfTargetsPosition();
            _canSwitchTargetPosition = selectedAction.CanSwitchTargetPosition;
            _canTargetThroughUnits = selectedAction.CanTargetThroughUnits;
            if (!_canTargetThroughUnits) DefaultTargetPosition = 13;
            _formationUI.TargetPositions = GetCorrectedTargets();
            IsInFormationPanel = true;
        }

        /// <summary>
        /// Determines which targets an action will hit, depending on the displacement from the target position and where the action's
        /// center of target is.
        /// </summary>
        /// <returns>A readonly list of the corrected targets.</returns>
        private IReadOnlyList<int> GetCorrectedTargets()
        {
            // In case of static targeting actions, change the default position to a static one
            if (!_canSwitchTargetPosition)
            {
                DefaultTargetPosition = 13;
                return _tempTargetPositions;
            }
            else
            {
                var correctedTargets = new List<int>(_tempTargetPositions);
                if (DefaultTargetPosition > 18 || DefaultTargetPosition <= 0)
                    DefaultTargetPosition = 13;
                
                int targetOffset = DefaultTargetPosition - _centerOfTargetsPosition;
                int invertedOffset = 9;

                // If the target is the enemy's side of the field, offset the positions to account for it
                if (DefaultTargetPosition > 9)
                {
                    for (int i = 0; i < correctedTargets.Count; i++)
                    {
                        correctedTargets[i] += 9;
                    }
                    targetOffset -= 9;
                    invertedOffset = 0;
                }

                // Remove positions depending on where the center of targets position and default target positions are at.
                // Grid system is 3 x 3 and starts at position 1, so to check if a position is in the right column, you can
                // do position % 3 == 0.

                // If the player's center of target is in the right column and the center of target for an action is not the 
                // right column, remove all targets in the right column
                if (DefaultTargetPosition % 3 == 0 && _centerOfTargetsPosition % 3 != 0)
                    correctedTargets.RemoveAll(val => val % 3 == 0);
                // Target position is in the left column so all targets in the left column must be culled
                if (DefaultTargetPosition % 3 == 1 && _centerOfTargetsPosition % 3 != 1)
                    correctedTargets.RemoveAll(val => val % 3 == 1);
                // Target position is in the top row
                if (DefaultTargetPosition < 13 - invertedOffset && _centerOfTargetsPosition >= 4)
                    correctedTargets.RemoveAll(val => val < 13 - invertedOffset);
                // Target position is in the bottom row
                if (DefaultTargetPosition > 15 - invertedOffset && _centerOfTargetsPosition <= 6)
                    correctedTargets.RemoveAll(val => val > 15 - invertedOffset);

                // If action's CoT is in left column and the player's CoT is right column, remove center column
                if (_centerOfTargetsPosition % 3 == 1 && DefaultTargetPosition % 3 == 0)
                    correctedTargets.RemoveAll(val => val % 3 == 2);
                // If action's CoT is in left column and the player's CoT is middle column, remove right column
                if (_centerOfTargetsPosition % 3 == 1 && DefaultTargetPosition % 3 == 2)
                    correctedTargets.RemoveAll(val => val % 3 == 0);
                // If action's CoT is in right column and the player's CoT is middle column, remove left column
                if (_centerOfTargetsPosition % 3 == 0 && DefaultTargetPosition % 3 == 2)
                    correctedTargets.RemoveAll(val => val % 3 == 1);
                // If action's CoT is in right column and the player's CoT is left column, remove center column
                if (_centerOfTargetsPosition % 3 == 0 && DefaultTargetPosition % 3 == 1)
                    correctedTargets.RemoveAll(val => val % 3 == 2);
                // If action's CoT is in bottom row and player's CoT is in top row on enemy's side, remove middle row
                if (_centerOfTargetsPosition >= 7 && DefaultTargetPosition <= 12 && DefaultTargetPosition >= 10)
                    correctedTargets.RemoveAll(val => val > 12 && val <= 15);
                // If action's CoT is in bottom row and player's CoT is in middle row on enemy's side, remove top row
                if (_centerOfTargetsPosition >= 7 && DefaultTargetPosition <= 15 && DefaultTargetPosition >= 13)
                    correctedTargets.RemoveAll(val => val >= 10 && val < 13);
                // If action's CoT is in top row and player's CoT is in middle row on enemy's side, remove bottom row
                if (_centerOfTargetsPosition <= 3 && DefaultTargetPosition <= 15 && DefaultTargetPosition >= 13)
                    correctedTargets.RemoveAll(val => val >= 16);
                // If action's CoT is in top row and player's CoT is in bottom row on enemy's side, remove middle row
                if (_centerOfTargetsPosition <= 3 && DefaultTargetPosition >= 16)
                    correctedTargets.RemoveAll(val => val >= 13 && val < 16);

                for (int i = 0; i < correctedTargets.Count; i++)
                {
                    correctedTargets[i] += targetOffset;
                }

                return correctedTargets;
            }
        }

        /// <summary>
        /// Starts an infinite loop to listen for key presses.
        /// </summary>
        private void ListenForKeyPress()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
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
                }
            }
        }
        
        private void EscapeKeyPressed()
        {
            if (IsInFormationPanel)
            {
                IsInFormationPanel = false;
                IsInSubPanel = true;
            }
            else if (IsInSubPanel && (Actions)FocusNumber != Actions.Attack)
            {
                IsInSubPanel = false;
                IsInCategory = true;
            }
            else if (IsInSubPanel && (Actions)FocusNumber == Actions.Attack)
            {
                IsInSubPanel = false;
            }
            else
                IsInCategory = false;
            RefreshUI();
        }

        private void EnterKeyPressed()
        {
            HandleAction();
        }

        private void DownArrowKeyPressed()
        {
            bool isBlocked = IsDownArrowBlocked();
            if (IsInFormationPanel && !isBlocked &&
                                ((DefaultTargetPosition <= 6 && DefaultTargetPosition > 0) ||
                                (DefaultTargetPosition <= 15 && DefaultTargetPosition > 9)))
                ChangeFormationPanelFocus(3);
            else if (!IsInFormationPanel && IsInSubPanel && SubFocusNumber + 2 <= SubPanelItemCount)
            {
                ChangeSubPanelFocus(2, true);
            }
            else if (IsInCategory && CategoryFocusNumber + 2 <= CategoryItemCount)
            {
                ChangeCategoryFocus(2, true);
            }
            else if (!IsInSubPanel && !IsInFormationPanel && !IsInCategory)
            {
                ChangeActionFocus(1);
            }
        }
        
        /// <summary>
        /// Returns true if an action cannot bypass a character blocking it's path downwards.
        /// </summary>
        /// <returns></returns>
        private bool IsDownArrowBlocked()
        {
            bool isBlocked = !_canTargetThroughUnits;
            // Action can only be blocked when not on the player's side of the field and not in the enemy's first column
            isBlocked = isBlocked && DefaultTargetPosition % 3 != 1 && DefaultTargetPosition >= 10;
            // If target position is on the last column, check for
            if (DefaultTargetPosition % 3 == 0 && isBlocked)
                isBlocked = _combatInstance.IsPositionOccupied(DefaultTargetPosition + 2, false) ||
                                _combatInstance.IsPositionOccupied(DefaultTargetPosition + 1, false);
            else
                // Action can only be blocked if there is a character in front of the spot the player is trying to reach
                isBlocked = isBlocked && _combatInstance.IsPositionOccupied(DefaultTargetPosition + 2, false);
            return isBlocked;
        }

        private void LeftArrowKeyPressed()
        {
            if (IsInFormationPanel &&
                                ((DefaultTargetPosition % 3 != 1 && DefaultTargetPosition < 10) || DefaultTargetPosition >= 10))
            {
                if (DefaultTargetPosition % 3 == 1 && DefaultTargetPosition >= 10)
                    ChangeFormationPanelFocus(-7);
                else
                    ChangeFormationPanelFocus(-1);
            }
            else if (!IsInFormationPanel && IsInSubPanel && SubFocusNumber % 2 == 0)
            {
                ChangeSubPanelFocus(-1, false);
            }
            else if (IsInCategory && CategoryFocusNumber % 2 == 0)
            {
                ChangeCategoryFocus(-1, false);
            }
        }

        private void RightArrowKeyPressed()
        {
            bool isBlocked = IsRightArrowBlocked();

            // Prevents movement in the formation panel if movement would go out of bounds
            if (IsInFormationPanel && !isBlocked &&
                                ((DefaultTargetPosition % 3 != 0 && DefaultTargetPosition >= 10) || DefaultTargetPosition < 10))
            {
                if (DefaultTargetPosition % 3 == 0 && DefaultTargetPosition < 10)
                    ChangeFormationPanelFocus(7);
                else
                    ChangeFormationPanelFocus(1);
            }
            else if (!IsInFormationPanel && IsInSubPanel && SubFocusNumber % 2 == 1 && SubFocusNumber + 1 <= SubPanelItemCount)
            {
                ChangeSubPanelFocus(1, false);
            }
            else if (IsInCategory && CategoryFocusNumber % 2 == 1 && CategoryFocusNumber + 1 <= CategoryItemCount)
                ChangeCategoryFocus(1, false);
        }
        
        /// <summary>
        /// Returns true if an action cannot bypass a character blocking its path to the right
        /// </summary>
        /// <returns></returns>
        private bool IsRightArrowBlocked()
        {
            bool isBlocked = !_canTargetThroughUnits;
            // If there is a character occupying the current position the player is targeting, it is blocked
            isBlocked = isBlocked && _combatInstance.IsPositionOccupied(DefaultTargetPosition, false);
            // Only block if the target is on the enemy's side of the field
            isBlocked = isBlocked && DefaultTargetPosition >= 10;
            return isBlocked;
        }

        private void UpArrowKeyPressed()
        {
            bool isBlocked = IsUpArrowBlocked();
            // Prevents up movement in the formation panel if doing so would go out of bounds
            if (IsInFormationPanel && !isBlocked &&
                ((DefaultTargetPosition > 3 && DefaultTargetPosition <= 9) ||
                DefaultTargetPosition > 12))
                ChangeFormationPanelFocus(-3);
            // Only allow up arrow key if not already on the top 2 items in the subpanel
            else if (!IsInFormationPanel && IsInSubPanel && SubFocusNumber > 2)
            {
                ChangeSubPanelFocus(-2, true);
            }
            else if (IsInCategory && CategoryFocusNumber > 2)
                ChangeCategoryFocus(-2, true);
            else if (!IsInSubPanel && !IsInFormationPanel && !IsInCategory)
                ChangeActionFocus(-1);
        }

        /// <summary>
        /// Checks to see if movement upward in the formation panel should be blocked based on the current position of the
        /// player's target.
        /// </summary>
        /// <returns>Returns true if movement should be blocked.</returns>
        private bool IsUpArrowBlocked()
        {
            bool isBlocked = !_canTargetThroughUnits;
            // Action can only be blocked when not on the player's side of the field and not in the enemy's first column
            isBlocked = isBlocked && DefaultTargetPosition % 3 != 1 && DefaultTargetPosition >= 10;
            // If the target position is in the 3rd column, check if the 1st or 2nd column of the row is occupied, if so action is blocked
            if (DefaultTargetPosition % 3 == 0 && isBlocked)
                isBlocked = _combatInstance.IsPositionOccupied(DefaultTargetPosition - 4) || 
                                _combatInstance.IsPositionOccupied(DefaultTargetPosition - 5, false);
            else
                // Action can only be blocked if there is a character in front of the spot the player is trying to reach
                isBlocked = isBlocked && _combatInstance.IsPositionOccupied(DefaultTargetPosition - 4, false);
            return isBlocked;
        }

        /// <summary>
        /// Changes the focus target in the formation panel.
        /// </summary>
        /// <param name="changeAmount">The amount to change the focus target by.</param>
        private void ChangeFormationPanelFocus(int changeAmount)
        {
            if (IsInFormationPanel && changeAmount != 0)
            {
                DefaultTargetPosition += changeAmount;
                if (DefaultTargetPosition < 1) DefaultTargetPosition = 1;
                if (DefaultTargetPosition > 18) DefaultTargetPosition = 18;
                _formationUI.TargetPositions = GetCorrectedTargets();
                RefreshUI();
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
            if (IsInCategory && changeAmount != 0)
            {
                CategoryFocusNumber += changeAmount;
                // Check if the category focus number went out of bounds
                if (CategoryFocusNumber > ActionCategories.Count()) CategoryFocusNumber = ActionCategories.Count();
                if (CategoryFocusNumber < 1) CategoryFocusNumber = 1;
                // If down key was pressed and the current position is the bottom-most row
                if (CategoryFocusNumber > _subActionPanel.MaxSubPanelItems - 2 + CategoryLineOffset * 2 && mayChangeOffset)
                {
                    CategoryLineOffset++;
                    if (CategoryLineOffset * 2 + _subActionPanel.MaxSubPanelItems >= CategoryFocusNumber)
                        CategoryLineOffset--;
                }
                // If up key was pressed and the current position is in the 2nd from the top row
                if (CategoryFocusNumber <= 4 + CategoryLineOffset * 2 && CategoryLineOffset >= 1 && mayChangeOffset) CategoryLineOffset--;
                SubFocusNumber = 1;
                RefreshUI();
            }
        }

        /// <summary>
        /// Changes the subpanel focus if change amount is not 0. If mayChangeOffset is true, also checks to see if movement excedes the
        /// maximum number of items in a panel.
        /// </summary>
        /// <param name="changeAmount">The amount to change the focus by.</param>
        /// <param name="mayChangeOffset">Whether or not the change in focus may go pass the maximum items in the subpanel.</param>
        private void ChangeSubPanelFocus(int changeAmount, bool mayChangeOffset)
        {
            if (IsInSubPanel && changeAmount != 0)
            {
                SubFocusNumber += changeAmount;
                if (SubFocusNumber > ActiveSubPanelList.Count()) SubFocusNumber = ActiveSubPanelList.Count();
                if (SubFocusNumber < 1) SubFocusNumber = 1;
                // If down key was pressed and the current position is the bottom-most row
                if (SubFocusNumber > _subActionPanel.MaxSubPanelItems - 2 + SubPanelLineOffset * 2 && mayChangeOffset)
                {
                    SubPanelLineOffset++;
                    if (SubPanelLineOffset * 2 + _subActionPanel.MaxSubPanelItems >= SubFocusNumber)
                        SubPanelLineOffset--;
                }
                // If up key was pressed and the current position is in the 2nd from the top row
                if (SubFocusNumber <= 4 + SubPanelLineOffset * 2 && SubPanelLineOffset >= 1 && mayChangeOffset) SubPanelLineOffset--;
                RefreshUI();
            }
        }

        /// <summary>
        /// Changes the focus of the main action by an amount.
        /// </summary>
        /// <param name="changeAmount">The amount to change the focus by.</param>
        private void ChangeActionFocus(int changeAmount)
        {
            if (!IsInFormationPanel && !IsInSubPanel && changeAmount != 0)
            {
                int maxFocusNumber = Enum.GetValues(typeof(Actions)).Cast<int>().Max();
                FocusNumber += changeAmount;
                if (FocusNumber <= 0) FocusNumber = maxFocusNumber;
                if (FocusNumber > maxFocusNumber) FocusNumber = 1;
                GetActiveSubPanelList();
                ActionCategories = _combatInstance.GetCategories((Actions)FocusNumber);
                CategoryItemCount = ActionCategories.Count();
                RefreshUI();
            }
        }

        /// <summary>
        /// Renders and prints the battlefield, containing all the characters in combat as well as their healthbars.
        /// </summary>
        private void PrintCombatFormations()
        {
            // If its the enemy's turn, display focus on the enemy, else display focus on the currently active
            // player character.
            int id = (_combatInstance.GetNextActivePlayerId() != _combatInstance.GetActiveCharacterID())
                        ? _combatInstance.GetActiveCharacterID() : _combatInstance.GetNextActivePlayerId();
            var formations = _formationUI.Render(_combatInstance.GetAllDisplayableCharacters(), id);
            foreach (var item in formations)
            {
                Console.WriteLine(item);
            }
        }

        /// <summary>
        /// Renders and prints the target UI component and the turn order UI component.
        /// </summary>
        private void PrintTargetAndTurnOrder()
        {
            // Must have 2 spaces extra to correctly print out boxes to console
            int spaces = Console.WindowWidth - _targetUI.MaxWidth - _turnOrder.MaxWidth;
            var targetUI = StartRenderTarget();
            var turnOrderUI = _turnOrder.Render(IsInFormationPanel, 
                                                GetCorrectedTargets(), 
                                                _combatInstance.GetTurnOrderAsDisplayCharacters());

            for(int i = 0; i < targetUI.Count; i++)
            {
                Console.WriteLine(targetUI[i] + new string(' ', spaces) + turnOrderUI[i]);
            }
            Console.WriteLine(new string(' ', spaces + _targetUI.MaxWidth) + turnOrderUI[turnOrderUI.Count - 1]);
        }

        /// <summary>
        /// Renders and then prints the action panel, subpanel, and information panel.
        /// </summary>
        private void PrintUserPanels()
        {
            var actionPanel = _actionPanel.Render(FocusNumber);
            var informationPanel = StartRenderInformationPanel();
            List<string> offsetModifiedList;
            int offsetModifiedFocus = 0;
            if (IsInSubPanel)
            {
                offsetModifiedList = new List<string>(ActiveSubPanelList.Select(item => item.GetDisplayName()));
                offsetModifiedFocus = SubFocusNumber - SubPanelLineOffset * 2;
            }
            // If the focus is on the attack action, show the available attacks
            else if ((Actions)FocusNumber == Actions.Attack)
            {
                offsetModifiedList = new List<string>(ActiveSubPanelList.Select(item => item.GetDisplayName()));
                offsetModifiedFocus = SubFocusNumber - SubPanelLineOffset * 2;
            }
            // In categories section
            else
            {
                offsetModifiedList = new List<string>(ActionCategories.Select(array => array[0]));
                offsetModifiedFocus = CategoryFocusNumber - CategoryLineOffset * 2;
            }
            // Determines if there is an offset in the list; if so remove the offset items from the front of the list
            if (offsetModifiedList.Count > _subActionPanel.MaxSubPanelItems + SubPanelLineOffset * 2)
                offsetModifiedList.RemoveRange(0, SubPanelLineOffset * 2);

            var subActionPanel = _subActionPanel.Render(offsetModifiedList, IsInSubPanel || IsInFormationPanel || IsInCategory, offsetModifiedFocus);

            for(int i = 0; i < actionPanel.Count; i++)
            {
                Console.WriteLine(actionPanel[i] + subActionPanel[i] + informationPanel[i]);
            }
        }

        /// <summary>
        /// Starts the process of rendering the information panel. Displays different data in the information panel
        /// depending on which panel is currently in focus.
        /// </summary>
        /// <returns>A read-only list containing the information panel.</returns>
        private IReadOnlyList<string> StartRenderInformationPanel()
        {
            IReadOnlyList<string> informationPanel;
            // Display the current target character's details if in the formation panel
            if (IsInFormationPanel)
            {
                IDisplayCharacter display;
                // If there are no characters within any of our target positions, return null
                if (!GetCorrectedTargets().Any(targetPosition => _combatInstance.GetDisplayCharacterFromPosition(targetPosition) != null))
                    display = null;
                // If our main target position is occupied, display that target
                else if (_combatInstance.IsPositionOccupied(DefaultTargetPosition) && GetCorrectedTargets().Contains(DefaultTargetPosition))
                    display = _combatInstance.GetDisplayCharacterFromPosition(DefaultTargetPosition);
                // If our main target position isn't occupied, display any target that occupies a spot in our target list
                else
                    display = _combatInstance.GetAllDisplayableCharacters().First(character => GetCorrectedTargets().Contains(character.GetPosition()));
                informationPanel = _informationPanel.RenderCharacterDetails(display);
            }

            // Display category information if in categories subpanel
            else if (IsInCategory)
                informationPanel = _informationPanel.RenderCategoryDetails
                        (_combatInstance.GetCategories((Actions)FocusNumber)[CategoryFocusNumber - 1]);
            // Display action details if the current selection is an action
            else if (IsInSubPanel)
            {
                var data = _combatInstance.GetSubActionViewData((Actions)FocusNumber, ActiveCategory, SubFocusNumber - 1);
                informationPanel = _informationPanel.RenderActionDetails(
                                                    _combatInstance.GetSubActionFromCategory(
                                                    (Actions)FocusNumber, ActiveCategory, SubFocusNumber - 1),
                                                    data);
            }
                
            else
                informationPanel = _informationPanel.RenderCharacterDetails(_combatInstance.GetDisplayCharacterFromId(_activeCharacterId));
            return informationPanel;
        }

        /// <summary>
        /// Returns a read-only list of string containing the target UI component.
        /// </summary>
        /// <returns>A read-only list of string containing the target UI component.</returns>
        private IReadOnlyList<string> StartRenderTarget()
        {
            if (IsInFormationPanel)
            {
                IDisplayCharacter renderTarget = null;
                // If there is a character in the player's default target position, render that target's details
                if (_combatInstance.GetAllDisplayableCharacters().Any(chr => chr.GetPosition() == DefaultTargetPosition))
                    renderTarget = _combatInstance.GetDisplayCharacterFromPosition(DefaultTargetPosition);
                // Finds any character that is in the player's target list and render that target's details
                else
                    renderTarget = _combatInstance.GetAllDisplayableCharacters()
                        .FirstOrDefault(chr => GetCorrectedTargets().Contains(chr.GetPosition()));
                // If there are no characters that occupy the positions the player is targeting, render the active character's details
                if (renderTarget == null) renderTarget = _combatInstance.GetDisplayCharacterFromId(_activeCharacterId);

                return _targetUI.RenderTargetDetails(renderTarget);
            }
            else
                return _targetUI.RenderTargetDetails(_combatInstance.GetDisplayCharacterFromId(_activeCharacterId));
        }

        /// <summary>
        /// Sets the active sub panel list to a new list depending on the currently active category
        /// and the current action focus.
        /// </summary>
        private void GetActiveSubPanelList()
        {
            switch ((Actions)FocusNumber)
            {
                case Actions.Attack:
                case Actions.Spells:
                case Actions.Skills:
                case Actions.Items:
                    ActiveSubPanelList = _combatInstance.GetActionListFromCategory(
                                    (Actions)FocusNumber, ActiveCategory);
                    break;
                default:
                    ActiveSubPanelList = new List<IDisplayAction>();
                    break;
            }
            SubPanelItemCount = ActiveSubPanelList.Count;
        }
    }
}
