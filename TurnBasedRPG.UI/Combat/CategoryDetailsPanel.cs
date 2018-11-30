using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.UI.Combat
{
    public class CategoryDetailsPanel
    {
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public CategoryDetailsPanel()
        {
            MaxWidth = 55;
            MaxHeight = 16;
        }

        /// <summary>
        /// Returns an information panel injected with a category name and it's description.
        /// </summary>
        /// <param name="category">A string array of 2 indeces, containing the category name and description.</param>
        /// <returns>A read-only list of string containing the information panel.</returns>
        public IReadOnlyList<string> RenderCategoryDetails(string[] category)
        {
            if (category == null) return RenderBlankPanel();

            var informationPanel = new List<string>();
            int maxLineWidth = MaxWidth - 3;


            informationPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            informationPanel.Add("║ " + category[0] + new string(' ', maxLineWidth - category[0].Count()) + "║");
            informationPanel.Add("║" + new string('─', MaxWidth - 2) + "║");

            informationPanel.AddRange(RenderCategoryDescription(category));

            informationPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return informationPanel;
        }

        /// <summary>
        /// In case of null objects, renders a panel with no data.
        /// </summary>
        /// <returns>A panel with no data.</returns>
        private List<string> RenderBlankPanel()
        {
            var informationPanel = new List<string>();
            informationPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            for (int i = 0; i < MaxHeight - 2; i++)
            {
                informationPanel.Add("║ " + new string(' ', MaxWidth - 3) + "║");
            }
            informationPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return informationPanel;
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
