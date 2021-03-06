﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Enums;

namespace TurnBasedRPG.UI.Combat.EventArgs
{
    public class ActionSelectedEventArgs: System.EventArgs
    {
        public Commands CommandFocus { get; set; }
        public string CategoryName { get; set; }
        public int ActionFocusNumber { get; set; }
    }
}
