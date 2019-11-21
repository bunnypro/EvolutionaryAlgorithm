using System;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IChromosome<T> : IEquatable<IChromosome<T>>
        where T : IComparable<T>
    {
        T Fitness { get; }
    }
}
