using System.Collections.Immutable;

namespace EvolutionaryAlgorithm.Abstraction.Utility
{
    public interface IChromosomeFactory
    {
        ImmutableHashSet<IChromosome> Create(uint count);
    }
}
