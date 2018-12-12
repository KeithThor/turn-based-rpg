using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Controller.EventArgs;
using System.Threading;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.Shared.Combat;
using TurnBasedRPG.UI.Combat.Panels;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// Class that contains other UI related components.
    /// </summary>
    public class CombatUI
    {
        private readonly CombatController _combatInstance;
        private readonly UICharacterManager _uiCharacterManager;
        private readonly DefaultsHandler _defaultsHandler;
        private readonly UIContainer _uiContainer;
        private readonly UserInput _userInput;
        private readonly DisplayManager _displayManager;
        private readonly CombatStateHandler _combatStateHandler;

        public CombatUI(CombatController combatInstance,
                        UICharacterManager uiCharacterManager,
                        GameUIConstants gameUIConstants,
                        UIContainer uiContainer,
                        UserInput userInput,
                        DefaultsHandler defaultsHandler,
                        DisplayManager displayManager,
                        CombatStateHandler combatStateHandler)
        {
            _combatInstance = combatInstance;
            _displayManager = displayManager;
            _combatStateHandler = combatStateHandler;
            _defaultsHandler = defaultsHandler;

            _uiCharacterManager = uiCharacterManager;
            _uiCharacterManager.Characters = _displayManager.GetDisplayCharacters();
            _uiCharacterManager.CurrentRoundOrderIds = _combatStateHandler.GetRoundOrderIds()[0];
            _uiCharacterManager.NextRoundOrderIds = _combatStateHandler.GetRoundOrderIds()[1];

            _uiContainer = uiContainer;

            _userInput = userInput;

            BindEvents();
            RefreshActionPanelList();
        }
        
        private void BindEvents()
        {
            _combatInstance.CharactersHealthChanged += OnCharactersHealthChanged;
            _combatInstance.CharactersHealthChanged += _uiContainer.OnCombatLoggableEvent;
            _combatInstance.CharactersDied += _uiContainer.OnCombatLoggableEvent;
            _combatInstance.StatusEffectApplied += _uiContainer.OnCombatLoggableEvent;
            _combatInstance.StatusEffectApplied += OnStatusEffectApplied;
            _combatInstance.DelayedActionBeginChannel += _uiContainer.OnCombatLoggableEvent;
            _combatInstance.CharacterBeginWait += _uiContainer.OnCombatLoggableEvent;
            _combatInstance.StartOfTurn += OnStartOfTurn;
            _combatInstance.StatusEffectsRemoved += _uiContainer.OnCombatLoggableEvent;

            _uiContainer.ActionSelectedEvent += OnActionSelected;
            _uiContainer.ActionStartedEvent += OnActionStarted;
            _uiContainer.UpdateActionListEvent += OnUpdateActionList;
            _uiContainer.UpdateCategories += OnUpdateCategories;

            _combatInstance.EndOfTurn += EndOfTurnTriggered;
            _combatInstance.AIChoseTarget += OnAIChoseTarget;
        }

        /// <summary>
        /// Provides the UI character manager with an updated set of round order ids at the start of a new turn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnStartOfTurn(object sender, StartOfTurnEventArgs args)
        {
            _uiCharacterManager.CurrentRoundOrderIds = args.CurrentRoundOrderIds;
            _uiCharacterManager.NextRoundOrderIds = args.NextRoundOrderIds;
            _defaultsHandler.IsInCommandPanel = true;
        }

        /// <summary>
        /// Handles any actions that occurs at the end of a turn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EndOfTurnTriggered(object sender, EndOfTurnEventArgs args)
        {
            Thread.Sleep(1000);

            _defaultsHandler.ActiveCharacterId = _combatStateHandler.GetActiveCharacterID();
            _defaultsHandler.IsInActionPanel = false;
            _defaultsHandler.IsInCategoryPanel = false;
            _defaultsHandler.IsInFormationPanel = false;
            _defaultsHandler.IsInCharacterPanel = false;
            _defaultsHandler.IsInCommandPanel = false;
            _defaultsHandler.IsInStatusCommand = false;
            _defaultsHandler.CurrentTargetPositions = new List<int>();

            RefreshActionPanelList();
            _uiContainer.PrintUI();
        }

        /// <summary>
        /// Refreshes the action panel list depending on the currently active command focus.
        /// </summary>
        private void RefreshActionPanelList()
        {
            if (!_combatStateHandler.IsPlayerTurn())
            {
                _defaultsHandler.ActionPanelList = new List<IDisplayAction>();
                _defaultsHandler.ActionPanelItemCount = 0;
            }
            else
            {
                switch ((Commands)_defaultsHandler.CommandFocusNumber)
                {
                    case Commands.Attack:
                    case Commands.Spells:
                    case Commands.Skills:
                    case Commands.Items:
                        _defaultsHandler.ActionPanelList = _displayManager.GetActionListFromCategory(
                                                                            (Commands)_defaultsHandler.CommandFocusNumber,
                                                                            _defaultsHandler.ActiveCategory);
                        break;
                    default:
                        _defaultsHandler.ActionPanelList = new List<IDisplayAction>();
                        break;
                }
            }
        }

        /// <summary>
        /// Whenever an action is selected by the player, changes the defaults to the new action to be used in combat.
        /// </summary>
        /// <param name="sender">The object invoking the event.</param>
        /// <param name="args">Event parameters for the ActionSelectedEvent</param>
        private void OnActionSelected(object sender, ActionSelectedEventArgs args)
        {
            var activeAction = _displayManager.GetActionFromCategory(args.CommandFocus,
                                                                     args.CategoryName,
                                                                     args.ActionFocusNumber - 1);

            _defaultsHandler.ActiveAction = new DefaultsHandler.ActionStore()
            {
                TargetPositions = activeAction.GetActionTargets(),
                CenterOfTargets = activeAction.GetCenterOfTargetsPosition(),
                CanSwitchTargetPosition = activeAction.CanSwitchTargetPosition,
                CanTargetThroughUnits = activeAction.CanTargetThroughUnits
            };

            if (_defaultsHandler.CurrentTargetPosition < 1 
                || _defaultsHandler.CurrentTargetPosition > 18
                || !activeAction.CanTargetThroughUnits)
                _defaultsHandler.CurrentTargetPosition = 13;

            _defaultsHandler.CurrentTargetPositions = CombatTargeter.GetTranslatedTargetPositions(_defaultsHandler.ActiveAction.TargetPositions,
                                                                                                  _defaultsHandler.ActiveAction.CenterOfTargets,
                                                                                                  _defaultsHandler.ActiveAction.CanSwitchTargetPosition,
                                                                                                  _defaultsHandler.CurrentTargetPosition);
        }

        /// <summary>
        /// Whenever the UpdateActionListEvent is invoked, updates the UI with a new action list depending on the
        /// focused command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name=""></param>
        private void OnUpdateActionList(object sender, UpdateActionListEventArgs args)
        {
            RefreshActionPanelList();
        }

        /// <summary>
        /// Handles the start of an action whenever the ActionStartedEvent is invoked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnActionStarted(object sender, ActionStartedEventArgs args)
        {
            _combatInstance.StartAction(args.ActionType,
                                        args.CategoryName,
                                        args.ActionIndex,
                                        args.TargetPosition);
        }

        /// <summary>
        /// Updates the current list of categories whenever the UpdateCategoriesEvent is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUpdateCategories(object sender, UpdateCategoriesEventArgs args)
        {
            if (args.CommandFocus == Commands.Attack)
            {
                RefreshActionPanelList();
            }
            var categories = _displayManager.GetCategories((Commands)_defaultsHandler.CommandFocusNumber);
            _defaultsHandler.ActionCategories = categories;
        }

        /// <summary>
        /// Called whenever one or more characters have their health changed. Refresh the UI over several frames to
        /// visualize the health change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCharactersHealthChanged(object sender, CharactersHealthChangedEventArgs args)
        {
            int frameUpdates = 30;
            var healthChangeDict = new Dictionary<int, float>();
            foreach (var key in args.ChangeAmount.Keys)
            {
                // Difference in health / frameUpdates, may have remainders that must be accounted for
                float gradualHealthChange = args.ChangeAmount[key] / ((float)frameUpdates);
                healthChangeDict[key] = gradualHealthChange;
            }

            var task = Task.Run(() => _uiContainer.UpdateHealthGradually(args.PreCharactersChanged, 
                                                                         args.PostCharactersChanged, 
                                                                         healthChangeDict, 
                                                                         frameUpdates));

            _userInput.ClearInputBuffer();
            _userInput.ListenForUISkip();

            task.Wait();
        }
        
        /// <summary>
        /// Invoked whenever a status effect is applied. Replaces the affected characters with a fresh set of display characters
        /// that represent the status applied characters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnStatusEffectApplied(object sender, StatusEffectAppliedEventArgs args)
        {
            var replacements = _displayManager.GetDisplayCharactersFromIds(args.AffectedCharacterIds);
            _uiCharacterManager.RefreshCharacters(replacements);
        }

        /// <summary>
        /// Called whenever an AI has chosen an action and it's target. Refreshes the UI to visualize
        /// the selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAIChoseTarget(object sender, AIChoseTargetEventArgs args)
        {
            int msWait = 1000;
            int frames = 2;
            _defaultsHandler.CurrentTargetPositions = args.TargetPositions;
            _uiContainer.RenderFormationTargets = true;
            _uiContainer.PrintUI(frames, msWait);
        }

        /// <summary>
        /// Begins the combat UI.
        /// </summary>
        public void StartCombat()
        {
            // First render
            _uiContainer.PrintUI();

            _userInput.ListenForKeyPress();
        }
    }
}
