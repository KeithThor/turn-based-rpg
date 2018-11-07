using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared
{
    public static class MathExtensions
    {
        /// <summary>
        /// Horribly inefficient way to get a median value from a list of items without altering the original list.
        /// </summary>
        /// <typeparam name="T">The type of the median value to get.</typeparam>
        /// <param name="list">Extends the IList interface.</param>
        /// <returns>Returns the median value.</returns>
        public static int GetMedian(IList<int> list)
        {
            if (list.Count == 0) return 0;
            var copy = new List<int>(list);
            copy.Sort((item1, item2) => item2.CompareTo(item1));

            int medianIndex = copy.Count() / 2;
            if (copy.Count() % 2 == 0)
            {
                return (copy[medianIndex - 1] + copy[medianIndex]) / 2;
            }

            return copy[medianIndex];
        }
    }
}
