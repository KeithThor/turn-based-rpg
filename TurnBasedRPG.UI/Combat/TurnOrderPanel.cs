using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI.Combat
{
    // Handles the rendering of turn order boxes
    public class TurnOrderPanel
    {
        private int _maxWidth;
        public int MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value - (value - 1) % 3; }
        }

        public TurnOrderPanel()
        {
            MaxWidth = 51;
        }

        // Renders the turn order boxes at the top right of the screen. This gets rendered with the target details ui.
        public IReadOnlyList<string> Render(bool renderTargets, IReadOnlyList<int> targetPositions, IReadOnlyList<IDisplayCharacter>[] characters)
        {
            var turnOrder = RenderTurnOrderBoxes(characters);
            turnOrder.Add(RenderFocusTargets(renderTargets, targetPositions, characters));
            return turnOrder;
        }

        private List<string> RenderTurnOrderBoxes(IReadOnlyList<IDisplayCharacter>[] characters)
        {
            var turnOrderParts = new List<string>();
            var turnOrderLabel = new StringBuilder();
            var topTurnOrder = new StringBuilder();
            var middleTurnOrder = new StringBuilder();
            var bottomTurnOrder = new StringBuilder();
            var currentTurnOrder = characters[0];
            var nextTurnOrder = characters[1];

            turnOrderLabel.Append($"Current Turn: {currentTurnOrder[0].GetName()} ({currentTurnOrder[0].GetSymbol()})");

            int limit = GetMaxBoxes(characters);

            for (int i = 0; i < limit; i++)
            {
                // Highlight current turn of the round
                if (i == 0)
                {
                    topTurnOrder.Append("╔═╗");
                    middleTurnOrder.Append($"║{currentTurnOrder[i].GetSymbol()}║");
                    bottomTurnOrder.Append("╚═╝");
                }
                // Render the next round's turn order
                else if (i >= currentTurnOrder.Count)
                {
                    // If at the end of current round, create barrier to signal end of round
                    if (i == currentTurnOrder.Count)
                    {
                        topTurnOrder.Append("║");
                        middleTurnOrder.Append("║");
                        bottomTurnOrder.Append("║");
                    }

                    int offset = i - currentTurnOrder.Count;
                    topTurnOrder.Append("┌─┐");
                    middleTurnOrder.Append($"│{nextTurnOrder[offset].GetSymbol()}│");
                    bottomTurnOrder.Append("└─┘");
                }
                // Render current round's turn order
                else
                {
                    topTurnOrder.Append("┌─┐");
                    middleTurnOrder.Append($"│{currentTurnOrder[i].GetSymbol()}│");
                    bottomTurnOrder.Append("└─┘");
                }
            }

            turnOrderParts.Add(turnOrderLabel.ToString());
            turnOrderParts.Add(topTurnOrder.ToString());
            turnOrderParts.Add(middleTurnOrder.ToString());
            turnOrderParts.Add(bottomTurnOrder.ToString());
            return turnOrderParts;
        }

        // Prints target triangles under a character's turn order box if it is selected by the player in the formation panel
        private string RenderFocusTargets(bool renderTargets, IReadOnlyList<int> targetPositions, IReadOnlyList<IDisplayCharacter>[] characters)
        {
            var currentTurnOrder = characters[0];
            var nextTurnOrder = characters[1];
            // Spaces to skip in between each potential focus target triangle
            int skip = 2;
            if (renderTargets)
            {
                int limit = GetMaxBoxes(characters);
                int iterations = 1;
                // Offset 1 empty space to account for the start of the turn order box
                var sb = new StringBuilder(" ");
                for (int i = 0; i < currentTurnOrder.Count && iterations <= limit; i++)
                {
                    if (targetPositions.Contains(currentTurnOrder[i].GetPosition()))
                        sb.Append("▲");
                    else
                        sb.Append(" ");
                    sb.Append(' ', skip);
                    iterations++;
                }
                sb.Append(" ");
                for (int i = 0; i < nextTurnOrder.Count && iterations <= limit; i++)
                {
                    if (targetPositions.Contains(nextTurnOrder[i].GetPosition()))
                        sb.Append("▲");
                    else
                        sb.Append(" ");
                    sb.Append(' ', skip);
                    iterations++;
                }
                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        public int GetMaxBoxes(IReadOnlyList<IDisplayCharacter>[] characters)
        {
            var currentTurnOrder = characters[0];
            var nextTurnOrder = characters[1];
            return currentTurnOrder.Count + nextTurnOrder.Count < MaxWidth / 3 - 1 ?
                        currentTurnOrder.Count + nextTurnOrder.Count : MaxWidth / 3 - 1;
        }
    }
}
