using System;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Collections2.Tests
{
    [TestFixture]
    public class HybridReadOnlyArrayDictionaryTests
    {
        [Test]
        public void Returns_items_from_empty_dictionary()
        {
            var orig = CreateDictionary(0);

            // act
            var d = new HybridReadOnlyArrayDictionary<int, int>(orig);

            // assert
            TestDictionary(d, 0);
        }

        [Test]
        public void Returns_items_from_small_dictionary()
        {
            var orig = CreateDictionary(3);

            // act
            var d = new HybridReadOnlyArrayDictionary<int, int>(orig);

            // assert
            TestDictionary(d, 3);
        }

        [Test]
        public void Returns_items_from_medium_dictionary()
        {
            var orig = CreateDictionary(6);

            // act
            var d = new HybridReadOnlyArrayDictionary<int, int>(orig);

            // assert
            TestDictionary(d, 6);
        }

        [Test]
        public void Returns_items_from_large_dictionary()
        {
            var orig = CreateDictionary(16);

            // act
            var d = new HybridReadOnlyArrayDictionary<int, int>(orig);

            // assert
            TestDictionary(d, 16);
        }

        #region Helper Methods

        private static IDictionary<int, int> CreateDictionary(int numItems)
        {
            Dictionary<int, int> orig = new Dictionary<int, int>(numItems);
            for (int i = 0; i < numItems; i++)
            {
                orig.Add(i, i);
            }

            return orig;
        }

        private static void TestDictionary(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            TestCount(d, numItems);
            TestKeys(d, numItems);
            TestValues(d, numItems);
            TestItemIndexer(d, numItems);
            TestContainsKey(d, numItems);
            TestTryGetValue(d, numItems);
            TestGetEnumerator(d, numItems);
            TestGetEnumeratorExplicit(d, numItems);
        }

        private static void TestGetEnumeratorExplicit(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            IEnumerable e = d;
            IEnumerator enumerator = e.GetEnumerator();

            int[] keys = new int[numItems];
            int[] values = new int[numItems];

            int i = 0;
            while (enumerator.MoveNext())
            {
                keys[i] = ((KeyValuePair<int, int>)enumerator.Current).Key;
                values[i] = ((KeyValuePair<int, int>)enumerator.Current).Value;
                i++;
            }

            TestArrayAscendance(numItems, keys);
            TestArrayAscendance(numItems, values);
        }

        private static void TestGetEnumerator(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            TestArrayAscendance(numItems, d.Select(x => x.Key).ToArray());
            TestArrayAscendance(numItems, d.Select(x => x.Value).ToArray());
        }

        private static void TestContainsKey(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            for (int i = 0; i < numItems; i++)
            {
                Assert.IsTrue(d.ContainsKey(i));
            }

            Assert.IsFalse(d.ContainsKey(-1));
        }

        private static void TestItemIndexer(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            for (int i = 0; i < numItems; i++)
            {
                Assert.AreEqual(i, d[i]);
            }

            Exception exc = null;
            try
            {
                int ignore = d[-1];
            }
            catch (KeyNotFoundException e)
            {
                exc = e;
            }

            Assert.IsNotNull(exc);
            Assert.AreEqual(typeof(KeyNotFoundException), exc.GetType());
        }

        private static void TestTryGetValue(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            for (int i = 0; i < numItems; i++)
            {
                int value;
                bool success = d.TryGetValue(i, out value);

                Assert.IsTrue(success);
                Assert.AreEqual(i, value);
            }

            int value2;
            bool success2 = d.TryGetValue(-1, out value2);
            Assert.IsFalse(success2);
            Assert.AreEqual(value2, default(int));
        }

        private static void TestCount(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            Assert.AreEqual(d.Count, numItems);
        }

        private static void TestKeys(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            TestArrayAscendance(numItems, d.Keys.ToArray());
        }

        private static void TestValues(HybridReadOnlyArrayDictionary<int, int> d, int numItems)
        {
            TestArrayAscendance(numItems, d.Values.ToArray());
        }

        private static void TestArrayAscendance(int numItems, int[] array)
        {
            Assert.AreEqual(numItems, array.Length);
            for (int i = 0; i < numItems; i++)
            {
                Assert.AreEqual(i, array[i]);
            }
        }

        #endregion
    }
}
