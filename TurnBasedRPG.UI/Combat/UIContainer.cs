using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TurnBasedRPG.Controller;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Panels;

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
        private readonly CategoryDetailsPanel _categoryDetailsPanel;
        private readonly CombatLogPanel _combatLogPanel;
        private readonly CharacterPanel _characterPanel;
        private readonly StatusEffectsPanel _statusEffectsPanel;
        private readonly CategoryPanel _categoryPanel;

        public UIContainer(FormationPanel formationPanel,
                           TargetPanel targetPanel,
                           CommandPanel commandPanel,
                           ActionPanel actionPanel,
                           TurnOrderPanel turnOrderPanel,
                           ActionDetailsPanel actionDetailsPanel,
                           CategoryDetailsPanel categoryDetailsPanel,
                           CombatLogPanel combatLogPanel,
                           CharacterPanel characterPanel,
                           StatusEffectsPanel statusEffectsPanel,
                           CategoryPanel categoryPanel,
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
            _categoryDetailsPanel = categoryDetailsPanel;
            _combatLogPanel = combatLogPanel;
            _characterPanel = characterPanel;
            _statusEffectsPanel = statusEffectsPanel;
            _categoryPanel = categoryPanel;
            _characterPanel.MaxHeight = _formationPanel.MaxHeight;
            _defaultsHandler = defaultsHandler;
            _uiCharacterManager = uiCharacterManager;

            _commandPanel.IsActive = true;

            BindEvents();
        }

        private void BindEvents()
        {
            _defaultsHandler.InFormationPanelChanged += (obj, args) => RenderFormationTargets = _defaultsHandler.IsInFormationPanel;
            _commandPanel.CommandFocusChanged += OnCommandFocusChanged;
            KeyPressed += _commandPanel.OnKeyPressed;
            KeyPressed += _categoryPanel.OnKeyPressed;
            KeyPressed += _actionPanel.OnKeyPressed;
            KeyPressed += _formationPanel.OnKeyPressed;
        }

        /// <summary>
        /// Event called whenever the UI should update categories.
        /// </summary>
        public event EventHandler<UpdateCategoriesEventArgs> UpdateCategories;

        /// <summary>
        /// Event called whenever the player presses a key.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        /// Event called whenever the player selects an action from an action list.
        /// </summary>
        public event EventHandler<ActionSelectedEventArgs> ActionSelectedEvent;

        /// <summary>
        /// Event called whenever the UI should update the action list.
        /// </summary>
        public event EventHandler<UpdateActionListEventArgs> UpdateActionListEvent;

        /// <summary>
        /// Event called whenever the player has chosen an action with its targets.
        /// </summary>
        public event EventHandler<ActionStartedEventArgs> ActionStartedEvent;

        /// <summary>
        /// Assigns an event listener to the given eventhandler.
        /// </summary>
        /// <param name="eventHandler"></param>
        public void RegisterKeyPressEvent(ref EventHandler<KeyPressedEventArgs> eventHandler)
        {
            eventHandler += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            KeyPressed?.Invoke(sender, args);
            switch (args.PressedKey.Key)
            {
                case ConsoleKey.Tab:
                    if (_actionDetailsPanel.IsActive && _canDisplayStatusPanel)
                        ToggleStatusPanel();
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                    _canDisplayStatusPanel = false;
                    IsStatusPanelActive = false;
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
            PrintUI();
        }

        /// <summary>
        /// Tabs through multiple status effects if the active action has more than one status effect, otherwise
        /// toggles between on and off.
        /// </summary>
        private void ToggleStatusPanel()
        {
            // If more than one status, toggle through the statuses
            if (_maxStatusPanels > 1)
            {
                if (!IsStatusPanelActive)
                    IsStatusPanelActive = true;
                else if (_statusEffectsPanel.FocusNumber != _maxStatusPanels)
                    _statusEffectsPanel.FocusNumber++;
                else
                {
                    _statusEffectsPanel.FocusNumber = 1;
                    IsStatusPanelActive = false;
                }
            }
            // If only one status, toggle between active and disabled
            else
            {
                IsStatusPanelActive = !IsStatusPanelActive;
            }
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

        public void OnCommandFocusChanged(object sender, CommandFocusChangedEventArgs args)
        {
            _defaultsHandler.CommandFocusNumber = (int)args.NewCommand;
            UpdateCategories?.Invoke(this, new UpdateCategoriesEventArgs()
            {
                CommandFocus = args.NewCommand
            });
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
        public bool IsStatusPanelActive { get; private set; }
        private int _maxStatusPanels;
        private bool _canDisplayStatusPanel;

        /// <summary>
        /// Called on updates to print the UI.
        /// </summary>
        public void PrintUI()
        {
            Console.Clear();
            PrintTargetAndTurnOrder();
            PrintFormationsAndCharacterPanel();
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
                        _defaultsHandler.IsInCommandPanel = false;
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
                        {
                            _defaultsHandler.IsInCategoryPanel = true;
                            _defaultsHandler.IsInCommandPanel = false;
                        }
                        break;
                    case Commands.Status:
                        _defaultsHandler.IsInFormationPanel = true;
                        _defaultsHandler.IsInCommandPanel = false;
                        int? position = _uiCharacterManager.GetPositionOfCharacter(_defaultsHandler.ActiveCharacterId);

                        if (position == null) throw new Exception("Active character was not found in UICharacterManager.");
                        else
                        {
                            _defaultsHandler.CurrentTargetPosition = position.GetValueOrDefault();
                            _defaultsHandler.CurrentTargetPositions = new List<int>() { position.GetValueOrDefault() };
                            _defaultsHandler.ActiveAction.CanSwitchTargetPosition = true;
                            _defaultsHandler.ActiveAction.CanTargetThroughUnits = true;
                            _defaultsHandler.ActiveAction.CenterOfTargets = 5;
                            _defaultsHandler.ActiveAction.TargetPositions = new List<int>() { 5 };
                            _defaultsHandler.IsInStatusCommand = true;
                        }
                        break;
                    case Commands.Wait:
                        ActionStartedEvent?.Invoke(this, new ActionStartedEventArgs()
                        {
                            ActionType = Commands.Wait,
                            CategoryName = "",
                            ActionIndex = -1,
                            TargetPosition = 0
                        });
                        break;
                }
            }
            // If the player is in the formation panel, start an action
            else if (_defaultsHandler.IsInFormationPanel && !_defaultsHandler.IsInStatusCommand)
            {
                var target = _defaultsHandler.CurrentTargetPosition;

                ActionStartedEvent?.Invoke(this, new ActionStartedEventArgs()
                {
                    ActionType = (Commands)_defaultsHandler.CommandFocusNumber,
                    CategoryName = _defaultsHandler.ActiveCategory,
                    ActionIndex = _defaultsHandler.ActionFocusNumber - 1,
                    TargetPosition = target
                });
            }
            else if (_defaultsHandler.IsInStatusCommand)
            {
                if (_uiCharacterManager.CharacterInPositionExists(_defaultsHandler.CurrentTargetPosition))
                {
                    _defaultsHandler.IsInCharacterPanel = true;
                }
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
                        _defaultsHandler.IsInActionPanel = false;
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
        }

        /// <summary>
        /// Handles any actions that occurs after the escape key is pressed.
        /// </summary>
        private void EscapeKeyPressed()
        {
            if (_defaultsHandler.IsInFormationPanel && !_defaultsHandler.IsInStatusCommand)
            {
                _defaultsHandler.IsInFormationPanel = false;
                _defaultsHandler.IsInActionPanel = true;
            }
            else if (_defaultsHandler.IsInStatusCommand)
            {
                _defaultsHandler.IsInStatusCommand = false;
                _defaultsHandler.IsInFormationPanel = false;
                _defaultsHandler.IsInCommandPanel = true;
            }
            else if (_defaultsHandler.IsInActionPanel && (Commands)_defaultsHandler.CommandFocusNumber != Commands.Attack)
            {
                _defaultsHandler.IsInActionPanel = false;
                _defaultsHandler.IsInCategoryPanel = true;
            }
            else if (_defaultsHandler.IsInActionPanel && (Commands)_defaultsHandler.CommandFocusNumber == Commands.Attack)
            {
                _defaultsHandler.IsInActionPanel = false;
                _defaultsHandler.IsInCommandPanel = true;
            }
            else
            {
                _defaultsHandler.IsInCategoryPanel = false;
                _defaultsHandler.IsInCommandPanel = true;
            }
        }

        /// <summary>
        /// Renders and prints the battlefield, containing all the characters in combat as well as their healthbars.
        /// </summary>
        private void PrintFormationsAndCharacterPanel()
        {
            var formations = _formationPanel.Render();

            var characterPanel = _characterPanel.Render();

            for (int i = 0; i < formations.Count(); i++)
            {
                Console.WriteLine(formations[i] + characterPanel[i]);
            }
        }

        /// <summary>
        /// Renders and prints the target UI component and the turn order UI component.
        /// </summary>
        private void PrintTargetAndTurnOrder()
        {
            // Must have 2 spaces extra to correctly print out boxes to console
            int spaces = Console.WindowWidth - _targetPanel.MaxWidth - _turnOrderPanel.MaxWidth;

            var targetPanel = _targetPanel.Render();
            var turnOrderUI = _turnOrderPanel.Render();

            for (int i = 0; i < targetPanel.Count; i++)
            {
                Console.WriteLine(targetPanel[i] + new string(' ', spaces) + turnOrderUI[i]);
            }
            Console.WriteLine(new string(' ', spaces + _targetPanel.MaxWidth) + turnOrderUI[turnOrderUI.Count - 1]);
        }

        /// <summary>
        /// Renders and then prints the action panel, subpanel, and details panel.
        /// </summary>
        private void PrintUserPanels()
        {
            var commandPanel = _commandPanel.Render();
            var detailsPanel = StartRenderDetailsPanels();

            IReadOnlyList<string> actionOrDetailsPanel;
            if (_defaultsHandler.IsInActionPanel 
                || _defaultsHandler.CommandFocusNumber == (int)Commands.Attack
                || _defaultsHandler.IsInFormationPanel)
            {
                actionOrDetailsPanel = _actionPanel.Render();
            }
            else
            {
                actionOrDetailsPanel = _categoryPanel.Render();
            }
            
            var combatLogPanel = _combatLogPanel.Render();

            for (int i = 0; i < commandPanel.Count; i++)
            {
                Console.WriteLine(commandPanel[i] + actionOrDetailsPanel[i] + detailsPanel[i] + combatLogPanel[i]);
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

            bool inStatusPanel = _defaultsHandler.IsInActionPanel && IsStatusPanelActive && _canDisplayStatusPanel;

            // Display category information if in categories subpanel
            if (_defaultsHandler.IsInCategoryPanel)
            {
                detailsPanel = _categoryDetailsPanel.Render();
            }
            // Display status details if currently in status panel
            else if (inStatusPanel)
            {
                detailsPanel = _statusEffectsPanel.Render();
            }
            // Display action details if the current selection is an action
            else if ((_defaultsHandler.IsInActionPanel || _defaultsHandler.IsInFormationPanel) && !_defaultsHandler.IsInStatusCommand)
            {
                int statusCount = _defaultsHandler.ActionPanelList[_defaultsHandler.ActionFocusNumber - 1].GetStatusCount();
                if (statusCount > 0)
                    _canDisplayStatusPanel = true;
                if (_canDisplayStatusPanel)
                {
                    _maxStatusPanels = statusCount;
                    _statusEffectsPanel.FocusNumber = 1;
                }
                detailsPanel = _actionDetailsPanel.Render();
            }
            else
            {
                detailsPanel = _categoryDetailsPanel.RenderBlankPanel();
            }

            return detailsPanel;
        }
    }
}
