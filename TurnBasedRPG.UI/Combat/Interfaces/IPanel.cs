using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    public interface IPanel
    {
        int MaxHeight { get; set; }
        int MaxWidth { get; set; }
        IReadOnlyList<string> Render();
    }
}
