using System.Collections.Immutable;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IPopulation
    {
        ImmutableHashSet<IChromosome> Chromosomes { get; set; }
    }
}