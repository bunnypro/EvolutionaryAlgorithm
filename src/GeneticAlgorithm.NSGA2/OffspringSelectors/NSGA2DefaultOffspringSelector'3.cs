using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2.OffspringSelectors
{
    public class NSGA2DefaultOffspringSelector<TChromosome, TObjective, TObjectivesValue> :
        IOffspringSelector<TChromosome>
        where TChromosome : IChromosome<TObjectivesValue>
    {
        private readonly IEnumerable<TObjective> _objectives;
        private readonly IObjectivesValueMapper<TObjective, TObjectivesValue> _mapper;

        public NSGA2DefaultOffspringSelector(
            IEnumerable<TObjective> objectives,
            IObjectivesValueMapper<TObjective, TObjectivesValue> mapper)
        {
            _objectives = objectives;
            _mapper = mapper;
        }

        public Task<ImmutableHashSet<TChromosome>> SelectAsync(
            IEnumerable<TChromosome> eliteOffspring,
            IEnumerable<TChromosome> lastFront,
            int expectedOffspringCount,
            CancellationToken token)
        {
            var elite = eliteOffspring.ToImmutableHashSet();
            var last = lastFront.Except(elite).ToImmutableHashSet();
            var joined = last.Union(elite);
            var remainingCount = expectedOffspringCount - elite.Count;

            var distances = last.ToDictionary(chromosome => chromosome, _ => 0d);

            foreach (var objective in _objectives)
            {
                var orderedGrouped = joined.GroupBy(chromosome =>
                        _mapper.GetValue(objective, chromosome.Fitness))
                    .OrderBy(group => group.Key)
                    .ToImmutableArray();

                if (orderedGrouped.Length == 1) continue;

                var longestDistance = Math.Abs(orderedGrouped.First().Key - orderedGrouped.Last().Key);
                var firstLast = orderedGrouped.First().Union(orderedGrouped.Last());

                foreach (var chromosome in firstLast)
                {
                    if (!distances.ContainsKey(chromosome)) continue;
                    distances[chromosome] += longestDistance;
                }

                if (orderedGrouped.Length == 2) continue;

                for (var i = 1; i < orderedGrouped.Length - 1; i++)
                {
                    var lower = orderedGrouped[i - 1].Key;
                    var higher = orderedGrouped[i + 1].Key;
                    var distance = Math.Abs(higher - lower);

                    foreach (var chromosome in orderedGrouped[i])
                    {
                        if (!distances.ContainsKey(chromosome)) continue;
                        distances[chromosome] += distance;
                    }
                }
            }

            var selected = elite.Union(
                    distances.OrderByDescending(kv => kv.Value)
                        .Take(remainingCount)
                        .Select(kv => kv.Key))
                .ToImmutableHashSet();

            return Task.FromResult(selected);
        }
    }
}
