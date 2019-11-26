using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class EuclideanBasedOffspringSelector<TChromosome, TObjective, TObjectivesValue> : IReinsertion<TChromosome>
        where TChromosome : IChromosome<TObjectivesValue>
    {
        private readonly IEnumerable<TObjective> _objectives;
        private readonly IObjectivesValueMapper<TObjective, TObjectivesValue> _mapper;
        private readonly IComparer<TChromosome> _offspringComparer;

        public EuclideanBasedOffspringSelector(IEnumerable<TObjective> objectives,
            IObjectivesValueMapper<TObjective, TObjectivesValue> mapper,
            IComparer<TObjectivesValue> comparer)
        {
            _objectives = objectives;
            _mapper = mapper;
            _offspringComparer = new ChromosomeFitnessComparer(comparer);
        }

        public async Task<ImmutableHashSet<TChromosome>> SelectAsync(
            IEnumerable<TChromosome> parents,
            IEnumerable<TChromosome> offspring,
            int expectedOffspringCount,
            CancellationToken token)
        {
            var uniqueOffspring = offspring.ToImmutableHashSet();

            if (uniqueOffspring.Count <= expectedOffspringCount)
            {
                return uniqueOffspring;
            }

            var fronts = FastNondominatedSorter.Sort(uniqueOffspring, _offspringComparer);

            var selectedOffspring = new List<TChromosome>();
            IEnumerable<TChromosome> lastFront = new TChromosome[] { };

            foreach (var front in fronts)
            {
                if (selectedOffspring.Count + front.Count() > expectedOffspringCount)
                {
                    lastFront = front;
                    break;
                }
                selectedOffspring.AddRange(front);
            }

            if (selectedOffspring.Count >= expectedOffspringCount)
            {
                return selectedOffspring.Take(expectedOffspringCount).ToImmutableHashSet();
            }

            var calculatable = lastFront.Union(selectedOffspring).ToArray();

            var tasks = _objectives.Select(async objective =>
            {
                double CalculateDistance(TChromosome left, TChromosome right)
                {
                    return Math.Sqrt(_objectives.Sum(innerObjective =>
                        Math.Pow(_mapper.GetValue(innerObjective, left.Fitness) -
                            _mapper.GetValue(innerObjective, right.Fitness), 2)));
                }

                var orderedChromosome = calculatable.OrderBy(chromosome =>
                        _mapper.GetValue(objective, chromosome.Fitness))
                    .ThenBy(chromosome =>
                        _objectives.Average(obj => _mapper.GetValue(obj, chromosome.Fitness)))
                    .ToImmutableArray();

                var first = orderedChromosome.First();
                var last = orderedChromosome.Last();

                var distances = calculatable.ToDictionary(chromosome => chromosome, _ => 0d);
                var longestDistance = CalculateDistance(first, last);
                distances[first] += longestDistance;
                distances[last] += longestDistance;

                var innerTasks = orderedChromosome.Skip(1).Take(orderedChromosome.Length - 2)
                    .Select((chromosome, i) => Task.Run(() =>
                    {
                        var upper = orderedChromosome[i + 2];
                        var lower = orderedChromosome[i];
                        distances[chromosome] += CalculateDistance(upper, lower);
                    }, token));

                await Task.WhenAll(innerTasks);
                return distances;
            });

            var allDistances = await Task.WhenAll(tasks);

            return calculatable.ToDictionary(chromosome => chromosome, chromosome =>
                    allDistances.Sum(distances => distances[chromosome]))
                .OrderByDescending(kv => kv.Value)
                .Take(expectedOffspringCount)
                .Select(kv => kv.Key)
                .ToImmutableHashSet();
        }

        private class ChromosomeFitnessComparer : IComparer<TChromosome>
        {
            private readonly IComparer<TObjectivesValue> _comparer;

            public ChromosomeFitnessComparer(IComparer<TObjectivesValue> comparer)
            {
                _comparer = comparer;
            }

            public int Compare(TChromosome x, TChromosome y)
            {
                return _comparer.Compare(x.Fitness, y.Fitness);
            }
        }
    }
}
