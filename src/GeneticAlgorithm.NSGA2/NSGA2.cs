using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class NSGA2<TChromosome, TObjective> :
        IEvolutionaryAlgorithm<TChromosome, IObjectiveValues<TObjective>>
        where TChromosome : IChromosome<IObjectiveValues<TObjective>>
    {
        private readonly IReproduction<TChromosome, IObjectiveValues<TObjective>> _crossover;
        private readonly IReproduction<TChromosome, IObjectiveValues<TObjective>> _mutation;
        private readonly IEvaluator<TChromosome, IObjectiveValues<TObjective>> _evaluator;
        private IReinsertion<TChromosome, IObjectiveValues<TObjective>> _reinsertion =
            new OffspringSelector<TChromosome, TObjective>();

        public NSGA2(IReproduction<TChromosome, IObjectiveValues<TObjective>> crossover,
            IReproduction<TChromosome, IObjectiveValues<TObjective>> mutation,
            IEvaluator<TChromosome, IObjectiveValues<TObjective>> evaluator) : this(crossover, evaluator)
        {
            _mutation = mutation;
        }

        public NSGA2(IReproduction<TChromosome, IObjectiveValues<TObjective>> crossover,
            IEvaluator<TChromosome, IObjectiveValues<TObjective>> evaluator)
        {
            _crossover = crossover;
            _evaluator = evaluator;
        }

        public event Action<ImmutableHashSet<TChromosome>> OnEvolvedOnce;

        public int? ExpectedResultCount { get; set; } = null;
        public IObjectiveNormalizer<TChromosome, TObjective> Normalizer
        {
            set
            {
                _reinsertion = new OffspringSelector<TChromosome, TObjective>(value);
            }
        }

        public async Task<ImmutableHashSet<TChromosome>> EvolveAsync(
            ImmutableHashSet<TChromosome> parents,
            CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var offspring = await _crossover.ReproduceAsync(parents, token);
                var uniqueOffspring = offspring.Union(
                    await _mutation?.ReproduceAsync(parents, token) ?? new TChromosome[0])
                        .ToImmutableHashSet();
                await _evaluator.EvaluateAsync(uniqueOffspring, token);
                parents = await _reinsertion
                    .SelectAsync(parents, offspring, ExpectedResultCount ?? parents.Count, token)
                    ?? offspring.ToImmutableHashSet();
                OnEvolvedOnce?.Invoke(parents);
            }

            return parents;
        }
    }
}
