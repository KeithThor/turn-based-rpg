using System;
using System.Collections.Generic;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Responsible for rendering the command panel for the UI, containing the commands a player can use such as
    /// Attack, Flee, and Spells.
    /// </summary>
    public class CommandPanel : ICommandPanel
    {
        private const int _MaxNumOfCommands = 7;
        private readonly IUIStateTracker _defaultsHandler;

        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        private int _maxActionNameLength;
        public int MaxActionNameLength
        {
            get { return _maxActionNameLength; }
            set
            {
                _maxActionNameLength = value;
                if (MaxWidth < value + 5) MaxWidth += 5;
            }
        }

        public bool IsActive
        {
            get { return _defaultsHandler.IsInCommandPanel; }
            set { _defaultsHandler.IsInCommandPanel = value; }
        }
        public int FocusNumber
        {
            get { return _defaultsHandler.CommandFocusNumber; }
            set { _defaultsHandler.CommandFocusNumber = value; }
        }

        private int _cachedFocus;
        private IReadOnlyList<string> _cachedRender;

        public CommandPanel(IUIStateTracker defaultsHandler)
        {
            _maxActionNameLength = 12;
            MaxHeight = 16;
            MaxWidth = 17;
            _defaultsHandler = defaultsHandler;
            FocusNumber = 1;
        }

        /// <summary>
        /// Event invoked whenever the player changes the Command focus.
        /// </summary>
        public event EventHandler<CommandFocusChangedEventArgs> CommandFocusChanged;

        /// <summary>
        /// Renders a command panel filled with the names of the commands a character can perform.
        /// </summary>
        /// <returns>A list of string containing the command panel render.</returns>
        public IReadOnlyList<string> Render()
        {
            if (_cachedFocus == FocusNumber) return _cachedRender;
            else _cachedFocus = FocusNumber;

            _cachedRender = RenderCommandPanel();
            return _cachedRender;
        }

        /// <summary>
        /// Renders the names of all the commands a character can perform a long with a focus triangle depending on which command is focused.
        /// </summary>
        /// <returns></returns>
        private List<string> RenderCommandPanel()
        {
            var commandPanel = new List<string>();
            string actionName = "";
            
            commandPanel.Add("╔" + new string('═', MaxWidth - 1));
            for(int i = 1; i <= _MaxNumOfCommands; i++)
            {
                string focus = FocusNumber == i ? "► " : "  ";
                // Determines the name of the action to display
                switch ((Commands)i)
                {
                    case Commands.Attack:
                        actionName = "Attack";
                        break;
                    case Commands.Spells:
                        actionName = "Spells";
                        break;
                    case Commands.Skills:
                        actionName = "Skills";
                        break;
                    case Commands.Items:
                        actionName = "Items";
                        break;
                    case Commands.Status:
                        actionName = "Status";
                        break;
                    case Commands.Wait:
                        actionName = "Wait";
                        break;
                    case Commands.Run:
                        actionName = "Run";
                        break;
                    default:
                        actionName = "";
                        break;
                }
                int spaces = MaxWidth - actionName.Length - 5;
                commandPanel.Add("║ " + focus + actionName + new string(' ', spaces) + "│");
                commandPanel.Add("║ " + new string(' ', MaxWidth - 3) + "│");
            }
            commandPanel.Add("╚" + new string('═', MaxWidth - 1));
            return commandPanel;
        }

        /// <summary>
        /// Handles key press events if the command panel is focused.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (IsActive)
            {
                if (args.PressedKey.Key == ConsoleKey.UpArrow)
                {
                    FocusNumber = ((Commands)FocusNumber == Commands.Attack) ? (int)Commands.Run : --FocusNumber;
                    CommandFocusChanged?.Invoke(this, new CommandFocusChangedEventArgs()
                    {
                        NewCommand = (Commands)FocusNumber
                    });
                }
                else if (args.PressedKey.Key == ConsoleKey.DownArrow)
                {
                    FocusNumber = ((Commands)FocusNumber == Commands.Run) ? (int)Commands.Attack : ++FocusNumber;
                    CommandFocusChanged?.Invoke(this, new CommandFocusChangedEventArgs()
                    {
                        NewCommand = (Commands)FocusNumber
                    });
                }
            }
        }
    }
}
