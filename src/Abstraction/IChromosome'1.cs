using System;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IChromosome<TFitness> : IEquatable<IChromosome<TFitness>>
    {
        TFitness Fitness { get; }
    }
}
