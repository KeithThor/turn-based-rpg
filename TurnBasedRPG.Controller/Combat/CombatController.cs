using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.Controller.AI;
using TurnBasedRPG.Controller.AI.Interfaces;
using TurnBasedRPG.Controller.Combat.Interfaces;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Controller.Interfaces;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared.Combat;
using TurnBasedRPG.Shared.Enums;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Controller responsible for handling combat interactions.
    /// </summary>
    public class CombatController : ICombatController
    {
        private readonly IActionController _actionController;
        private readonly ConsumablesHandler _consumablesHandler;
        public readonly IViewModelController ViewModelController;
        private readonly CombatStateHandler CombatStateHandler;
        public readonly DisplayManager DisplayManager;
        private readonly ICombatAI _combatAI;
        public int TurnCounter = 1;

        /// <summary>
        /// Event that is triggered at the start of a turn.
        /// </summary>
        public event EventHandler<StartOfTurnEventArgs> StartOfTurn;

        /// <summary>
        /// Event that is triggered at the end of a turn.
        /// </summary>
        public event EventHandler<EndOfTurnEventArgs> EndOfTurn;

        /// <summary>
        /// Event triggered whenever the AI has chosen it's target.
        /// </summary>
        public event EventHandler<AIChoseTargetEventArgs> AIChoseTarget;

        /// <summary>
        /// Event triggered whenever one or more characters have their health changed.
        /// </summary>
        public event EventHandler<CharactersHealthChangedEventArgs> CharactersHealthChanged;

        /// <summary>
        /// Event triggered whenever one or more characters die.
        /// </summary>
        public event EventHandler<CharactersDiedEventArgs> CharactersDied;

        /// <summary>
        /// Event triggered whenever a status effect is applied to one or more characters.
        /// </summary>
        public event EventHandler<StatusEffectAppliedEventArgs> StatusEffectApplied;

        /// <summary>
        /// Event triggered whenever one or many status effects are removed from a character.
        /// </summary>
        public event EventHandler<CombatLoggableEventArgs> StatusEffectsRemoved;

        /// <summary>
        /// Event triggered whenever a character has begun to channel a delayed action.
        /// </summary>
        public event EventHandler<CombatLoggableEventArgs> DelayedActionBeginChannel;

        /// <summary>
        /// Event triggered whenever a character waits in combat.
        /// </summary>
        public event EventHandler<CombatLoggableEventArgs> CharacterBeginWait;

        /// <summary>
        /// Event triggered whenever a character has it's speed changed.
        /// </summary>
        public event EventHandler<CharacterSpeedChangedEventArgs> CharacterSpeedChanged;

        public CombatController(IActionController actionController,
                                CombatStateHandler combatStateHandler,
                                IViewModelController viewModelController,
                                DisplayManager displayManager,
                                ConsumablesHandler consumablesHandler,
                                ICombatAI combatAI)
        {
            _actionController = actionController;
            CombatStateHandler = combatStateHandler;
            _consumablesHandler = consumablesHandler;
            DisplayManager = displayManager;
            ViewModelController = viewModelController;
            _combatAI = combatAI;

            BindEvents();
        }

        private void BindEvents()
        {
            _actionController.CharactersDied += OnCharactersDying;
            _actionController.CharactersHealthChanged += OnCharactersHealthChanged;
            _actionController.CharacterSpeedChanged += OnCharacterSpeedChanged;
            _actionController.StatusEffectApplied += OnStatusEffectsApplied;
            _actionController.DelayedActionBeginChannel += OnDelayedActionBeginChannel;
        }

        private void OnCharactersDying(object sender, CharactersDiedEventArgs args)
        {
            CombatStateHandler.CharactersDied(args);
            CharactersDied?.Invoke(sender, args);
        }

        private void OnCharactersHealthChanged(object sender, CharactersHealthChangedEventArgs args)
        {
            CharactersHealthChanged?.Invoke(sender, args);
        }

        private void OnCharacterSpeedChanged(object sender, CharacterSpeedChangedEventArgs args)
        {
            CombatStateHandler.OnCharacterSpeedChanged(sender, args);
            CharacterSpeedChanged?.Invoke(sender, args);
        }

        private void OnStatusEffectsApplied(object sender, StatusEffectAppliedEventArgs args)
        {
            StatusEffectApplied?.Invoke(sender, args);
        }

        private void OnDelayedActionBeginChannel(object sender, CombatLoggableEventArgs args)
        {
            DelayedActionBeginChannel?.Invoke(sender, args);
        }

        /// <summary>
        /// Ends the current turn and broadcasts the EndOfTurn event.
        /// </summary>
        public void EndTurn()
        {
            // Prepare event information before information is changed
            var eventArgs = new EndOfTurnEventArgs()
            {
                EndOfTurnCharacterId = CombatStateHandler.CurrentRoundOrder[0].Id,
                CurrentTurnNumber = TurnCounter
            };

            CombatStateHandler.EndTurn();
            EndOfTurn?.Invoke(this, eventArgs);

            StartTurn();
        }

        /// <summary>
        /// Calls events that should happen at the start of the turn.
        /// </summary>
        private void StartTurn()
        {
            StartOfTurn?.Invoke(this, new StartOfTurnEventArgs()
            {
                CharacterId = CombatStateHandler.GetActiveCharacterID(),
                IsPlayerTurn = CombatStateHandler.IsPlayerTurn(),
                CurrentRoundOrderIds = CombatStateHandler.CurrentRoundOrder.Select(chr => chr.Id).ToList(),
                NextRoundOrderIds = CombatStateHandler.NextRoundOrder.Select(chr => chr.Id).ToList()
            });

            _actionController.StartTurn(CombatStateHandler.CurrentRoundOrder[0]);

            // Currently Ai's turn
            if (CombatStateHandler.EnemyCharacters.Contains(CombatStateHandler.CurrentRoundOrder[0]))
                StartAITurn();
        }

        /// <summary>
        /// Called to start an AI's turn.
        /// <para>Throws an exception if the AI returns a null ActionBase.</para>
        /// </summary>
        private void StartAITurn()
        {
            var aiDecision = _combatAI.GetAIDecision(CombatStateHandler.CurrentRoundOrder[0],
                                                     CombatStateHandler.EnemyCharacters,
                                                     CombatStateHandler.PlayerCharacters);

            if (aiDecision.ActionChoice == null) throw new Exception("AI action choice cannot be null.");

            // Invoke AIChoseTarget event once the AI has come to a decision.
            if (AIChoseTarget != null)
            {
                var eventArgs = new AIChoseTargetEventArgs()
                {
                    AICharacter = CombatStateHandler.CurrentRoundOrder[0],
                    CenterOfTarget = aiDecision.TargetPosition,
                    TargetPositions = AITargets.GetModifiedSelection(aiDecision.ActionChoice, aiDecision.TargetPosition)
                };
                AIChoseTarget(this, eventArgs);
            }

            StartAIAction(aiDecision.ActionChoice, aiDecision.TargetPosition);

            // todo: Create class to handle consumables
            if (aiDecision.ConsumableUsed != null)
            {
                _consumablesHandler.UseConsumable(aiDecision.ConsumableUsed,
                                                  CombatStateHandler.CurrentRoundOrder[0]);
            }
        }
        
        /// <summary>
        /// Performs a character action given an ActionBase and a target position.
        /// </summary>
        /// <param name="action">The action being performed.</param>
        /// <param name="targetPosition">The position the action will target.</param>
        private void StartAIAction(ActionBase action, int targetPosition)
        {
            var targetPositions = AITargets.GetModifiedSelection(action, targetPosition);
            _actionController.StartAction(CombatStateHandler.CurrentRoundOrder[0], action, targetPositions);
            EndTurn();
        }

        /// <summary>
        /// Performs a character action.
        /// </summary>
        /// <param name="commandType">The type of command that is being performed, such as Attack or Spells.</param>
        /// <param name="category">The category of the action being performed, may be left blank if the action has no categories.</param>
        /// <param name="index">The index of the action being performed.</param>
        /// <param name="targetPosition">The target position of the action being performed.</param>
        public void StartAction(Commands commandType, string category, int index, int targetPosition)
        {
            bool isInvalidAction = false;
            ActionBase action;
            IReadOnlyList<int> targets;
            switch (commandType)
            {
                case Commands.Attack:
                    action = DisplayManager.GetActionsFromCategory<Attack>(commandType, category)[index];
                    isInvalidAction = !IsValidTarget(action, targetPosition);
                    if (!isInvalidAction)
                    {
                        targets = CombatTargeter.GetTranslatedTargetPositions(action.TargetPositions,
                                                                          action.CenterOfTargetsPosition,
                                                                          action.CanSwitchTargetPosition,
                                                                          targetPosition);
                        _actionController.StartAction(CombatStateHandler.CurrentRoundOrder[0], action, targets);
                    }
                    break;
                case Commands.Spells:
                    action = DisplayManager.GetActionsFromCategory<Spell>(commandType, category)[index];
                    isInvalidAction = !IsValidTarget(action, targetPosition);
                    if (!isInvalidAction)
                    {
                        targets = CombatTargeter.GetTranslatedTargetPositions(action.TargetPositions,
                                                                          action.CenterOfTargetsPosition,
                                                                          action.CanSwitchTargetPosition,
                                                                          targetPosition);
                        _actionController.StartAction(CombatStateHandler.CurrentRoundOrder[0], action, targets);
                    }
                    break;
                case Commands.Items:
                    var item = DisplayManager.GetConsumablesFromCategory(category)[index];
                    isInvalidAction = !IsValidTarget(item.ItemSpell, targetPosition);
                    if (!isInvalidAction)
                    {
                        targets = CombatTargeter.GetTranslatedTargetPositions(item.ItemSpell.TargetPositions,
                                                                          item.ItemSpell.CenterOfTargetsPosition,
                                                                          item.ItemSpell.CanSwitchTargetPosition,
                                                                          targetPosition);
                        _actionController.StartAction(CombatStateHandler.CurrentRoundOrder[0], item.ItemSpell, targets);
                        _consumablesHandler.UseConsumable(item, CombatStateHandler.CurrentRoundOrder[0]);
                    }
                    break;
                case Commands.Wait:
                    if (CombatStateHandler.CurrentRoundOrder.Count() == 1)
                        isInvalidAction = true;
                    else
                    {
                        CombatStateHandler.BeginWait();
                        CharacterBeginWait?.Invoke(this, new CombatLoggableEventArgs()
                        {
                            LogMessage = CombatMessenger.GetBeginWaitMessage(CombatStateHandler.CurrentRoundOrder[0].Name)
                        });
                    }
                    break;
                default:
                    isInvalidAction = true;
                    break;
            }
            
            if (!isInvalidAction)
                EndTurn();
        }

        /// <summary>
        /// Checks and returns whether or not an action can be used at a target position.
        /// </summary>
        /// <param name="action">The action used by a character.</param>
        /// <param name="targetPosition">The center target position of the action being used.</param>
        /// <returns>Whether or not an action can be used at a target position.</returns>
        private bool IsValidTarget(ActionBase action, int targetPosition)
        {
            if (!action.CanSwitchTargetPosition && action.CenterOfTargetsPosition != targetPosition)
                return false;
            if (action.CanResurrect)
            {
                if (!action.CanTargetThroughUnits && targetPosition > 10)
                {
                    // If target position is last column, check both the middle and front column for a target
                    if (targetPosition % 3 == 0 && CombatStateHandler.IsPositionOccupied(targetPosition - 2))
                    {
                        return false;
                    }
                    if (targetPosition % 3 != 1 && CombatStateHandler.IsPositionOccupied(targetPosition - 1))
                        return false;
                }
                var targets = CombatTargeter.GetTranslatedTargetPositions(action.TargetPositions,
                                                                          action.CenterOfTargetsPosition,
                                                                          action.CanSwitchTargetPosition,
                                                                          targetPosition);

                return CombatStateHandler.AllCharacters.Where(chr => chr.CurrentHealth <= 0 && targets.Contains(chr.Position)).Any();
            }
            else
            {
                if (!action.CanTargetThroughUnits && targetPosition > 10)
                {
                    // If target position is last column, check both the middle and front column for a target
                    if (targetPosition % 3 == 0 && CombatStateHandler.IsPositionOccupied(targetPosition - 2, false))
                    {
                        return false;
                    }
                    if (targetPosition % 3 != 1 && CombatStateHandler.IsPositionOccupied(targetPosition - 1, false))
                        return false;
                }

                var targets = CombatTargeter.GetTranslatedTargetPositions(action.TargetPositions,
                                                                          action.CenterOfTargetsPosition,
                                                                          action.CanSwitchTargetPosition,
                                                                          targetPosition);

                return CombatStateHandler.AllCharacters.Where(chr => chr.CurrentHealth > 0 && targets.Contains(chr.Position)).Any();
            }
        }
    }
}
