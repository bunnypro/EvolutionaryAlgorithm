using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2.OffspringSelectors
{
    public class NSGA2LastFrontOffspringSelector<TChromosome, TObjective, TObjectivesValue> :
        NSGA2OffspringSelectorBase<TChromosome, TObjective, TObjectivesValue>
        where TChromosome : IChromosome<TObjectivesValue>
    {
        public NSGA2LastFrontOffspringSelector(
            IEnumerable<TObjective> objectives,
            IObjectivesValueMapper<TObjective, TObjectivesValue> mapper)
            : base(objectives, mapper)
        {
        }

        protected override ImmutableHashSet<TChromosome> SelectMeasurableOffspring(
            ImmutableHashSet<TChromosome> elite, ImmutableHashSet<TChromosome> last)
        {
            return last;
        }

        protected override ImmutableHashSet<TChromosome> FilterMeasuredOffspring(
            ImmutableHashSet<TChromosome> elite,
            IReadOnlyDictionary<TChromosome, double> measured,
            int requiredCount,
            int remainingCount)
        {
            return elite.Union(measured.OrderByDescending(kv => kv.Value)
                .Take(remainingCount)
                .Select(kv => kv.Key));
        }
    }
}
