using System;
using System.Collections.Immutable;
using System.Linq;
using EvolutionaryAlgorithm.Abstraction;
using EvolutionaryAlgorithm.Abstraction.Utility;
using EvolutionaryAlgorithm.Primitive;
using Moq;
using Xunit;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.Core.Test
{
    public class PopulationTest
    {
        [Fact]
        public void Can_Be_Initialized()
        {
            const uint size = 10;
            Population pop = CreatePopulation(size);
            IChromosomeFactory factory = CreateChromosomeFactory();
            pop.Initialize(factory);
            Assert.Equal((int) size, pop.Chromosomes.Count());
        }

        [Fact]
        public void Chromosomes_Can_Be_Replaced()
        {
            const uint size = 10;
            IPopulation pop = CreatePopulation(size);
            IChromosomeFactory factory = CreateChromosomeFactory();
            pop.Chromosomes = factory.Create(size);
            Assert.Equal((int) size, pop.Chromosomes.Count());
        }

        private Population CreatePopulation(uint size)
        {
            return new Population(PopulationSize.Create(size));
        }

        private IChromosomeFactory CreateChromosomeFactory()
        {
            var chromosomeMock = new Mock<IChromosome>();
            chromosomeMock.Setup(chromosome => chromosome.Equals(It.IsAny<IChromosome>()))
                .Returns(false);
            chromosomeMock.Setup(chromosome => chromosome.GetHashCode())
                .Returns(() => Guid.NewGuid().GetHashCode());

            var factoryMock = new Mock<IChromosomeFactory>();
            factoryMock.Setup(factory => factory.Create(It.IsAny<uint>()))
                .Returns<uint>(count =>
                    Enumerable.Range(0, (int) count)
                        .Select(_ => chromosomeMock.Object)
                        .ToImmutableHashSet()
                );
            return factoryMock.Object;
        }
    }
}
