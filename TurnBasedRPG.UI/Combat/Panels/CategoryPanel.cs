using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
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

        public int MaxItems
        {
            get { return MaxHeight - 2; }
        }
        public int MaxCategoriesPerLine { get; set; } = 2;
        
        private IReadOnlyList<string> _cachedNames = new List<string>();
        private bool _cachedFocus;
        private int _cachedFocusNumber;
        private IReadOnlyList<string> _cachedRender;
        private readonly DefaultsHandler _defaultsHandler;

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

        public event EventHandler<CategoryChangedEventArgs> CategoryChanged;
        public event EventHandler<ActivePanelChangedEventArgs> ActivePanelChanged;

        public CategoryPanel(DefaultsHandler defaultsHandler)
        {
            _defaultsHandler = defaultsHandler;
            _lineOffset = 0;
            FocusNumber = 1;
        }

        // Renders an action panel along with action names
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

        // Renders the max  actions per line along with a focus triangle if an action is focused by the player
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

        // Renders a single  action name along with a focus triangle if it is focused by the player
        private string RenderCategoryName(string name, bool isFocus)
        {
            string focus = "  ";
            int spaces = MaxNameLength - name.Length;
            if (isFocus)
                focus = "► ";
            return focus + name + new string(' ', spaces);
        }

        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (IsActive)
            {
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
                        break;
                }
            }
        }

        private void OnUpArrowPressed()
        {
            if (FocusNumber > MaxCategoriesPerLine)
            {
                FocusNumber -= MaxCategoriesPerLine;
                if (FocusNumber < _lineOffset * MaxCategoriesPerLine)
                {
                    _lineOffset--;
                }
                CategoryChanged?.Invoke(this, new CategoryChangedEventArgs()
                {
                    FocusNumber = FocusNumber
                });
            }
        }

        private void OnDownArrowPressed()
        {
            if (FocusNumber <= _defaultsHandler.ActionCategories.Count() - MaxCategoriesPerLine)
            {
                FocusNumber += MaxCategoriesPerLine;
                if (FocusNumber > MaxItems + _lineOffset * MaxCategoriesPerLine)
                {
                    _lineOffset++;
                }
                CategoryChanged?.Invoke(this, new CategoryChangedEventArgs()
                {
                    FocusNumber = FocusNumber
                });
            }
        }

        private void OnRightArrowPressed()
        {
            if (FocusNumber % 2 == 1 && FocusNumber <= _defaultsHandler.ActionCategories.Count() - 1)
            {
                FocusNumber++;
                CategoryChanged?.Invoke(this, new CategoryChangedEventArgs()
                {
                    FocusNumber = FocusNumber
                });
            }

        }

        private void OnLeftArrowPressed()
        {
            if (FocusNumber % 2 == 0)
            {
                FocusNumber--;
                CategoryChanged?.Invoke(this, new CategoryChangedEventArgs()
                {
                    FocusNumber = FocusNumber
                });
            }
        }
    }
}
