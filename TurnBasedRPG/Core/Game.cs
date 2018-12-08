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
        private GameUIConstants UIInstance { get; set; }
        private CombatUI _combatUI;

        private List<Character> PlayerCharacters { get; set; }
        private List<Character> EnemyCharacters { get; set; }

        public Game(GameUIConstants uiInstance, CombatUI combatUI)
        {
            ShutdownTriggered = false;
            UIInstance = uiInstance;
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
