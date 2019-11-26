using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class OffspringSelector<TChromosome, TObjectivesValues, TObjective> : IReinsertion<TChromosome>
        where TChromosome : IChromosome<TObjectivesValues>
    {
        private readonly IComparer<TChromosome> _offspringComparer;
        private readonly IObjectivesValuesMapper<TObjectivesValues, TObjective> _mapper;

        public OffspringSelector(IObjectivesValuesMapper<TObjectivesValues, TObjective> mapper)
        {
            _mapper = mapper;
            _offspringComparer = new OffspringFitnessComparer(_mapper.Comparer);
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

            var tasks = _mapper.Objectives.Select(async objective =>
            {
                double CalculateDistance(TChromosome left, TChromosome right)
                {
                    return Math.Sqrt(_mapper.Objectives.Sum(innerObjective =>
                        Math.Pow(_mapper.GetObjectiveValue(innerObjective, left.Fitness) -
                            _mapper.GetObjectiveValue(innerObjective, right.Fitness), 2)));
                }

                var orderedChromosome = calculatable.OrderBy(chromosome =>
                    _mapper.GetObjectiveValue(objective, chromosome.Fitness))
                .ToImmutableArray();

                var first = orderedChromosome.First();
                var last = orderedChromosome.Last();
                var longestDistance = CalculateDistance(first, last);

                var distances = calculatable.ToDictionary(chromosome => chromosome, _ => 0d);
                distances[first] += longestDistance;
                distances[last] += longestDistance;

                if (orderedChromosome.Length == 2)
                    return distances;

                var innerTasks = orderedChromosome.Skip(1).Take(orderedChromosome.Length - 2)
                    .Select((chromosome, i) => Task.Run(() =>
                    {
                        var upper = orderedChromosome[i + 1];
                        var lower = orderedChromosome[i - 1];
                        distances[chromosome] += CalculateDistance(upper, lower);
                    }, token));

                await Task.WhenAll(innerTasks);
                return distances;
            });

            var allDistances = await Task.WhenAll(tasks);

            return calculatable.ToDictionary(chromosome => chromosome, chromosome =>
                    allDistances.Sum(distances => distances[chromosome]))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToImmutableHashSet();
        }

        private class OffspringFitnessComparer : IComparer<TChromosome>
        {
            private readonly IComparer<TObjectivesValues> _mapper;

            public OffspringFitnessComparer(IComparer<TObjectivesValues> mapper)
            {
                _mapper = mapper;
            }

            public int Compare(TChromosome x, TChromosome y)
            {
                return _mapper.Compare(x.Fitness, y.Fitness);
            }
        }
    }
}
