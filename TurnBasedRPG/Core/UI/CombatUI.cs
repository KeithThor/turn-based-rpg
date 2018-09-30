﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core.Controllers;
using TurnBasedRPG.Core.EventArgs;
using TurnBasedRPG.Core.Interfaces;
using TurnBasedRPG.Core.UI.Enums;

namespace TurnBasedRPG.Core.UI
{

    public class CombatUI
    {
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
                ActiveSubPanelList = new List<IDisplayAction>();
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
            get { return ActionCategories[CategoryFocusNumber - 1][0]; }
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
        private bool _canSwitchTargetPosition;
        private bool _canTargetThroughUnits;
        
        private CombatController _combatInstance;
        private CombatFormationUI _formationUI;
        private TargetUI _targetUI;
        private ActionPanelUI _actionPanel;
        private SubActionPanelUI _subActionPanel;
        private TurnOrderUI _turnOrder;

        public CombatUI(CombatController combatInstance)
        {
            _combatInstance = combatInstance;
            _characterDefaults = new Dictionary<int, PlayerCharacterDefaults>();
            foreach (var characterId in _combatInstance.GetPlayerCharacterIds())
            {
                _characterDefaults.Add(characterId, new PlayerCharacterDefaults());
            }
            _activeCharacterId = _combatInstance.GetNextActivePlayerId();
            _combatInstance.EndOfTurn += EndOfTurnTriggered;
            _formationUI = new CombatFormationUI();
            _targetUI = new TargetUI(42);
            _turnOrder = new TurnOrderUI();
            _actionPanel = new ActionPanelUI();
            _subActionPanel = new SubActionPanelUI();
        }

        private void EndOfTurnTriggered(object sender, EndOfTurnEventArgs args)
        {
            _activeCharacterId = _combatInstance.GetNextActivePlayerId();
            IsInSubPanel = false;
            IsInFormationPanel = false;
            RefreshUI();
        }

        // Start the combat UI
        public void StartRender()
        {
            // First render
            RefreshUI();

            ListenForKeyPress();
        }

        // Called on updates to refresh the UI
        private void RefreshUI()
        {
            Console.Clear();
            PrintTargetAndTurnOrder();
            PrintCombatFormations();
            PrintActionPanels();
        }

        // Handles actions selected by the player
        private void HandleAction()
        {
            if (!IsInSubPanel && !IsInFormationPanel && !IsInCategory)
            {
                switch ((Actions)FocusNumber)
                {
                    case Actions.Attack:
                        var attackDetail = _combatInstance.GetActionFromCategory(Actions.Attack, "", 0);
                        _tempTargetPositions = attackDetail.GetActionTargets();
                        _canSwitchTargetPosition = attackDetail.CanSwitchTargetPosition();
                        _canTargetThroughUnits = attackDetail.CanTargetThroughUnits();
                        DefaultTargetPosition = 13;
                        _formationUI.TargetPositions = GetCorrectedTargets();
                        IsInFormationPanel = true;
                        break;
                    case Actions.Spells:
                    case Actions.Skills:
                        if (CategoryItemCount > 0)
                            IsInCategory = true;
                        break;
                    case Actions.Pass:
                        _combatInstance.StartAction("", -1, Actions.Pass, null);
                        break;
                }
            }
            else if (IsInFormationPanel)
            {
                if (GetCorrectedTargets().Any(position => _combatInstance.IsPositionOccupied(position)))
                    _combatInstance.StartAction("spell", 1, (Actions)FocusNumber, _formationUI.TargetPositions);
            }
            else if (IsInSubPanel)
            {
                switch ((Actions)FocusNumber)
                {
                    case Actions.Spells:
                    case Actions.Skills:
                        HandleSubPanelSelection();
                        break;
                    default:
                        break;
                }
            }
            else if (IsInCategory)
            {
                // Get all spells or skills in category
                ActiveSubPanelList = GetActiveSubPanelList();
                IsInSubPanel = true;
                IsInCategory = false;
            }

            RefreshUI();
        }

        // Gets and handles the sub panel action selected by the player
        private void HandleSubPanelSelection()
        {
            var category = ActiveCategory;
            var selectedAction = _combatInstance.GetActionFromCategory((Actions)FocusNumber, category, SubFocusNumber - 1);
            _tempTargetPositions = selectedAction.GetActionTargets();
            _canSwitchTargetPosition = selectedAction.CanSwitchTargetPosition();
            _canTargetThroughUnits = selectedAction.CanTargetThroughUnits();
            _formationUI.TargetPositions = GetCorrectedTargets();
            IsInFormationPanel = true;
        }

        // Determines which targets an action will hit, depending on the displacement from the target position and where the action's
        // default target is
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

                int targetOffset = DefaultTargetPosition - 5;
                int additionalOffset = 9;

                // If the target is the enemy's side of the field, offset the positions to account for it
                if (DefaultTargetPosition > 9)
                {
                    for (int i = 0; i < correctedTargets.Count; i++)
                    {
                        correctedTargets[i] += 9;
                    }
                    targetOffset -= 9;
                    additionalOffset = 0;
                }

