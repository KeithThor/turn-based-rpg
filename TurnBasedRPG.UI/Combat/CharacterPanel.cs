using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI.Combat
{
    public class CharacterPanel
    {
        /// <summary>
        /// The minimum possible height of the formation panel.
        /// </summary>
        public const int Min_Height = 18;

        public int MaxWidth { get; set; }
        public int MinPrimaryBlockSize { get; set; }
        private int _maxHeight;
        public int MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                if (value < Min_Height) _maxHeight = Min_Height;
                else _maxHeight = value;
            }
        }

        public CharacterPanel()
        {
            MaxHeight = 31;
            MaxWidth = 40;
            MinPrimaryBlockSize = 6;
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
        public IReadOnlyList<string> Render(IDisplayCharacter character)
        {
            if (character == null) throw new NullReferenceException();
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
            characterDetails.AddRange(RenderCharacter(character));
            
            _cachedRender = characterDetails;
            return characterDetails;
        }

        /// <summary>
        /// Renders a character panel with a group of characters and a focused character target that will show
        /// more stats.
        /// </summary>
        /// <param name="characters">The list of characters to render to the character panel.</param>
        /// <param name="focusedTarget">The character that is focused and will show more stats.</param>
        /// <returns>A list of string containing the rendered character panel.</returns>
        public IReadOnlyList<string> Render(IReadOnlyList<IDisplayCharacter> characters, IDisplayCharacter focusedTarget)
        {
            if (characters.Count == 1) return Render(focusedTarget);

            // Prevent target from being rendered twice
            var modifiedList = new List<IDisplayCharacter>(characters);
            modifiedList.Remove(focusedTarget);

            var render = new List<string>();
            var otherCharacters = RenderMany(modifiedList);
            var targetDetails = RenderCharacter(focusedTarget);

            // Remove extra details from the focusedTarget render depending on how much space there is left to fit the max height
            int removeIndex = MaxHeight - otherCharacters.Count();
            targetDetails.RemoveRange(removeIndex, targetDetails.Count() - removeIndex);

            render.AddRange(targetDetails);
            render.AddRange(otherCharacters);

            return render;
        }

        /// <summary>
        /// Renders many small character blocks inside the panel.
        /// </summary>
        /// <param name="characters">A list containing the characters to render inside the panel.</param>
        /// <returns>A list containing the render.</returns>
        private List<string> RenderMany(IReadOnlyList<IDisplayCharacter> characters)
        {
            // 1 to account for top border, 5 for size of each block
            int maxBlocks = (MaxHeight - 1 - MinPrimaryBlockSize) / 5;
            int totalBlocks = (characters.Count() > maxBlocks) ? maxBlocks : characters.Count();

            var characterBlock = new List<string>();
            for (int i = 0; i < totalBlocks; i++)
            {
                characterBlock.Add("║" + new string('═', MaxWidth - 2) + "║");
                int headerLength = characters[i].GetName().Count() + 7;
                characterBlock.Add("║ " + characters[i].GetName() 
                                    + " (" + characters[i].GetSymbol() + ")" 
                                    + new string(' ', MaxWidth - headerLength) 
                                    + "║");
                characterBlock.Add("║" + new string('─', MaxWidth - 2) + "║");
                characterBlock.Add(GetStatDisplay("Health",
                                                    characters[i].GetCurrenthealth().ToString(),
                                                    characters[i].GetMaxHealth().ToString()));
                // Last iteration
                if (i == totalBlocks - 1)
                {
                    characterBlock.Add("╚" + new string('═', MaxWidth - 2) + "╝");
                }
                else
                {
                    characterBlock.Add("║" + new string('─', MaxWidth - 2) + "║");
                }
            }
            return characterBlock;
        }

        /// <summary>
        /// Renders a character's name and all it's stats wrapped in a panel.
        /// </summary>
        /// <param name="character">The character to render in the panel.</param>
        /// <returns>A list of strings containing the panel render.</returns>
        private List<string> RenderCharacter(IDisplayCharacter character)
        {
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

            return characterDetails;
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
