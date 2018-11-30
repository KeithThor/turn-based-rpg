using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.Combat
{
    /// <summary>
    /// Class responsible for the combat targetting of actions.
    /// </summary>
    public static class CombatTargeter
    {
        private static readonly IEnumerable<int> TopRowValues = new int[] { 1, 2, 3 };
        private static readonly IEnumerable<int> MiddleRowValues = new int[] { 4, 5, 6 };
        private static readonly IEnumerable<int> BottomRowValues = new int[] { 7, 8, 9 };
        private static readonly IEnumerable<int> LeftColumnValues = new int[] { 1, 4, 7 };
        private static readonly IEnumerable<int> MiddleColumnValues = new int[] { 2, 5, 8 };
        private static readonly IEnumerable<int> RightColumnValues = new int[] { 3, 6, 9 };

        /// <summary>
        /// Returns a list of target positions affixed to the new target position, checking and removing
        /// positions that go out of bounds.
        /// </summary>
        /// <param name="defaultTargets">The targets positions to translate to a new position.</param>
        /// <param name="centerOfTargets">The center of the target positions to use as the base of translation.</param>
        /// <param name="canSwitchTargetPosition">Whether or not the targets can change. If not, returns the original target positions.</param>
        /// <param name="targetPosition">The center position to translate the target positions to.</param>
        /// <returns>A list of target positions translated to the new target position.</returns>
        public static IReadOnlyList<int> GetTranslatedTargetPositions(IReadOnlyList<int> defaultTargets,
                                                            int centerOfTargets,
                                                            bool canSwitchTargetPosition,
                                                            int targetPosition)
        {
            if (targetPosition > 18 || targetPosition < 1)
                throw new ArgumentOutOfRangeException("Target position is out of bounds.");

            // In case of static targeting actions, change the default position to a static one
            // Or in case of ai's turn
            if (!canSwitchTargetPosition)
            {
                return defaultTargets;
            }
            else
            {
                var correctedTargets = new List<int>(defaultTargets);

                // Take into account if the target is in the enemy's formation, if so, deduct targetPosition so that
                // vertical and horizontal movement can be accounted for
                int formationOffset = targetPosition / 10;
                targetPosition = targetPosition % 10 + formationOffset;

                // The change from the center of targets position to the translation target without taking into account
                // a change of formations
                int targetOffset = targetPosition - centerOfTargets;

                int verticalMovement = GetVerticalMovement(centerOfTargets, targetPosition);
                int horizontalMovement = targetOffset - (verticalMovement * 3);

                // Remove rows depending on vertical movement
                if (verticalMovement >= 1)
                    correctedTargets.RemoveAll(target => BottomRowValues.Contains(target));
                else if (verticalMovement <= -1)
                    correctedTargets.RemoveAll(target => TopRowValues.Contains(target));
                if (verticalMovement == -2 || verticalMovement == 2)
                    correctedTargets.RemoveAll(target => MiddleRowValues.Contains(target));

                // Remove columns depending on horizontal movement
                if (horizontalMovement >= 1)
                    correctedTargets.RemoveAll(target => RightColumnValues.Contains(target));
                else if (horizontalMovement <= -1)
                    correctedTargets.RemoveAll(target => LeftColumnValues.Contains(target));
                if (horizontalMovement == -2 || horizontalMovement == 2)
                    correctedTargets.RemoveAll(target => MiddleColumnValues.Contains(target));

                for (int i = 0; i < correctedTargets.Count; i++)
                {
                    correctedTargets[i] += targetOffset + formationOffset * 9;
                }

                return correctedTargets;
            }
        }

        /// <summary>
        /// Given a start and end position in a formation, without taking into account which side of the formation,
        /// returns the amount of vertical spaces you travel from moving to the end position from the start.
        /// </summary>
        /// <param name="startPosition">The starting position to calculate vertical movement from.</param>
        /// <param name="endPosition">The ending position to calculate vertical movement from.</param>
        /// <returns>The amount of vertical movement made moving from the start to end positions.</returns>
        private static int GetVerticalMovement(int startPosition, int endPosition)
        {
            if (TopRowValues.Contains(startPosition))
            {
                if (TopRowValues.Contains(endPosition)) return 0;
                else if (MiddleRowValues.Contains(endPosition)) return 1;
                else return 2;
            }
            else if (MiddleRowValues.Contains(startPosition))
            {
                if (TopRowValues.Contains(endPosition)) return -1;
                else if (MiddleRowValues.Contains(endPosition)) return 0;
                else return 1;
            }
            else
            {
                if (TopRowValues.Contains(endPosition)) return -2;
                else if (MiddleRowValues.Contains(endPosition)) return -1;
                else return 0;
            }
        }
    }
}
