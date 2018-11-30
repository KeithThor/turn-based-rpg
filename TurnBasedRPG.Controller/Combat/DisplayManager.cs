using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Class responsible for handling the retrieval of display data.
    /// </summary>
    public class DisplayManager
    {
        private readonly CombatStateHandler _combatStateHandler;
        private readonly DisplayCharacterFactory _displayCharacterFactory;

        public DisplayManager(CombatStateHandler combatStateHandler)
        {
            _combatStateHandler = combatStateHandler;
            _displayCharacterFactory = new DisplayCharacterFactory();
        }

        /// <summary>
        /// Gets all characters currently in battle as IDisplayCharacters
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IDisplayCharacter> GetAllDisplayableCharacters()
        {
            return _combatStateHandler.AllCharacters;
        }

        /// <summary>
        /// Gets all characters currently in battle and returns them as a List of DisplayCharacters.
        /// </summary>
        /// <returns></returns>
        public List<DisplayCharacter> GetDisplayCharacters()
        {
            var displayList = new List<DisplayCharacter>();
            foreach (var character in _combatStateHandler.AllCharacters)
            {
                displayList.Add(_displayCharacterFactory.Create(character));
            }
            return displayList;
        }

        /// <summary>
        /// Given an action type, returns all the categories that exist for that character.
        /// </summary>
        /// <param name="commandType">The command type to return categories for.</param>
        /// <returns>A List of string arrays where the first index is the category name and the second 
        /// the category description </returns>
        public IReadOnlyList<string[]> GetCategories(Commands commandType)
        {
            var categories = new List<string[]>();
            var activeCharacter = _combatStateHandler.CurrentRoundOrder[0];
            switch (commandType)
            {
                case Commands.Spells:
                    foreach (var spell in activeCharacter.SpellList)
                    {
                        // Only get unique categories
                        if (!categories.Exists(category => category[0] == spell.Category))
                            categories.Add(new string[] { spell.Category, spell.CategoryDescription });
                    }
                    break;
                case Commands.Skills:
                    foreach (var skill in activeCharacter.SkillList)
                    {
                        if (!categories.Exists(category => category[0] == skill.Category))
                            categories.Add(new string[] { skill.Category, skill.CategoryDescription });
                    }
                    break;
                case Commands.Items:
                    foreach (var item in activeCharacter.Inventory)
                    {
                        if (item is Consumable)
                        {
                            if (!categories.Exists(category => ((Consumable)item).ConsumableCategory.Name == category[0]))
                                categories.Add(new string[] { ((Consumable)item).ConsumableCategory.Name,
                                                          ((Consumable)item).ConsumableCategory.Description });
                        }
                    }
                    break;
                default:
                    break;
            }
            return categories;
        }

        /// <summary>
        /// Retrieves a Action depending on the action type, the category, and the index of the Action in the
        /// category modified list. Returns null if not found.
        /// </summary>
        /// <param name="commandType">The type of command the Action belongs to.</param>
        /// <param name="category">The category the Action exists in.</param>
        /// <param name="index">The index of the Action in the category-modified Action list.</param>
        /// <returns>The Action that was found.</returns>
        public IDisplayAction GetActionFromCategory(Commands commandType, string category, int index)
        {
            // Todo: finish returning IDisplayable for each action type that can return an IDisplayable
            switch (commandType)
            {
                case Commands.Attack:
                    return GetActionsFromCategory<Attack>(commandType, category)[index];
                case Commands.Spells:
                    return GetActionsFromCategory<Spell>(commandType, category)[index];
                case Commands.Skills:
                    return GetActionsFromCategory<Skill>(commandType, category)[index];
                case Commands.Items:
                    return GetConsumablesFromCategory(category)[index];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Given the action type and a category name, returns a full list of Actions of that action type
        /// that belongs to the category.
        /// </summary>
        /// <param name="commandType">The command type the Action belongs to.</param>
        /// <param name="category">The category of the Actions to return.</param>
        /// <returns></returns>
        public IReadOnlyList<IDisplayAction> GetActionListFromCategory(Commands commandType, string category)
        {
            switch (commandType)
            {
                case Commands.Attack:
                    return new List<IDisplayAction>(GetActionsFromCategory<Attack>(commandType, category));
                case Commands.Spells:
                    return new List<IDisplayAction>(GetActionsFromCategory<Spell>(commandType, category));
                case Commands.Skills:
                    return new List<IDisplayAction>(GetActionsFromCategory<Skill>(commandType, category));
                case Commands.Items:
                    return new List<IDisplayAction>(GetConsumablesFromCategory(category));
                default:
                    return new List<IDisplayAction>();
            }
        }

        /// <summary>
        /// Given an action type and a category name, returns all Actions that belongs to both that action type and category
        /// contained by the currently active character as type T.
        /// </summary>
        /// <typeparam name="T">The type to return the actions as. Must inherit ActionBase class.</typeparam>
        /// <param name="commandType">The type of command the Action belongs to.</param>
        /// <param name="category">The name of the category the Action belongs to.</param>
        /// <returns>A list of T containing the Actions.</returns>
        internal List<T> GetActionsFromCategory<T>(Commands commandType, string category) where T : ActionBase
        {
            // Todo: Finish returning all actions that have a category
            var actions = new List<T>();
            switch (commandType)
            {
                case Commands.Attack:
                    foreach (var attack in _combatStateHandler.CurrentRoundOrder[0].Attacks)
                    {
                        actions.Add(attack as T);
                    }
                    return actions;
                case Commands.Spells:
                    foreach (var spell in _combatStateHandler.CurrentRoundOrder[0].SpellList.Where(spell => spell.Category == category))
                    {
                        actions.Add(spell as T);
                    }
                    return actions;
                case Commands.Skills:
                    foreach (var skill in _combatStateHandler.CurrentRoundOrder[0].SkillList.Where(skill => skill.Category == category))
                    {
                        actions.Add(skill as T);
                    }
                    return actions;
                default:
                    return actions;
            }
        }

        /// <summary>
        /// Given a category name, gets a list of consumables that belong to the current player character that
        /// belong to that category.
        /// </summary>
        /// <param name="category">The name of the category the consumables belong to.</param>
        /// <returns>A list of consumables belonging to the player character.</returns>
        internal List<Consumable> GetConsumablesFromCategory(string category)
        {
            var consumables = _combatStateHandler.CurrentRoundOrder[0].Inventory
                                                                      .Where(item => item is Consumable)
                                                                      .Select(item => (Consumable)item);
            if (consumables != null)
                return consumables.Where(item => item.ConsumableCategory.Name == category).ToList();
            else
                return new List<Consumable>();
        }
    }
}
