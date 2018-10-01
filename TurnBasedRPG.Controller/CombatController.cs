using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.EventArgs;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Controllers
{
    public class CombatController
    {
        private List<Character> AllCharacters;
        private List<Character> PlayerCharacters;
        private List<Character> EnemyCharacters;

        private List<Character> CurrentTurnOrder;
        private List<Character> NextTurnOrder;
        public int TurnCounter = 1;

        public event EventHandler<EndOfTurnEventArgs> EndOfTurn;

        public CombatController(List<Character> playerCharacters, List<Character> enemyCharacters)
        {
            PlayerCharacters = playerCharacters;
            EnemyCharacters = enemyCharacters;
            AllCharacters = new List<Character>(PlayerCharacters);
            AllCharacters.AddRange(EnemyCharacters);
            CurrentTurnOrder = DetermineTurnOrder();
            NextTurnOrder = new List<Character>(CurrentTurnOrder);
        }

        // Called whenever a turn is ending to advance to the next character's turn
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
        }

        // Gets the ID of the character who's turn is now
        public int GetActiveCharacterID()
        {
            if (CurrentTurnOrder == null || CurrentTurnOrder.Count == 0)
                DetermineTurnOrder();
            return CurrentTurnOrder[0].Id;
        }

        private List<Character> GetAllLivingCharacters()
        {
            var livingCharacters = new List<Character>(PlayerCharacters);
            livingCharacters.AddRange(EnemyCharacters);
            livingCharacters.RemoveAll(character => character.CurrentHealth <= 0);
            return livingCharacters;
        }

        // Gets the current (or next if it is the enemy's turn) player character who's turn is now
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
        
        // Returns a List of characters ordered by which characters perform first to last
        private List<Character> DetermineTurnOrder()
        {
            var turnOrderList = GetAllLivingCharacters();
            turnOrderList.Sort((character1, character2) => character1.CurrentSpeed.CompareTo(character2.CurrentSpeed));
            return turnOrderList;
        }

        // Performs an action
        public void StartAction(string category, int actionId, Actions actionType, IReadOnlyList<int> actionTargetPositions)
        {
            switch(actionType)
            {
                case Actions.Attack:
                case Actions.Spells:
                case Actions.Skills:
                case Actions.Items:
                    var targetCharacters = new List<Character>();
                    foreach (var position in actionTargetPositions)
                    {
                        for (int i = 0; i < AllCharacters.Count; i++)
                        {
                            if (AllCharacters[i].Position == position)
                                targetCharacters.Add(AllCharacters[i]);
                        }
                    }

                    foreach (var character in targetCharacters)
                    {
                        character.CurrentHealth -= 40;
                    }
                    break;
                case Actions.Pass:
                    CurrentTurnOrder.Add(CurrentTurnOrder[0]);
                    break;
            }
            
            EndTurn();
        }

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

        public IEnumerable<int> GetPlayerCharacterIds()
        {
            return PlayerCharacters.Select(character => character.Id);
        }

        public IDisplayCharacter GetDisplayCharacterFromPosition(int targetPosition)
        {
            var target = AllCharacters.Find(character => character.Position == targetPosition);
            if (target == null)
                return null;
            return target;
        }

        public IDisplayCharacter GetDisplayCharacterFromId(int targetId)
        {
            var target = AllCharacters.Find(character => character.Id == targetId);
            if (target == null)
                return null;
            return target;
        }

        public IReadOnlyList<IDisplayCharacter>[] GetTurnOrderAsDisplayCharacters()
        {
            var displayCharacters = new List<IDisplayCharacter>[2];
            displayCharacters[0] = new List<IDisplayCharacter>(CurrentTurnOrder);
            displayCharacters[1] = new List<IDisplayCharacter>(NextTurnOrder);
            return displayCharacters;
        }

        public IReadOnlyList<IDisplayCharacter> GetAllDisplayableCharacters()
        {
            return new List<IDisplayCharacter>(AllCharacters);
        }

        public IReadOnlyList<string[]> GetCategories(Actions actionType)
        {
            var categories = new List<string[]>();
            var activeCharacter = CurrentTurnOrder[0];
            switch(actionType)
            {
                case Actions.Spells:
                    foreach (var spell in activeCharacter.SpellList)
                    {
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
                default:
                    break;
            }
            return categories;
        }

        public IDisplayAction GetActionFromCategory(Actions actionType, string category, int index)
        {
            // Todo: finish returning IDisplayable for each action type that can return an IDisplayable
            switch (actionType)
            {
                case Actions.Attack:
                    return GetActionsFromCategory<Attack>(actionType, category).ElementAt(index);
                case Actions.Spells:
                    return GetActionsFromCategory<Spell>(actionType, category).ElementAt(index);
                case Actions.Skills:
                    return GetActionsFromCategory<Skill>(actionType, category).ElementAt(index);
                default:
                    return null;
            }
        }

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
                default:
                    return new List<IDisplayAction>();
            }
        }

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
    }
}
