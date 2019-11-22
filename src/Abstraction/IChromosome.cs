using System;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IChromosome<T> : IEquatable<IChromosome<T>>
    {
        T Fitness { get; }
    }
}
