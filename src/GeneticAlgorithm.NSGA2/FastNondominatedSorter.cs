using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class FastNondominatedSorter
    {
        public IEnumerable<IEnumerable<T>> Sort<T>(IEnumerable<T> items) where T : IComparable
        {
            return Sort(items, new Comparer<T>());
        }

        public IEnumerable<IEnumerable<T>> Sort<T>(IEnumerable<T> items, IComparer<T> comparer)
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

        private (ImmutableArray<T>.Builder, Dictionary<T, int>, Dictionary<T, List<T>>)
            CalculateFirstFront<T>(IEnumerable<T> shuffledItem, IComparer<T> comparer)
        {
            var items = shuffledItem.ToArray();
            var countBag = items.ToDictionary(item => item, _ => 0);
            var itemsBag = items.ToDictionary(item => item, _ => new List<T>());
            var front = ImmutableArray.CreateBuilder<T>();

            for (var i = 0; i < items.Length; i++)
            {
                for (var j = i + 1; j < items.Length; j++)
                {
                    var sign = comparer.Compare(items[i], items[j]);

                    if (sign > 0)
                    {
                        itemsBag[items[i]].Add(items[j]);
                        countBag[items[j]]++;
                    }
                    else if (sign < 0)
                    {
                        itemsBag[items[j]].Add(items[i]);
                        countBag[items[i]]++;
                    }
                }

                if (countBag[items[i]] == 0)
                {
                    front.Add(items[i]);
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
