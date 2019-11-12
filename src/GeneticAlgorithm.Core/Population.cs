using System;
using System.Collections.Immutable;
using EvolutionaryAlgorithm.Abstraction;
using EvolutionaryAlgorithm.Abstraction.Utility;
using EvolutionaryAlgorithm.Primitive;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.Core
{
    public class Population : IPopulation
    {
        private readonly PopulationSize _size;

        public Population(PopulationSize size)
        {
            _size = size;
        }

        public ImmutableHashSet<IChromosome> Chromosomes { get; set; }

        public void Initialize(IChromosomeFactory factory)
        {
            Chromosomes = factory.Create(_size.Min);
        }
    }
}
