using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction.Utility
{
    public interface IChromosomeEvaluator
    {
        Task EvaluateAsync(ImmutableHashSet<IChromosome> parents, CancellationToken token);
    }
}
