using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public interface IObjectiveNormalizer<TChromosome>
    {
        Task<IReadOnlyDictionary<TChromosome, double>> Normalize(IEnumerable<TChromosome> chromosomes);
    }
}
