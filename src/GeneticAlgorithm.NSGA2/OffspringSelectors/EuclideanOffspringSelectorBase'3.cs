using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2.OffspringSelectors
{
    public abstract class EuclideanOffspringSelectorBase<TChromosome, TObjective, TObjectivesValue> :
        IOffspringSelector<TChromosome>
        where TChromosome : IChromosome<TObjectivesValue>
    {
        private readonly IEnumerable<TObjective> _objectives;
        private readonly IObjectivesValueMapper<TObjective, TObjectivesValue> _mapper;

        public EuclideanOffspringSelectorBase(
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
                var ordered = joined.OrderBy(chromosome =>
                        _mapper.GetValue(objective, chromosome.Fitness))
                    .ThenBy(chromosome =>
                        _objectives.Average(obj => _mapper.GetValue(obj, chromosome.Fitness)))
                    .ToImmutableArray();

                var first = ordered.First();
                var last = ordered.Last();

                if (measurable.ContainsKey(first) || measurable.ContainsKey(last))
                {
                    var longestDistance = CalculateDistance(first, last);

                    if (measurable.ContainsKey(first))
                        measurable[first] += longestDistance;
                    if (measurable.ContainsKey(last))
                        measurable[last] += longestDistance;
                }

                for (var i = 1; i < ordered.Length - 1; i++)
                {
                    if (!measurable.ContainsKey(ordered[i])) continue;
                    var lower = ordered[i - 1];
                    var higher = ordered[i + 1];
                    var distance = CalculateDistance(lower, higher);
                    measurable[ordered[i]] += distance;
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
            ImmutableHashSet<TChromosome> eliteOffspring,
            ImmutableHashSet<TChromosome> lastFront);

        protected abstract ImmutableHashSet<TChromosome> FilterMeasuredOffspring(
            ImmutableHashSet<TChromosome> elite,
            IReadOnlyDictionary<TChromosome, double> measured,
            int requiredCount,
            int remainingCount);

        private double CalculateDistance(TChromosome left, TChromosome right)
        {
            return Math.Sqrt(_objectives.Sum(innerObjective =>
                Math.Pow(_mapper.GetValue(innerObjective, left.Fitness) -
                    _mapper.GetValue(innerObjective, right.Fitness), 2)));
        }
    }
}