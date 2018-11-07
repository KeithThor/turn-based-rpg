using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TurnBasedRPG.Controller.AI;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared;

namespace TurnBasedRPG.Controller.Test.AI
{
    [TestClass]
    public class BasicCombatAITest
    {
        private List<Attack> _actions;

        public BasicCombatAITest()
        {
            LoadData();
        }

        /// <summary>
        /// Loads test data from testData.json located in the Controller.Test project folder.
        /// </summary>
        private void LoadData()
        {
            string location = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\testData.json";
            using (var reader = new StreamReader(location))
            {
                _actions = JsonConvert.DeserializeObject<List<Attack>>(reader.ReadToEnd());
            }
        }

        private Character GetBlankCharacter()
        {
            return new Character()
            {
                CurrentHealth = 100,
                CurrentMaxHealth = 100,
                Stats = new PrimaryStat(),
                CurrentStats = new PrimaryStat(),
                DamageModifier = new DamageTypes(),
                Inventory = new List<Item>(),
                DamagePercentageModifier = new DamageTypes(),
                Armor = new DamageTypes(),
                ArmorPercentage = new DamageTypes()
            };
        }

        /// <summary>
        /// AI should attack the target with the most threat when all else are equivalent.
        /// </summary>
        [TestMethod]
        public void GetAIDecision_WithBasicThreatAndAttack_AttacksHighestThreat()
        {
            var attack = _actions.Find(action => action.Id == 2);
            var playerCharacters = new List<Character>()
            {
                GetBlankCharacter(),
                GetBlankCharacter()
            };
            playerCharacters[0].Position = 4;
            playerCharacters[0].Threat = 100;
            playerCharacters[1].Position = 8;
            playerCharacters[1].Threat = 25;
            var aiC1 = GetBlankCharacter();
            aiC1.Attacks.Add(attack);
            aiC1.Position = 14;
            var aiCharacters = new List<Character>() { aiC1 };
            var ai = new BasicCombatAI();
            int expectedTargetPosition = 4;

            var aiDecisionData = ai.GetAIDecision(aiC1, aiCharacters, playerCharacters);
            int actualTargetPosition = aiDecisionData.TargetPosition;

            Assert.AreEqual(expectedTargetPosition, actualTargetPosition);
        }

        /// <summary>
        /// AI should attack the both targets rather than 1.
        /// </summary>
        [TestMethod]
        public void GetAIDecision_WithAreaAttack_AttacksBothTargets()
        {
            var attack = _actions.Find(action => action.Id == 1);
            var playerCharacters = new List<Character>()
            {
                GetBlankCharacter(),
                GetBlankCharacter()
            };
            playerCharacters[0].Position = 1;
            playerCharacters[0].Threat = 100;
            playerCharacters[1].Position = 5;
            playerCharacters[1].Threat = 25;
            var aiC1 = GetBlankCharacter();
            aiC1.Attacks.Add(attack);
            aiC1.Position = 14;
            var aiCharacters = new List<Character>() { aiC1 };
            var ai = new BasicCombatAI();
            int expectedTargetPosition = 6;

            var aiDecisionData = ai.GetAIDecision(aiC1, aiCharacters, playerCharacters);
            int actualTargetPosition = aiDecisionData.TargetPosition;

            Assert.AreEqual(expectedTargetPosition, actualTargetPosition);
        }

        /// <summary>
        /// AI should heal self if badly wounded and enemies are high on health.
        /// </summary>
        [TestMethod]
        public void GetAIDecision_WithHealingAndLowHealth_HealsInsteadOfAttack()
        {
            var attack = _actions.Find(action => action.Id == 2);
            var heal = _actions.Find(action => action.Id == 3);
            var playerCharacters = new List<Character>()
            {
                GetBlankCharacter(),
                GetBlankCharacter()
            };
            playerCharacters[0].Position = 4;
            playerCharacters[0].Threat = 25;
            playerCharacters[1].Position = 2;
            playerCharacters[1].Threat = 25;
            var aiC1 = GetBlankCharacter();
            aiC1.Attacks.Add(attack);
            aiC1.Attacks.Add(heal);
            aiC1.Position = 14;
            aiC1.CurrentHealth = 25;
            var aiCharacters = new List<Character>() { aiC1 };
            var ai = new BasicCombatAI();
            int expectedTargetPosition = 14;

            var aiDecisionData = ai.GetAIDecision(aiC1, aiCharacters, playerCharacters);
            int actualTargetPosition = aiDecisionData.TargetPosition;

            Assert.AreEqual(expectedTargetPosition, actualTargetPosition);
            Assert.AreSame(heal, aiDecisionData.ActionChoice as Attack);
        }

