using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Abstraction;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class NSGAReinsertion<TChromosome, TObjectivesValue> : IReinsertion<TChromosome>
        where TChromosome : IChromosome<TObjectivesValue>
    {
        private readonly IComparer<TChromosome> _offspringComparer;
        private readonly IOffspringSelector<TChromosome> _selector;

        public NSGAReinsertion(
            IComparer<TObjectivesValue> comparer,
            IOffspringSelector<TChromosome> selector)
        {
            _offspringComparer = new ChromosomeFitnessComparer(comparer);
            _selector = selector;
        }

        public async Task<ImmutableHashSet<TChromosome>> SelectAsync(
            IEnumerable<TChromosome> parents,
            IEnumerable<TChromosome> offspring,
            int expectedOffspringCount,
            CancellationToken token)
        {
            var uniqueOffspring = offspring.ToImmutableHashSet();

            if (uniqueOffspring.Count <= expectedOffspringCount)
            {
                return uniqueOffspring;
            }

            var fronts = FastNondominatedSorter.Sort(uniqueOffspring, _offspringComparer);

            var selectedOffspring = new List<TChromosome>();
            IEnumerable<TChromosome> lastFront = new TChromosome[] { };

            foreach (var front in fronts)
            {
                if (selectedOffspring.Count + front.Count() > expectedOffspringCount)
                {
                    lastFront = front;
                    break;
                }
                selectedOffspring.AddRange(front);
            }

            if (selectedOffspring.Count >= expectedOffspringCount)
            {
                return selectedOffspring.Take(expectedOffspringCount).ToImmutableHashSet();
            }

            return await _selector.SelectAsync(
                selectedOffspring.ToImmutableHashSet(),
                lastFront.ToImmutableHashSet(),
                expectedOffspringCount, token);
        }

        private class ChromosomeFitnessComparer : IComparer<TChromosome>
        {
            private readonly IComparer<TObjectivesValue> _comparer;

            public ChromosomeFitnessComparer(IComparer<TObjectivesValue> comparer)
            {
                _comparer = comparer;
            }

            public int Compare(TChromosome x, TChromosome y)
            {
                return _comparer.Compare(x.Fitness, y.Fitness);
            }
        }
    }
}
