using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Class responsible for handling the current state of combat.
    /// </summary>
    public class CombatStateHandler
    {
        /// <summary>
        /// Contains all characters currently in combat, including dead characters.
        /// </summary>
        internal List<Character> AllCharacters;

        /// <summary>
        /// Contains all player characters currently in combat, including dead characters.
        /// </summary>
        internal List<Character> PlayerCharacters;

        /// <summary>
        /// Contains all enemy ai characters currently in combat, including dead characters.
        /// </summary>
        internal List<Character> EnemyCharacters;

        /// <summary>
        /// Contains all the characters that still have a turn in the current round, sorted from characters performing their
        /// turns first to last.
        /// </summary>
        internal List<Character> CurrentRoundOrder;

        /// <summary>
        /// Contains all the characters that still have a turn in the next round, sorted from characters performing their
        /// turns first to last.
        /// </summary>
        internal List<Character> NextRoundOrder;

        private readonly CharacterFactory _characterFactory;

        public CombatStateHandler(CharacterFactory characterFactory)
        {
            _characterFactory = characterFactory;

            // Create debug data
            PlayerCharacters = new List<Character>()
            {
                _characterFactory.Create(1),
                _characterFactory.Create(2)
            };
            PlayerCharacters[0].Position = 3;
            PlayerCharacters[1].Position = 9;
            EnemyCharacters = new List<Character>()
            {
                _characterFactory.Create(4),
                _characterFactory.Create(3),
                _characterFactory.Create(3),
                _characterFactory.Create(3),
                _characterFactory.Create(3),
            };
            EnemyCharacters[0].Position = 14;
            EnemyCharacters[1].Position = 13;
            EnemyCharacters[2].Position = 15;
            EnemyCharacters[3].Position = 12;
            EnemyCharacters[4].Position = 17;
            AllCharacters = new List<Character>(PlayerCharacters);
            AllCharacters.AddRange(EnemyCharacters);

            CurrentRoundOrder = DetermineTurnOrder();
            NextRoundOrder = new List<Character>(CurrentRoundOrder);
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
        /// Checks if it is the player's turn.
        /// </summary>
        /// <returns></returns>
        public bool IsPlayerTurn()
        {
            return PlayerCharacters.Contains(CurrentRoundOrder[0]);
        }

        /// <summary>
        /// Gets the Id of the character whose turn it is now.
        /// </summary>
        /// <returns>The Id of the character whose turn it is now.</returns>
        public int GetActiveCharacterID()
        {
            if (CurrentRoundOrder == null || CurrentRoundOrder.Count == 0)
                DetermineTurnOrder();
            return CurrentRoundOrder[0].Id;
        }

        /// <summary>
        /// Returns a list of characters containing all living characters currently in combat.
        /// </summary>
        /// <returns>A list of character containing living characters.</returns>
        internal List<Character> GetAllLivingCharacters()
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
            foreach (var character in CurrentRoundOrder)
            {
                if (PlayerCharacters.Contains(character) && character.CurrentHealth > 0)
                    return character.Id;
            }
            foreach (var character in NextRoundOrder)
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
        /// Returns an IEnumerable containing the Ids of all the player's characters
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetPlayerCharacterIds()
        {
            return PlayerCharacters.Select(character => character.Id);
        }

        /// <summary>
        /// Gets a List of ids from the current and next turns.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<int>[] GetRoundOrderIds()
        {
            var displayCharacters = new List<int>[2];
            displayCharacters[0] = new List<int>(CurrentRoundOrder.Select(chara => chara.Id));
            displayCharacters[1] = new List<int>(NextRoundOrder.Select(chara => chara.Id));
            return displayCharacters;
        }

        /// <summary>
        /// Called whenever a character's turn has ended. Removes that character from the current round order.
        /// </summary>
        internal void EndTurn()
        {
            // If this is the last turn of the round, start the next round and prepare the round after
            if (CurrentRoundOrder.Count == 1)
            {
                CurrentRoundOrder = new List<Character>(NextRoundOrder);
                NextRoundOrder = DetermineTurnOrder();
            }
            // Remove the current turn from the turn order list
            else
            {
                var indecesToCull = new List<int>();
                int maxIndex = CurrentRoundOrder.Count - 1;
                Character charToMove = null;
                for (int i = maxIndex; i >= 0; i--)
                {
                    var temp = charToMove;
                    charToMove = CurrentRoundOrder[i];
                    CurrentRoundOrder[i] = temp;
                    if (CurrentRoundOrder[i] == null || CurrentRoundOrder[i].CurrentHealth <= 0)
                        indecesToCull.Add(i);
                }
                foreach (var index in indecesToCull)
                {
                    CurrentRoundOrder.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Called whenever a character has died. Removes that character from the current and next rounds.
        /// </summary>
        /// <param name="args"></param>
        internal void CharactersDied(CharactersDiedEventArgs args)
        {
            CurrentRoundOrder.RemoveAll(character => args.DyingCharacters.Contains(character));
            NextRoundOrder.RemoveAll(character => args.DyingCharacters.Contains(character));
        }
    }
}