                // Target position is in the right column so all targets in the right column must be culled
                if (DefaultTargetPosition % 3 == 0)
                    correctedTargets.RemoveAll(val => val % 3 == 0);
                // Target position is in the left column so all targets in the left column must be culled
                if (DefaultTargetPosition % 3 == 1)
                    correctedTargets.RemoveAll(val => val % 3 == 1);
                // Target position is in the top row
                if (DefaultTargetPosition < 13 - additionalOffset)
                    correctedTargets.RemoveAll(val => val < 13 - additionalOffset);
                // Target position is in the bottom row
                if (DefaultTargetPosition > 15 - additionalOffset)
                    correctedTargets.RemoveAll(val => val > 15 - additionalOffset);

                for (int i = 0; i < correctedTargets.Count; i++)
                {
                    correctedTargets[i] += targetOffset;
                }

                return correctedTargets;
            }
        }

        // Loops to listen for a key press event
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
            if (IsInFormationPanel && FocusNumber != (int)Actions.Attack)
            {
                IsInFormationPanel = false;
                IsInSubPanel = true;
            }
            else if (IsInFormationPanel && FocusNumber == (int)Actions.Attack)
            {
                IsInFormationPanel = false;
            }
            else if (IsInSubPanel)
            {
                IsInSubPanel = false;
                IsInCategory = true;
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

        // Returns true if an action cannot bypass a character blocking it's path downwards
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

        // Returns true if an action cannot bypass a character blocking its path to the right
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

        // Changes the category focus if change amount is not 0. If mayChangeOffset is true, also checks to see if movement will change
        // the line offset
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

        // Similiar to ChangeCategoryFocus
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

        private void ChangeActionFocus(int changeAmount)
        {
            if (!IsInFormationPanel && !IsInSubPanel && changeAmount != 0)
            {
                int maxFocusNumber = Enum.GetValues(typeof(Actions)).Cast<int>().Max();
                FocusNumber += changeAmount;
                if (FocusNumber <= 0) FocusNumber = maxFocusNumber;
                if (FocusNumber > maxFocusNumber) FocusNumber = 1;
                // ActiveSubPanelList = GetActiveSubPanelList();
                ActionCategories = _combatInstance.GetCategories((Actions)FocusNumber);
                CategoryItemCount = ActionCategories.Count();
                RefreshUI();
            }
        }

        private void PrintCombatFormations()
        {
            var formations = _formationUI.Render(_combatInstance.GetAllDisplayableCharacters(), _activeCharacterId);
            foreach (var item in formations)
            {
                Console.WriteLine(item);
            }
        }

        // Renders the target and turn order UIs as well as the turn order target triangles, then print them with a space between
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

        // Renders the action and sub action panels, then prints them out
        private void PrintActionPanels()
        {
            var actionPanel = _actionPanel.Render(FocusNumber);

            List<string> offsetModifiedList;
            int offsetModifiedFocus = 0;
            if (IsInSubPanel)
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
                informationPanel = _informationPanel.RenderActionDetails(
                                                    _combatInstance.GetActionFromCategory((Actions)FocusNumber, ActiveCategory, SubFocusNumber - 1));
            else
                informationPanel = _informationPanel.RenderCharacterDetails(_combatInstance.GetDisplayCharacterFromId(_activeCharacterId));
            return informationPanel;
        }

        // Calls functions to render the current target on the top left of console
        private IReadOnlyList<string> StartRenderTarget()
        {
            if (IsInFormationPanel)
            {
                dynamic renderTarget = null;
                if (_formationUI.TargetPositions.Contains(DefaultTargetPosition))
                    renderTarget = _combatInstance.GetDisplayCharacterFromPosition(DefaultTargetPosition);
                else
                    renderTarget = _combatInstance.GetDisplayCharacterFromPosition(_formationUI.TargetPositions[0]);
                if (renderTarget == null) renderTarget = _combatInstance.GetDisplayCharacterFromId(_activeCharacterId);

                return _targetUI.RenderTargetDetails(renderTarget);
            }
            else
                return _targetUI.RenderTargetDetails(_combatInstance.GetDisplayCharacterFromId(_activeCharacterId));
        }

        // Gets data to populate the sub panel, depending on which action was focused
        private IReadOnlyList<IDisplayAction> GetActiveSubPanelList()
        {
            IReadOnlyList<IDisplayAction> subPanelList;
            switch ((Actions)FocusNumber)
            {
                case Actions.Spells:
                case Actions.Skills:
                    subPanelList = _combatInstance.GetActionListFromCategory(
                                    (Actions)FocusNumber, ActiveCategory);
                    break;
                default:
                    subPanelList = new List<IDisplayAction>();
                    break;
            }
            SubPanelItemCount = subPanelList.Count;
            return subPanelList;
        }
    }
}
