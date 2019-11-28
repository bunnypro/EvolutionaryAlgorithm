using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public interface IOffspringSelector<TChromosome>
    {
        Task<ImmutableHashSet<TChromosome>> SelectAsync(
            ImmutableHashSet<TChromosome> eliteOffspring,
            ImmutableHashSet<TChromosome> lastFront,
            int expectedOffspringCount,
            CancellationToken token);
    }
}