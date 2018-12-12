using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.UI;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.UI.Combat;

namespace TurnBasedRPG.Core
{
    public class Game
    {
        public bool ShutdownTriggered { get; set; }
        private readonly GameUIConstants _gameUIConstants;
        private readonly CombatUI _combatUI;

        public Game(GameUIConstants gameUIConstants, CombatUI combatUI)
        {
            ShutdownTriggered = false;
            _gameUIConstants = gameUIConstants;
            _combatUI = combatUI;
            }

        public void Start()
        {
            StartCombat();
        }

        public void StartCombat()
        {
            _combatUI.StartCombat();
        }
    }
}
