using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for rendering the names of all categories of all the spells and skills a character knows how to use.
    /// </summary>
    public class CategoryPanel : ICategoryPanel
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
                if (value < MaxNameLength * 2 + 7)
                    _maxWidth = MaxNameLength * 2 + 7;
                else
                    _maxWidth = value;
            }
        }
        public int MaxNameLength { get; set; } = 20;
        public int MaxHeight { get; set; } = 16;

        /// <summary>
        /// The maximum number of categories that can exist in the panel at a time.
        /// </summary>
        public int MaxItems
        {
            get { return MaxHeight - 2; }
        }
        public int MaxCategoriesPerLine { get; set; } = 2;
        
        private IReadOnlyList<string> _cachedNames = new List<string>();
        private bool _cachedFocus;
        private int _cachedFocusNumber;
        private IReadOnlyList<string> _cachedRender;
        private readonly IUIStateTracker _defaultsHandler;

        public bool IsActive
        {
            get { return _defaultsHandler.IsInCategoryPanel; }
            set { _defaultsHandler.IsInCategoryPanel = value; }
        }
        public int FocusNumber
        {
            get { return _defaultsHandler.CategoryFocusNumber; }
            set { _defaultsHandler.CategoryFocusNumber = value; }
        }

        /// <summary>
        /// The amount of lines the panel is offset from the top.
        /// </summary>
        private int _lineOffset;

        public CategoryPanel(IUIStateTracker defaultsHandler)
        {
            _defaultsHandler = defaultsHandler;
            _lineOffset = 0;
            FocusNumber = 1;
        }

        /// <summary>
        /// Renders a category panel filled with all the categories of the skills and spells that a character knows how to use.
        /// </summary>
        /// <returns>A list of string containing the panel render.</returns>
        public IReadOnlyList<string> Render()
        {
            var categories = _defaultsHandler.ActionCategories.Select(items => items[0]).ToList();
            // Return the cached render if the parameters are the same as the previous render
            if (IsActive == _cachedFocus
                && FocusNumber == _cachedFocusNumber
                && categories.SequenceEqual(_cachedNames))
            {
                return _cachedRender;
            }
            else
            {
                _cachedNames = categories;
                _cachedFocus = IsActive;
                _cachedFocusNumber = FocusNumber;
            }

            var actionPanel = new List<string>();

            actionPanel.Add(new string('═', MaxWidth - 1) + "╗");
            actionPanel.AddRange(RenderCategories(categories));
            actionPanel.Add(new string('═', MaxWidth - 1) + "╝");

            _cachedRender = actionPanel;
            return actionPanel;
        }

        /// <summary>
        /// Renders the category names.
        /// </summary>
        /// <param name="categories">The names of the categories.</param>
        /// <returns>A list of string containing the category names.</returns>
        private List<string> RenderCategories(IReadOnlyList<string> categories)
        {
            var actions = new List<string>();
            string actionLine = "";
            for (int i = 0; i < MaxItems; i++)
            {
                bool isFocus = IsActive && FocusNumber == i + 1;
                if (i < categories.Count)
                    actionLine += RenderCategoryName(categories[i], isFocus);
                else
                    actionLine += RenderCategoryName("", false);

                if (i % MaxCategoriesPerLine == MaxCategoriesPerLine - 1)
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
        /// Renders a single category name along with a focus triangle if the category is currently focused by the player.
        /// </summary>
        /// <param name="name">The name of the category.</param>
        /// <param name="isFocus">Whether or not this category is being focused by the player.</param>
        /// <returns>A string containing the category name along with a focus triangle if the category is focused by the player.</returns>
        private string RenderCategoryName(string name, bool isFocus)
        {
            string focus = "  ";
            int spaces = MaxNameLength - name.Length;
            if (isFocus)
                focus = "► ";
            return focus + name + new string(' ', spaces);
        }

        /// <summary>
        /// Handles key press events if this panel is active.
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
                    case ConsoleKey.RightArrow:
                        OnRightArrowPressed();
                        break;
                    case ConsoleKey.LeftArrow:
                        OnLeftArrowPressed();
                        break;
                    default:
                        args.Handled = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Moves the focus upward if the focus is not at the top-most row of the panel. Scrolls the entire panel if
        /// there are more categories than can fit on the category panel.
        /// </summary>
        private void OnUpArrowPressed()
        {
            if (FocusNumber > MaxCategoriesPerLine)
            {
                FocusNumber -= MaxCategoriesPerLine;
                if (FocusNumber < _lineOffset * MaxCategoriesPerLine)
                {
                    _lineOffset--;
                }
            }
        }

        /// <summary>
        /// Moves the focus downward if the focus is not at the bottom-most row of the panel. Scrolls the entire panel if
        /// there are more categories than can fit on the category panel.
        /// </summary>
        private void OnDownArrowPressed()
        {
            if (FocusNumber <= _defaultsHandler.ActionCategories.Count() - MaxCategoriesPerLine)
            {
                FocusNumber += MaxCategoriesPerLine;
                if (FocusNumber > MaxItems + _lineOffset * MaxCategoriesPerLine)
                {
                    _lineOffset++;
                }
            }
        }

        /// <summary>
        /// Moves the focus to the right if the focus is in the left column.
        /// </summary>
        private void OnRightArrowPressed()
        {
            if (FocusNumber % 2 == 1 && FocusNumber <= _defaultsHandler.ActionCategories.Count() - 1)
            {
                FocusNumber++;
            }

        }

        /// <summary>
        /// Moves the focus to the left if the focus is in the right column.
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
