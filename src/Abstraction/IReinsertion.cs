using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IReinsertion<TChromosome>
    {
        Task<ImmutableHashSet<TChromosome>> SelectAsync(
            IEnumerable<TChromosome> parents,
            IEnumerable<TChromosome> offspring,
            int expectedOffspringCount,
            CancellationToken token);
    }
}
