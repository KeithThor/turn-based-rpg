using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared
{
    /// <summary>
    /// Performs calculations with percentages using only integer numbers.
    /// </summary>
    public static class PercentageCalculator
    {
        /// <summary>
        /// Gets the integer percentage of a number.
        /// </summary>
        /// <param name="percentage">The percentage to get of a number. 100 is 100% of a number.</param>
        /// <param name="number">The number to get a percentage of.</param>
        /// <returns>A number that represents the percentage of another number.</returns>
        public static int GetPercentOf(int percentage, int number)
        {
            return number * percentage / 100;
        }

        /// <summary>
        /// Given two numbers, calculates their differences in percentages.
        /// </summary>
        /// <param name="number1">The base number to calculate the percentage from.</param>
        /// <param name="number2">The number to determine the percentage of the base.</param>
        /// <returns>A number representing the percentage the base number is of the second number.</returns>
        public static int GetPercentage(int number1, int number2)
        {
            return number1 * 100 / number2;
        }
    }
}
