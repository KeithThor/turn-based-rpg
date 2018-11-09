using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Controller.EventArgs
{
    public class CharactersHealthChangedEventArgs
    {
        public IReadOnlyDictionary<int, int> PostCharactersChanged;
        public IReadOnlyDictionary<int, int> PreCharactersChanged;
        public IReadOnlyDictionary<int, int> ChangeAmount;
    }
}
