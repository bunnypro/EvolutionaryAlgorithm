using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction.Utility
{
    public interface IOffspringSelector
    {
        Task<ImmutableHashSet<IChromosome>> SelectAsync(ImmutableHashSet<IChromosome> uniqueOffsping, CancellationToken token);
    }
}
