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

        /// <summary>
        /// Constructs and returns a message whenever a character has its health changed by a status effect.
        /// </summary>
        /// <param name="statusNames">The names of the status effect affecting the character.</param>
        /// <param name="characterName">The name of the character affected by the status effect.</param>
        /// <param name="healthChange">The amount of health the status effect changed.</param>
        /// <returns>A string containing the message that a character had its health changed by a status effect.</returns>
        public static string GetHealthChangedByStatusMessage(IReadOnlyList<string> statusNames, string characterName, int healthChange)
        {
            string message = $"{characterName}({healthChange}) is affected by ";
            if (statusNames.Count() == 1)
            {
                return message + $"{statusNames[0]}.";
            }
            else if (statusNames.Count() == 2)
            {
                return message + $"{statusNames[0]} and {statusNames[1]}.";
            }
            else
            {
                for (int i = 0; i < statusNames.Count(); i++)
                {
                    if (i == statusNames.Count() - 1)
                        message += $"and {statusNames[i]}.";
                    else
                        message += $"{statusNames[i]}, ";
                }
                return message;
            }
        }

        /// <summary>
        /// Constructs and returns a message whenever a character loses the effects of a status effect.
        /// </summary>
        /// <param name="statusNames">The names of the status effects the character lost.</param>
        /// <param name="characterName">The name of the character losing the status effects.</param>
        /// <returns>A string containing a message of whenever a character loses the effects of a status effect.</returns>
        public static string GetRemoveStatusMessage(IReadOnlyList<string> statusNames, string characterName)
        {
            string message = $"{characterName} is no longer affected by ";
            if (statusNames.Count() == 1)
            {
                return message + $"{statusNames[0]}.";
            }
            else if (statusNames.Count() == 2)
            {
                return message + $"{statusNames[0]} and {statusNames[1]}.";
            }
            else
            {
                for (int i = 0; i < statusNames.Count(); i++)
                {
                    if (i == statusNames.Count() - 1)
                        message += $"and {statusNames[i]}.";
                    else
                        message += $"{statusNames[i]}, ";
                }
                return message;
            }
        }

        /// <summary>
        /// Constructs and returns a message whenever a character is preparing a delayed action.
        /// </summary>
        /// <param name="channelerName">The name of the character preparing the delayed action.</param>
        /// <param name="actionName">The name of the action being channeled.</param>
        /// <returns>A message containing the channeler's name and the action it is channeling.</returns>
        public static string GetBeginChannelMessage(string channelerName, string actionName)
        {
            return $"{channelerName} prepares to use {actionName}.";
        }

        /// <summary>
        /// Constructs and returns a message whenever a character waits for it's turn.
        /// </summary>
        /// <param name="characterName">The name of the character waiting.</param>
        /// <returns></returns>
        public static string GetBeginWaitMessage(string characterName)
        {
            return $"{characterName} is waiting until later to act.";
        }
    }
}
