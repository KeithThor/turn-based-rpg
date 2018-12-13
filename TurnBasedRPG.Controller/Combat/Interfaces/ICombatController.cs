using System;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Shared.Enums;

namespace TurnBasedRPG.Controller.Combat.Interfaces
{
    public interface ICombatController
    {
        event EventHandler<AIChoseTargetEventArgs> AIChoseTarget;
        event EventHandler<CombatLoggableEventArgs> CharacterBeginWait;
        event EventHandler<CharactersDiedEventArgs> CharactersDied;
        event EventHandler<CharactersHealthChangedEventArgs> CharactersHealthChanged;
        event EventHandler<CharacterSpeedChangedEventArgs> CharacterSpeedChanged;
        event EventHandler<CombatLoggableEventArgs> DelayedActionBeginChannel;
        event EventHandler<EndOfTurnEventArgs> EndOfTurn;
        event EventHandler<StartOfTurnEventArgs> StartOfTurn;
        event EventHandler<StatusEffectAppliedEventArgs> StatusEffectApplied;
        event EventHandler<CombatLoggableEventArgs> StatusEffectsRemoved;

        void EndTurn();
        void StartAction(Commands commandType, string category, int index, int targetPosition);
    }
}