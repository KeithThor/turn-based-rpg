using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnBasedRPG.Shared.Enums;
using System.Collections.Generic;

namespace TurnBasedRPG.UI.Test
{
    [TestClass]
    public class ActionPanelTest
    {
        [TestMethod]
        public void Render_WithAttackFocus_RendersCorrectly()
        {
            int focus = (int)Actions.Attack;
            ActionPanelUI actionPanel = new ActionPanelUI();
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
                "║   Pass        │",
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
            int focus = (int)Actions.Skills;
            ActionPanelUI actionPanel = new ActionPanelUI();
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
                "║   Pass        │",
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
            int focus = (int)Actions.Attack;
            ActionPanelUI actionPanel = new ActionPanelUI
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
                "║   Pass           │",
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
