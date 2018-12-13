using System;
using System.Collections.Generic;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.Combat.Interfaces
{
    public interface IStatusController
    {
        event EventHandler<CharactersDiedEventArgs> CharactersDied;
        event EventHandler<CharactersHealthChangedEventArgs> CharactersHealthChanged;
        event EventHandler<CharacterSpeedChangedEventArgs> CharacterSpeedChanged;
        event EventHandler<StatusEffectAppliedEventArgs> StatusEffectApplied;
        event EventHandler<CombatLoggableEventArgs> StatusEffectsRemoved;

        void ApplyStatus(Character applicator, StatusEffect status, Character character, bool invokeAppliedStatusEvent = true);
        void ApplyStatus(Character applicator, StatusEffect status, IEnumerable<Character> characters);
        void BeginStartTurn(Character character);
        void CreateDelayedStatus(Character applicator, StatusEffect statusBase, IReadOnlyList<int> targets, int spellDelay);
        void FinishStartTurn(Character character);
        void RemoveAllStatuses(Character character, bool removePermanent = false);
    }
}