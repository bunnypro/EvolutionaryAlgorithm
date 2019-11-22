using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2.Test
{
    public class FastNondominatedSortingTest
    {
        [Theory]
        [MemberData(nameof(GetTestItems))]
        public void Fast_Nondominated_Sort_Test(IEnumerable<IEnumerable<double[]>> sortedItem)
        {
            //Given
            var random = new Random();
            var shuffledItem = sortedItem.SelectMany(items => items).OrderBy(_ => random.Next()).ToArray();

            // When
            var result = FastNondominatedSorter.Sort(shuffledItem, new ObjectiveComparer()).ToArray();

            //Then
            Assert.True(sortedItem.SequenceEqual(result, new FrontEqualityComparer()));
        }

        public static IEnumerable<object[]> GetTestItems()
        {
            yield return new object[] { GetSortedItems() };
            yield return new object[] { GetMaxSortedItems() };
        }

        private static IEnumerable<IEnumerable<double[]>> GetSortedItems()
        {
            return new[] {
                new [] {
                    new double[] { .9, .9, .8},
                    new double[] { .6, .9, .7},
                    new double[] { .8, .6, .8},
                },
                new [] {
                    new double[] { .8, .6, .7},
                    new double[] { .7, .8, .6},
                    new double[] { .6, .6, .7},
                    new double[] { .7, .6, .7},
                    new double[] { .8, .8, .6},
                },
                new [] {
                    new double[] { .7, .7, .4},
                    new double[] { .7, .5, .6},
                    new double[] { .5, .7, .5},
                    new double[] { .4, .5, .6},
                },
            };
        }

        private static IEnumerable<IEnumerable<double[]>> GetMaxSortedItems()
        {
            return new [] {
                new [] {
                    new double[] { .9, .9, .9 },
                },
                new [] {
                    new double[] { .8, .8, .8 },
                },
                new [] {
                    new double[] { .7, .7, .7 },
                },
                new [] {
                    new double[] { .6, .6, .6 },
                },
                new [] {
                    new double[] { .5, .5, .5 },
                },
                new [] {
                    new double[] { .4, .4, .4 },
                },
                new [] {
                    new double[] { .3, .3, .3 },
                },
                new [] {
                    new double[] { .2, .2, .2 },
                },
                new [] {
                    new double[] { .1, .1, .1 },
                },
                new [] {
                    new double[] { 0, 0, 0 },
                },
            };
        }

        private class FrontEqualityComparer : IEqualityComparer<IEnumerable<double[]>>
        {
            private readonly IEqualityComparer<double[]> _comparer = new ObjectiveEqualityComparer();

            public bool Equals([AllowNull] IEnumerable<double[]> x, [AllowNull] IEnumerable<double[]> y)
            {
                if (x == null || y == null) return false;
                return !x.Union(y).Except(x.Intersect(y, _comparer), _comparer).Any();
            }

            public int GetHashCode([DisallowNull] IEnumerable<double[]> obj)
            {
                unchecked
                {
                    return obj.Aggregate(1, (aggregate, value) => (aggregate * 397) ^ _comparer.GetHashCode(value));
                }
            }
        }

        private class ObjectiveEqualityComparer : IEqualityComparer<double[]>
        {
            public bool Equals([AllowNull] double[] x, [AllowNull] double[] y)
            {
                if (x == null || y == null) return false;
                return x.SequenceEqual(y);
            }

            public int GetHashCode([DisallowNull] double[] obj)
            {
                unchecked
                {
                    return obj.Aggregate(1, (aggregate, value) => (aggregate * 297) ^ value.GetHashCode());
                }
            }
        }

        private class ObjectiveComparer : IComparer<double[]>
        {
            public int Compare([AllowNull] double[] x, [AllowNull] double[] y)
            {
                if (x.Length != y.Length) return 0;

                var sign = 0;
                for(var i = 0; i < x.Length; i++)
                {
                    var lsign = x[i].CompareTo(y[i]);
                    if (lsign == 0 || sign != 0 && sign != lsign) return 0;
                    sign = lsign;
                }

                return sign;
            }
        }
    }
}
