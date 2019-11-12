using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;
using EvolutionaryAlgorithm.Abstraction.Utility;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.Core
{
    public sealed class GeneticAlgorithm : Abstraction.EvolutionaryAlgorithm
    {
        private readonly IChromosomeEvaluator _evaluator;
        private readonly ICrossoverOperation _crossover;

        public GeneticAlgorithm(IChromosomeEvaluator evaluator,
            ICrossoverOperation crossover)
        {
            _evaluator = evaluator;
            _crossover = crossover;
        }

        public IMutationOperation MutationOperation { get; set; }
        public IOffspringSelector OffspringSelector { get; set; }

        protected override async Task<ImmutableHashSet<IChromosome>> OperateAsync(ImmutableHashSet<IChromosome> parents, CancellationToken token)
        {
            await _evaluator.EvaluateAsync(parents, token);
            var offspring = new List<IChromosome>();
            offspring.AddRange(await _crossover.OperateAsync(parents, token));
            if (MutationOperation is IMutationOperation operation) {
                offspring.AddRange(await operation.OperateAsync(parents, token));
            }
            var uniqueOffsping = ImmutableHashSet.CreateRange(offspring);
            await _evaluator.EvaluateAsync(uniqueOffsping, token);
            if (OffspringSelector is IOffspringSelector selector)
            {
                return await selector.SelectAsync(uniqueOffsping, token);
            }
            return uniqueOffsping;
        }
    }
}