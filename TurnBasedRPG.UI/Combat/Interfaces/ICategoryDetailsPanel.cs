using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Panel responsible for rendering the details for a currently active category.
    /// </summary>
    public interface ICategoryDetailsPanel : IPanel
    {
        /// <summary>
        /// Renders a blank panel if there is nothing to display.
        /// </summary>
        /// <returns>A list of string containing an empty panel.</returns>
        IReadOnlyList<string> RenderBlankPanel();
    }
}