        /// <summary>
        /// AI should attack the target with the most threat when there are more than 2 enemies.
        /// </summary>
        [TestMethod]
        public void GetAIDecision_WithBasicAttackAndMultipleEnemies_AttacksHighestThreat()
        {
            var attack = _actions.Find(action => action.Id == 2);
            var playerCharacters = new List<Character>()
            {
                GetBlankCharacter(),
                GetBlankCharacter(),
                GetBlankCharacter()
            };
            playerCharacters[0].Position = 4;
            playerCharacters[0].Threat = 25;
            playerCharacters[1].Position = 8;
            playerCharacters[1].Threat = 100;
            playerCharacters[2].Position = 2;
            playerCharacters[2].Threat = 50;
            var aiC1 = GetBlankCharacter();
            aiC1.Attacks.Add(attack);
            aiC1.Position = 14;
            var aiCharacters = new List<Character>() { aiC1 };
            var ai = new BasicCombatAI();
            int expectedTargetPosition = 8;

            var aiDecisionData = ai.GetAIDecision(aiC1, aiCharacters, playerCharacters);
            int actualTargetPosition = aiDecisionData.TargetPosition;

            Assert.AreEqual(expectedTargetPosition, actualTargetPosition);
        }

        /// <summary>
        /// AI should attack the target with the 2nd most threat if the target with the most threat is blocked.
        /// </summary>
        [TestMethod]
        public void GetAIDecision_WithBlockedTarget_AttacksNextHighestThreat()
        {
            var attack = _actions.Find(action => action.Id == 2);
            var playerCharacters = new List<Character>()
            {
                GetBlankCharacter(),
                GetBlankCharacter(),
                GetBlankCharacter(),
                GetBlankCharacter(),
            };
            playerCharacters[0].Position = 4;
            playerCharacters[0].Threat = 100;
            playerCharacters[1].Position = 8;
            playerCharacters[1].Threat = 50;
            playerCharacters[2].Position = 6;
            playerCharacters[2].Threat = 25;
            playerCharacters[3].Position = 1;
            playerCharacters[3].Threat = 10;
            var aiC1 = GetBlankCharacter();
            aiC1.Attacks.Add(attack);
            aiC1.Position = 14;
            var aiCharacters = new List<Character>() { aiC1 };
            var ai = new BasicCombatAI();
            int expectedTargetPosition = 8;
            var aiDecisionData = ai.GetAIDecision(aiC1, aiCharacters, playerCharacters);
            int actualTargetPosition = aiDecisionData.TargetPosition;

            Assert.AreEqual(expectedTargetPosition, actualTargetPosition);
        }

        /// <summary>
        /// AI should attack the target with the 2nd most threat if the target with the most threat is dead.
        /// </summary>
        [TestMethod]
        public void GetAIDecision_WithDeadTarget_AttacksNextHighestThreat()
        {
            var attack = _actions.Find(action => action.Id == 2);
            var playerCharacters = new List<Character>()
            {
                GetBlankCharacter(),
                GetBlankCharacter(),
                GetBlankCharacter(),
                GetBlankCharacter(),
            };
            playerCharacters[0].Position = 3;
            playerCharacters[0].Threat = 100;
            playerCharacters[0].CurrentHealth = 0;
            playerCharacters[1].Position = 8;
            playerCharacters[1].Threat = 50;
            playerCharacters[2].Position = 6;
            playerCharacters[2].Threat = 25;
            var aiC1 = GetBlankCharacter();
            aiC1.Attacks.Add(attack);
            aiC1.Position = 14;
            var aiCharacters = new List<Character>() { aiC1 };
            var ai = new BasicCombatAI();
            int expectedTargetPosition = 8;
            var aiDecisionData = ai.GetAIDecision(aiC1, aiCharacters, playerCharacters);
            int actualTargetPosition = aiDecisionData.TargetPosition;

            Assert.AreEqual(expectedTargetPosition, actualTargetPosition);
        }

        /// <summary>
        /// AI should attack the center of targets position for an action that can't move it's target position.
        /// </summary>
        [TestMethod]
        public void GetAIDecision_WithUnmovableTarget_AttacksDefaultTarget()
        {
            var attack = _actions.Find(action => action.Id == 4);
            var playerCharacters = new List<Character>()
            {
                GetBlankCharacter(),
                GetBlankCharacter(),
                GetBlankCharacter()
            };
            playerCharacters[0].Position = 4;
            playerCharacters[0].Threat = 25;
            playerCharacters[1].Position = 8;
            playerCharacters[1].Threat = 25;
            playerCharacters[2].Position = 6;
            playerCharacters[2].Threat = 25;
            var aiC1 = GetBlankCharacter();
            aiC1.Attacks.Add(attack);
            aiC1.Position = 14;
            var aiCharacters = new List<Character>() { aiC1 };
            var ai = new BasicCombatAI();
            int expectedTargetPosition = 4;
            var aiDecisionData = ai.GetAIDecision(aiC1, aiCharacters, playerCharacters);
            int actualTargetPosition = aiDecisionData.TargetPosition;

            Assert.AreEqual(expectedTargetPosition, actualTargetPosition);
        }
    }
}
