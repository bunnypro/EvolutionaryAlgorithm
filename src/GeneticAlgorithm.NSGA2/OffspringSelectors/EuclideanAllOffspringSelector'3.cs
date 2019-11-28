using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2.OffspringSelectors
{
    public class EuclideanAllOffspringSelector<TChromosome, TObjective, TObjectivesValue> :
        EuclideanOffspringSelectorBase<TChromosome, TObjective, TObjectivesValue>
        where TChromosome : IChromosome<TObjectivesValue>
    {
        public EuclideanAllOffspringSelector(
            IEnumerable<TObjective> objectives,
            IObjectivesValueMapper<TObjective, TObjectivesValue> mapper) :
            base(objectives, mapper)
        {
        }

        protected override ImmutableHashSet<TChromosome> SelectMeasurableOffspring(
            ImmutableHashSet<TChromosome> eliteOffspring,
            ImmutableHashSet<TChromosome> lastFront)
        {
            return lastFront.Union(eliteOffspring);
        }

        protected override ImmutableHashSet<TChromosome> FilterMeasuredOffspring(
            ImmutableHashSet<TChromosome> elite,
            IReadOnlyDictionary<TChromosome, double> measured,
            int requiredCount,
            int remainingCount)
        {
            return measured.OrderByDescending(kv => kv.Value)
                .Take(requiredCount)
                .Select(kv => kv.Key)
                .ToImmutableHashSet();
        }
    }
}
