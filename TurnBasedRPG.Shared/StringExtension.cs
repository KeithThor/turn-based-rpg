using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared
{
    public static class StringExtension
    {
        /// <summary>
        /// Returns a read-only list of string from an origin string, split up by a max length.
        /// Will avoid splitting words in half.
        /// </summary>
        /// <param name="str">The string to split up.</param>
        /// <param name="maxLength">The maximum length of each string. Strings may be shorter to avoid splitting.</param>
        /// <returns>A read-only list containing slices of the origin string.</returns>
        public static IReadOnlyList<string> GetStringAsList(this string str, int maxLength)
        {
            var reducedLengthList = new List<string>();
            if (str[0] == ' ') str = str.Remove(0, 1);
            int iterations = 0;
            int startIndex = 0;
            int totalListLength = 0;
            for (int i = 0; str.Count() - 1 != startIndex; i++)
            {
                string reducedStr = "";
                // If the unrendered parts of the category description has more characters than fits in the next line
                if (str.Count() > maxLength + totalListLength)
                {
                    reducedStr = str.Substring(startIndex, maxLength);
                    int tempIndex = 0;
                    // If the line ends in a period or space
                    if (reducedStr.Last() == '.' || reducedStr.Last() == ' ')
                    {
                        tempIndex = maxLength + startIndex;
                    }
                    // If the next line starts in a space
                    else if (str.Count() > startIndex + maxLength && 
                            str[startIndex + maxLength] == ' ')
                    {
                        tempIndex = maxLength + startIndex;
                    }
                    else
                    {
                        tempIndex = reducedStr.LastIndexOfAny(new char[] { '.', ' ' }) + startIndex + 1;
                    }
                    reducedStr = str.Substring(startIndex, tempIndex - startIndex);
                    startIndex = tempIndex;
                }
                // Unrendered parts of the category description less than the maximum width of 1 line
                else
                {
                    reducedStr = str.Substring(startIndex);
                    startIndex = str.Count() - 1;
                }
                // Remove spaces at the beginning of the new line
                if (reducedStr.Count() > 0 && reducedStr[0] == ' ') reducedStr = reducedStr.Remove(0, 1);
                // Add spaces to the end of the description line if there are extra spaces left
                reducedLengthList.Add(reducedStr);
                totalListLength += reducedStr.Length;
                iterations++;
            }
            return reducedLengthList;
        }
    }
}
