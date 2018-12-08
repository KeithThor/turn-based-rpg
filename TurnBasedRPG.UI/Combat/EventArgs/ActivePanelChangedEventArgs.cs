using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.UI.Combat.EventArgs
{
    public class ActivePanelChangedEventArgs : System.EventArgs
    {
        public bool InCommandPanel { get; set; }
        public bool InCategoryPanel { get; set; }
        public bool InActionPanel { get; set; }
        public bool InFormationPanel { get; set; }
    }
}
