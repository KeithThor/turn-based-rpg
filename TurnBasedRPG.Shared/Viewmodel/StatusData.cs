using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.Viewmodel
{
    public class StatusData
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<KeyValuePair<string, int>> StatusEffectsData { get; set; }
        public IReadOnlyList<KeyValuePair<string, int>> DamageData { get; set; }
    }
}
