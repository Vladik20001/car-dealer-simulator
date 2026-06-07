using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarDealerSimulator.Core.Utilities
{
    /// <summary>
    /// Extension methods for collections used throughout the project.
    /// Prevents repeated utility code for random selection, filtering, and weighted picks.
    /// </summary>
    public static class CollectionExtensions
    {
        private static readonly System.Random _random = new();

        /// <summary>
        /// Returns a random element from the list.
        /// </summary>
        public static T RandomElement<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                throw new InvalidOperationException("Cannot select from an empty collection.");

            return list[_random.Next(list.Count)];
        }

        /// <summary>
        /// Returns a random element using weighted probabilities.
        /// </summary>
        public static T WeightedRandom<T>(this IList<T> items, Func<T, float> weightSelector)
        {
            if (items == null || items.Count == 0)
                throw new InvalidOperationException("Cannot select from an empty collection.");

            float totalWeight = 0f;
            foreach (var item in items)
            {
                totalWeight += weightSelector(item);
            }

            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var item in items)
            {
                currentWeight += weightSelector(item);
                if (randomValue <= currentWeight)
                    return item;
            }

            return items[items.Count - 1];
        }

        /// <summary>
        /// Shuffles a list in-place using Fisher-Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// Returns the element with the minimum value according to a selector.
        /// </summary>
        public static T MinBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
            where TKey : IComparable<TKey>
        {
            using var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements.");

            T minItem = enumerator.Current;
            TKey minValue = selector(minItem);

            while (enumerator.MoveNext())
            {
                TKey value = selector(enumerator.Current);
                if (value.CompareTo(minValue) < 0)
                {
                    minItem = enumerator.Current;
                    minValue = value;
                }
            }

            return minItem;
        }

        /// <summary>
        /// Returns the element with the maximum value according to a selector.
        /// </summary>
        public static T MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
            where TKey : IComparable<TKey>
        {
            using var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements.");

            T maxItem = enumerator.Current;
            TKey maxValue = selector(maxItem);

            while (enumerator.MoveNext())
            {
                TKey value = selector(enumerator.Current);
                if (value.CompareTo(maxValue) > 0)
                {
                    maxItem = enumerator.Current;
                    maxValue = value;
                }
            }

            return maxItem;
        }

        /// <summary>
        /// Partitions a collection into chunks of a specified size.
        /// </summary>
        public static IEnumerable<IList<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize <= 0)
                throw new ArgumentException("Chunk size must be positive.", nameof(chunkSize));

            var chunk = new List<T>(chunkSize);
            foreach (var item in source)
            {
                chunk.Add(item);
                if (chunk.Count == chunkSize)
                {
                    yield return chunk;
                    chunk = new List<T>(chunkSize);
                }
            }

            if (chunk.Count > 0)
                yield return chunk;
        }
    }
}
