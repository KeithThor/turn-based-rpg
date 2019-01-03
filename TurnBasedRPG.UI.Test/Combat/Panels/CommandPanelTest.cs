using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TurnBasedRPG.UI.Combat.Panels;
using TurnBasedRPG.UI.Test.Combat.Mocks;

namespace TurnBasedRPG.UI.Test.Combat
{
    [TestClass]
    public class ActionPanelTest
    {
        [TestMethod]
        public void Render_WithAttackFocus_RendersCorrectly()
        {
            var mockUIStateTracker = new MockUIStateTracker
            {
                CommandFocusNumber = 1
            };
            var commandPanel = new CommandPanel(mockUIStateTracker);

            List<string> expected = new List<string>()
            {
                "╔════════════════",
                "║ ► Attack      │",
                "║               │",
                "║   Spells      │",
                "║               │",
                "║   Skills      │",
                "║               │",
                "║   Items       │",
                "║               │",
                "║   Status      │",
                "║               │",
                "║   Wait        │",
                "║               │",
                "║   Run         │",
                "║               │",
                "╚════════════════"
            };

            var actual = commandPanel.Render();
            var actualAsList = new List<string>(actual);

            CollectionAssert.AreEqual(expected, actualAsList);
        }

        [TestMethod]
        public void Render_WithSkillsFocus_RendersCorrectly()
        {
            var mockUIStateTracker = new MockUIStateTracker();
            var commandPanel = new CommandPanel(mockUIStateTracker);
            commandPanel.FocusNumber = 3;

            List<string> expected = new List<string>()
            {
                "╔════════════════",
                "║   Attack      │",
                "║               │",
                "║   Spells      │",
                "║               │",
                "║ ► Skills      │",
                "║               │",
                "║   Items       │",
                "║               │",
                "║   Status      │",
                "║               │",
                "║   Wait        │",
                "║               │",
                "║   Run         │",
                "║               │",
                "╚════════════════"
            };

            var actual = commandPanel.Render();
            var actualAsList = new List<string>(actual);

            CollectionAssert.AreEqual(expected, actualAsList);
        }

        [TestMethod]
        public void Render_WithAttackFocusAndDifferentWidth_RendersCorrectly()
        {
            var mockUIStateTracker = new MockUIStateTracker
            {
                CommandFocusNumber = 1
            };
            var commandPanel = new CommandPanel(mockUIStateTracker)
            {
                MaxWidth = 20
            };
            List<string> expected = new List<string>()
            {
                "╔═══════════════════",
                "║ ► Attack         │",
                "║                  │",
                "║   Spells         │",
                "║                  │",
                "║   Skills         │",
                "║                  │",
                "║   Items          │",
                "║                  │",
                "║   Status         │",
                "║                  │",
                "║   Wait           │",
                "║                  │",
                "║   Run            │",
                "║                  │",
                "╚═══════════════════"
            };

            var actual = commandPanel.Render();
            var actualAsList = new List<string>(actual);

            CollectionAssert.AreEqual(expected, actualAsList);
        }
    }
}
