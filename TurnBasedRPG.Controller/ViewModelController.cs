using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Viewmodel;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Controller responsible for constructing view model data.
    /// </summary>
    public class ViewModelController
    {
        private readonly DisplayManager _displayManager;
        private readonly CombatStateHandler _combatStateHandler;

        public ViewModelController(DisplayManager displayManager,
                                   CombatStateHandler combatStateHandler)
        {
            _displayManager = displayManager;
            _combatStateHandler = combatStateHandler;
        }

        /// <summary>
        /// Gets the viewdata from an action, provided the action type, category and index of the action.
        /// </summary>
        /// <param name="commandType">The type of the command being performed.</param>
        /// <param name="category">The name of the category the action belongs to.</param>
        /// <param name="index">The index of the action being performed.</param>
        /// <returns>Viewdata for a given action.</returns>
        public ActionData GetActionViewData(Commands commandType, string category, int index)
        {
            switch (commandType)
            {
                case Commands.Attack:
                    var attack = _displayManager.GetActionsFromCategory<Attack>(commandType, category)[index];
                    return CreateActionData(_combatStateHandler.CurrentRoundOrder[0], attack);
                case Commands.Spells:
                    var spell = _displayManager.GetActionsFromCategory<Spell>(commandType, category)[index];
                    return CreateActionData(_combatStateHandler.CurrentRoundOrder[0], spell);
                case Commands.Items:
                    var item = _displayManager.GetConsumablesFromCategory(category)[index];
                    return CreateActionData(_combatStateHandler.CurrentRoundOrder[0], item.ItemSpell);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates ActionData using data from a character and that character's action.
        /// </summary>
        /// <param name="character">The character to use data from.</param>
        /// <param name="action">The action to use data from.</param>
        /// <returns>A struct containing display data about the action.</returns>
        private ActionData CreateActionData(Character character, ActionBase action)
        {
            var data = new ActionData();
            var damage = DamageCalculator.GetDamage(character, action);
            data.Damage = action.Damage;
            data.ModifiedDamage = damage;
            data.Heal = action.HealAmount;
            data.StatusEffects = new List<string>(action.BuffsToApply.Select(status => status.Name));
            return data;
        }
    }
}
