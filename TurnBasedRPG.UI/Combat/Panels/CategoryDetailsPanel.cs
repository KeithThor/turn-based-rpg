using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.Shared;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for rendering the name and description of the currently active category.
    /// </summary>
    public class CategoryDetailsPanel : ICategoryDetailsPanel
    {
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public CategoryDetailsPanel(IUIStateTracker defaultsHandler)
        {
            MaxWidth = 55;
            MaxHeight = 16;
            _defaultsHandler = defaultsHandler;
        }

        private string[] _cachedCategory = new string[] { "" };
        private IReadOnlyList<string> _cachedRender;
        private readonly IUIStateTracker _defaultsHandler;

        /// <summary>
        /// Returns an details panel injected with a category name and it's description.
        /// </summary>
        /// <param name="category">A string array of 2 indeces, containing the category name and description.</param>
        /// <returns>A read-only list of string containing the details panel.</returns>
        public IReadOnlyList<string> Render()
        {
            var category = _defaultsHandler.ActionCategories[_defaultsHandler.CategoryFocusNumber - 1];

            if (category == null) return RenderBlankPanel();
            if (category.SequenceEqual(_cachedCategory)) return _cachedRender;
            else _cachedCategory = category;

            var detailsPanel = new List<string>();
            int maxLineWidth = MaxWidth - 3;


            detailsPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            detailsPanel.Add("║ " + category[0] + new string(' ', maxLineWidth - category[0].Count()) + "║");
            detailsPanel.Add("║" + new string('─', MaxWidth - 2) + "║");

            detailsPanel.AddRange(RenderCategoryDescription(category));

            detailsPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");

            _cachedRender = detailsPanel;
            return detailsPanel;
        }

        /// <summary>
        /// In case of null objects, renders a panel with no data.
        /// </summary>
        /// <returns>A panel with no data.</returns>
        public IReadOnlyList<string> RenderBlankPanel()
        {
            var detailsPanel = new List<string>();
            detailsPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            for (int i = 0; i < MaxHeight - 2; i++)
            {
                detailsPanel.Add("║ " + new string(' ', MaxWidth - 3) + "║");
            }
            detailsPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return detailsPanel;
        }

        /// <summary>
        /// Returns a list of string containing the description of an action's category.
        /// </summary>
        /// <param name="category">A string array containing the category name and the category description.</param>
        /// <returns>Contains the description of an action's category.</returns>
        private List<string> RenderCategoryDescription(string[] category)
        {
            List<string> descriptionFull = new List<string>();
            int maxLineWidth = MaxWidth - 3;
            var descriptionAsArr = category[1].GetStringAsList(maxLineWidth);
            for (int i = 0; i < MaxHeight - 4; i++)
            {
                if (i < descriptionAsArr.Count())
                {
                    string description = descriptionAsArr.ElementAt(i);
                    description = description + new string(' ', maxLineWidth - description.Count());
                    descriptionFull.Add("║ " + description + "║");
                }
                else
                {
                    descriptionFull.Add("║" + new string(' ', MaxWidth - 2) + "║");
                }
            }
            return descriptionFull;
        }
    }
}
