using System;
using System.Collections.Generic;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public interface IObjectiveValues<T> : IReadOnlyDictionary<T, double>
    {
    }
}
