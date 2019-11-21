using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public interface IObjectiveNormalizer<TChromosome, TObjective> :
        IReadOnlyDictionary<TObjective, Func<IEnumerable<TChromosome>, Task<IReadOnlyDictionary<TChromosome, double>>>>
        where TChromosome : IChromosome<IObjectiveValues<TObjective>>
    {
    }
}
