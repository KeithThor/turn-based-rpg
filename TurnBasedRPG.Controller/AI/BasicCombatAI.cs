using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.AI.Interfaces;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.Controller.AI
{
    /// <summary>
    /// Basic combat AI that makes a decision on which action to take and which positions to use that action.
    /// </summary>
    public class BasicCombatAI: ICombatAI
    {
        /// <summary>
        /// Contains characters returned by a selection, split by Ai and player characters.
        /// </summary>
        protected struct SelectionCharacters
        {
            public IReadOnlyList<Character> MyCharacters;
            public IReadOnlyList<Character> PlayerCharacters;
        }

        /// <summary>
        /// Contains data for the maximum priority target of an action.
        /// </summary>
        protected struct MaxPriorityTarget
        {
            public int TotalPriority;
            public int TargetPosition;
        }

        /// <summary>
        /// Contains the priorities for offensive and defensive actions.
        /// </summary>
        protected struct ActionPriorities
        {
            public Dictionary<ActionBase, int> DefensiveActionPriorities;
            public Dictionary<ActionBase, int> OffensiveActionPriorities;
        }
        
        protected Character _activeCharacter;

        public IReadOnlyList<Character> AICharacters { get; set; }
        public IReadOnlyList<Character> LivingPlayerCharacters { get; set; }
        private Random _rand;

        public BasicCombatAI()
        {
            _rand = new Random();
        }

        /// <summary>
        /// Given a currently active AI character, returns an action to take and a target position, based on 
        /// the currently available actions the active character can take and which will have the highest impact.
        /// </summary>
        /// <param name="character">The currently active ai character.</param>
        /// <returns>The AIDecision package that contains information about the action and target the AI chose to make.</returns>
        public AIDecision GetAIDecision(Character character, 
                                        IReadOnlyList<Character> aiCharacters, 
                                        IReadOnlyList<Character> playerCharacters)
        {
            _activeCharacter = character;
            AICharacters = aiCharacters;
            LivingPlayerCharacters = playerCharacters.Where(chara => chara.CurrentHealth > 0).ToList();

            return MakeDecision();
        }

        /// <summary>
        /// Loops through all actions and character targets on the field to see which action and target combination
        /// returns the highest priority, then makes that action and target the choice for this turn.
        /// </summary>
        /// <returns>The AIDecision package that contains information about the action and target the AI chose to make.</returns>
        protected AIDecision MakeDecision()
        {
            var actionPriorities = EvaluateActions();
            var enemyPriorities = EvaluateEnemies();
            var allyPriorities = EvaluateAllies();
            var allPriorities = enemyPriorities.Concat(allyPriorities).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            MaxPriorityTarget maxPriorityTarget = new MaxPriorityTarget();
            ActionBase highestPriorityAction = null;

            foreach (var action in actionPriorities.OffensiveActionPriorities.Keys)
            {
                var temp = GetMaxPriorityTarget(action, allPriorities, true);
                // If this action's total priority is greater than the current highest priority action, replace it
                // In case of equal priorities, 50% chance to replace current action
                if (temp.TotalPriority > maxPriorityTarget.TotalPriority || 
                    (_rand.Next(0, 2) == 0 && temp.TotalPriority == maxPriorityTarget.TotalPriority))
                {
                    maxPriorityTarget = temp;
                    highestPriorityAction = action;
                }
            }

            foreach (var action in actionPriorities.DefensiveActionPriorities.Keys)
            {
                var temp = GetMaxPriorityTarget(action, allPriorities);
                // If this action's total priority is greater than the current highest priority action, replace it
                // In case of equal priorities, 50% chance to replace current action
                if (temp.TotalPriority > maxPriorityTarget.TotalPriority ||
                    (_rand.Next(0, 2) == 0 && temp.TotalPriority == maxPriorityTarget.TotalPriority))
                {
                    maxPriorityTarget = temp;
                    highestPriorityAction = action;
                }
            }

            var consumable = _activeCharacter.Inventory
                                              .Where(item => item is Consumable
                                                    && ((Consumable)item).ItemSpell == highestPriorityAction)
                                                    .FirstOrDefault();

            return new AIDecision()
            {
                ActionChoice = highestPriorityAction,
                TargetPosition = maxPriorityTarget.TargetPosition,
                ConsumableUsed = consumable as Consumable
            };
        }
        
        /// <summary>
        /// Checks all allies for those in need of healing, with priority increasing by 1 for every 20% of max health the
        /// ally is missing. If the active character is severely wounded, will place extra priority on itself.
        /// <para>Include dead should only be true for spells that can ressurect the dead.</para>
        /// </summary>
        /// <param name="includeDead">If true, check priorities for dead characters too.</param>
        /// <returns>A dictionary of character keys and integer priority values, with higher values being higher priority targets.</returns>
        protected virtual Dictionary<Character,int> EvaluateAllies(bool includeDead = false)
        {
            var characterPriorities = new Dictionary<Character, int>();
            foreach (var character in AICharacters)
            {
                float percentHealthLost = 1.0f - (((float)character.CurrentHealth) / character.CurrentMaxHealth);
                int priority = (int)(percentHealthLost * 100) / 20;

                // Only dead characters and the active character, if it is severely wounded, can be priority 5
                if (priority == 5 && !includeDead) priority = 0;
                if (character == _activeCharacter && priority > 2) priority++;
                characterPriorities[character] = priority;
            }
            return characterPriorities;
        }
        
        /// <summary>
        /// Loops through all player characters and creates a dictionary of integer priorities based on the amount of threat
        /// a character has generated.
        /// <para>Priority is based on the threat a character generates compared to the rest of it's squad.</para>
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<Character,int> EvaluateEnemies()
        {
            var enemies = new List<Character>(LivingPlayerCharacters);
            var characterPriorities = new Dictionary<Character, int>();

            int median = MathExtensions.GetMedian(enemies.Select(enemy => enemy.Threat).ToList());
            
            if (enemies.Count() == 2)
            {
                if (enemies[0].Threat > enemies[1].Threat)
                {
                    characterPriorities[enemies[0]] = 2;
                    characterPriorities[enemies[1]] = 1;
                }
                else
                {
                    characterPriorities[enemies[1]] = 2;
                    characterPriorities[enemies[0]] = 1;
                }
                foreach (var enemy in enemies)
                {
                    int percentHealth = (int)(((float)enemy.CurrentHealth) / enemy.CurrentMaxHealth * 100);
                    // If percentage of health is less than 25%, increase threat level by 2
                    if (percentHealth <= 25) characterPriorities[enemy] += 2;
                    else if (percentHealth <= 50) characterPriorities[enemy] += 1;
                }
            }
            else
            {
                foreach (var enemy in enemies)
                {
                    int threatAsMedianPercentage = (int)(((float)enemy.Threat) / median * 100);

                    // If threat is 50% higher than the median, threat level is 3
                    if (threatAsMedianPercentage >= 150) characterPriorities[enemy] = 3;
                    // If threat is 20% higher than the median, threat level is 2
                    else if (threatAsMedianPercentage >= 120) characterPriorities[enemy] = 2;
                    else characterPriorities[enemy] = 1;

                    int percentHealth = (int)(((float)enemy.CurrentHealth) / enemy.CurrentMaxHealth * 100);

                    // If percentage of health is less than 25%, increase threat level by 2
                    if (percentHealth <= 25) characterPriorities[enemy] += 2;
                    else if (percentHealth <= 50) characterPriorities[enemy] += 1;
                }
            }
            return characterPriorities;
        }

        /// <summary>
        /// Gets all possible actions for the active character and splits them into offensive and defensive actions,
        /// giving each a priority based on the amount of healing, damage, and/or ai weight provided by the action.
        /// </summary>
        /// <returns>A struct providing the action priorities of offensive and defensive actions.</returns>
        protected virtual ActionPriorities EvaluateActions()
        {
            var defensiveActionsPoints = new Dictionary<ActionBase, int>();
            var offensiveActionsPoints = new Dictionary<ActionBase, int>();
            var defensiveActionPriorities = new Dictionary<ActionBase, int>();
            var offensiveActionPriorities = new Dictionary<ActionBase, int>();
            var allActions = new List<ActionBase>();

            allActions.AddRange(_activeCharacter.Attacks);
            allActions.AddRange(_activeCharacter.SpellList);
            allActions.AddRange(_activeCharacter.SkillList);
            var consumables = _activeCharacter.Inventory
                                              .Where(item => item is Consumable)
                                              .Select(item => ((Consumable)item).ItemSpell);
            allActions.AddRange(consumables);

            // Use average max health as basis for effectiveness of damage and healing
            int averageMaxHealth = 0;
            foreach (var character in AICharacters)
            {
                averageMaxHealth += character.CurrentMaxHealth;
            }
            averageMaxHealth /= AICharacters.Count();

            // Calculate total healing and damage and use it to determine if an action is offensive or defensive
            foreach (var action in allActions)
            {
                int damage = DamageCalculator.GetDamageAsInt(_activeCharacter, action);
                int healing = DamageCalculator.GetHealing(_activeCharacter, action);
                int percentHealing = DamageCalculator.GetHealingPercentage(_activeCharacter, action);

                int netDamage = damage - healing;

                if (netDamage < 0 || percentHealing > 0 || !action.IsOffensive)
                {
                    int maxPotential = -netDamage;
                    maxPotential += (percentHealing * averageMaxHealth / 100);
                    defensiveActionsPoints[action] = maxPotential;
                }
                else
                {
                    int maxPotential = netDamage;
                    offensiveActionsPoints[action] = maxPotential;
                }
            }

            int medianHealing = MathExtensions.GetMedian(defensiveActionsPoints.Values.ToList());
            int medianDamage = MathExtensions.GetMedian(offensiveActionsPoints.Values.ToList());

            // Assign action priorities based on how an action performs on par with other actions

            foreach (var key in defensiveActionsPoints.Keys)
            {
                int healing = defensiveActionsPoints[key];
                defensiveActionPriorities[key] = key.AiWeight;
                if (medianHealing > 0)
                {
                    if (PercentageCalculator.GetPercentage(healing, medianHealing) >= 150)
                        defensiveActionPriorities[key] += 3;
                    else if (PercentageCalculator.GetPercentage(healing, medianHealing) >= 120)
                        defensiveActionPriorities[key] += 2;
                }
                else defensiveActionPriorities[key] += 1;
            }

            foreach (var key in offensiveActionsPoints.Keys)
            {
                int damage = offensiveActionsPoints[key];
                offensiveActionPriorities[key] = key.AiWeight;
                if (medianDamage > 0)
                {
                    if (PercentageCalculator.GetPercentage(damage, medianDamage) >= 150)
                        offensiveActionPriorities[key] += 3;
                    else if (PercentageCalculator.GetPercentage(damage, medianDamage) >= 120)
                        offensiveActionPriorities[key] += 2;
                }
                else offensiveActionPriorities[key] += 1;
            }

            return new ActionPriorities()
            {
                DefensiveActionPriorities = defensiveActionPriorities,
                OffensiveActionPriorities = offensiveActionPriorities
            };
        }

        /// <summary>
        /// Given an action, a dictionary of character priorities, and whether or not an action is considered an
        /// offensive action, gets the maximum possible priority of the action and it's target.
        /// </summary>
        /// <param name="action">The action to get the max priority target for.</param>
        /// <param name="priorities">A dictionary containing the priorities for each character target.</param>
        /// <param name="isOffensive">Whether or not an action is considered an offensive action.</param>
        /// <returns>A struct containing the max priority and it's target position.</returns>
        protected virtual MaxPriorityTarget GetMaxPriorityTarget(ActionBase action, Dictionary<Character, int> priorities, bool isOffensive = false)
        {
            int maxPriority = 0;
            int maxPotentialTarget = 0;

            if (!action.CanSwitchTargetPosition)
            {
                var characterTargets = GetSelectionTargets(action, action.CenterOfTargetsPosition);
                maxPotentialTarget = action.CenterOfTargetsPosition;

                foreach (var character in characterTargets.MyCharacters)
                {
                    maxPriority += isOffensive ? -priorities[character] : priorities[character];
                }
                foreach (var character in characterTargets.PlayerCharacters)
                {
                    maxPriority += isOffensive ? priorities[character] : -priorities[character];
                }
            }
            else
            {
                // Loops through all possible target positions and find the highest priority for this action
                for (int i = 0; i <= 18; i++)
                {
                    int totalPriority = 0;
                    var characterTargets = GetSelectionTargets(action, i);

                    foreach (var character in characterTargets.MyCharacters)
                    {
                        totalPriority += isOffensive ? -priorities[character] : priorities[character];
                    }
                    foreach (var character in characterTargets.PlayerCharacters)
                    {
                        totalPriority += isOffensive ? priorities[character] : -priorities[character];
                    }

                    // If the current target's total priority is greater than the previous highest priority, or if equal,
                    // change highest priority with 50% chance
                    if (totalPriority > maxPriority || (totalPriority == maxPriority && _rand.Next(0, 2) == 1))
                    {
                        maxPriority = totalPriority;
                        maxPotentialTarget = i;
                    }
                }
            }

            return new MaxPriorityTarget()
            {
                TotalPriority = maxPriority,
                TargetPosition = maxPotentialTarget
            };
        }

        /// <summary>
        /// Given an ActionBase and a selection position, gets the characters that
        /// action will target seperated by AI characters and player characters.
        /// </summary>
        /// <param name="action">The action to check the targets for.</param>
        /// <param name="selectionPosition">A number representing the position the action targets.</param>
        /// <returns>A struct containing two IReadOnlyList of Characters, one for AI characters and another for player characters.</returns>
        protected SelectionCharacters GetSelectionTargets(ActionBase action, int selectionPosition)
        {
            var myCharacters = new List<Character>();
            var playerCharacters = new List<Character>();

            // If selection is in left column and something is in the middle or right column on the same row, can't select this position
            bool leftColumnBlocked = selectionPosition % 3 == 1 &&
                    LivingPlayerCharacters.Where(
                        character =>
                        (selectionPosition + 1 == character.Position || selectionPosition + 2 == character.Position)
                        && character.CurrentHealth > 0).Any();

            // If selection is in middle column and something is in the right column on the same row, can't select this position
            bool middleColumnBlocked = selectionPosition % 3 == 2 &&
                    LivingPlayerCharacters.Where(
                        character =>
                        (selectionPosition + 1 == character.Position && character.CurrentHealth > 0)).Any();

            if (!action.CanSwitchTargetPosition)
            {
                var targets = AITargets.GetModifiedTargets(action);
                myCharacters.AddRange(AICharacters.Where(character => targets.Contains(character.Position)));
                playerCharacters.AddRange(LivingPlayerCharacters.Where(character => targets.Contains(character.Position)));
                return new SelectionCharacters()
                {
                    MyCharacters = myCharacters,
                    PlayerCharacters = playerCharacters
                };
            }
            // If the action can't target through units and the selection is on the player's field
            else if (!action.CanTargetThroughUnits && selectionPosition <= 9 && 
                    (leftColumnBlocked || middleColumnBlocked))
            {
                myCharacters.Clear();
                playerCharacters.Clear();
                return new SelectionCharacters()
                {
                    MyCharacters = myCharacters,
                    PlayerCharacters = playerCharacters
                };
            }
            else
            {
                var modifiedSelection = AITargets.GetModifiedSelection(action, selectionPosition);

                foreach (var position in modifiedSelection)
                {
                    if (position >= 10)
                    {
                        Character selectedCharacter = AICharacters.FirstOrDefault(character => character.Position == position);
                        if (selectedCharacter != null)
                            myCharacters.Add(selectedCharacter);
                    }
                    else if (position <= 9)
                    {
                        Character selectedCharacter = LivingPlayerCharacters.FirstOrDefault(character => character.Position == position);
                        if (selectedCharacter != null)
                            playerCharacters.Add(selectedCharacter);
                    }
                }

                return new SelectionCharacters()
                {
                    MyCharacters = myCharacters,
                    PlayerCharacters = playerCharacters
                };
            }
        }
    }
}
