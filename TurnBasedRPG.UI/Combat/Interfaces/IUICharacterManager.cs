using System.Collections.Generic;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Responsible for handling and keeping track of the UI versions of characters.
    /// </summary>
    public interface IUICharacterManager
    {
        /// <summary>
        /// Contains all characters currently in combat.
        /// </summary>
        List<DisplayCharacter> Characters { get; set; }

        /// <summary>
        /// A list of character ids sorted in the order of who will perform first in the round to last for the current round.
        /// </summary>
        IReadOnlyList<int> CurrentRoundOrderIds { get; set; }

        /// <summary>
        /// A list of character ids sorted in the order of who will perform first in the round to last for the next round.
        /// </summary>
        IReadOnlyList<int> NextRoundOrderIds { get; set; }

        /// <summary>
        /// Gets whether or not a character exists in a position.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="includeDead">Whether or not to include dead characters.</param>
        /// <returns></returns>
        bool CharacterInPositionExists(int position, bool includeDead = true);

        /// <summary>
        /// Gets all characters as a list of IDisplayCharacters.
        /// </summary>
        /// <returns>A list of IDisplayCharacters containing all the characters in combat.</returns>
        List<IDisplayCharacter> GetAllCharacters();

        /// <summary>
        /// Gets a display character from id.
        /// </summary>
        /// <param name="id">The id of the character to retrieve.</param>
        /// <returns></returns>
        DisplayCharacter GetCharacterFromId(int id);

        /// <summary>
        /// Gets a character that occupies a target position.
        /// </summary>
        /// <param name="targetPosition">The position to retrieve a character from.</param>
        /// <returns></returns>
        DisplayCharacter GetCharacterFromPosition(int targetPosition);

        /// <summary>
        /// Gets a list of characters that match a set of ids.
        /// </summary>
        /// <param name="ids">The ids of the characters to get.</param>
        /// <returns>A list of display characters that match the ids.</returns>
        List<DisplayCharacter> GetCharactersFromIds(IEnumerable<int> ids);

        /// <summary>
        /// Gets a list of characters that occupies a set of positions.
        /// </summary>
        /// <param name="targetPositions">A positions to retrieve characters from.</param>
        /// <returns></returns>
        IReadOnlyList<DisplayCharacter> GetCharactersFromPositions(IEnumerable<int> targetPositions);

        /// <summary>
        /// Gets the character that is performing its turn now.
        /// </summary>
        /// <returns></returns>
        DisplayCharacter GetCurrentTurnCharacter();

        /// <summary>
        /// Gets the position of a character given its id.
        /// </summary>
        /// <param name="chrId">The id of the character to get the position of.</param>
        /// <returns>A nullable int containing the position of the character.</returns>
        int? GetPositionOfCharacter(int chrId);

        /// <summary>
        /// Given a list of ids for the current and next rounds, constructs and returns lists of current and next round characters.
        /// </summary>
        /// <param name="currentTurnIds">The ids of the current round characters.</param>
        /// <param name="nextTurnIds">The ids of the next round characters.</param>
        /// <returns>A size 2 array of lists of display characters containing the current and next round characters.</returns>
        List<DisplayCharacter>[] GetTurnOrderCharacters(IReadOnlyList<int> currentTurnIds, IReadOnlyList<int> nextTurnIds);

        /// <summary>
        /// Given a list of characters, refreshes the stats on those characters if they match any of the characters already present.
        /// </summary>
        /// <param name="characters">The newly modified characters to refresh.</param>
        void RefreshCharacters(IReadOnlyList<DisplayCharacter> characters);

        /// <summary>
        /// Sets the current health of a character given a character's id and the new health amount.
        /// </summary>
        /// <param name="chrId">The id of the character to change health of.</param>
        /// <param name="newHealth">The new amount of health to set the character's to.</param>
        void SetCurrentHealth(int chrId, int newHealth);

        /// <summary>
        /// Sets the max health of a character given a character's id and the new health amount.
        /// </summary>
        /// <param name="chrId">The id of the character to change max health of.</param>
        /// <param name="newHealth">The new amount of max health to set the character's to.</param>
        void SetMaxHealth(int chrId, int newHealth);

        /// <summary>
        /// Changes the position of a character given its id and new position.
        /// </summary>
        /// <param name="chrId">The id of the character to switch positions.</param>
        /// <param name="position">The new position to set the character to.</param>
        void SetPosition(int chrId, int position);
    }
}