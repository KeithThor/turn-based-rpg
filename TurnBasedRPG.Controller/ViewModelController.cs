using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Controller.Interfaces;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Viewmodel;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Controller responsible for constructing view model data.
    /// </summary>
    public class ViewModelController : IViewModelController
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
            var action = GetAction(commandType, category, index);
            return CreateActionData(_combatStateHandler.CurrentRoundOrder[0], action);
        }

        /// <summary>
        /// Gets the viewdata from a status effect, provided the command type, category and index of the action the status
        /// belongs to.
        /// </summary>
        /// <param name="commandType">The type of the command of the action that the status effect belongs to.</param>
        /// <param name="category">The category of the action the status belongs to.</param>
        /// <param name="actionIndex">The index of the action the status belongs to.</param>
        /// <param name="statusIndex">The index of the status if there are more than one status effects for the action.</param>
        /// <returns></returns>
        public StatusData GetStatusViewData(Commands commandType, string category, int actionIndex, int statusIndex)
        {
            var action = GetAction(commandType, category, actionIndex);
            var statuses = action.BuffsToApply;
            var data = new StatusData()
            {
                Name = statuses[statusIndex].Name,
                Description = statuses[statusIndex].Description,
                DamageData = GetStatusDamageData(_combatStateHandler.CurrentRoundOrder[0], statuses[statusIndex]),
                StatusEffectsData = GetStatusEffectsData(statuses[statusIndex])
            };

            return data;
        }

        /// <summary>
        /// Gets an action corresponding to the command type, category and index of the action. Returns null if no action was found.
        /// </summary>
        /// <param name="commandType">The type of command the action belongs to.</param>
        /// <param name="category">The category the action belongs to.</param>
        /// <param name="index">The index of the action to find.</param>
        /// <returns></returns>
        private ActionBase GetAction(Commands commandType, string category, int index)
        {
            switch (commandType)
            {
                case Commands.Attack:
                    var attack = _displayManager.GetActionsFromCategory<Attack>(commandType, category)[index];
                    return attack;
                case Commands.Spells:
                    var spell = _displayManager.GetActionsFromCategory<Spell>(commandType, category)[index];
                    return spell;
                case Commands.Items:
                    var item = _displayManager.GetConsumablesFromCategory(category)[index];
                    return item.ItemSpell;
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

        /// <summary>
        /// Gets the bulk of the display data for a status effect. If the status does not affect a particular stat,
        /// does not add that to the data list.
        /// </summary>
        /// <param name="status">The status to display data for.</param>
        /// <returns>A list containing a key value pair where the key is the display name of the stat, and the 
        /// value the amount of the stat</returns>
        private IReadOnlyList<KeyValuePair<string, int>> GetStatusEffectsData(StatusEffect status)
        {
            var data = new List<KeyValuePair<string, int>>();

            // Get bulk data
            var armor = GetElementalData(status.Armor, "Armor");
            var resistances = GetElementalData(status.ArmorPercentage, "Resistance");
            var damageModifier = GetElementalData(status.DamageModifier, "Bonus Damage");
            var damagePercentage = GetElementalData(status.DamagePercentageModifier, "% Bonus Damage");
            var stats = GetStatsData(status.ModifyStats);
            var duration = new KeyValuePair<string, int>("Duration", status.Duration);

            // Add any damage modifiers
            data.AddRange(damageModifier);
            data.AddRange(damagePercentage);

            if (status.CritChance != 0)
                data.Add(new KeyValuePair<string, int>("+ Crit Chance", status.CritChance));
            if (status.CritMultiplier != 0)
                data.Add(new KeyValuePair<string, int>("+ Crit Damage", status.CritChance));
            if (status.SpellDamageModifier != 0)
                data.Add(new KeyValuePair<string, int>("Spell Damage", status.SpellDamageModifier));
            if (status.SpellDamagePercentageModifier != 0)
                data.Add(new KeyValuePair<string, int>("% Spell Damage", status.SpellDamagePercentageModifier));

            // Add any defensive stats
            data.AddRange(armor);
            data.AddRange(resistances);

            if (status.ResistAll != 0)
                data.Add(new KeyValuePair<string, int>("Resist All", status.ResistAll));
            if (status.ResistAllPercentage != 0)
                data.Add(new KeyValuePair<string, int>("% Resist All", status.ResistAllPercentage));

            // Add any bonuses to primary stats
            if (status.ModifyMaxHealth != 0)
                data.Add(new KeyValuePair<string, int>("Max Health", status.ModifyMaxHealth));
            if (status.ModifyMaxMana != 0)
                data.Add(new KeyValuePair<string, int>("Max Mana", status.ModifyMaxMana));

            data.AddRange(stats);

            // Lastly, add the duration
            if (status.Stackable)
                data.Add(new KeyValuePair<string, int>("Stacks", status.StackSize));

            data.Add(duration);

            return data;
        }

        /// <summary>
        /// Gets the amount of damage, healing, and crit chance/damage a status effect does as a list of key value pairs.
        /// </summary>
        /// <param name="character">The character to use to calculate the amount of damage and healing a status effect does.</param>
        /// <param name="status">The status effect to get data for.</param>
        /// <returns>A list of key value pairs containing the name of the stat and the value of that stat.</returns>
        private List<KeyValuePair<string, int>> GetStatusDamageData(Character character, StatusEffect status)
        {
            var data = new List<KeyValuePair<string, int>>();

            var damage = GetElementalData(DamageCalculator.GetDamage(character, status), "Damage/Turn");
            // Calculate heals
            int healAmount = DamageCalculator.GetHealing(character, status);
            int healPercent = DamageCalculator.GetHealingPercentage(character, status);
            bool healsOrDamage = damage.Any() || healAmount != 0 || healPercent != 0;

            // Add damage first
            data.AddRange(damage);

            if (healsOrDamage)
            {
                data.Add(new KeyValuePair<string, int>("Crit Chance", status.DamageCritChance + character.CritChance));
                data.Add(new KeyValuePair<string, int>("Crit Damage", status.DamageCritMultiplier + character.CritMultiplier));
            }

            // Add any healing amounts
            if (healAmount != 0)
                data.Add(new KeyValuePair<string, int>("Health Restored/Turn", healAmount));
            if (healPercent != 0)
                data.Add(new KeyValuePair<string, int>("% Health Restored/Turn", healPercent));

            return data;
        }

        /// <summary>
        /// Constructs a list of key value pairs containing any element values that are not 0 along
        /// with the proper UI displayable name.
        /// </summary>
        /// <param name="damageTypes">The object containing the element values.</param>
        /// <param name="appendString">Whether this is Armor, Damage or Resistance.</param>
        /// <returns></returns>
        private List<KeyValuePair<string, int>> GetElementalData(DamageTypes damageTypes, string appendString)
        {
            var data = new List<KeyValuePair<string, int>>();

            if (damageTypes.Physical != 0) data.Add(new KeyValuePair<string, int>($"Physical {appendString}", damageTypes.Physical));
            if (damageTypes.Fire != 0) data.Add(new KeyValuePair<string, int>($"Fire {appendString}", damageTypes.Fire));
            if (damageTypes.Frost != 0) data.Add(new KeyValuePair<string, int>($"Frost {appendString}", damageTypes.Frost));
            if (damageTypes.Lightning != 0) data.Add(new KeyValuePair<string, int>($"Lightning {appendString}", damageTypes.Lightning));
            if (damageTypes.Shadow != 0) data.Add(new KeyValuePair<string, int>($"Shadow {appendString}", damageTypes.Shadow));
            if (damageTypes.Light != 0) data.Add(new KeyValuePair<string, int>($"Light {appendString}", damageTypes.Light));

            return data;
        }

        private List<KeyValuePair<string, int>> GetStatsData(PrimaryStat stats)
        {
            var data = new List<KeyValuePair<string, int>>();

            if (stats.Strength != 0) data.Add(new KeyValuePair<string, int>("Strength", stats.Strength));
            if (stats.Stamina != 0) data.Add(new KeyValuePair<string, int>("Stamina", stats.Stamina));
            if (stats.Intellect != 0) data.Add(new KeyValuePair<string, int>("Intellect", stats.Intellect));
            if (stats.Agility != 0) data.Add(new KeyValuePair<string, int>("Agility", stats.Agility));
            if (stats.Speed != 0) data.Add(new KeyValuePair<string, int>("Speed", stats.Speed));

            return data;
        }
    }
}
