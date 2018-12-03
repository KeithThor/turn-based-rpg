﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.Shared.Viewmodel;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Controller.AI.Interfaces;
using TurnBasedRPG.Controller.AI;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Combat;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Controller responsible for handling combat interactions.
    /// </summary>
    public class CombatController
    {
        private readonly ActionController _actionController;
        private readonly ConsumablesHandler _consumablesHandler;
        public readonly ViewModelController ViewModelController;
        public readonly CombatStateHandler CombatStateHandler;
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

        public CombatController(ActionController actionController,
                                CombatStateHandler combatStateHandler,
                                ConsumablesHandler consumablesHandler,
                                ICombatAI combatAI)
        {
            _actionController = actionController;
            CombatStateHandler = combatStateHandler;
            _consumablesHandler = consumablesHandler;
            DisplayManager = new DisplayManager(CombatStateHandler);
            ViewModelController = new ViewModelController(DisplayManager, CombatStateHandler);
            _combatAI = combatAI;
            _actionController.AllCharacters = CombatStateHandler.AllCharacters;

            BindEvents();
        }

        private void BindEvents()
        {
            _actionController.CharactersDied += OnCharactersDying;
            _actionController.CharactersHealthChanged += OnCharactersHealthChanged;
            _actionController.CharacterSpeedChanged += OnCharacterSpeedChanged;
        }

        private void OnCharactersDying(object sender, CharactersDiedEventArgs args)
        {
            CombatStateHandler.CharactersDied(args);
        }

        private void OnCharactersHealthChanged(object sender, CharactersHealthChangedEventArgs args)
        {
            CharactersHealthChanged?.Invoke(sender, args);
        }

        private void OnCharacterSpeedChanged(object sender, CharacterSpeedChangedEventArgs args)
        {
            CombatStateHandler.OnCharacterSpeedChanged(sender, args);
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
            _actionController.StartTurn(CombatStateHandler.CurrentRoundOrder[0]);
            
            StartOfTurn?.Invoke(this, new StartOfTurnEventArgs()
            {
                CharacterId = CombatStateHandler.GetActiveCharacterID(),
                IsPlayerTurn = CombatStateHandler.IsPlayerTurn()
            });

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
                    targets = CombatTargeter.GetTranslatedTargetPositions(action.TargetPositions,
                                                                          action.CenterOfTargetsPosition,
                                                                          action.CanSwitchTargetPosition,
                                                                          targetPosition);
                    _actionController.StartAction(CombatStateHandler.CurrentRoundOrder[0], action, targets);
                    break;
                case Commands.Spells:
                    action = DisplayManager.GetActionsFromCategory<Spell>(commandType, category)[index];
                    targets = CombatTargeter.GetTranslatedTargetPositions(action.TargetPositions,
                                                                          action.CenterOfTargetsPosition,
                                                                          action.CanSwitchTargetPosition,
                                                                          targetPosition);
                    _actionController.StartAction(CombatStateHandler.CurrentRoundOrder[0], action, targets);
                    break;
                case Commands.Items:
                    var item = DisplayManager.GetConsumablesFromCategory(category)[index];
                    _consumablesHandler.UseConsumable(item, CombatStateHandler.CurrentRoundOrder[0]);
                    targets = CombatTargeter.GetTranslatedTargetPositions(item.ItemSpell.TargetPositions,
                                                                          item.ItemSpell.CenterOfTargetsPosition,
                                                                          item.ItemSpell.CanSwitchTargetPosition,
                                                                          targetPosition);
                    _actionController.StartAction(CombatStateHandler.CurrentRoundOrder[0], item.ItemSpell, targets);
                    break;
                case Commands.Wait:
                    if (CombatStateHandler.CurrentRoundOrder.Count() == 1)
                        isInvalidAction = true;
                    else
                        CombatStateHandler.BeginWait();
                    break;
                default:
                    isInvalidAction = true;
                    break;
            }
            
            if (!isInvalidAction)
                EndTurn();
        }
    }
}
