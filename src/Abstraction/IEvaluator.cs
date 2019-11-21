using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IEvaluator<TChromosome, TFitness> where TChromosome : IChromosome<TFitness>
    {
        Task EvaluateAsync(IEnumerable<TChromosome> chromosomes, CancellationToken token);
    }
}
