using System;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IChromosome<T> : IEquatable<IChromosome<T>>, IComparable<T>
    {
        T Fitness { get; }
    }
}
