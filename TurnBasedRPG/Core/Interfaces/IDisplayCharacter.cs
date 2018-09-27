using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Core.Interfaces
{
    public interface IDisplayCharacter
    {
        int GetId();
        int GetCurrenthealth();
        int GetMaxHealth();
        string GetName();
        char GetSymbol();
        int GetPosition();
    }
}
