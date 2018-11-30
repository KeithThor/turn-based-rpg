using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.Viewmodel
{
    public class ActionData
    {
        public DamageTypes Damage { get; set; }
        public DamageTypes ModifiedDamage { get; set; }
        public int Heal { get; set; }

        public IEnumerable<string> StatusEffects { get; set; }

        /// <summary>
        /// Checks the viewmodel for any data that is not 0 or null, and returns a dictionary containing
        /// the not null or 0 values, using a displayable name for the key.
        /// </summary>
        /// <returns>A dictionary containing string keys and dynamic values.</returns>
        public Dictionary<string, dynamic> GetDisplayableValues()
        {
            var dictionary = new Dictionary<string, dynamic>();
            if (ModifiedDamage.Physical > 0) dictionary["Physical"] = ModifiedDamage.Physical;
            if (ModifiedDamage.Fire > 0) dictionary["Fire"] = ModifiedDamage.Fire;
            if (ModifiedDamage.Frost > 0) dictionary["Frost"] = ModifiedDamage.Frost;
            if (ModifiedDamage.Lightning > 0) dictionary["Lightning"] = ModifiedDamage.Lightning;
            if (ModifiedDamage.Shadow > 0) dictionary["Shadow"] = ModifiedDamage.Shadow;
            if (ModifiedDamage.Light > 0) dictionary["Light"] = ModifiedDamage.Light;
            if (Heal > 0) dictionary["Heal"] = Heal;
            if (StatusEffects != null && StatusEffects.Count() > 0) dictionary["StatusEffects"] = StatusEffects;

            return dictionary;
        }
    }
}
