using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// Class responsible for handling the UI version of characters in combat.
    /// </summary>
    public class UICharacterManager
    {
        public List<DisplayCharacter> Characters { get; set; } = new List<DisplayCharacter>();
        public IReadOnlyList<int> CurrentRoundOrderIds { get; set; }
        public IReadOnlyList<int> NextRoundOrderIds { get; set; }

        /// <summary>
        /// Retrieves a character given it's id. May return null if no character with a given id exists.
        /// </summary>
        /// <param name="id">The id of the character to retrieve.</param>
        /// <returns>A UI displayable version of a character retrieved by id.</returns>
        public DisplayCharacter GetCharacterFromId(int id)
        {
            var target = Characters.FirstOrDefault(character => character.Id == id);
            if (target == null)
                return null;
            return target;
        }

        /// <summary>
        /// Retrieves all the characters currently in combat.
        /// </summary>
        /// <returns></returns>
        public List<IDisplayCharacter> GetAllCharacters()
        {
            return new List<IDisplayCharacter>(Characters);
        }

        /// <summary>
        /// Gets a group of characters, given a set of ids.
        /// </summary>
        /// <param name="ids">The ids of the characters to retrieve.</param>
        /// <returns>A list of UI displayable characters retrieved by Id.</returns>
        public List<DisplayCharacter> GetCharactersFromIds(IEnumerable<int> ids)
        {
            return Characters.Where(chara => ids.Contains(chara.Id)).ToList();
        }

        /// <summary>
        /// Gets a list of DisplayCharacters from a set of ids in the same order as the ids are arranged.
        /// <para>
        /// Throws an exception if a character cannot be found from an Id.
        /// </para>
        /// </summary>
        /// <param name="ids">The set of ids to return DisplayCharacters for.</param>
        /// <returns>The list of DisplayCharacters found from the set of ids.</returns>
        private List<DisplayCharacter> GetCharactersPreservingPosition(IEnumerable<int> ids)
        {
            var displayCharacters = new List<DisplayCharacter>();

            foreach (var id in ids)
            {
                var character = Characters.FirstOrDefault(chara => chara.Id == id);
                if (character == null)
                    throw new Exception("Turn order id does not exist in display character manager list.");
                else
                    displayCharacters.Add(character);
            }

            return displayCharacters;
        }

        /// <summary>
        /// Gets the character whose turn is now.
        /// </summary>
        /// <returns>The displayable version of the character who is acting now.</returns>
        public DisplayCharacter GetCurrentTurnCharacter()
        {
            return Characters.First(chr => CurrentRoundOrderIds[0] == chr.Id);
        }

        /// <summary>
        /// Gets a character who occupies a position on the field. Will return null if no character is found.
        /// </summary>
        /// <param name="targetPosition">The position to check for the character.</param>
        /// <returns>A UI displayable version of a character that exists in a position.</returns>
        public DisplayCharacter GetCharacterFromPosition(int targetPosition)
        {
            var target = Characters.FirstOrDefault(character => character.Position == targetPosition);
            if (target == null)
                return null;
            return target;
        }

        /// <summary>
        /// Gets a list of characters who occupies a list of target positions.
        /// </summary>
        /// <param name="targetPositions">A list of target positions to check if characters exist.</param>
        /// <returns>A list of UI displayable characters that exists in a set of positions.</returns>
        public IReadOnlyList<DisplayCharacter> GetCharactersFromPositions(IEnumerable<int> targetPositions)
        {
            return Characters.Where(chr => targetPositions.Contains(chr.Position)).ToList();
        }

        /// <summary>
        /// Returns true if a character exists in a given position.
        /// <para>
        /// Defaults to including dead characters in the query.
        /// </para>
        /// </summary>
        /// <param name="position">The position to check if a character occupies.</param>
        /// <param name="includeDead">Returns true if a dead character occupies the target position, else returns false even
        /// if a dead character occupies it.</param>
        /// <returns>True if a character occupies a position.</returns>
        public bool CharacterInPositionExists(int position, bool includeDead = true)
        {
            var character = GetCharacterFromPosition(position);
            if (character == null)
                return false;
            else if (character.CurrentHealth <= 0 && !includeDead)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Returns an array with two Lists of DisplayCharacters sorted in the way the current and next
        /// turn orders are arranged.
        /// </summary>
        /// <param name="currentTurnIds">The ids of all characters in the current round.</param>
        /// <param name="nextTurnIds">The ids of all characters in the next round.</param>
        /// <returns>An array containing DisplayCharacter representations of the current and next turn orders.</returns>
        public List<DisplayCharacter>[] GetTurnOrderCharacters(IReadOnlyList<int> currentTurnIds, IReadOnlyList<int> nextTurnIds)
        {
            var displayCharacters = new List<DisplayCharacter>[2];

            displayCharacters[0] = GetCharactersPreservingPosition(currentTurnIds);
            displayCharacters[1] = GetCharactersPreservingPosition(nextTurnIds);

            return displayCharacters;
        }

        /// <summary>
        /// Replaces the currently stored display characters with another set of those characters with updated stats.
        /// </summary>
        /// <param name="characters">The new characters to replace the original.</param>
        public void RefreshCharacters(IReadOnlyList<DisplayCharacter> characters)
        {
            var ids = characters.Select(chr => chr.Id);
            Characters.RemoveAll(chr => ids.Contains(chr.Id));
            Characters.AddRange(characters);
        }

        /// <summary>
        /// Sets the current health of a character to a new number, given the character's Id.
        /// </summary>
        /// <param name="chrId">The Id of the character whose health is being set.</param>
        /// <param name="newHealth">The new amount of health the character's health should be set to.</param>
        public void SetCurrentHealth(int chrId, int newHealth)
        {
            var target = GetCharacterFromId(chrId);
            if (target != null)
                target.CurrentHealth = newHealth;
            if (target.CurrentHealth < 0)
                target.CurrentHealth = 0;
        }

        /// <summary>
        /// Sets the max health of a character to a new number, given the character's Id.
        /// </summary>
        /// <param name="chrId">The Id of the character whose health is being set.</param>
        /// <param name="newHealth">The new amount of health the character's health should be set to.</param>
        public void SetMaxHealth(int chrId, int newHealth)
        {
            var target = GetCharacterFromId(chrId);
            if (target != null)
                target.MaxHealth = newHealth;
        }

        /// <summary>
        /// Sets the position of a character to a different position, given the character's Id.
        /// </summary>
        /// <param name="chrId">The Id of the character whose position is being set.</param>
        /// <param name="position">The new position to change the character's position to.</param>
        public void SetPosition(int chrId, int position)
        {
            var target = GetCharacterFromId(chrId);
            if (target != null)
                target.Position = position;
        }

        /// <summary>
        /// Gets the position of a character given the character's id. Returns null if no character with that id exists.
        /// </summary>
        /// <param name="chrId">The id of the character to get the position from.</param>
        /// <returns>The id of the character to get the position from. May be null if no character with that id exists.</returns>
        public int? GetPositionOfCharacter(int chrId)
        {
            var character = Characters.First(chr => chr.Id == chrId);
            if (character == null) return null;
            else return character.Position;
        }
    }
}
