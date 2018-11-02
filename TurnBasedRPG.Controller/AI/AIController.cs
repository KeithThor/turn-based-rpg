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
    public class AIController: ICombatAI
    {
        private struct SelectionCharacters
        {
            public IReadOnlyList<Character> MyCharacters;
            public IReadOnlyList<Character> PlayerCharacters;
        }

        
        private Character _activeCharacter;

        public IReadOnlyList<Character> MyCharacters { get; set; }
        public IReadOnlyList<Character> PlayerCharacters { get; set; }

        public AIController(IReadOnlyList<Character> myCharacters, IReadOnlyList<Character> playerCharacters)
        {
            MyCharacters = myCharacters;
            PlayerCharacters = playerCharacters;
        }

        public AIDecision GetAIDecision(Character character)
        {
            var decision = new AIDecision();
            _activeCharacter = character;

            EvaluateChoices();

            return decision;
        }

        private void EvaluateChoices()
        {
            EvaluateActions();
            EvaluateEnemies();
            EvaluateAllies();
        }

        /// <summary>
        /// Checks all allies for those in need of healing, with priority increasing by 1 for every 20% of max health the
        /// ally is missing. If the active character is severely wounded, will place extra priority on itself.
        /// <para>Include dead should only be true for spells that can ressurect the dead.</para>
        /// </summary>
        /// <param name="includeDead">If true, check priorities for dead characters too.</param>
        /// <returns>A dictionary of character keys and integer priority values, with higher values being higher priority targets.</returns>
        private Dictionary<Character,int> EvaluateAllies(bool includeDead = false)
        {
            var characterPriorities = new Dictionary<Character, int>();
            foreach (var character in MyCharacters)
            {
                float percentHealthLost = 1 - (character.CurrentHealth / character.CurrentMaxHealth);
                int priority = (int)percentHealthLost * 100 / 20;

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
        private Dictionary<Character,int> EvaluateEnemies()
        {
            var enemies = new List<Character>(PlayerCharacters);
            var characterPriorities = new Dictionary<Character, int>();

            int median = enemies.Select(enemy => enemy.Threat).ToList().GetMedian();

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
            return characterPriorities;
        }

        private void EvaluateActions()
        {
            var defensiveActions = new Dictionary<ActionBase, int>();
            var offensiveActions = new Dictionary<ActionBase, int>();
            var allActions = new List<ActionBase>();
            allActions.AddRange(_activeCharacter.Attacks);
            allActions.AddRange(_activeCharacter.SpellList);
            allActions.AddRange(_activeCharacter.SkillList);
            var consumables = _activeCharacter.Inventory
                                              .Where(item => item is Consumable)
                                              .Select(item => ((Consumable)item).ItemSpell);
            allActions.AddRange(consumables);

            int averageMaxHealth = 0;
            foreach (var character in MyCharacters)
            {
                averageMaxHealth += character.CurrentMaxHealth;
            }
            averageMaxHealth /= MyCharacters.Count();

            foreach (var action in allActions)
            {
                int damage = DamageCalculator.GetDamageAsInt(_activeCharacter, action);
                int healing = DamageCalculator.GetHealing(_activeCharacter, action);
                int percentHealing = DamageCalculator.GetHealingPercentage(_activeCharacter, action);

                int netDamage = damage - healing;

                if (netDamage < 0 || percentHealing > 0)
                {
                    int maxPotential = -netDamage * action.TargetPositions.Count();
                    maxPotential += (percentHealing * averageMaxHealth / 100) * action.TargetPositions.Count();
                    defensiveActions[action] = maxPotential;
                }
                else
                {
                    int maxPotential = netDamage * action.TargetPositions.Count();
                    offensiveActions[action] = maxPotential;
                }
            }

            int medianHealing = defensiveActions.Values.ToList().GetMedian();
            int medianDamage = offensiveActions.Values.ToList().GetMedian();

            foreach (var key in defensiveActions.Keys)
            {
                int healing = defensiveActions[key];
                if (PercentageCalculator.GetPercentage(healing, medianHealing) >= 150) defensiveActions[key] = 3;
                if (PercentageCalculator.GetPercentage(healing, medianHealing) >= 120) defensiveActions[key] = 2;
                else defensiveActions[key] = 1;
            }

            foreach (var key in offensiveActions.Keys)
            {
                int damage = offensiveActions[key];
                if (PercentageCalculator.GetPercentage(damage, medianDamage) >= 150) offensiveActions[key] = 3;
                if (PercentageCalculator.GetPercentage(damage, medianDamage) >= 120) offensiveActions[key] = 2;
                else offensiveActions[key] = 1;
            }
        }

        private void GetMaximumPotentialTarget(ActionBase action, Dictionary<Character, int> priorities, bool isOffensive = false)
        {
            var modifiedTargets = GetModifiedTargets(action);
            var modifiedCenter = GetModifiedCenter(action.CenterOfTargetsPosition);

            // Loops through all possible target positions and find the highest priority for this action
            for (int i = 0; i <= 18; i++)
            {
                int totalPriority = 0;
                var characterTargets = GetSelectionTargets(modifiedTargets, modifiedCenter, i, action);

                foreach (var character in characterTargets.MyCharacters)
                {
                    totalPriority += isOffensive ? -priorities[character] : priorities[character];
                }
                foreach (var character in characterTargets.PlayerCharacters)
                {
                    totalPriority += isOffensive ? priorities[character] : -priorities[character];
                }
            }

        }

        /// <summary>
        /// Gets a new list of target positions for an action for use with the AI.
        /// </summary>
        /// <param name="action">The action to generate a new list of target positions for.</param>
        /// <returns>A list of integers containing the AI modified positions.</returns>
        private IReadOnlyList<int> GetModifiedTargets(ActionBase action)
        {
            var modifiedTargets = new List<int>();

            foreach (var position in action.TargetPositions)
            {
                modifiedTargets.Add(19 - position);
            }
            return modifiedTargets;
        }

        /// <summary>
        /// Gets the center of targets position for an action for use with the AI.
        /// </summary>
        /// <param name="centerOfTargetsPosition">The original center of targets position for an action.</param>
        /// <returns>The AI modified center of targets position.</returns>
        private int GetModifiedCenter(int centerOfTargetsPosition)
        {
            return 19 - centerOfTargetsPosition;
        }

        /// <summary>
        /// Given an ActionBase, it's modified targets and center of targets, and a selection position, gets the positions that
        /// action will target, removing positions that go out of bounds.
        /// </summary>
        /// <param name="targets">A list containing the target positions modified for AI usage.</param>
        /// <param name="centerOfTargets">A number representing the center of the target positions modified for AI usage.</param>
        /// <param name="selectionPosition">A number representing the position the action targets.</param>
        /// <param name="action">The action to check the targets for.</param>
        /// <returns>A list of integers containing the AI modified selection of targets. May return a list with no items in the case an action hits nothing.</returns>
        private IReadOnlyList<int> GetModifiedSelection(IReadOnlyList<int> targets, int centerOfTargets, int selectionPosition, ActionBase action)
        {
            var selectedPositions = new List<int>(targets);
            // If the action can't target through units and the selection is on the enemy field
            if (!action.CanTargetThroughUnits && selectionPosition <= 9)
            {
                // If selection is in left column and something is in the middle or right column on the same row, can't select this position
                if (selectionPosition % 3 == 1 && 
                    PlayerCharacters.Where(
                        character => 
                        (selectionPosition + 1 == character.Position || selectionPosition + 2 == character.Position) 
                        && character.CurrentHealth > 0).Any())
                {
                    selectedPositions.Clear();
                    return selectedPositions;
                }
                // If selection is in middle column and something is in the right column on the same row, can't select this position
                else if (selectionPosition % 3 == 2 &&
                    PlayerCharacters.Where(
                        character =>
                        (selectionPosition + 1 == character.Position && character.CurrentHealth > 0)).Any())
                {
                    selectedPositions.Clear();
                    return selectedPositions;
                }
            }

            int offset = (selectionPosition > 9) ? 9 : 0;

            // If the center of target for the action is in a different column than the current selection's column
            if (centerOfTargets % 3 != selectionPosition % 3)
            {
                // If the player's center of target is in the right column and the center of target for an action is not the 
                // right column, remove all targets in the right column
                if (selectionPosition % 3 == 0 && centerOfTargets % 3 != 0)
                    selectedPositions.RemoveAll(val => val % 3 == 0);
                // Target position is in the left column and the action's CoT not in left column,
                // all targets in the left column must be culled
                if (selectionPosition % 3 == 1 && centerOfTargets % 3 != 1)
                    selectedPositions.RemoveAll(val => val % 3 == 1);

                // If action's CoT is in left column and the selection is right column, remove center column
                if (centerOfTargets % 3 == 1 && selectionPosition % 3 == 0)
                    selectedPositions.RemoveAll(val => val % 3 == 2);
                // If action's CoT is in left column and the selection is middle column, remove right column
                if (centerOfTargets % 3 == 1 && selectionPosition % 3 == 2)
                    selectedPositions.RemoveAll(val => val % 3 == 0);
                // If action's CoT is in right column and the selection is middle column, remove left column
                if (centerOfTargets % 3 == 0 && selectionPosition % 3 == 2)
                    selectedPositions.RemoveAll(val => val % 3 == 1);
                // If action's CoT is in right column and the selection is left column, remove center column
                if (centerOfTargets % 3 == 0 && selectionPosition % 3 == 1)
                    selectedPositions.RemoveAll(val => val % 3 == 2);
            }
            for (int i = 0; i < selectedPositions.Count(); i++)
            {
                selectedPositions[i] += selectionPosition - centerOfTargets;
            }
            selectedPositions.RemoveAll(position => position > 9 + offset || position < 1 + offset);


            return selectedPositions;
        }

        /// <summary>
        /// Given an ActionBase, it's modified targets and center of targets, and a selection position, gets the characters that
        /// action will target seperated by AI characters and player characters.
        /// </summary>
        /// <param name="targets">A list containing the target positions modified for AI usage.</param>
        /// <param name="centerOfTargets">A number representing the center of the target positions modified for AI usage.</param>
        /// <param name="selectionPosition">A number representing the position the action targets.</param>
        /// <param name="action">The action to check the targets for.</param>
        /// <returns>A struct containing two IReadOnlyList of Characters, one for AI characters and another for player characters.</returns>
        private SelectionCharacters GetSelectionTargets(IReadOnlyList<int> targets, int centerOfTargets, int selectionPosition, ActionBase action)
        {
            var myCharacters = new List<Character>();
            var playerCharacters = new List<Character>();

            if (!action.CanSwitchTargetPosition)
            {
                myCharacters.AddRange(MyCharacters.Where(character => targets.Contains(character.Position)));
                playerCharacters.AddRange(PlayerCharacters.Where(character => targets.Contains(character.Position)));
                return new SelectionCharacters()
                {
                    MyCharacters = myCharacters,
                    PlayerCharacters = playerCharacters
                };
            }
            else
            {
                var modifiedSelection = GetModifiedSelection(targets, centerOfTargets, selectionPosition, action);

                foreach (var position in modifiedSelection)
                {
                    if (position >= 10)
                    {
                        Character selectedCharacter = MyCharacters.FirstOrDefault(character => character.Position == position);
                        if (selectedCharacter != null)
                            myCharacters.Add(selectedCharacter);
                    }
                    else if (position <= 9)
                    {
                        Character selectedCharacter = PlayerCharacters.FirstOrDefault(character => character.Position == position);
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
