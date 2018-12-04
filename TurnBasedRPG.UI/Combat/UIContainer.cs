using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TurnBasedRPG.Controller;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Shared.Combat;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// Class that contains all UI components and is responsible for rendering the entire UI output to the console.
    /// </summary>
    public class UIContainer
    {
        private readonly FormationPanel _formationPanel;
        private readonly TargetPanel _targetPanel;
        private readonly CommandPanel _commandPanel;
        private readonly ActionPanel _actionPanel;
        private readonly TurnOrderPanel _turnOrderPanel;
        private readonly ActionDetailsPanel _actionDetailsPanel;
        private readonly DefaultsHandler _defaultsHandler;
        private readonly UICharacterManager _uiCharacterManager;
        private readonly ViewModelController _viewModelController;
        private readonly DisplayManager _displayManager;
        private readonly CombatStateHandler _combatStateHandler;
        private readonly CharacterDetailsPanel _characterDetailsPanel;
        private readonly CategoryDetailsPanel _categoryDetailsPanel;
        private readonly CombatLogPanel _combatLogPanel;

        public UIContainer(FormationPanel formationPanel,
                           TargetPanel targetPanel,
                           CommandPanel commandPanel,
                           ActionPanel actionPanel,
                           TurnOrderPanel turnOrderPanel,
                           ActionDetailsPanel actionDetailsPanel,
                           CharacterDetailsPanel characterDetailsPanel,
                           CategoryDetailsPanel categoryDetailsPanel,
                           CombatLogPanel combatLogPanel,
                           DefaultsHandler defaultsHandler,
                           UICharacterManager uiCharacterManager,
                           ViewModelController viewModelController,
                           DisplayManager displayManager,
                           CombatStateHandler combatStateHandler)
        {
            _formationPanel = formationPanel;
            _targetPanel = targetPanel;
            _commandPanel = commandPanel;
            _actionPanel = actionPanel;
            _turnOrderPanel = turnOrderPanel;
            _actionDetailsPanel = actionDetailsPanel;
            _characterDetailsPanel = characterDetailsPanel;
            _categoryDetailsPanel = categoryDetailsPanel;
            _combatLogPanel = combatLogPanel;
            _defaultsHandler = defaultsHandler;
            _uiCharacterManager = uiCharacterManager;
            _viewModelController = viewModelController;
            _displayManager = displayManager;
            _combatStateHandler = combatStateHandler;
            
            BindEvents();
        }
        
        private void BindEvents()
        {
            _defaultsHandler.InFormationPanelChanged += (obj, args) => RenderFormationTargets = _defaultsHandler.IsInFormationPanel;
        }

        public bool IsPlayerTurn
        {
            get { return _combatStateHandler.IsPlayerTurn(); }
        }

        /// <summary>
        /// Whenever a combat loggable event is invoked, add the log message to the combat log panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnCombatLoggableEvent(object sender, CombatLoggableEventArgs args)
        {
            _combatLogPanel.AddToLog(args.LogMessage);
        }

        /// <summary>
        /// Gets or sets whether or not formation targets should be rendered for the formation panel.
        /// </summary>
        public bool RenderFormationTargets
        {
            get { return _formationPanel.RenderFocus; }
            set { _formationPanel.RenderFocus = value; }
        }

        public bool IsUIUpdating { get; set; } = false;
        public bool IsUIUpdatingFinished { get; private set; } = false;
        public bool SkipUIUpdating { get; set; } = false;

        /// <summary>
        /// Called on updates to print the UI.
        /// </summary>
        public void PrintUI()
        {
            Console.Clear();
            PrintTargetAndTurnOrder();
            PrintCombatFormations();
            PrintUserPanels();
        }

        /// <summary>
        /// Called to print the UI multiple times, with a wait time in between each update.
        /// </summary>
        /// <param name="times">The amount of times to update the UI.</param>
        /// <param name="msWaitPerFrame">The amount of milliseconds to wait before calling each frame update.</param>
        public void PrintUI(int times, int msWaitPerFrame)
        {
            for (int i = 0; i < times; i++)
            {
                Thread.Sleep(msWaitPerFrame);
                PrintUI();
            }
        }

        /// <summary>
        /// Update the health of multiple characters over a specified number of frames with a 50ms wait per frame.
        /// <para>Will terminate prematurely if _skipUpdating is triggered.</para>
        /// </summary>
        /// <param name="preCharactersChanged">Contains the health values for the health-modified characters before the health change.</param>
        /// <param name="postCharactersChanged">Contains the heath values for the characters after the health change.</param>
        /// <param name="gradualChanges">The amount of health each character should have their health changed by per frame.</param>
        /// <param name="frameUpdates">The number of frames to update the health of the characters for.</param>
        internal void UpdateHealthGradually(IReadOnlyDictionary<int, int> preCharactersChanged,
                                           IReadOnlyDictionary<int, int> postCharactersChanged,
                                           IReadOnlyDictionary<int, float> gradualChanges,
                                           int frameUpdates)
        {
            IsUIUpdating = true;
            SkipUIUpdating = false;
            IsUIUpdatingFinished = false;
            bool lastUpdate = false;

            int msPerFrame = 50;
            int i = 1;
            while (i <= frameUpdates && !lastUpdate)
            {
                // Make sure that there is 1 more frame update after SkipUIUpdating is toggled true
                if (SkipUIUpdating)
                    lastUpdate = true;

                // If current iteration is not the last frame, show gradual health changes over each ui update
                if (i < frameUpdates && !lastUpdate)
                {
                    foreach (var key in gradualChanges.Keys)
                    {
                        int newHealth = (int)(preCharactersChanged[key] + gradualChanges[key] * i);
                        _uiCharacterManager.SetCurrentHealth(key, newHealth);
                    }
                }
                // Current iteration is last, set current health to the changed amount because dividing by number of iterations
                // might create remainders that will cause the UI and Controller versions of characters to go out of sync
                else if (i == frameUpdates || lastUpdate)
                {
                    foreach (var key in postCharactersChanged.Keys)
                    {
                        _uiCharacterManager.SetCurrentHealth(key, postCharactersChanged[key]);
                    }
                }
                PrintUI(1, msPerFrame);
                i++;
            }
            IsUIUpdatingFinished = true;
            IsUIUpdating = false;
            SkipUIUpdating = false;
        }

        /// <summary>
        /// Renders and prints the battlefield, containing all the characters in combat as well as their healthbars.
        /// </summary>
        private void PrintCombatFormations()
        {
            var formations = _formationPanel.Render(_uiCharacterManager.Characters, 
                                                    _defaultsHandler.ActiveCharacterId,
                                                    _defaultsHandler.CurrentTargetPositions);
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
            int spaces = Console.WindowWidth - _targetPanel.MaxWidth - _turnOrderPanel.MaxWidth;
            var targetUI = StartRenderTarget();
            var turnOrderIds = _combatStateHandler.GetRoundOrderIds();
            var turnOrderCharacters = _uiCharacterManager.GetTurnOrderCharacters(turnOrderIds[0], turnOrderIds[1]);
            bool renderTargets = _defaultsHandler.IsInFormationPanel || !IsPlayerTurn;

            var correctedTargets = _defaultsHandler.CurrentTargetPositions;

            var turnOrderUI = _turnOrderPanel.Render(renderTargets,
                                                correctedTargets,
                                                turnOrderCharacters);

            for (int i = 0; i < targetUI.Count; i++)
            {
                Console.WriteLine(targetUI[i] + new string(' ', spaces) + turnOrderUI[i]);
            }
            Console.WriteLine(new string(' ', spaces + _targetPanel.MaxWidth) + turnOrderUI[turnOrderUI.Count - 1]);
        }

        /// <summary>
        /// Renders and then prints the action panel, subpanel, and information panel.
        /// </summary>
        private void PrintUserPanels()
        {
            var commandPanel = _commandPanel.Render(_defaultsHandler.CommandFocusNumber);
            var detailsPanel = StartRenderDetailsPanels();

            // Contains names of actions or categories
            List<string> offsetModifiedNames;
            int offsetModifiedFocus = 0;
            if (_defaultsHandler.IsInActionPanel)
            {
                offsetModifiedNames = new List<string>(_defaultsHandler.ActionPanelList.Select(item => item.GetDisplayName()));
                offsetModifiedFocus = _defaultsHandler.ActionFocusNumber - _defaultsHandler.ActionPanelLineOffset * 2;
            }
            // If the focus is on the attack action, show the available attacks
            else if ((Commands)_defaultsHandler.CommandFocusNumber == Commands.Attack)
            {
                offsetModifiedNames = new List<string>(_defaultsHandler.ActionPanelList.Select(item => item.GetDisplayName()));
                offsetModifiedFocus = _defaultsHandler.ActionFocusNumber - _defaultsHandler.ActionPanelLineOffset * 2;
            }
            // In categories section
            else
            {
                offsetModifiedNames = new List<string>(_defaultsHandler.ActionCategories.Select(array => array[0]));
                offsetModifiedFocus = _defaultsHandler.CategoryFocusNumber - _defaultsHandler.CategoryLineOffset * 2;
            }
            // Determines if there is an offset in the list; if so remove the offset items from the front of the list
            if (offsetModifiedNames.Count > _actionPanel.MaxActionPanelItems + _defaultsHandler.ActionPanelLineOffset * 2)
                offsetModifiedNames.RemoveRange(0, _defaultsHandler.ActionPanelLineOffset * 2);

            var actionPanel = _actionPanel.Render(offsetModifiedNames, 
                                                  _defaultsHandler.IsInActionPanel 
                                                     || _defaultsHandler.IsInFormationPanel 
                                                     || _defaultsHandler.IsInCategoryPanel,
                                                  offsetModifiedFocus);
            
            var combatLogPanel = _combatLogPanel.Render();

            for (int i = 0; i < commandPanel.Count; i++)
            {
                Console.WriteLine(commandPanel[i] + actionPanel[i] + detailsPanel[i] + combatLogPanel[i]);
            }
        }

        /// <summary>
        /// Starts the process of rendering the information panel. Displays different data in the information panel
        /// depending on which panel is currently in focus.
        /// </summary>
        /// <returns>A read-only list containing the information panel.</returns>
        private IReadOnlyList<string> StartRenderDetailsPanels()
        {
            IReadOnlyList<string> detailsPanel;
            // Display the current target character's details if in the formation panel
            if (_defaultsHandler.IsInFormationPanel || !IsPlayerTurn)
            {
                IDisplayCharacter display;
                var targets = _defaultsHandler.CurrentTargetPositions;

                // If there are no characters within any of our target positions, return null
                if (!targets.Any(targetPosition => 
                                 _uiCharacterManager.GetCharacterFromPosition(targetPosition) != null))
                {
                    display = null;
                }
                // If our main target position is occupied, display that target
                else if (_uiCharacterManager.CharacterInPositionExists(_defaultsHandler.CurrentTargetPosition) 
                         && targets.Contains(_defaultsHandler.CurrentTargetPosition))
                    display = _uiCharacterManager.GetCharacterFromPosition(_defaultsHandler.CurrentTargetPosition);
                // If our main target position isn't occupied, display any target that occupies a spot in our target list
                else
                    display = _uiCharacterManager.Characters.First(
                                            character => targets.Contains(character.Position));
                detailsPanel = _characterDetailsPanel.RenderCharacterDetails(display);
            }

            // Display category information if in categories subpanel
            else if (_defaultsHandler.IsInCategoryPanel)
                detailsPanel = _categoryDetailsPanel.RenderCategoryDetails(
                                    _defaultsHandler.ActionCategories[_defaultsHandler.CategoryFocusNumber - 1]);
            // Display action details if the current selection is an action
            else if (_defaultsHandler.IsInActionPanel)
            {
                var data = _viewModelController.GetActionViewData((Commands)_defaultsHandler.CommandFocusNumber, 
                                                                  _defaultsHandler.ActiveCategory, 
                                                                  _defaultsHandler.ActionFocusNumber - 1);

                detailsPanel = _actionDetailsPanel.RenderActionDetails(
                                                    _displayManager.GetActionFromCategory(
                                                        (Commands)_defaultsHandler.CommandFocusNumber, 
                                                        _defaultsHandler.ActiveCategory, 
                                                        _defaultsHandler.ActionFocusNumber - 1),
                                                    data);
            }

            else
                detailsPanel = _characterDetailsPanel.RenderCharacterDetails(
                                                    _uiCharacterManager.GetCharacterFromId(_defaultsHandler.ActiveCharacterId));
            return detailsPanel;
        }

        /// <summary>
        /// Returns a read-only list of string containing the target UI component.
        /// </summary>
        /// <returns>A read-only list of string containing the target UI component.</returns>
        private IReadOnlyList<string> StartRenderTarget()
        {
            if (_defaultsHandler.IsInFormationPanel || !IsPlayerTurn)
            {
                IDisplayCharacter renderTarget = null;
                // If there is a character in the player's default target position, render that target's details
                if (_uiCharacterManager.Characters.Any(chr => chr.GetPosition() == _defaultsHandler.CurrentTargetPosition))
                    renderTarget = _uiCharacterManager.GetCharacterFromPosition(_defaultsHandler.CurrentTargetPosition);
                // Finds any character that is in the player's target list and render that target's details
                else
                {
                    var targets = _defaultsHandler.CurrentTargetPositions;

                    renderTarget = _uiCharacterManager.Characters.FirstOrDefault(chr => targets.Contains(chr.Position));
                }
                // If there are no characters that occupy the positions the player is targeting, render the active character's details
                if (renderTarget == null) renderTarget = _uiCharacterManager.GetCharacterFromId(_defaultsHandler.ActiveCharacterId);

                return _targetPanel.RenderTargetDetails(renderTarget);
            }
            else
                return _targetPanel.RenderTargetDetails(_uiCharacterManager.GetCharacterFromId(_defaultsHandler.ActiveCharacterId));
        }
    }
}
