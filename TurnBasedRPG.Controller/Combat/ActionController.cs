using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Combat;

namespace TurnBasedRPG.Controller.Combat
{
    /// <summary>
    /// Handles the activation and effects of a character action.
    /// </summary>
    public class ActionController
    {
        /// <summary>
        /// A wrapper around the ActionBase class that represents an action with delayed effects, keeping track
        /// of the action's damage and healing at the time of casting.
        /// </summary>
        private class DelayedAction
        {
            public Character Actor { get; set; }
            public ActionBase BaseAction { get; set; }
            public DamageTypes TotalDamage { get; set; }
            public int HealAmount { get; set; }
            public int HealPercentage { get; set; }
            public int TurnsRemaining { get; set; }
            public IReadOnlyList<int> Targets { get; set; }
        }

        private Dictionary<Character, List<DelayedAction>> _delayedActions = new Dictionary<Character, List<DelayedAction>>();

        private CharacterController _characterController;
        private StatusController StatusEffectController;
        private ThreatController _threatController;
        private IReadOnlyList<Character> _allCharacters;
        private Random _random;
        public IReadOnlyList<Character> AllCharacters
        {
            get { return _allCharacters; }
            set
            {
                StatusEffectController.AllCharacters = value;
                _allCharacters = value;
            }
        }

        /// <summary>
        /// Event invoked whenever one or multiple characters die as a result of an action or the status effect caused by an action.
        /// </summary>
        public event EventHandler<CharactersDiedEventArgs> CharactersDied;

        /// <summary>
        /// Event invoked whenever one or multiple characters have their health changed as a result of an action or the status
        /// effect caused by an action.
        /// </summary>
        public event EventHandler<CharactersHealthChangedEventArgs> CharactersHealthChanged;

        /// <summary>
        /// Event invoked whenever a character has it's speed changed by a status effect caused by an action.
        /// </summary>
        public event EventHandler<CharacterSpeedChangedEventArgs> CharacterSpeedChanged;

        /// <summary>
        /// Event invoked whenever a character has begun channeling a delayed action.
        /// </summary>
        public event EventHandler<CombatLoggableEventArgs> DelayedActionBeginChannel;

        /// <summary>
        /// Event invoked whenever a status effect is applied.
        /// </summary>
        public event EventHandler<StatusEffectAppliedEventArgs> StatusEffectApplied;

        public ActionController(CharacterController characterController,
                                StatusController statusController,
                                ThreatController threatController)
        {
            _characterController = characterController;
            StatusEffectController = statusController;
            _threatController = threatController;
            _random = new Random();

            BindEvents();
        }

        private void BindEvents()
        {
            StatusEffectController.CharacterSpeedChanged += OnCharacterSpeedChanged;
            StatusEffectController.CharactersDied += OnCharactersDying;
            StatusEffectController.CharactersHealthChanged += OnCharactersHealthChanged;
            StatusEffectController.StatusEffectApplied += OnStatusEffectApplied;
        }

        /// <summary>
        /// Invoked on a character's death. Calls the character died event on this controller.
        /// </summary>
        /// <param name="sender">The instance that is sending this event.</param>
        /// <param name="args">The characters who died.</param>
        private void OnCharactersDying(object sender, CharactersDiedEventArgs args)
        {
            CharactersDied?.Invoke(sender, args);
        }

        private void OnCharactersHealthChanged(object sender, CharactersHealthChangedEventArgs args)
        {
            CharactersHealthChanged?.Invoke(sender, args);
        }

        private void OnCharacterSpeedChanged(object sender, CharacterSpeedChangedEventArgs args)
        {
            CharacterSpeedChanged?.Invoke(sender, args);
        }

        private void OnStatusEffectApplied(object sender, StatusEffectAppliedEventArgs args)
        {
            StatusEffectApplied?.Invoke(sender, args);
        }

