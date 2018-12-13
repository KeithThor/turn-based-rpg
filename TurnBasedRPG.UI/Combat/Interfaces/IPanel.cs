using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// The root of all UI panels, responsible for rendering a panel element.
    /// </summary>
    public interface IPanel
    {
        int MaxHeight { get; set; }
        int MaxWidth { get; set; }

        /// <summary>
        /// Renders a list of string containing the UI panel and any other details.
        /// </summary>
        /// <returns>A list of string containing the UI panel and any other details.</returns>
        IReadOnlyList<string> Render();
    }
}
