using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Collections2.Benchmarks
{
    public class DictionariesLookupComparison
    {
        private static readonly string Empty = "";

        private static readonly string[][] CachedKeys = new string[1000][];

        private readonly IReadOnlyDictionary<string, string> _regularSmall = Create(3, true);
        private readonly IReadOnlyDictionary<string, string> _regularMedium = Create(6, true);
        private readonly IReadOnlyDictionary<string, string> _regularLarge = Create(512, true);

        private readonly IReadOnlyDictionary<string, string> _arraySmall = Create(3, false);
        private readonly IReadOnlyDictionary<string, string> _arrayMedium = Create(6, false);
        private readonly IReadOnlyDictionary<string, string> _arrayLarge = Create(512, false);

        [Benchmark]
        public void SmallRegular()
        {
            TestLookup(_regularSmall);
        }

        [Benchmark]
        public void SmallArray()
        {
            TestLookup(_arraySmall);
        }

        [Benchmark]
        public void MediumRegular()
        {
            TestLookup(_regularMedium);
        }

        [Benchmark]
        public void MediumArray()
        {
            TestLookup(_arrayMedium);
        }

        [Benchmark]
        public void LargeRegular()
        {
            TestLookup(_regularLarge);
        }

        [Benchmark]
        public void LargeArray()
        {
            TestLookup(_arrayLarge);
        }

        private static void TestLookup(IReadOnlyDictionary<string, string> d)
        {
            int numItems = d.Count;
            if (CachedKeys[numItems] == null)
            {
                CachedKeys[numItems] = Enumerable.Range(1, numItems).Select(x => x.ToString()).ToArray();
            }

            string[] keys = CachedKeys[numItems];

            for (int i = numItems - 1; i >= 0; i--)
            {
                string v = d[keys[i]];
                if (ReferenceEquals(Empty, v))
                {
                    Console.WriteLine("");
                }
            }

            for (int i = 0; i < numItems; i++)
            {
                string v = d[keys[i]];
                if (ReferenceEquals(Empty, v))
                {
                    Console.WriteLine("");
                }
            }
        }

        private static IReadOnlyDictionary<string, string> Create(int numItems, bool regular)
        {
            var reg = Enumerable.Range(1, numItems).Select(x => x.ToString()).ToDictionary(x => x, x => x);
            return regular
                ? (IReadOnlyDictionary<string, string>)reg
                : new HybridReadOnlyArrayDictionary<string, string>(reg);
        }
    }
}