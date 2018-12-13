using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Viewmodel;

namespace TurnBasedRPG.Controller.Interfaces
{
    public interface IViewModelController
    {
        ActionData GetActionViewData(Commands commandType, string category, int index);
        StatusData GetStatusViewData(Commands commandType, string category, int actionIndex, int statusIndex);
    }
}