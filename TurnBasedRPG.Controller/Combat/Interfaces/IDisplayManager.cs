using System.Collections.Generic;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Controller.Combat.Interfaces
{
    public interface IDisplayManager
    {
        IDisplayAction GetActionFromCategory(Commands commandType, string category, int index);
        IReadOnlyList<IDisplayAction> GetActionListFromCategory(Commands commandType, string category);
        IReadOnlyList<IDisplayCharacter> GetAllDisplayableCharacters();
        IReadOnlyList<string[]> GetCategories(Commands commandType);
        List<DisplayCharacter> GetDisplayCharacters();
        List<DisplayCharacter> GetDisplayCharactersFromIds(IEnumerable<int> ids);
    }
}