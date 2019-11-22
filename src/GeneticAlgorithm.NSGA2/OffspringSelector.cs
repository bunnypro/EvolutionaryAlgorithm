using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class OffspringSelector<TChromosome, TObjective> : IReinsertion<TChromosome>
        where TChromosome : IChromosome<IObjectiveValues<TObjective>>
    {
        private readonly FastNondominatedSorter _sorter = new FastNondominatedSorter();
        private readonly IReadOnlyDictionary<TObjective, IObjectiveNormalizer<TChromosome>> _normalizer;

        public OffspringSelector() { }
        public OffspringSelector(IReadOnlyDictionary<TObjective, IObjectiveNormalizer<TChromosome>> normalizer)
        {
            _normalizer = normalizer;
        }

        public async Task<ImmutableHashSet<TChromosome>> SelectAsync(
            IEnumerable<TChromosome> parents,
            IEnumerable<TChromosome> offspring,
            int requiredOffspringCount,
            CancellationToken token)
        {
            var fronts = _sorter.Sort(offspring.Union(parents).ToImmutableHashSet(), new OffspringComparer());
            var selectedOffspring = new List<TChromosome>();
            var lastFront = new List<TChromosome>();

            foreach (var front in fronts)
            {
                lastFront = front.ToList();
                if (selectedOffspring.Count + lastFront.Count > requiredOffspringCount)
                {
                    break;
                }
                selectedOffspring.AddRange(lastFront);
            }

            if (selectedOffspring.Count >= requiredOffspringCount)
            {
                return selectedOffspring.Take(requiredOffspringCount).ToImmutableHashSet();
            }

            var calculatable = lastFront.Union(selectedOffspring).ToArray();
            var objectives = calculatable.First().Fitness.Keys;

            var calculationTasks = objectives.Select(objective => CalculateDistance(objective, calculatable));
            var distances = await Task.WhenAll(calculationTasks);

            var aggregatedDistances = distances.Aggregate(calculatable.ToDictionary(c => c, _ => 0d),
                (aggregate, distance) =>
                {
                    return aggregate.ToDictionary(c => c.Key, c => c.Value + distance[c.Key]);
                });

            return aggregatedDistances.OrderByDescending(c => c.Value)
                .Take(requiredOffspringCount)
                .Select(c => c.Key)
                .ToImmutableHashSet();
        }

        private async Task<IReadOnlyDictionary<TChromosome, double>> CalculateDistance(
            TObjective objective,
            IEnumerable<TChromosome> chromosomes)
        {
            var distances = chromosomes.ToDictionary(c => c, _ => 0d);
            var normalized = await _normalizer?[objective]?.Normalize(distances.Keys)
                ?? chromosomes.ToDictionary(c => c, c => c.Fitness[objective]);
            var ordered = normalized.OrderBy(n => n.Value).ToArray();
            var lowest = ordered.First();
            var highest = ordered.Last();
            if (!lowest.Key.Equals(highest.Key))
            {
                var maxRange = Math.Abs(lowest.Value - highest.Value);
                distances[lowest.Key] += maxRange;
                distances[highest.Key] += maxRange;
            }
            for (var i = 1; i < ordered.Length - 1; i++)
            {
                var lower = ordered[i - 1].Value;
                var higher = ordered[i + 1].Value;
                var distance = Math.Abs(lower - higher);
                distances[ordered[i].Key] += distance;
            }
            return distances;
        }

        private class OffspringComparer : IComparer<TChromosome>
        {
            private readonly IComparer<IObjectiveValues<TObjective>> _comparer =
                new ObjectiveValues<TObjective>.Comparer();

            public int Compare(TChromosome x, TChromosome y)
            {
                if (x.Fitness is IComparable<IObjectiveValues<TObjective>> comparable)
                {
                    return comparable.CompareTo(y.Fitness);
                }

                return _comparer.Compare(x.Fitness, y.Fitness);
            }
        }
    }
}
