using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.AI
{
    public static class AITargets
    {
        /// <summary>
        /// Gets a new list of target positions for an action for use with the AI.
        /// </summary>
        /// <param name="action">The action to generate a new list of target positions for.</param>
        /// <returns>A list of integers containing the AI modified positions.</returns>
        public static IReadOnlyList<int> GetModifiedTargets(ActionBase action)
        {
            var modifiedTargets = new List<int>();

            foreach (var position in action.TargetPositions)
            {
                modifiedTargets.Add(19 - position);
            }
            return modifiedTargets;
        }

        /// <summary>
        /// Gets the center of targets position for an action for use with the AI.
        /// </summary>
        /// <param name="centerOfTargetsPosition">The original center of targets position for an action.</param>
        /// <returns>The AI modified center of targets position.</returns>
        public static int GetModifiedCenter(int centerOfTargetsPosition)
        {
            return 19 - centerOfTargetsPosition;
        }

        /// <summary>
        /// Given an ActionBase, it's modified targets and center of targets, and a selection position, gets the positions that
        /// action will target, removing positions that go out of bounds.
        /// </summary>
        /// <param name="targets">A list containing the target positions modified for AI usage.</param>
        /// <param name="centerOfTargets">A number representing the center of the target positions modified for AI usage.</param>
        /// <param name="selectionPosition">A number representing the position the action targets.</param>
        /// <param name="action">The action to check the targets for.</param>
        /// <returns>A list of integers containing the AI modified selection of targets. May return a list with no items in the case an action hits nothing.</returns>
        public static IReadOnlyList<int> GetModifiedSelection(ActionBase action, int selectionPosition)
        {
            var targets = GetModifiedTargets(action);
            var centerOfTargets = GetModifiedCenter(action.CenterOfTargetsPosition);
            var selectedPositions = new List<int>(targets);

            int offset = (selectionPosition > 9) ? 9 : 0;

            // If the center of target for the action is in a different column than the current selection's column
            if (centerOfTargets % 3 != selectionPosition % 3)
            {
                // If the player's center of target is in the right column and the center of target for an action is not the 
                // right column, remove all targets in the right column
                if (selectionPosition % 3 == 0 && centerOfTargets % 3 != 0)
                    selectedPositions.RemoveAll(val => val % 3 == 0);
                // Target position is in the left column and the action's CoT not in left column,
                // all targets in the left column must be culled
                if (selectionPosition % 3 == 1 && centerOfTargets % 3 != 1)
                    selectedPositions.RemoveAll(val => val % 3 == 1);

                // If action's CoT is in left column and the selection is right column, remove center column
                if (centerOfTargets % 3 == 1 && selectionPosition % 3 == 0)
                    selectedPositions.RemoveAll(val => val % 3 == 2);
                // If action's CoT is in left column and the selection is middle column, remove right column
                if (centerOfTargets % 3 == 1 && selectionPosition % 3 == 2)
                    selectedPositions.RemoveAll(val => val % 3 == 0);
                // If action's CoT is in right column and the selection is middle column, remove left column
                if (centerOfTargets % 3 == 0 && selectionPosition % 3 == 2)
                    selectedPositions.RemoveAll(val => val % 3 == 1);
                // If action's CoT is in right column and the selection is left column, remove center column
                if (centerOfTargets % 3 == 0 && selectionPosition % 3 == 1)
                    selectedPositions.RemoveAll(val => val % 3 == 2);
            }
            for (int i = 0; i < selectedPositions.Count(); i++)
            {
                selectedPositions[i] += selectionPosition - centerOfTargets;
            }
            selectedPositions.RemoveAll(position => position > 9 + offset || position < 1 + offset);
            
            return selectedPositions;
        }
    }
}
