using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public abstract class EvolutionaryAlgorithm : IEvolutionaryAlgorithm
    {
        public virtual async Task EvolveAsync(IPopulation population, CancellationToken token)
        {
            while(true)
            {
                population.Chromosomes = await OperateAsync(population.Chromosomes, token);
                token.ThrowIfCancellationRequested();
            }
        }

        protected abstract Task<ImmutableHashSet<IChromosome>> OperateAsync(ImmutableHashSet<IChromosome> parents, CancellationToken token);
    }
}
