using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Test.Combat.Mocks
{
    internal class MockUIStateTracker : IUIStateTracker
    {
        public int CommandFocusNumber { get; set; }
        public ActionStore ActiveAction { get; set; }
        public bool IsInFormationPanel { get; set; }
        public int ActiveCharacterId { get; set; }
        public int CurrentTargetPosition { get; set; }
        public IReadOnlyList<int> CurrentTargetPositions { get; set; } = new List<int>();

        // Unused properties
        public IReadOnlyList<string[]> ActionCategories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ActionFocusNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ActionPanelItemCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ActionPanelLineOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IReadOnlyList<IDisplayAction> ActionPanelList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ActiveCategory => throw new NotImplementedException();
        public int CategoryFocusNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int CategoryItemCount => throw new NotImplementedException();
        public int CategoryLineOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsPlayerTurn => throw new NotImplementedException();
        public bool IsInActionPanel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsInCategoryPanel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsInCharacterPanel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsInCommandPanel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsInStatusCommand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
