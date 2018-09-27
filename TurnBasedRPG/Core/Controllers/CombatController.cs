using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core.EventArgs;
using TurnBasedRPG.Core.Interfaces;
using TurnBasedRPG.Core.UI.Enums;
using TurnBasedRPG.Entities;

namespace TurnBasedRPG.Core.Controllers
{
    public class CombatController
    {
        private List<Character> AllCharacters;
        public List<Character> PlayerCharacters;
        public List<Character> EnemyCharacters;

        public List<Character> CurrentTurnOrder;
        public List<Character> NextTurnOrder;
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
                EndOfTurnCharacter = CurrentTurnOrder[0],
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

        public List<Character> GetAllLivingCharacters()
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
        public List<Character> DetermineTurnOrder()
        {
            var turnOrderList = GetAllLivingCharacters();
            turnOrderList.Sort((character1, character2) => character1.CurrentSpeed.CompareTo(character2.CurrentSpeed));
            return turnOrderList;
        }

        public void StartAction(int actorId, string category, int actionId, List<int> actionTargetPositions)
        {
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
            EndTurn();
        }

        public List<ActionBase> GetActiveActionList(int activeCharacterId, Actions actionType, string category)
        {
            var activeCharacter = AllCharacters.Find(character => character.Id == activeCharacterId);
            switch(actionType)
            {
                case Actions.Attack:
                    return activeCharacter.GetAttackAction();
                case Actions.Spells:
                    return new List<ActionBase>(activeCharacter.SpellList.Where(spell => spell.Category == category));
                case Actions.Skills:
                    return new List<ActionBase>(activeCharacter.SkillList.Where(skill => skill.Category == category));
                default:
                    return new List<ActionBase>();
            }
        }

        public List<IDisplayable> GetDisplayableActionList(int activeCharacterId, Actions actionType, string category)
        {
            var activeCharacter = AllCharacters.Find(character => character.Id == activeCharacterId);
            switch (actionType)
            {
                case Actions.Spells:
                    return new List<IDisplayable>(activeCharacter.SpellList.Where(spell => spell.Category == category));
                case Actions.Skills:
                    return new List<IDisplayable>(activeCharacter.SkillList.Where(skill => skill.Category == category));
                default:
                    return new List<IDisplayable>();
            }
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

        public List<int> GetPlayerCharacterIds()
        {
            return PlayerCharacters.Select(character => character.Id).ToList();
        }

        public IDisplayCharacter GetTargetDetailsFromPosition(int targetPosition)
        {
            var target = AllCharacters.Find(character => character.Position == targetPosition);
            if (target == null)
                return null;
            return target;
        }

        public IDisplayCharacter GetTargetDetailsFromId(int targetId)
        {
            var target = AllCharacters.Find(character => character.Id == targetId);
            if (target == null)
                return null;
            return target;
        }

        public List<IDisplayCharacter>[] GetTurnOrderDisplayCharacters()
        {
            var displayCharacters = new List<IDisplayCharacter>[2];
            displayCharacters[0] = new List<IDisplayCharacter>(CurrentTurnOrder);
            displayCharacters[1] = new List<IDisplayCharacter>(NextTurnOrder);
            return displayCharacters;
        }

        public List<IDisplayCharacter> GetAllDisplayableCharacters()
        {
            return new List<IDisplayCharacter>(AllCharacters);
        }

        public List<string[]> GetCategories(int activeCharacterId, Actions categoryType)
        {
            var categories = new List<string[]>();
            var activeCharacter = PlayerCharacters.FirstOrDefault(character => character.Id == activeCharacterId);
            switch(categoryType)
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
    }
}
