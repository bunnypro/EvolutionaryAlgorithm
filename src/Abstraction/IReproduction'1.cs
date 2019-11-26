using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IReproduction<TChromosome>
    {
        Task<IEnumerable<TChromosome>> ReproduceAsync(
            ImmutableHashSet<TChromosome> parents,
            CancellationToken token);
    }
}
