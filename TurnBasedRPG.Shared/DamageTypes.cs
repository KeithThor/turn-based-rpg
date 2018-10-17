using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared
{
    /// <summary>
    /// Contains properties representing each type of damage to be used as damage or armor amounts.
    /// </summary>
    public class DamageTypes
    {
        public DamageTypes() { }
        public DamageTypes(DamageTypes damageType)
        {
            Physical = damageType.Physical;
            Fire = damageType.Fire;
            Frost = damageType.Frost;
            Lightning = damageType.Lightning;
            Shadow = damageType.Shadow;
            Light = damageType.Light;
        }
        public DamageTypes(int[] values)
        {
            if (values.Count() != 6)
                throw new Exception("Can only initialize DamageTypes with an int array of size 6.");
            else
            {
                Physical = values[0];
                Fire = values[1];
                Frost = values[2];
                Lightning = values[3];
                Shadow = values[4];
                Light = values[5];
            }
        }

        public int Physical { get; set; } = 0;
        public int Fire { get; set; } = 0;
        public int Frost { get; set; } = 0;
        public int Lightning { get; set; } = 0;
        public int Shadow { get; set; } = 0;
        public int Light { get; set; } = 0;

        /// <summary>
        /// Returns all the possible damage types as one int array.
        /// </summary>
        /// <returns>An array containing all the possible damage types.</returns>
        public int[] AsArray()
        {
            return new int[] { Physical, Fire, Frost, Lightning, Shadow, Light };
        }

        public static DamageTypes operator- (DamageTypes a, DamageTypes b)
        {
            var damage = new DamageTypes
            {
                Physical = a.Physical - b.Physical,
                Fire = a.Fire - b.Fire,
                Frost = a.Frost - b.Frost,
                Lightning = a.Lightning - b.Lightning,
                Shadow = a.Shadow - b.Shadow,
                Light = a.Light - b.Light
            };
            return damage;
        }

        public static DamageTypes operator +(DamageTypes a, DamageTypes b)
        {
            var damage = new DamageTypes
            {
                Physical = a.Physical + b.Physical,
                Fire = a.Fire + b.Fire,
                Frost = a.Frost + b.Frost,
                Lightning = a.Lightning + b.Lightning,
                Shadow = a.Shadow + b.Shadow,
                Light = a.Light + b.Light
            };
            return damage;
        }

        public static DamageTypes operator *(DamageTypes a, int b)
        {
            return new DamageTypes
            {
                Physical = a.Physical * b,
                Fire = a.Fire * b,
                Frost = a.Frost * b,
                Lightning = a.Lightning * b,
                Shadow = a.Shadow * b,
                Light = a.Light * b
            };
        }

        public static DamageTypes operator /(DamageTypes a, int b)
        {
            return new DamageTypes
            {
                Physical = a.Physical / b,
                Fire = a.Fire / b,
                Frost = a.Frost / b,
                Lightning = a.Lightning / b,
                Shadow = a.Shadow / b,
                Light = a.Light / b
            };
        }
    }
}
