using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI.Combat
{
    public class CharacterDetailsPanel
    {
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public CharacterDetailsPanel()
        {
            MaxWidth = 55;
            MaxHeight = 16;
        }
        
        private struct CachedCharacter
        {
            public int Id;
            public int CurrentHealth;
            public int MaxHealth;
        }

        private CachedCharacter _cachedCharacter;
        private IReadOnlyList<string> _cachedRender;

        /// <summary>
        /// Returns a details panel injected with the data from a character.
        /// </summary>
        /// <param name="character">The character whose data will be used to fill the details panel.</param>
        /// <returns>A read-only list of string that contains the details panel.</returns>
        public IReadOnlyList<string> RenderCharacterDetails(IDisplayCharacter character)
        {
            if (character == null) return RenderBlankPanel();
            if (IsCachedData(character)) return _cachedRender;
            else
            {
                _cachedCharacter = new CachedCharacter()
                {
                    Id = character.GetId(),
                    CurrentHealth = character.GetCurrenthealth(),
                    MaxHealth = character.GetMaxHealth()
                };
            }

            var characterDetails = new List<string>();
            characterDetails.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            int headerLength = character.GetName().Count() + 7;
            characterDetails.Add("║ " + character.GetName() + " (" + character.GetSymbol() + ")" + new string(' ', MaxWidth - headerLength) + "║");
            characterDetails.Add("║" + new string('─', MaxWidth - 2) + "║");
            characterDetails.Add(GetStatDisplay("Health",
                                                character.GetCurrenthealth().ToString(),
                                                character.GetMaxHealth().ToString()));

            // Fill empty spaces 
            for (int i = 0; i < MaxHeight - 5; i++)
            {
                characterDetails.Add("║" + new string(' ', MaxWidth - 2) + "║");
            }
            characterDetails.Add("╚" + new string('═', MaxWidth - 2) + "╝");

            _cachedRender = characterDetails;
            return characterDetails;
        }

        /// <summary>
        /// In case of null objects, renders a panel with no data.
        /// </summary>
        /// <returns>A panel with no data.</returns>
        private List<string> RenderBlankPanel()
        {
            var detailsPanel = new List<string>();
            detailsPanel.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            for (int i = 0; i < MaxHeight - 2; i++)
            {
                detailsPanel.Add("║ " + new string(' ', MaxWidth - 3) + "║");
            }
            detailsPanel.Add("╚" + new string('═', MaxWidth - 2) + "╝");
            return detailsPanel;
        }

        /// <summary>
        /// Returns a string containing the display details for a single stat.
        /// </summary>
        /// <param name="statName">The name of the stat to display.</param>
        /// <param name="statAmount1">The amount of the stat.</param>
        /// <param name="statAmount2">If the stat is modified, this is the modified stat amount.</param>
        /// <returns>Contains the display details for a single stat.</returns>
        private string GetStatDisplay(string statName, string statAmount1, string statAmount2 = "")
        {
            int length = statName.Count() + statAmount1.Count() + statAmount2.Count() + 6;
            if (statAmount2.Count() == 0)
                return "║ " + statName + ": " + statAmount1 + new string(' ', MaxWidth - length + 1) + "║";
            else
                return "║ " + statName + ": " + statAmount1 + "/" + statAmount2 + new string(' ', MaxWidth - length) + "║";
        }

        /// <summary>
        /// Returns whether or not the stats of the character being passed in is the same as the cached data.
        /// </summary>
        /// <param name="character">The character to check with the cached data.</param>
        /// <returns>Returns whether or not the stats of the character being passed in is the same as the cached data.</returns>
        private bool IsCachedData(IDisplayCharacter character)
        {
            if (character.GetId() != _cachedCharacter.Id) return false;
            if (character.GetCurrenthealth() != _cachedCharacter.CurrentHealth) return false;
            if (character.GetMaxHealth() != _cachedCharacter.MaxHealth) return false;

            return true;
        }
    }
}
