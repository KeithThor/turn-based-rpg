using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared
{
    /// <summary>
    /// Represents the primary stats of a character, such as strength and agility.
    /// </summary>
    public class PrimaryStat
    {
        public PrimaryStat() { }
        public PrimaryStat(PrimaryStat stats)
        {
            Agility = stats.Agility;
            Intellect = stats.Intellect;
            Speed = stats.Speed;
            Stamina = stats.Stamina;
            Strength = stats.Strength;
        }

        public int Agility { get; set; } = 0;
        public int Intellect { get; set; } = 0;
        public int Speed { get; set; } = 0;
        public int Stamina { get; set; } = 0;
        public int Strength { get; set; } = 0;

        /// <summary>
        /// Returns all the possible stat types as one int array.
        /// </summary>
        /// <returns>An array containing all the possible stat types.</returns>
        public int[] GetStatTypesAsArray()
        {
            return new int[] { Agility, Intellect, Speed, Stamina, Strength };
        }
        
        public static PrimaryStat operator - (PrimaryStat a, PrimaryStat b)
        {
            return new PrimaryStat()
            {
                Agility = a.Agility - b.Agility,
                Intellect = a.Intellect - b.Intellect,
                Speed = a.Speed - b.Speed,
                Stamina = a.Stamina - b.Stamina,
                Strength = a.Strength - b.Strength
            };
        }

        public static PrimaryStat operator +(PrimaryStat a, PrimaryStat b)
        {
            return new PrimaryStat()
            {
                Agility = a.Agility + b.Agility,
                Intellect = a.Intellect + b.Intellect,
                Speed = a.Speed + b.Speed,
                Stamina = a.Stamina + b.Stamina,
                Strength = a.Strength + b.Strength
            };
        }

        public static PrimaryStat operator *(PrimaryStat a, int b)
        {
            return new PrimaryStat()
            {
                Agility = a.Agility * b,
                Intellect = a.Intellect * b,
                Speed = a.Speed * b,
                Stamina = a.Stamina * b,
                Strength = a.Strength * b
            };
        }
    }
}
