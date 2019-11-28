using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2.OffspringSelectors
{
    public abstract class NSGA2OffspringSelectorBase<TChromosome, TObjective, TObjectivesValue> :
        IOffspringSelector<TChromosome>
        where TChromosome : IChromosome<TObjectivesValue>
    {
        private readonly IEnumerable<TObjective> _objectives;
        private readonly IObjectivesValueMapper<TObjective, TObjectivesValue> _mapper;

        public NSGA2OffspringSelectorBase(
            IEnumerable<TObjective> objectives,
            IObjectivesValueMapper<TObjective, TObjectivesValue> mapper)
        {
            _objectives = objectives;
            _mapper = mapper;
        }

        public Task<ImmutableHashSet<TChromosome>> SelectAsync(
            ImmutableHashSet<TChromosome> eliteOffspring,
            ImmutableHashSet<TChromosome> lastFront,
            int expectedOffspringCount,
            CancellationToken token)
        {
            var joined = lastFront.Union(eliteOffspring);
            var remainingCount = expectedOffspringCount - eliteOffspring.Count;

            var measurable = SelectMeasurableOffspring(eliteOffspring, lastFront)
                .ToDictionary(chromosome => chromosome, _ => 0d);

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
                    if (!measurable.ContainsKey(chromosome)) continue;
                    measurable[chromosome] += longestDistance;
                }

                if (orderedGrouped.Length == 2) continue;

                for (var i = 1; i < orderedGrouped.Length - 1; i++)
                {
                    var lower = orderedGrouped[i - 1].Key;
                    var higher = orderedGrouped[i + 1].Key;
                    var distance = Math.Abs(higher - lower);

                    foreach (var chromosome in orderedGrouped[i])
                    {
                        if (!measurable.ContainsKey(chromosome)) continue;
                        measurable[chromosome] += distance;
                    }
                }
            }

            var selected = FilterMeasuredOffspring(
                eliteOffspring,
                measurable,
                expectedOffspringCount,
                remainingCount);

            return Task.FromResult(selected);
        }

        protected abstract ImmutableHashSet<TChromosome> SelectMeasurableOffspring(
            ImmutableHashSet<TChromosome> elite,
            ImmutableHashSet<TChromosome> last);

        protected abstract ImmutableHashSet<TChromosome> FilterMeasuredOffspring(
            ImmutableHashSet<TChromosome> elite,
            IReadOnlyDictionary<TChromosome, double> measured,
            int requiredCount,
            int remainingCount);
    }
}
