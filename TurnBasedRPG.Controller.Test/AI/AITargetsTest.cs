using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.AI;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.Test.AI
{
    [TestClass]
    public class AITargetsTest
    {
        public AITargetsTest()
        {
            LoadActions();
        }

        private List<ActionBase> _actions;

        private void LoadActions()
        {
            string location = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\testData.json";
            using (var reader = new StreamReader(location))
            {
                _actions = JsonConvert.DeserializeObject<List<ActionBase>>(reader.ReadToEnd());
            }
        }

        [TestMethod]
        public void GetModifiedTargets_WithLShape_ReturnsCorrectTargets()
        {
            // Target positions: 1, 2, 3, 6
            // Center of Targets Position: 1
            var testAction = _actions.FirstOrDefault(action => action.Id == 1);
            var expected = new List<int>() { 18, 17, 16, 13 };

            var actual = AITargets.GetModifiedTargets(testAction).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void GetModifiedCenter_WithDisplacedCenter_ReturnsCorrect()
        {
            // Target positions: 1, 2, 3, 6
            // Center of Targets Position: 1
            var testAction = _actions.FirstOrDefault(action => action.Id == 1);
            int expected = 18;

            var actual = AITargets.GetModifiedCenter(testAction.CenterOfTargetsPosition);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetModifiedSelection_WithLShape_ReturnsLine()
        {
            // Target positions: 1, 2, 3, 6
            // Center of Targets Position: 1
            var testAction = _actions.FirstOrDefault(action => action.Id == 1);
            int selectionTarget = 12;
            var expected = new List<int>() { 10, 11, 12 };

            var actual = new List<int>(AITargets.GetModifiedSelection(testAction, selectionTarget));

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void GetModifiedSelection_WithLShape_ReturnsSinglePosition()
        {
            // Target positions: 1, 2, 3, 6
            // Center of Targets Position: 1
            var testAction = _actions.FirstOrDefault(action => action.Id == 1);
            int selectionTarget = 13;
            var expected = new List<int>() { 13 };

            var actual = new List<int>(AITargets.GetModifiedSelection(testAction, selectionTarget));

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void GetModifiedSelection_WithLShapeInEnemy_ReturnsLine()
        {
            // Target positions: 1, 2, 3, 6
            // Center of Targets Position: 1
            var testAction = _actions.FirstOrDefault(action => action.Id == 1);
            int selectionTarget = 3;
            var expected = new List<int>() { 1, 2, 3 };

            var actual = new List<int>(AITargets.GetModifiedSelection(testAction, selectionTarget));

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void GetModifiedSelect_WithLShapeInEnemy_ReturnsSmallLine()
        {
            // Target positions: 1, 2, 3, 6
            // Center of Targets Position: 1
            var testAction = _actions.FirstOrDefault(action => action.Id == 1);
            int selectionTarget = 5;
            var expected = new List<int>() { 4, 5 };

            var actual = new List<int>(AITargets.GetModifiedSelection(testAction, selectionTarget));

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
