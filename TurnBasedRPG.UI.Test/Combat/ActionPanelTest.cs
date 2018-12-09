using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnBasedRPG.Shared.Enums;
using System.Collections.Generic;
using TurnBasedRPG.UI.Combat.Panels;

namespace TurnBasedRPG.UI.Test.Combat
{
    [TestClass]
    public class ActionPanelTest
    {
        [TestMethod]
        public void Render_WithAttackFocus_RendersCorrectly()
        {
            int focus = (int)Commands.Attack;
            CommandPanel actionPanel = new CommandPanel();
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

            var actual = actionPanel.Render(focus);
            var actualAsList = new List<string>(actual);

            CollectionAssert.AreEqual(expected, actualAsList);
        }

        [TestMethod]
        public void Render_WithSkillsFocus_RendersCorrectly()
        {
            int focus = (int)Commands.Skills;
            CommandPanel actionPanel = new CommandPanel();
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

            var actual = actionPanel.Render(focus);
            var actualAsList = new List<string>(actual);

            CollectionAssert.AreEqual(expected, actualAsList);
        }

        [TestMethod]
        public void Render_WithAttackFocusAndDifferentWidth_RendersCorrectly()
        {
            int focus = (int)Commands.Attack;
            CommandPanel actionPanel = new CommandPanel
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

            var actual = actionPanel.Render(focus);
            var actualAsList = new List<string>(actual);

            CollectionAssert.AreEqual(expected, actualAsList);
        }
    }
}
