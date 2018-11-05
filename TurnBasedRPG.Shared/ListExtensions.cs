using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared
{
    public static class ListExtensions
    {
        /// <summary>
        /// Horribly inefficient way to get a median value from a list of items without altering the original list.
        /// </summary>
        /// <typeparam name="T">The type of the median value to get.</typeparam>
        /// <param name="list">Extends the IList interface.</param>
        /// <returns>Returns the median value.</returns>
        public static T GetMedian<T>(this IList<T> list) where T: IComparable<T>
        {
            if (list.Count == 0) return default(T);
            var copy = new List<T>(list);
            copy.Sort((item1, item2) => item2.CompareTo(item1));

            int medianIndex = copy.Count() / 2;
            if (copy.Count() % 2 == 0) medianIndex--;

            return copy[medianIndex];
        }
    }
}
