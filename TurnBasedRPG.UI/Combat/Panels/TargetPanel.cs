using System.Collections.Generic;
using System.Linq;
using System.Text;
using TurnBasedRPG.Controller.Combat.Interfaces;
using TurnBasedRPG.Shared.Interfaces;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// UI component that is responsible for rendering a target's name and
    /// a health bar that shows a target's current health.
    /// </summary>
    public class TargetPanel : ITargetPanel
    {
        private int _maxWidth = 42;
        /// <summary>
        /// Gets or sets the max width of the component. MaxWidth will always be 2 units larger than
        /// the width of the healthbar.
        /// </summary>
        public int MaxWidth
        {
            get { return _maxWidth; }
            set {
                WidthOfHealthBar = value - 2;
                _maxWidth = value;
            }
        }
        private int _widthOfHealthBar;
        /// <summary>
        /// Gets or sets the max width of the healthbar. MaxWidth will always be 2 units smaller than
        /// the max width of the component.
        /// </summary>
        public int WidthOfHealthBar
        {
            get { return _widthOfHealthBar; }
            set
            {
                _maxWidth = value + 2;
                _widthOfHealthBar = value;
            }
        }
        public int MaxDetailsLength { get; set; } = 32;
        public int MaxHeight { get; set; }

        public TargetPanel(IUIStateTracker defaultsHandler,
                           IUICharacterManager uiCharacterManager,
                           IDisplayCombatState combatStateHandler)
        {
            _widthOfHealthBar = _maxWidth - 2;
            _defaultsHandler = defaultsHandler;
            _uiCharacterManager = uiCharacterManager;
            _combatStateHandler = combatStateHandler;
        }

        private IReadOnlyList<string> _cachedRender;
        private CachedData _cachedData;
        private readonly IUIStateTracker _defaultsHandler;
        private readonly IUICharacterManager _uiCharacterManager;
        private readonly IDisplayCombatState _combatStateHandler;

        private class CachedData
        {
            public int Id;
            public int CurrentHealth;
            public int MaxHealth;
        }

        /// <summary>
        /// Creates and returns a list of string that represents the targetUI component with a target's
        /// details and a healthbar.
        /// </summary>
        /// <returns>A read-only list of string that represents the targetUI component.</returns>
        public IReadOnlyList<string> Render()
        {
            var target = GetTarget();
            if (target == null) throw new System.NullReferenceException("No target to render in target panel!");

            if (IsCacheData(target)) return _cachedRender;
            else
            {
                _cachedData = new CachedData()
                {
                    Id = target.Id,
                    CurrentHealth = target.CurrentHealth,
                    MaxHealth = target.MaxHealth
                };
            }

            var targetDetails = new List<string>();
            // Calculate healthbar string
            int healthBars = target.CurrentHealth * 100 / target.MaxHealth * WidthOfHealthBar / 100;
            if (healthBars > WidthOfHealthBar) healthBars = WidthOfHealthBar;
            if (healthBars < 0) healthBars = 0;
            StringBuilder healthString = new StringBuilder("│");
            healthString.Append('█', healthBars);
            healthString.Append(' ', WidthOfHealthBar - healthBars);
            healthString.Append("│");

            // Calculate enemy name and health numbers
            StringBuilder enemyDetails = new StringBuilder("  ");
            enemyDetails.Append(target.Name + " (" + target.Symbol + ")");
            enemyDetails.Append(' ', MaxDetailsLength - target.Name.Length - 
                                        target.MaxHealth.ToString().Length - target.CurrentHealth.ToString().Length);
            enemyDetails.Append("HP " + target.CurrentHealth.ToString() + "/" + target.MaxHealth.ToString());
            
            targetDetails.Add(enemyDetails.ToString());
            targetDetails.Add("┌" + new string('─', WidthOfHealthBar) + "┐");
            targetDetails.Add(healthString.ToString());
            targetDetails.Add("└" + new string('─', WidthOfHealthBar) + "┘");

            _cachedRender = targetDetails;
            return targetDetails;
        }

        /// <summary>
        /// Checks whether or not the character passed in contains the same values as the cached data.
        /// </summary>
        /// <param name="character">The character whose values to check.</param>
        /// <returns>Returns whether or not the character contains the same values as the cached data.</returns>
        private bool IsCacheData(IDisplayCharacter character)
        {
            if (_cachedData == null) return false;
            if (character.Id != _cachedData.Id) return false;
            if (character.CurrentHealth != _cachedData.CurrentHealth) return false;
            if (character.MaxHealth != _cachedData.MaxHealth) return false;

            return true;
        }

        /// <summary>
        /// Gets the target to render in the target panel depending on which panel is active and where the target position is.
        /// </summary>
        /// <returns></returns>
        private IDisplayCharacter GetTarget()
        {
            if (_defaultsHandler.IsInFormationPanel || !_combatStateHandler.IsPlayerTurn())
            {
                IDisplayCharacter renderTarget = null;
                // If there is a character in the player's default target position, render that target's details
                if (_uiCharacterManager.Characters.Any(chr => chr.Position == _defaultsHandler.CurrentTargetPosition))
                    renderTarget = _uiCharacterManager.GetCharacterFromPosition(_defaultsHandler.CurrentTargetPosition);
                // Finds any character that is in the player's target list and render that target's details
                else
                {
                    var targets = _defaultsHandler.CurrentTargetPositions;

                    renderTarget = _uiCharacterManager.Characters.FirstOrDefault(chr => targets.Contains(chr.Position));
                }
                // If there are no characters that occupy the positions the player is targeting, render the active character's details
                if (renderTarget == null) renderTarget = _uiCharacterManager.GetCharacterFromId(_defaultsHandler.ActiveCharacterId);

                return renderTarget;
            }
            else
                return _uiCharacterManager.GetCharacterFromId(_defaultsHandler.ActiveCharacterId);
        }
    }
}
