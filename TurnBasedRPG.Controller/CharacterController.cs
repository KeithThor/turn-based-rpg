using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Controller that handles the change of data within the character entity.
    /// </summary>
    public class CharacterController
    {
        /// <summary>
        /// Called whenever a character dies.
        /// </summary>
        public event EventHandler<CharactersDiedEventArgs> CharacterDied;

        public void SetCurrentHealth(Character character, int currentHealth) { character.CurrentHealth = currentHealth; }

        /// <summary>
        /// Changes a character's current health by an amount. A character's current health cannot excede his current max health.
        /// If a character's health reaches 0, the character died event is raised.
        /// </summary>
        /// <param name="character">The character whose current health is modified.</param>
        /// <param name="amount">The amount to change current health by. Use negative numbers to decrease health.</param>
        public void ModifyCurrentHealth(Character character, int amount)
        {
            character.CurrentHealth += amount;
            if (character.CurrentHealth <= 0)
            {
                character.CurrentHealth = 0;
                CharacterDied?.Invoke(this, new CharactersDiedEventArgs() { DyingCharacters = new List<Character>() { character } });
            }
            else if (character.CurrentHealth > character.CurrentMaxHealth)
            {
                character.CurrentHealth = character.CurrentMaxHealth;
            }
        }

        /// <summary>
        /// Changes a character's max health. If the optional bool parameter is true, will modify a character's max health permanently.
        /// </summary>
        /// <param name="character">The character whose max health should be modified.</param>
        /// <param name="amount">The amount that the max health should be modified by. Use negative numbers to decrease.</param>
        /// <param name="permanent">Determines whether this change should be permanent.</param>
        public void ModifyMaxHealth(Character character, int amount, bool permanent = false)
        {
            if(permanent)
            {
                character.MaxHealth += amount;
                character.CurrentMaxHealth += amount;
            }
            else
            {
                character.CurrentMaxHealth += amount;
            }
        }

        /// <summary>
        /// Changes a character's speed. If the optional bool parameter is true, will modify a character's speed permanently.
        /// </summary>
        /// <param name="character">The character whose speed should be modified.</param>
        /// <param name="amount">The amount that the speed should be modified by. Use negative numbers to decrease.</param>
        /// <param name="permanent">Determines whether this change should be permanent.</param>
        public void ModifySpeed(Character character, int amount, bool permanent = false)
        {
            if (permanent)
            {
                character.Stats.Speed += amount;
                character.CurrentStats.Speed += amount;
            }
            else
            {
                character.CurrentStats.Speed += amount;
            }
        }
    }
}
