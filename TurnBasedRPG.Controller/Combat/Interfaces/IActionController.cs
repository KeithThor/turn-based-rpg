using System;
using System.Collections.Generic;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.Combat.Interfaces
{
    public interface IActionController
    {
        event EventHandler<CharactersDiedEventArgs> CharactersDied;
        event EventHandler<CharactersHealthChangedEventArgs> CharactersHealthChanged;
        event EventHandler<CharacterSpeedChangedEventArgs> CharacterSpeedChanged;
        event EventHandler<CombatLoggableEventArgs> DelayedActionBeginChannel;
        event EventHandler<StatusEffectAppliedEventArgs> StatusEffectApplied;
        event EventHandler<CombatLoggableEventArgs> StatusEffectsRemoved;

        void StartAction(Character actor, ActionBase action, IReadOnlyList<int> targets);
        void StartTurn(Character character);
    }
}