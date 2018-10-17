using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.Shared.Viewmodel;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Shared.EventArgs;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Controller responsible for handling combat interactions.
    /// </summary>
    public class CombatController
    {
        private CharacterFactory _characterFactory;
        private ActionController _actionController;
        private ViewModelController _viewModelController;
        private List<Character> AllCharacters;
        private List<Character> PlayerCharacters;
        private List<Character> EnemyCharacters;

        private List<Character> CurrentTurnOrder;
        private List<Character> NextTurnOrder;
        public int TurnCounter = 1;

        /// <summary>
        /// Event that is triggered at the end of a turn.
        /// </summary>
        public event EventHandler<EndOfTurnEventArgs> EndOfTurn;

        public CombatController(CharacterFactory characterFactory, 
                                ActionController actionController,
                                ViewModelController viewModelController)
        {
            _characterFactory = characterFactory;
            _actionController = actionController;
            _actionController.CharactersDied += OnCharactersDying;
            _viewModelController = viewModelController;
            PlayerCharacters = new List<Character>()
            {
                _characterFactory.Create(1),
                _characterFactory.Create(2)
            };
            PlayerCharacters[0].Position = 3;
            PlayerCharacters[1].Position = 9;
            EnemyCharacters = new List<Character>()
            {
                _characterFactory.Create(3),
                _characterFactory.Create(3),
                _characterFactory.Create(3),
                _characterFactory.Create(3),
                _characterFactory.Create(3)
            };
            EnemyCharacters[0].Position = 14;
            EnemyCharacters[1].Position = 13;
            EnemyCharacters[2].Position = 15;
            EnemyCharacters[3].Position = 12;
            EnemyCharacters[4].Position = 17;
            AllCharacters = new List<Character>(PlayerCharacters);
            AllCharacters.AddRange(EnemyCharacters);
            CurrentTurnOrder = DetermineTurnOrder();
            NextTurnOrder = new List<Character>(CurrentTurnOrder);
            _actionController.AllCharacters = AllCharacters;
        }

        private void OnCharactersDying(object sender, CharactersDiedEventArgs args)
        {
            CurrentTurnOrder.RemoveAll(character => args.DyingCharacters.Contains(character));
            NextTurnOrder.RemoveAll(character => args.DyingCharacters.Contains(character));
        }

        /// <summary>
        /// Ends the current turn and broadcasts the EndOfTurn event.
        /// </summary>
        public void EndTurn()
        {
            // Start event broadcast
            var eventArgs = new EndOfTurnEventArgs()
            {
                EndOfTurnCharacterId = CurrentTurnOrder[0].Id,
                CurrentTurnNumber = TurnCounter
            };
            // If this is the last turn of the round, start the next round and prepare the round after
            if(CurrentTurnOrder.Count == 1)
            {
                CurrentTurnOrder = new List<Character>(NextTurnOrder);
                NextTurnOrder = DetermineTurnOrder();
            }
            // Remove the current turn from the turn order list
            else
            {
                var indecesToCull = new List<int>();
                int maxIndex = CurrentTurnOrder.Count - 1;
                Character charToMove = null;
                for (int i = maxIndex; i >= 0; i--)
                {
                    var temp = charToMove;
                    charToMove = CurrentTurnOrder[i];
                    CurrentTurnOrder[i] = temp;
                    if (CurrentTurnOrder[i] == null || CurrentTurnOrder[i].CurrentHealth <= 0)
                        indecesToCull.Add(i);
                }
                foreach (var index in indecesToCull)
                {
                    CurrentTurnOrder.RemoveAt(index);
                }
            }

            EndOfTurn(this, eventArgs);
            StartTurn();
        }

        /// <summary>
        /// Calls events that should happen at the start of the turn.
        /// </summary>
        private void StartTurn()
        {
            _actionController.StartTurn(CurrentTurnOrder[0]);
        }

        /// <summary>
        /// Gets the Id of the character whose turn it is now.
        /// </summary>
        /// <returns>The Id of the character whose turn it is now.</returns>
        public int GetActiveCharacterID()
        {
            if (CurrentTurnOrder == null || CurrentTurnOrder.Count == 0)
                DetermineTurnOrder();
            return CurrentTurnOrder[0].Id;
        }

        /// <summary>
        /// Returns a list of characters containing all living characters currently in combat.
        /// </summary>
        /// <returns>A list of character containing living characters.</returns>
        private List<Character> GetAllLivingCharacters()
        {
            var livingCharacters = new List<Character>(PlayerCharacters);
            livingCharacters.AddRange(EnemyCharacters);
            livingCharacters.RemoveAll(character => character.CurrentHealth <= 0);
            return livingCharacters;
        }

        /// <summary>
        /// Gets the Id of the player character who will be taking it's turn next.
        /// If the currently acting character is a player character, return it.
        /// </summary>
        /// <returns>The Id of the next player character to act.</returns>
        public int GetNextActivePlayerId()
        {
            foreach (var character in CurrentTurnOrder)
            {
                if (PlayerCharacters.Contains(character) && character.CurrentHealth > 0)
                    return character.Id;
            }
            foreach (var character in NextTurnOrder)
            {
                if (PlayerCharacters.Contains(character) && character.CurrentHealth > 0)
                    return character.Id;
            }
            return -1;
        }
        
        /// <summary>
        /// Returns a list of character sorted by descending order of all characters' speed stats, with
        /// higher speed stats starting first.
        /// </summary>
        /// <returns></returns>
        private List<Character> DetermineTurnOrder()
        {
            var turnOrderList = GetAllLivingCharacters();
            turnOrderList.Sort((character1, character2) => character2.CurrentStats.Speed.CompareTo(character1.CurrentStats.Speed));
            return turnOrderList;
        }

        /// <summary>
        /// Performs a character action.
        /// </summary>
        /// <param name="actionType">The type of action that is being performed, such as Attack or Spells.</param>
        /// <param name="category">The category of the action being performed, may be left blank if the action has no categories.</param>
        /// <param name="index">The index of the action being performed.</param>
        /// <param name="actionTargetPositions">The list of positions the action is targeting.</param>
        public void StartAction(Actions actionType, string category, int index, IReadOnlyList<int> actionTargetPositions)
        {
            

            switch (actionType)
            {
                case Actions.Attack:
                    var attack = GetActionsFromCategory<Attack>(actionType, category)[index];
                    _actionController.StartAction(CurrentTurnOrder[0], attack, actionTargetPositions);
                    break;
                case Actions.Spells:
                    var spell = GetActionsFromCategory<Spell>(actionType, category)[index];
                    _actionController.StartAction(CurrentTurnOrder[0], spell, actionTargetPositions);
                    break;
                case Actions.Items:
                    var item = GetConsumablesFromCategory(category)[index];
                    item.Charges--;
                    _actionController.StartAction(CurrentTurnOrder[0], item.ItemSpell, actionTargetPositions);
                    break;
                case Actions.Pass:
                    CurrentTurnOrder.Add(CurrentTurnOrder[0]);
                    break;
                default:
                    break;
            }
            
            EndTurn();
        }

        /// <summary>
        /// Checks to see if a target position is occupied by a character. May choose whether to include checking
        /// for dead characters.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="includeDeadCharacters">Whether or not to count dead characters as occupying this position.</param>
        /// <returns>If true, this position is occupied.</returns>
        public bool IsPositionOccupied(int position, bool includeDeadCharacters = true)
        {
            Character target;
            if (includeDeadCharacters)
                target = AllCharacters.FirstOrDefault(character => character.Position == position);
            else
                target = GetAllLivingCharacters().FirstOrDefault(character => character.Position == position);
            if (target == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Returns an IEnumerable containing the Ids of all the player's characters
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetPlayerCharacterIds()
        {
            return PlayerCharacters.Select(character => character.Id);
        }

        /// <summary>
        /// Gets an IDisplayCharacter of a character that occupies a target position. Returns null if
        /// no characters occupy that position.
        /// </summary>
        /// <param name="targetPosition">The position of the target to return.</param>
        /// <returns>The character that occupied the target position.</returns>
        public IDisplayCharacter GetDisplayCharacterFromPosition(int targetPosition)
        {
            var target = AllCharacters.Find(character => character.Position == targetPosition);
            if (target == null)
                return null;
            return target;
        }

        /// <summary>
        /// Gets an IDisplayCharacter of a character using the character's Id. Returns null if no
        /// characters exist with that Id.
        /// </summary>
        /// <param name="targetId">The Id of the target to return.</param>
        /// <returns>The character whose Id matches the target Id.</returns>
        public IDisplayCharacter GetDisplayCharacterFromId(int targetId)
        {
            var target = AllCharacters.Find(character => character.Id == targetId);
            if (target == null)
                return null;
            return target;
        }

        /// <summary>
        /// Gets a List of IDisplayCharacters from the current and next turns.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IDisplayCharacter>[] GetTurnOrderAsDisplayCharacters()
        {
            var displayCharacters = new List<IDisplayCharacter>[2];
            displayCharacters[0] = new List<IDisplayCharacter>(CurrentTurnOrder);
            displayCharacters[1] = new List<IDisplayCharacter>(NextTurnOrder);
            return displayCharacters;
        }

        /// <summary>
        /// Gets all characters currently in battle as IDisplayCharacters
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IDisplayCharacter> GetAllDisplayableCharacters()
        {
            return new List<IDisplayCharacter>(AllCharacters);
        }

        /// <summary>
        /// Given an action type, returns all the categories that exist for that character.
        /// </summary>
        /// <param name="actionType">The action type to return categories for.</param>
        /// <returns>A List of string arrays where the first index is the category name and the second 
        /// the category description </returns>
        public IReadOnlyList<string[]> GetCategories(Actions actionType)
        {
            var categories = new List<string[]>();
            var activeCharacter = CurrentTurnOrder[0];
            switch(actionType)
            {
                case Actions.Spells:
                    foreach (var spell in activeCharacter.SpellList)
                    {
                        // Only get unique categories
                        if (!categories.Exists(category => category[0] == spell.Category))
                            categories.Add(new string[] { spell.Category, spell.CategoryDescription });
                    }
                    break;
                case Actions.Skills:
                    foreach (var skill in activeCharacter.SkillList)
                    {
                        if (!categories.Exists(category => category[0] == skill.Category))
                            categories.Add(new string[] { skill.Category, skill.CategoryDescription });
                    }
                    break;
                case Actions.Items:
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
        /// Retrieves a subaction depending on the action type, the category, and the index of the subaction in the
        /// category modified list. Returns null if not found.
        /// </summary>
        /// <param name="actionType">The type of action the subaction belongs to.</param>
        /// <param name="category">The category the subaction exists in.</param>
        /// <param name="index">The index of the subaction in the category-modified subaction list.</param>
        /// <returns>The subaction that was found.</returns>
        public IDisplayAction GetSubActionFromCategory(Actions actionType, string category, int index)
        {
            // Todo: finish returning IDisplayable for each action type that can return an IDisplayable
            switch (actionType)
            {
                case Actions.Attack:
                    return GetActionsFromCategory<Attack>(actionType, category)[index];
                case Actions.Spells:
                    return GetActionsFromCategory<Spell>(actionType, category)[index];
                case Actions.Skills:
                    return GetActionsFromCategory<Skill>(actionType, category)[index];
                case Actions.Items:
                    return GetConsumablesFromCategory(category)[index];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Given the action type and a category name, returns a full list of subactions of that action type
        /// that belongs to the category.
        /// </summary>
        /// <param name="actionType">The action type the subaction belongs to.</param>
        /// <param name="category">The category of the subactions to return.</param>
        /// <returns></returns>
        public IReadOnlyList<IDisplayAction> GetActionListFromCategory(Actions actionType, string category)
        {
            switch (actionType)
            {
                case Actions.Attack:
                    return new List<IDisplayAction>(GetActionsFromCategory<Attack>(actionType, category));
                case Actions.Spells:
                    return new List<IDisplayAction>(GetActionsFromCategory<Spell>(actionType, category));
                case Actions.Skills:
                    return new List<IDisplayAction>(GetActionsFromCategory<Skill>(actionType, category));
                case Actions.Items:
                    return new List<IDisplayAction>(GetConsumablesFromCategory(category));
                default:
                    return new List<IDisplayAction>();
            }
        }

        /// <summary>
        /// Gets the viewdata from an action, provided the action type, category and index of the action.
        /// </summary>
        /// <param name="actionType">The type of the action being performed.</param>
        /// <param name="category">The name of the category the action belongs to.</param>
        /// <param name="index">The index of the action being performed.</param>
        /// <returns>Viewdata for a given action.</returns>
        public SubActionData GetSubActionViewData(Actions actionType, string category, int index)
        {
            switch (actionType)
            {
                case Actions.Attack:
                    var attack = GetActionsFromCategory<Attack>(actionType, category)[index];
                    return _viewModelController.GetActionData(CurrentTurnOrder[0], attack);
                case Actions.Spells:
                    var spell = GetActionsFromCategory<Spell>(actionType, category)[index];
                    return _viewModelController.GetActionData(CurrentTurnOrder[0], spell);
                case Actions.Items:
                    var item = GetConsumablesFromCategory(category)[index];
                    return _viewModelController.GetActionData(CurrentTurnOrder[0], item.ItemSpell);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Given an action type and a category name, returns all subactions that belongs to both that action type and category
        /// contained by the currently active character as type T.
        /// </summary>
        /// <typeparam name="T">The type to return the actions as. Must inherit ActionBase class.</typeparam>
        /// <param name="actionType">The type of action the subaction belongs to.</param>
        /// <param name="category">The name of the category the subaction belongs to.</param>
        /// <returns>A list of T containing the subactions.</returns>
        private List<T> GetActionsFromCategory<T>(Actions actionType, string category) where T : ActionBase
        {
            // Todo: Finish returning all actions that have a category
            var actions = new List<T>();
            switch (actionType)
            {
                case Actions.Attack:
                    foreach (var attack in CurrentTurnOrder[0].Attacks)
                    {
                        actions.Add(attack as T);
                    }
                    return actions;
                case Actions.Spells:
                    foreach (var spell in CurrentTurnOrder[0].SpellList.Where(spell => spell.Category == category))
                    {
                        actions.Add(spell as T);
                    }
                    return actions;
                case Actions.Skills:
                    foreach (var skill in CurrentTurnOrder[0].SkillList.Where(skill => skill.Category == category))
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
        private List<Consumable> GetConsumablesFromCategory(string category)
        {
            var consumables = CurrentTurnOrder[0].Inventory
                                .Where(item => item is Consumable)
                                .Select(item => (Consumable)item);
            if (consumables != null)
                return consumables.Where(item => item.ConsumableCategory.Name == category).ToList();
            else
                return new List<Consumable>();
        }
    }
}
