using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared.Viewmodel;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Controller responsible for constructing view model data.
    /// </summary>
    public class ViewModelController
    {
        private ActionController _actionController;

        public ViewModelController(ActionController actionController)
        {
            _actionController = actionController;
        }

        public SubActionData GetSpellData(Character character, Spell spell)
        {
            var data = new SubActionData();
            var damage = _actionController.GetSpellDamage(character, spell);
            data.Damage = spell.Damage;
            data.ModifiedDamage = damage;
            data.Heal = spell.HealAmount;
            data.StatusEffects = new List<string>(spell.BuffsToApply.Select(status => status.Name));
            return data;
        }

        public SubActionData GetAttackData(Character character, Attack attack)
        {
            var data = new SubActionData();
            var damage = _actionController.GetAttackDamage(character, attack);
            data.Damage = character.DamageModifier;
            data.ModifiedDamage = damage;
            data.Heal = 0;
            return data;
        }
    }
}
