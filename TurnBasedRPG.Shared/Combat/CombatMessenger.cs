using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.Combat
{
    /// <summary>
    /// Static class responsible for creating messages for any combat events that needs to be combat logged.
    /// </summary>
    public static class CombatMessenger
    {
        /// <summary>
        /// Constructs and returns a message containing the list of characters who died.
        /// </summary>
        /// <param name="characterNames">The names of the characters who died.</param>
        /// <returns>A message containing the list of characters who died.</returns>
        public static string GetCharactersDiedMessage(IReadOnlyList<string> characterNames)
        {
            string message = "";

            if (characterNames.Count() == 1) message = $"{characterNames[0]} died.";
            else if (characterNames.Count() == 2) message = $"{characterNames[0]} and {characterNames[1]} died.";
            else if (characterNames.Count() > 2)
            {
                for (int i = 0; i < characterNames.Count(); i++)
                {
                    if (i != characterNames.Count - 1)
                    {
                        message += characterNames[i] + ", ";
                    }
                    else
                    {
                        message += "and " + characterNames[i] + " have died.";
                    }
                }
            }
            return message;
        }

        /// <summary>
        /// Constructs and returns a message containing an acting character's name, the action it used, and the affected
        /// characters as well as the amount of health they gained or lost.
        /// </summary>
        /// <param name="actorName">The name of the actor.</param>
        /// <param name="actionName">The name of the action used by the actor.</param>
        /// <param name="affectedCharacters">The name of the characters and the health the characters lost.</param>
        /// <returns>A string resembling the combat message for a health changed event.</returns>
        public static string GetHealthChangedMessage(string actorName, 
                                                     string actionName,
                                                     IReadOnlyList<KeyValuePair<string, int>> affectedCharacters)
        {
            string message = $"{actorName} used {actionName} on ";

            if (affectedCharacters.Count() == 1)
            {
                var pair = affectedCharacters[0];
                message += $"{pair.Key}({pair.Value}).";
            }
            else if (affectedCharacters.Count() == 2)
            {
                var pairOne = affectedCharacters[0];
                var pairTwo = affectedCharacters[1];
                message += $"{pairOne.Key}({pairOne.Value}) and {pairTwo.Key}({pairTwo.Value}).";
            }
            else if (affectedCharacters.Count() > 2)
            {
                for (int i = 0; i < affectedCharacters.Count(); i++)
                {
                    var pair = affectedCharacters[i];
                    if (i != affectedCharacters.Count - 1)
                    {
                        
                        message += $"{pair.Key}({pair.Value}), ";
                    }
                    else
                    {
                        message += $"and {pair.Key}({pair.Value}).";
                    }
                }
            }

            return message;
        }

        /// <summary>
        /// Constructs and returns a message whenever a character is affected by a status.
        /// </summary>
        /// <param name="statusName">The name of the status effect being applied to the character.</param>
        /// <param name="characterName">The name of the character the status effect is being applied to.</param>
        /// <returns>A string to display a message of a status effect being applied to a character.</returns>
        public static string GetAffectedByStatusMessage(string statusName, string characterName)
        {
            return $"{characterName} is now affected by {statusName}.";
        }

        /// <summary>
        /// Constructs and returns a message whenever a group of characters are affected by a status.
        /// </summary>
        /// <param name="statusName">The name of the status effect being applied to the characters.</param>
        /// <param name="characterNames">The names of the characters the status effect is being applied to.</param>
        /// <returns>A string to display a message of a status effect being applied to a group of characters.</returns>
        public static string GetAffectedByStatusMessage(string statusName, IReadOnlyList<string> characterNames)
        {
            if (characterNames.Count() == 1) return GetAffectedByStatusMessage(statusName, characterNames[0]);
            else if (characterNames.Count() == 2) 
            {
                return $"{characterNames[0]} and {characterNames[1]} are now affected by {statusName}.";
            }
            else if (characterNames.Count() > 2)
            {
                string message = "";
                for (int i = 0; i < characterNames.Count(); i++)
                {
                    if (i != characterNames.Count - 1)
                    {

                        message += $"{characterNames[i]}, ";
                    }
                    else
                    {
                        message += $"and {characterNames[i]} are now affected by {statusName}.";
                    }
                }
                return message;
            }
            return "";
        }
    }
}
