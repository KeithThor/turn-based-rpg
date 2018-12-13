using System.Collections.Generic;

namespace TurnBasedRPG.Controller.Combat.Interfaces
{
    public interface IDisplayCombatState
    {
        int GetActiveCharacterID();
        int GetNextActivePlayerId();
        IEnumerable<int> GetPlayerCharacterIds();
        IReadOnlyList<int>[] GetRoundOrderIds();
        bool IsPlayerTurn();
        bool IsPositionOccupied(int position, bool includeDeadCharacters = true);
    }
}