using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction.Utility
{
    public interface IReproductionOperation
    {
        Task<IEnumerable<IChromosome>> OperateAsync(ImmutableHashSet<IChromosome> parents, CancellationToken token);
    }
}
