using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Responsible for rendering the command panel for the UI, containing the commands a player can use such as
    /// Attack, Flee, and Spells.
    /// </summary>
    public class CommandPanel : IReceiveInputPanel
    {
        private const int _MaxNumOfCommands = 7;
        private readonly DefaultsHandler _defaultsHandler;

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

        public CommandPanel(DefaultsHandler defaultsHandler)
        {
            _maxActionNameLength = 12;
            MaxHeight = 16;
            MaxWidth = 17;
            _defaultsHandler = defaultsHandler;
            FocusNumber = 1;
        }

        public event EventHandler<CommandFocusChangedEventArgs> CommandFocusChanged;
        public event EventHandler<ActivePanelChangedEventArgs> ActivePanelChanged;

        // Renders the action panel with the names of the actions as well as a focus triangle if an action is focused by the player
        public IReadOnlyList<string> Render()
        {
            if (_cachedFocus == FocusNumber) return _cachedRender;
            else _cachedFocus = FocusNumber;

            _cachedRender = RenderActionPanel();
            return _cachedRender;
        }

        private List<string> RenderActionPanel()
        {
            var actionPanel = new List<string>();
            string actionName = "";
            
            actionPanel.Add("╔" + new string('═', MaxWidth - 1));
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
                actionPanel.Add("║ " + focus + actionName + new string(' ', spaces) + "│");
                actionPanel.Add("║ " + new string(' ', MaxWidth - 3) + "│");
            }
            actionPanel.Add("╚" + new string('═', MaxWidth - 1));
            return actionPanel;
        }

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
