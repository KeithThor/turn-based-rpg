using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat;
using TurnBasedRPG.UI.Combat.Panels;
using TurnBasedRPG.UI.Test.Combat.Mocks;

namespace TurnBasedRPG.UI.Test.Combat
{
    [TestClass]
    public class FormationPanelTest
    {
        [TestMethod]
        public void Render_WithPositionsOneTwoFour_RendersSymbols()
        {
            var mockUIStateTracker = new MockUIStateTracker();
            var uiCharacterManager = new UICharacterManager
            {
                Characters = new List<DisplayCharacter>()
                {
                    new DisplayCharacter()
                    {
                        Id = 1,
                        Name = "",
                        Symbol = 'A',
                        Position = 1,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 2,
                        Name = "",
                        Symbol = 'B',
                        Position = 2,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 3,
                        Name = "",
                        Symbol = 'C',
                        Position = 4,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    }
                }
            };
            var formationPanel = new FormationPanel(mockUIStateTracker, uiCharacterManager);
            mockUIStateTracker.CurrentTargetPosition = 1;
            mockUIStateTracker.ActiveCharacterId = 0;
            mockUIStateTracker.IsInFormationPanel = false;

            var expected = new List<string>()
            {
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "          │■■■■■│        │■■■■■│                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │A│            │B│            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "          │■■■■■│                                                                                                      ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │C│            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       "
            };

            var actual = new List<string>(formationPanel.Render());

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Render_WithPositionsSixNineElevenFourteen_RendersSymbols()
        {
            var mockUIStateTracker = new MockUIStateTracker();
            var uiCharacterManager = new UICharacterManager
            {
                Characters = new List<DisplayCharacter>()
                {
                    new DisplayCharacter()
                    {
                        Id = 1,
                        Name = "",
                        Symbol = 'A',
                        Position = 6,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 2,
                        Name = "",
                        Symbol = 'B',
                        Position = 9,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 3,
                        Name = "",
                        Symbol = 'C',
                        Position = 11,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 4,
                        Name = "",
                        Symbol = 'D',
                        Position = 14,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    }
                }
            };
            var formationPanel = new FormationPanel(mockUIStateTracker, uiCharacterManager);
            mockUIStateTracker.CurrentTargetPosition = 1;
            mockUIStateTracker.ActiveCharacterId = 0;
            mockUIStateTracker.IsInFormationPanel = false;

            var expected = new List<string>()
            {
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                        │■■■■■│                        ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │C│            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                        │■■■■■│                                         │■■■■■│                        ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │A│                              │ │            │D│            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                        │■■■■■│                                                                        ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │B│                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       "
            };

            var actual = new List<string>(formationPanel.Render());

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Render_WithPositionsTwoFive_RendersNames()
        {
            var mockUIStateTracker = new MockUIStateTracker();
            var uiCharacterManager = new UICharacterManager
            {
                Characters = new List<DisplayCharacter>()
                {
                    new DisplayCharacter()
                    {
                        Id = 1,
                        Name = "Anderson",
                        Symbol = 'A',
                        Position = 2,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 2,
                        Name = "Brock",
                        Symbol = 'B',
                        Position = 5,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    }
                }
            };
            var formationPanel = new FormationPanel(mockUIStateTracker, uiCharacterManager);
            mockUIStateTracker.CurrentTargetPosition = 1;
            mockUIStateTracker.ActiveCharacterId = 0;
            mockUIStateTracker.IsInFormationPanel = false;

            var expected = new List<string>()
            {
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                         Anderson                                                                                      ",
                "                         │■■■■■│                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │A│            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                          Brock                                                                                        ",
                "                         │■■■■■│                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │B│            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       "
            };

            var actual = new List<string>(formationPanel.Render());

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Render_WithPositionsTwoFiveAndDamaged_RendersHealthbars()
        {
            var mockUIStateTracker = new MockUIStateTracker();
            var uiCharacterManager = new UICharacterManager
            {
                Characters = new List<DisplayCharacter>()
                {
                    new DisplayCharacter()
                    {
                        Id = 1,
                        Name = "",
                        Symbol = 'A',
                        Position = 2,
                        CurrentHealth = 90,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 2,
                        Name = "",
                        Symbol = 'B',
                        Position = 5,
                        CurrentHealth = 15,
                        MaxHealth = 100
                    }
                }
            };
            var formationPanel = new FormationPanel(mockUIStateTracker, uiCharacterManager);
            mockUIStateTracker.CurrentTargetPosition = 1;
            mockUIStateTracker.ActiveCharacterId = 0;
            mockUIStateTracker.IsInFormationPanel = false;

            var expected = new List<string>()
            {
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                         │■■■■ │                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │A│            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                         │■    │                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │B│            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       "
            };

            var actual = new List<string>(formationPanel.Render());

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Render_WithPositionsTwoFiveAndDead_RendersDead()
        {
            var mockUIStateTracker = new MockUIStateTracker();
            var uiCharacterManager = new UICharacterManager
            {
                Characters = new List<DisplayCharacter>()
                {
                    new DisplayCharacter()
                    {
                        Id = 1,
                        Name = "",
                        Symbol = 'A',
                        Position = 2,
                        CurrentHealth = 0,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 2,
                        Name = "",
                        Symbol = 'B',
                        Position = 5,
                        CurrentHealth = 0,
                        MaxHealth = 100
                    }
                }
            };
            var formationPanel = new FormationPanel(mockUIStateTracker, uiCharacterManager);
            mockUIStateTracker.CurrentTargetPosition = 1;
            mockUIStateTracker.ActiveCharacterId = 0;
            mockUIStateTracker.IsInFormationPanel = false;

            var expected = new List<string>()
            {
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                         │     │                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            \\ /            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │             A             │ │                              │ │            │ │            │ │           ",
                "            └─┘            / \\            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                         │     │                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            \\ /            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │             B             │ │                              │ │            │ │            │ │           ",
                "            └─┘            / \\            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       "
            };

            var actual = new List<string>(formationPanel.Render());

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Render_WithPositionsTwoFiveAndActiveId_RendersActiveBox()
        {
            var mockUIStateTracker = new MockUIStateTracker();
            var uiCharacterManager = new UICharacterManager
            {
                Characters = new List<DisplayCharacter>()
                {
                    new DisplayCharacter()
                    {
                        Id = 1,
                        Name = "",
                        Symbol = 'A',
                        Position = 2,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    },
                    new DisplayCharacter()
                    {
                        Id = 2,
                        Name = "",
                        Symbol = 'B',
                        Position = 5,
                        CurrentHealth = 100,
                        MaxHealth = 100
                    }
                }
            };
            var formationPanel = new FormationPanel(mockUIStateTracker, uiCharacterManager);
            mockUIStateTracker.CurrentTargetPosition = 1;
            mockUIStateTracker.ActiveCharacterId = 1;
            mockUIStateTracker.IsInFormationPanel = false;

            var expected = new List<string>()
            {
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                         │■■■■■│                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ╔═╗            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            ║A║            │ │                              │ │            │ │            │ │           ",
                "            └─┘            ╚═╝            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                         │■■■■■│                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │B│            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       "
            };

            var actual = new List<string>(formationPanel.Render());

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Render_WithTargets_RendersTargets()
        {
            var mockUIStateTracker = new MockUIStateTracker()
            {
                CurrentTargetPositions = new List<int>() { 2, 4, 5, 6, 8 },
                CurrentTargetPosition = 1
            };
            var uiCharacterManager = new UICharacterManager();
            var formationPanel = new FormationPanel(mockUIStateTracker, uiCharacterManager);
            formationPanel.IsActive = true;

            var expected = new List<string>()
            {
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                            ▲                                                                                          ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "             ▲              ▲              ▲                                                                           ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "            ┌─┐            ┌─┐            ┌─┐                              ┌─┐            ┌─┐            ┌─┐           ",
                "            │ │            │ │            │ │                              │ │            │ │            │ │           ",
                "            └─┘            └─┘            └─┘                              └─┘            └─┘            └─┘           ",
                "                            ▲                                                                                          ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       ",
                "                                                                                                                       "
            };

            var actual = new List<string>(formationPanel.Render());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
