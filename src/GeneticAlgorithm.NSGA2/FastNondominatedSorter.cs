using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class FastNondominatedSorter
    {
        public static IEnumerable<IEnumerable<T>> Sort<T>(IEnumerable<T> items) where T : IComparable
        {
            return Sort(items, new Comparer<T>());
        }

        public static IEnumerable<IEnumerable<T>> Sort<T>(IEnumerable<T> items, IComparer<T> comparer)
        {
            var (front, countBag, itemsBag) = CalculateFirstFront(items, comparer);

            while (front.Count > 0)
            {
                var frontItems = front.ToImmutable();
                yield return frontItems;
                front = ImmutableArray.CreateBuilder<T>();
                foreach (var item in frontItems)
                {
                    foreach (var ditem in itemsBag[item])
                    {
                        countBag[ditem]--;
                        if (countBag[ditem] == 0)
                        {
                            front.Add(ditem);
                        }
                    }
                }
            }
        }

        private static (ImmutableArray<T>.Builder, Dictionary<T, int>, Dictionary<T, List<T>>)
            CalculateFirstFront<T>(IEnumerable<T> items, IComparer<T> comparer)
        {
            var itemsArray = items.ToArray();
            var countBag = itemsArray.ToDictionary(item => item, _ => 0);
            var itemsBag = itemsArray.ToDictionary(item => item, _ => new List<T>());
            var front = ImmutableArray.CreateBuilder<T>();

            for (var i = 0; i < itemsArray.Length; i++)
            {
                for (var j = i + 1; j < itemsArray.Length; j++)
                {
                    var sign = comparer.Compare(itemsArray[i], itemsArray[j]);

                    if (sign > 0)
                    {
                        itemsBag[itemsArray[i]].Add(itemsArray[j]);
                        countBag[itemsArray[j]]++;
                    }
                    else if (sign < 0)
                    {
                        itemsBag[itemsArray[j]].Add(itemsArray[i]);
                        countBag[itemsArray[i]]++;
                    }
                }

                if (countBag[itemsArray[i]] == 0)
                {
                    front.Add(itemsArray[i]);
                }
            }

            return (front, countBag, itemsBag);
        }

        private class Comparer<T> : IComparer<T> where T : IComparable
        {
            public int Compare(T x, T y)
            {
                return x.CompareTo(y);
            }
        }
    }
}
