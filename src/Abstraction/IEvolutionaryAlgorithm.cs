using System.Threading;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Abstraction
{
    public interface IEvolutionaryAlgorithm
    {
        Task EvolveAsync(IPopulation population, CancellationToken token);
    }
}