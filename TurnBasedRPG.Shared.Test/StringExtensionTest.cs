using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.Test
{
    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        public void GetStringAsList_WithPeriod_ReturnsPeriodAndWordTogether()
        {
            string test = "This is a string.";
            var expected = new List<string>() { "This is a ", "string." };

            var actual = new List<string>(test.GetStringAsList(16));

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetStringAsList_WithLongWord_ReturnsMultipleLines()
        {
            string test = "This is a stringstring";
            var expected = new List<string>() { "This is a ", "stringstring" };

            var actual = new List<string>(test.GetStringAsList(12));

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetStringAsList_WithLongSentence_ReturnsMultipleLines()
        {
            string test = "This is a very super long and original sentence.";
            var expected = new List<string>() { "This is a ", "very super", "long and ", "original ", "sentence." };

            var actual = new List<string>(test.GetStringAsList(10));

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetStringAsList_WithShortSentence_ReturnsOriginal()
        {
            string test = "This is short.";
            var expected = new List<string>() { "This is short." };

            var actual = new List<string>(test.GetStringAsList(20));

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
