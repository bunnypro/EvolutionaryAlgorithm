using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IEvolutionaryAlgorithm<TChromosome, TFitness> where TChromosome : IChromosome<TFitness>
    {
        Task<ImmutableHashSet<TChromosome>> EvolveAsync(ImmutableHashSet<TChromosome> population, CancellationToken token);
    }
}