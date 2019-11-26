using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IEvolutionaryAlgorithm<TChromosome>
    {
        Task<ImmutableHashSet<TChromosome>> EvolveAsync(ImmutableHashSet<TChromosome> population, CancellationToken token);
    }
}