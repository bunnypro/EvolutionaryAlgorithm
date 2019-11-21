using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IReinsertion<TChromosome, TFitness> where TChromosome : IChromosome<TFitness>
    {
        Task<ImmutableHashSet<TChromosome>> SelectAsync(
            IEnumerable<TChromosome> parents,
            IEnumerable<TChromosome> offspring,
            int requiredOffspringCount,
            CancellationToken token);
    }
}
