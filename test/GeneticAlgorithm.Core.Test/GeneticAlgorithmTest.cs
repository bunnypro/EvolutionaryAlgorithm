using System;
using Xunit;
using EvolutionaryAlgorithm.Abstraction;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using EvolutionaryAlgorithm.Abstraction.Utility;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.Core.Test
{
    public class GeneticAlgorithmTest
    {
        [Fact]
        public async Task Test1()
        {
            var ga = CreateGeneticAlgotihm();
            var pop = CreatePopulation();
            pop.Chromosomes = CreateChromosomeFactory().Create(5);

            using var tokenSource = new CancellationTokenSource();

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                var delay = Task.Delay(100).ContinueWith(_ => tokenSource.Cancel());
                await ga.EvolveAsync(pop, tokenSource.Token);
                await delay;
            });
        }

        private IEvolutionaryAlgorithm CreateGeneticAlgotihm()
        {
            IChromosomeEvaluator evaluator = CreateChromosomeEvaluator();
            ICrossoverOperation operation = CraeteCrossoverOperation();
            return new GeneticAlgorithm(evaluator, operation);
        }

        private IChromosomeEvaluator CreateChromosomeEvaluator()
        {
            var mock = new Mock<IChromosomeEvaluator>();
            mock.Setup(evaluator => evaluator.EvaluateAsync(
                It.IsAny<ImmutableHashSet<IChromosome>>(),
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);
            return mock.Object;
        }

        private ICrossoverOperation CraeteCrossoverOperation()
        {
            var mock = new Mock<ICrossoverOperation>();
            mock.Setup(op => op.OperateAsync(
                It.IsAny<ImmutableHashSet<IChromosome>>(),
                It.IsAny<CancellationToken>()
            )).Returns<ImmutableHashSet<IChromosome>, CancellationToken>((parents, _) =>
                Task.FromResult(parents as IEnumerable<IChromosome>));
            return mock.Object;
        }

        private IPopulation CreatePopulation()
        {
            var mock = new Mock<IPopulation>();
            mock.SetupProperty(pop => pop.Chromosomes);
            return mock.Object;
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
