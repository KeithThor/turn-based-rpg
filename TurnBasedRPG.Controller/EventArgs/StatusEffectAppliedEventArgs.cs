﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Controller.EventArgs
{
    public class StatusEffectAppliedEventArgs : CombatLoggableEventArgs
    {
        public IReadOnlyList<int> AffectedCharacterIds { get; set; }
    }
}
