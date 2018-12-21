using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for rendering a character's available actions.
    /// </summary>
    public class ActionPanel : IActionPanel
    {
        private int _maxWidth = 0;
        public int MaxWidth
        {
            get
            {
                if (_maxWidth == 0)
                    MaxWidth = 0;
                return _maxWidth;
            }
            set
            {
                if (value < MaxActionNameLength * 2 + 7)
                    _maxWidth = MaxActionNameLength * 2 + 7;
                else
                    _maxWidth = value;
            }
        }
        public int MaxActionNameLength { get; set; }
        public int MaxHeight { get; set; }
        private readonly IUIStateTracker _defaultsHandler;
        private int _lineOffset;

        public int MaxActionPanelItems
        {
            get { return MaxHeight - 2; }
        }

        /// <summary>
        /// Gets or sets the maximum number of actions that can be rendered per line.
        /// </summary>
        public int MaxActionsPerLine { get; set; }

        /// <summary>
        /// Whether or not the panel is active. Controls if the panel should consume input and render focus triangles.
        /// </summary>
        public bool IsActive
        {
            get { return _defaultsHandler.IsInActionPanel; }
            set { _defaultsHandler.IsInActionPanel = value; }
        }

        /// <summary>
        /// Represents the number one-based index of the action the player is currently focused on.
        /// </summary>
        public int FocusNumber
        {
            get { return _defaultsHandler.ActionFocusNumber; }
            set { _defaultsHandler.ActionFocusNumber = value; }
        }

        public ActionPanel(IUIStateTracker defaultsHandler)
        {
            _defaultsHandler = defaultsHandler;
            _lineOffset = 0;
            FocusNumber = 1;
            MaxHeight = 16;
            MaxActionNameLength = 20;
            MaxActionsPerLine = 2;
        }

        // Cache variables
        private IReadOnlyList<string> _cachedNames = new List<string>();
        private bool _cachedFocus;
        private int _cachedFocusNumber;
        private IReadOnlyList<string> _cachedRender;

        /// <summary>
        /// Renders an action panel filled with the names of the available actions a character can take.
        /// </summary>
        /// <returns>A list of string containing the render.</returns>
        public IReadOnlyList<string> Render()
        {
            var actionNames = _defaultsHandler.ActionPanelList.Select(action => action.GetDisplayName()).ToList();
            // Return the cached render if the parameters are the same as the previous render
            if (IsActive == _cachedFocus
                && FocusNumber == _cachedFocusNumber
                && actionNames.SequenceEqual(_cachedNames))
            {
                return _cachedRender;
            }
            else
            {
                _cachedNames = actionNames;
                _cachedFocus = IsActive;
                _cachedFocusNumber = FocusNumber;
            }

            var actionPanel = new List<string>();
            
            actionPanel.Add(new string('═', MaxWidth - 1) + "╗");
            actionPanel.AddRange(RenderAllActions(actionNames));
            actionPanel.Add(new string('═', MaxWidth - 1) + "╝");

            _cachedRender = actionPanel;
            return actionPanel;
        }

        /// <summary>
        /// Renders all the actions that can fit on one line, along with a focus triangle for the focused action.
        /// </summary>
        /// <param name="actionNames">A list of string containing the action names.</param>
        /// <returns>A list of string containing the render.</returns>
        private List<string> RenderAllActions(List<string> actionNames)
        {
            var actions = new List<string>();
            string actionLine = "";
            for (int i = 0; i < MaxActionPanelItems; i++)
            {
                bool isFocus = IsActive && FocusNumber == i + 1;
                if (i < actionNames.Count)
                    actionLine += RenderActionName(actionNames[i], isFocus);
                else
                    actionLine += RenderActionName("", false);

                if (i % MaxActionsPerLine == MaxActionsPerLine - 1)
                {
                    actions.Add(actionLine + "  ║");
                    // Render a blank line beneath each  action line
                    actions.Add(new string(' ', MaxWidth - 1) + "║");
                    actionLine = "";
                }
            }
            return actions;
        }

        /// <summary>
        /// Renders a single action name along with a focus triangle if it is focused by the player
        /// </summary>
        /// <param name="name">The name of the action to render.</param>
        /// <param name="isFocus">Whether or not this action is the focus.</param>
        /// <returns>A string containing the name of the action and a focus triangle if the action is focused.</returns>
        private string RenderActionName(string name, bool isFocus)
        {
            string focus = "  ";
            int spaces = MaxActionNameLength - name.Length;
            if (isFocus)
                focus = "► ";
            return focus + name + new string(' ', spaces);
        }

        /// <summary>
        /// Handles input whenever a key is pressed by the player and the panel is active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (IsActive && !args.Handled)
            {
                args.Handled = true;
                switch (args.PressedKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        OnUpArrowPressed();
                        break;
                    case ConsoleKey.DownArrow:
                        OnDownArrowPressed();
                        break;
                    case ConsoleKey.LeftArrow:
                        OnLeftArrowPressed();
                        break;
                    case ConsoleKey.RightArrow:
                        OnRightArrowPressed();
                        break;
                    default:
                        args.Handled = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Reduces the FocusNumber by the number of actions on a line and invokes the ActionChanged event.
        /// </summary>
        private void OnUpArrowPressed()
        {
            if (FocusNumber > MaxActionsPerLine)
            {
                FocusNumber -= MaxActionsPerLine;
                if (FocusNumber < _lineOffset * MaxActionsPerLine)
                {
                    _lineOffset--;
                }
            }
        }

        /// <summary>
        /// Increases the FocusNumber by the number of actions on a line and invokes the ActionChanged event.
        /// </summary>
        private void OnDownArrowPressed()
        {
            if (FocusNumber <= _defaultsHandler.ActionPanelList.Count() - MaxActionsPerLine)
            {
                FocusNumber += MaxActionsPerLine;
                if (FocusNumber > MaxActionPanelItems + _lineOffset * MaxActionsPerLine)
                {
                    _lineOffset++;
                }
            }
        }

        /// <summary>
        /// Increases the FocusNumber by 1 if not on the right-most column and invokes the ActionChanged event.
        /// </summary>
        private void OnRightArrowPressed()
        {
            if (FocusNumber % 2 == 1 && FocusNumber <= _defaultsHandler.ActionPanelList.Count() - 1)
            {
                FocusNumber++;
            }
        }

        /// <summary>
        /// Decreases the FocusNumber by 1 if not on the left-most column and invokes the ActionChanged event.
        /// </summary>
        private void OnLeftArrowPressed()
        {
            if (FocusNumber % 2 == 0)
            {
                FocusNumber--;
            }
        }
    }
}
