using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared.Viewmodel;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Controller responsible for constructing view model data.
    /// </summary>
    public class ViewModelController
    {
        public SubActionData GetActionData(Character character, ActionBase action)
        {
            var data = new SubActionData();
            var damage = DamageCalculator.GetDamage(character, action);
            data.Damage = action.Damage;
            data.ModifiedDamage = damage;
            data.Heal = action.HealAmount;
            data.StatusEffects = new List<string>(action.BuffsToApply.Select(status => status.Name));
            return data;
        }
    }
}
