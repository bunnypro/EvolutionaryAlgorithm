using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IReproduction<TChromosome, TFitness> where TChromosome : IChromosome<TFitness>
    {
        Task<IEnumerable<TChromosome>> ReproduceAsync(
            ImmutableHashSet<TChromosome> parents,
            CancellationToken token);
    }
}
