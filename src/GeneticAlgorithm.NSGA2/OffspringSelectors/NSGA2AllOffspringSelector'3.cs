using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2.OffspringSelectors
{
    public class NSGA2AllOffspringSelector<TChromosome, TObjective, TObjectivesValue> :
        NSGA2OffspringSelectorBase<TChromosome, TObjective, TObjectivesValue>
        where TChromosome : IChromosome<TObjectivesValue>
    {
        public NSGA2AllOffspringSelector(
            IEnumerable<TObjective> objectives,
            IObjectivesValueMapper<TObjective, TObjectivesValue> mapper)
            : base(objectives, mapper)
        {
        }

        protected override ImmutableHashSet<TChromosome> SelectMeasurableOffspring(
            ImmutableHashSet<TChromosome> elite,
            ImmutableHashSet<TChromosome> last)
        {
            return last.Union(elite);
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
