using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.Combat.Interfaces
{
    internal interface ICombatStateHandler
    {
        List<Character> AllCharacters { get; }
        List<Character> PlayerCharacters { get; }
        List<Character> EnemyCharacters { get; }
        List<Character> CurrentRoundOrder { get; }
        List<Character> NextRoundOrder { get; }

        void OnCharacterSpeedChanged(object sender, CharacterSpeedChangedEventArgs args);
        List<Character> GetAllLivingCharacters();
        void BeginWait();
        void BeginWait(Character character);
        void EndTurn();
        void CharactersDied(CharactersDiedEventArgs args);
    }
}
