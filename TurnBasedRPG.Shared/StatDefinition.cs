using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared
{
    /// <summary>
    /// Contains the definitions for how much a primary stat contributes to a character's other stats.
    /// </summary>
    public static class StatDefinition
    {
        public static int GetHealthFromStamina(int stamina)
        {
            return stamina * 15;
        }

        public static int GetManaFromIntellect(int intellect)
        {
            return intellect * 10;
        }

        public static int GetDamageFromStrength(int strength)
        {
            return strength;
        }
    }
}