        /// <summary>
        /// Starts the effects of an action, applying damage then status effects. If the action has a delay, register
        /// the action as a delayed action to have it's effects activated at a later time.
        /// </summary>
        /// <param name="actor">The actor performing the action.</param>
        /// <param name="action">The action being performed.</param>
        /// <param name="targets">The positions the action targets.</param>
        public void StartAction(Character actor, ActionBase action, IReadOnlyList<int> targets)
        {
            if (action.Delay > 0)
            {
                CreateDelayedAction(actor, action, targets);
                CreateDelayedStatus(actor, action, targets);
            }
            else
            {
                var targetCharacters = new List<Character>();
                // Check if character matches conditions to apply healing and damage to and exists in the target positions
                foreach (var character in AllCharacters)
                {
                    if (IsTargetableCharacter(character, action, targets))
                    {
                        targetCharacters.Add(character);
                    }
                }

                ApplyHealingAndDamage(actor, action, targetCharacters);
                CheckForDeadTargets(targetCharacters);

                targetCharacters.RemoveAll(chr => chr.CurrentHealth <= 0);
                ApplyStatusEffects(actor, action, targetCharacters);
            }
        }

        /// <summary>
        /// Gets whether or not a character can be targetable by an action and it's target positions. Filters our dead
        /// characters if the target cannot resurrect.
        /// </summary>
        /// <param name="character">The character to check if it is targetable.</param>
        /// <param name="action">The action to check targetability.</param>
        /// <param name="targetPositions">The target positions the action is affecting.</param>
        /// <returns>Returns whether or not a character can be targetted by an action.</returns>
        private bool IsTargetableCharacter(Character character, ActionBase action, IReadOnlyList<int> targetPositions)
        {
            bool includeDead = action.CanResurrect && character.CurrentHealth <= 0;
            bool isCharacterAlive = character.CurrentHealth > 0;
            if (targetPositions.Contains(character.Position) && (includeDead || isCharacterAlive))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called at the start of the turn to check for the activation of a delayed action and it's respective delayed
        /// status effects. Activates a delayed action if it's turn counter is 0.
        /// </summary>
        /// <param name="character">The character whose turn is starting.</param>
        public void StartTurn(Character character)
        {
            StatusEffectController.BeginStartTurn(character);
            if (_delayedActions.ContainsKey(character))
            {
                var removeActions = new List<DelayedAction>();
                foreach (var action in _delayedActions[character])
                {
                    action.TurnsRemaining--;
                    if (action.TurnsRemaining <= 0)
                    {
                        removeActions.Add(action);
                        ApplyHealingAndDamage(action);
                    }
                }
                foreach (var action in removeActions)
                {
                    _delayedActions[character].Remove(action);
                }
            }
            StatusEffectController.FinishStartTurn(character);
        }

        /// <summary>
        /// Creates a delayed action that has it's effects applied after an amount of rounds has passed.
        /// </summary>
        /// <param name="actor">The character performing the action.</param>
        /// <param name="action">The action being performed.</param>
        /// <param name="targets">The list of target positions the action targets.</param>
        private void CreateDelayedAction(Character actor, ActionBase action, IReadOnlyList<int> targets)
        {
            if (!_delayedActions.ContainsKey(actor)) _delayedActions[actor] = new List<DelayedAction>();

            DamageTypes totalDamage = DamageCalculator.GetDamage(actor, action);
            int totalHeal = DamageCalculator.GetHealing(actor, action);
            int percentageHealing = DamageCalculator.GetHealingPercentage(actor, action);

            int rand = _random.Next(1, 101);
            int totalCritChance = action.CritChance > 0 ? action.CritChance + actor.CritChance : 0;
            if (totalCritChance > rand)
            {
                int critMultiplier = actor.CritMultiplier + action.CritMultiplier + 100;
                totalDamage = totalDamage * critMultiplier / 100;
                totalHeal = totalHeal * critMultiplier / 100;
            }

            _delayedActions[actor].Add(new DelayedAction()
            {
                Actor = actor,
                BaseAction = action,
                TotalDamage = totalDamage,
                HealAmount = totalHeal,
                HealPercentage = percentageHealing,
                TurnsRemaining = action.Delay,
                Targets = targets
            });

            DelayedActionBeginChannel?.Invoke(this, new CombatLoggableEventArgs()
            {
                LogMessage = CombatMessenger.GetBeginChannelMessage(actor.Name, action.Name)
            });
        }

        /// <summary>
        /// Checks a list of characters if any have died. If a character has died, invokes the CharacterDied event.
        /// </summary>
        /// <param name="targets">The list of characters to check.</param>
        private void CheckForDeadTargets(IReadOnlyList<Character> targets)
        {
            var deadCharacters = targets.Where(target => target.CurrentHealth <= 0).ToList();
            if (deadCharacters.Count() > 0)
            {
                var logMessage = CombatMessenger.GetCharactersDiedMessage(deadCharacters.Select(chr => chr.Name).ToList());

                CharactersDied?.Invoke(this, new CharactersDiedEventArgs()
                {
                    DyingCharacters = deadCharacters.ToList(),
                    LogMessage = logMessage
                });
            }
        }

        /// <summary>
        /// Calculates healing and damage, then checks and applies any critical strikes, then applies healing and damage.
        /// </summary>
        /// <param name="actor">The character applying the healing and damage.</param>
        /// <param name="action">The action being performed.</param>
        /// <param name="targets">The list of characters the action is targeting.</param>
        private void ApplyHealingAndDamage(Character actor, ActionBase action, IReadOnlyList<Character> targets)
        {
            var damageTypes = DamageCalculator.GetDamage(actor, action);
            int[] damage = damageTypes.AsArray();
            var postHealthChangedDict = new Dictionary<int, int>();
            var changeAmount = new Dictionary<int, int>();
            var loggableData = new List<KeyValuePair<string, int>>();
            var preHealthChangedDict = new Dictionary<int, int>();

            for (int i = 0; i < targets.Count(); i++)
            {
                int startHealth = targets[i].CurrentHealth;
                preHealthChangedDict.Add(targets[i].Id, startHealth);
                // Calculate and apply healing
                int percentHealAmount = targets[i].CurrentMaxHealth * DamageCalculator.GetHealingPercentage(actor, action);
                int modifiedHealAmount = DamageCalculator.GetHealing(actor, action);
                int totalDamage = DamageCalculator.GetTotalDamage(damageTypes, targets[i]);

                // Determine if a critical happened
                int totalCritChance = action.CritChance > 0 ? action.CritChance + actor.CritChance : 0;
                int rand = _random.Next(1, 101);
                if (totalCritChance >= rand)
                {
                    int multiplier = action.CritMultiplier + actor.CritMultiplier + 100;
                    modifiedHealAmount = modifiedHealAmount * multiplier / 100;
                    totalDamage = totalDamage * multiplier / 100;
                }

                _characterController.ModifyCurrentHealth(targets[i], percentHealAmount);
                _characterController.ModifyCurrentHealth(targets[i], modifiedHealAmount);
                _characterController.ModifyCurrentHealth(targets[i], totalDamage);
                _threatController.ApplyThreat(actor,
                                              targets[i],
                                              percentHealAmount + modifiedHealAmount + totalDamage,
                                              action.Threat,
                                              action.ThreatMultiplier);

                // Prepare event data
                int healthChange = targets[i].CurrentHealth - startHealth;
                changeAmount.Add(targets[i].Id, healthChange);
                loggableData.Add(new KeyValuePair<string, int>(targets[i].Name, healthChange));
                postHealthChangedDict.Add(targets[i].Id, targets[i].CurrentHealth);
            }

            if (changeAmount.Values.Any(val => val != 0))
            {
                string logMessage = CombatMessenger.GetHealthChangedMessage(actor.Name,
                                                                            action.Name,
                                                                            loggableData);

                CharactersHealthChanged?.Invoke(this, new CharactersHealthChangedEventArgs()
                {
                    PostCharactersChanged = postHealthChangedDict,
                    PreCharactersChanged = preHealthChangedDict,
                    ChangeAmount = changeAmount,
                    LogMessage = logMessage
                });
            }
        }

        /// <summary>
        /// Applies healing then damage from a delayed action.
        /// </summary>
        /// <param name="action">The delayed action causing the healing and damage.</param>
        private void ApplyHealingAndDamage(DelayedAction action)
        {
            var targets = new List<Character>(AllCharacters.Where(
                                                  character => IsTargetableCharacter(character, action.BaseAction, action.Targets)));

            var postHealthChangedDict = new Dictionary<int, int>();
            var changeAmount = new Dictionary<int, int>();
            var loggableData = new List<KeyValuePair<string, int>>();
            var preHealthChangedDict = new Dictionary<int, int>();

            for (int i = 0; i < targets.Count(); i++)
            {
                int startingHealth = targets[i].CurrentHealth;
                preHealthChangedDict.Add(targets[i].Id, startingHealth);
                int totalDamage = DamageCalculator.GetTotalDamage(action.TotalDamage, targets[i]);
                int percentHealAmount = targets[i].CurrentMaxHealth * action.HealPercentage;

                _characterController.ModifyCurrentHealth(targets[i], percentHealAmount);
                _characterController.ModifyCurrentHealth(targets[i], action.HealAmount);
                _characterController.ModifyCurrentHealth(targets[i], totalDamage);
                _threatController.ApplyThreat(action.Actor,
                                              targets[i],
                                              percentHealAmount + action.HealAmount + totalDamage,
                                              action.BaseAction.Threat,
                                              action.BaseAction.ThreatMultiplier);
                int modifiedHealth = targets[i].CurrentHealth - startingHealth;
                changeAmount.Add(targets[i].Id, modifiedHealth);
                loggableData.Add(new KeyValuePair<string, int>(targets[i].Name, modifiedHealth));
                postHealthChangedDict.Add(targets[i].Id, targets[i].CurrentHealth);
            }

            if (changeAmount.Values.Any(val => val != 0))
            {
                string logMessage = CombatMessenger.GetHealthChangedMessage(action.Actor.Name,
                                                                            action.BaseAction.Name,
                                                                            loggableData);

                CharactersHealthChanged?.Invoke(this, new CharactersHealthChangedEventArgs()
                {
                    PostCharactersChanged = postHealthChangedDict,
                    PreCharactersChanged = preHealthChangedDict,
                    ChangeAmount = changeAmount,
                    LogMessage = logMessage
                });
            }

            CheckForDeadTargets(targets);
        }

        /// <summary>
        /// Applies any status effects that are part of this action.
        /// </summary>
        /// <param name="actor">The character applying the status effects.</param>
        /// <param name="action">The action being performed.</param>
        /// <param name="targets">A list of characters to apply status effects to.</param>
        private void ApplyStatusEffects(Character actor, ActionBase action, IReadOnlyList<Character> targets)
        {
            var livingTargets = new List<Character>(targets);
            livingTargets.RemoveAll(character => character.CurrentHealth <= 0);

            if (livingTargets.Count() > 0)
            {
                foreach (var status in action.BuffsToApply)
                {
                    StatusEffectController.ApplyStatus(actor, status, livingTargets);
                }
            }
        }

        /// <summary>
        /// Orders the creation of delayed status effects as a result of a delayed spell.
        /// </summary>
        /// <param name="actor">The character applying the delayed status effects.</param>
        /// <param name="action">The action applying the delayed status effects.</param>
        /// <param name="targets">The target positions of the delayed status effects.</param>
        private void CreateDelayedStatus(Character actor, ActionBase action, IReadOnlyList<int> targets)
        {
            foreach (var status in action.BuffsToApply)
            {
                StatusEffectController.CreateDelayedStatus(actor, status, targets, action.Delay);
            }
        }
    }
}
