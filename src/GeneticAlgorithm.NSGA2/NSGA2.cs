using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class NSGA2<TChromosome, TObjective> : IEvolutionaryAlgorithm<TChromosome>
        where TChromosome : IChromosome<IObjectiveValues<TObjective>>
    {
        private readonly IReproduction<TChromosome> _crossover;
        private readonly IReproduction<TChromosome> _mutation;
        private readonly IEvaluator<TChromosome> _evaluator;
        private IReinsertion<TChromosome> _reinsertion;

        public NSGA2(IReproduction<TChromosome> crossover,
            IReproduction<TChromosome> mutation,
            IEvaluator<TChromosome> evaluator,
            IReinsertion<TChromosome> reinsertion) :
            this(crossover, evaluator, reinsertion)
        {
            _mutation = mutation;
        }

        public NSGA2(IReproduction<TChromosome> crossover,
            IEvaluator<TChromosome> evaluator,
            IReinsertion<TChromosome> reinsertion)
        {
            _crossover = crossover;
            _evaluator = evaluator;
            _reinsertion = reinsertion;
        }

        public event Action<ImmutableHashSet<TChromosome>> OnEvolvedOnce;

        public int? ExpectedResultCount { get; set; } = null;

        public async Task<ImmutableHashSet<TChromosome>> EvolveAsync(
            ImmutableHashSet<TChromosome> parents,
            CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var expectedCount = ExpectedResultCount ?? parents.Count;
                var offspring = await _crossover.ReproduceAsync(parents, token);
                var uniqueOffspring = offspring.Union(
                    await _mutation?.ReproduceAsync(parents, token) ?? new TChromosome[0])
                        .ToImmutableHashSet();
                await _evaluator.EvaluateAsync(uniqueOffspring, token);
                parents = await _reinsertion.SelectAsync(parents, offspring, expectedCount, token);
                OnEvolvedOnce?.Invoke(parents);
            }

            return parents;
        }
    }
}
