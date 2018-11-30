using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Combat;

namespace TurnBasedRPG.Shared.Test.Combat
{
    [TestClass]
    public class CombatTargeterTest
    {
        [TestMethod]
        public void GetTranslatedTargetPositions_WithOneTarget_ReturnsTarget()
        {
            var defaultTargets = new List<int>() { 5 };
            int centerOfTargets = 5;
            bool canSwitchTargetPositions = true;
            int targetPosition = 5;

            var expected = new List<int>() { 5 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets, 
                                                                   centerOfTargets, 
                                                                   canSwitchTargetPositions, 
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }

        [TestMethod]
        public void GetTranslatedTargetPositions_WithMultipleTargets_ReturnsTargets()
        {
            var defaultTargets = new List<int>() { 2, 4, 5, 6, 8 };
            int centerOfTargets = 5;
            bool canSwitchTargetPositions = true;
            int targetPosition = 5;

            var expected = new List<int>() { 2, 4, 5, 6, 8 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets,
                                                                   centerOfTargets,
                                                                   canSwitchTargetPositions,
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }

        [TestMethod]
        public void GetTranslatedTargetPositions_WithTargetsOutOfBounds_ReturnsNoOutOfBounds()
        {
            var defaultTargets = new List<int>() { 2, 4, 5, 6, 8 };
            int centerOfTargets = 5;
            bool canSwitchTargetPositions = true;
            int targetPosition = 9;

            var expected = new List<int>() { 6, 8, 9 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets,
                                                                   centerOfTargets,
                                                                   canSwitchTargetPositions,
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }

        [TestMethod]
        public void GetTranslatedTargetPositions_WithModifiedCenter_ReturnsModifiedPositions()
        {
            var defaultTargets = new List<int>() { 1, 2, 3, 6, 9 };
            int centerOfTargets = 1;
            bool canSwitchTargetPositions = true;
            int targetPosition = 4;

            var expected = new List<int>() { 4, 5, 6, 9 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets,
                                                                   centerOfTargets,
                                                                   canSwitchTargetPositions,
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }

        [TestMethod]
        public void GetTranslatedTargetPositions_WithDifferentFormationPosition()
        {
            var defaultTargets = new List<int>() { 2, 5, 8 };
            int centerOfTargets = 5;
            bool canSwitchTargetPositions = true;
            int targetPosition = 14;

            var expected = new List<int>() { 11, 14, 17 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets,
                                                                   centerOfTargets,
                                                                   canSwitchTargetPositions,
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }

        [TestMethod]
        public void GetTranslatedTargetPositions_WithDifferentFormationPositionAndOutOfBounds()
        {
            var defaultTargets = new List<int>() { 2, 5, 6, 8 };
            int centerOfTargets = 5;
            bool canSwitchTargetPositions = true;
            int targetPosition = 15;

            var expected = new List<int>() { 12, 15, 18 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets,
                                                                   centerOfTargets,
                                                                   canSwitchTargetPositions,
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }

        [TestMethod]
        public void GetTranslatedTargetPositions_WithDifferentFormationAndMultipleOutOfBounds()
        {
            var defaultTargets = new List<int>() { 2, 5, 6, 8 };
            int centerOfTargets = 2;
            bool canSwitchTargetPositions = true;
            int targetPosition = 17;

            var expected = new List<int>() { 17 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets,
                                                                   centerOfTargets,
                                                                   canSwitchTargetPositions,
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }

        [TestMethod]
        public void GetTranslatedTargetPositions_OnBottomRightCorner()
        {
            var defaultTargets = new List<int>() { 2, 4, 5, 6, 8 };
            int centerOfTargets = 5;
            bool canSwitchTargetPositions = true;
            int targetPosition = 9;

            var expected = new List<int>() { 6, 8, 9 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets,
                                                                   centerOfTargets,
                                                                   canSwitchTargetPositions,
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }

        [TestMethod]
        public void GetTranslatedTargetPositions_OnTopRightCorner()
        {
            var defaultTargets = new List<int>() { 2, 4, 5, 6, 8 };
            int centerOfTargets = 5;
            bool canSwitchTargetPositions = true;
            int targetPosition = 3;

            var expected = new List<int>() { 2, 3, 6 };
            var actual = CombatTargeter.GetTranslatedTargetPositions(defaultTargets,
                                                                   centerOfTargets,
                                                                   canSwitchTargetPositions,
                                                                   targetPosition);
            var actualAsList = new List<int>(actual);

            CollectionAssert.AreEquivalent(expected, actualAsList);
        }
    }
}
