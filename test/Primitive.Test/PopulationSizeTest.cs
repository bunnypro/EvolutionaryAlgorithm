using System;
using Xunit;

namespace EvolutionaryAlgorithm.Primitive.Test
{
    public class PopulationSizeTest
    {
        [Fact]
        public void Can_Be_Creatd_With_Single_Value()
        {
            const uint size = 10;
            PopulationSize psize = PopulationSize.Create(size);
            uint min = psize.Min;
            uint max = psize.Max;
            Assert.Equal(size, min);
            Assert.Equal(size, max);
        }

        [Fact]
        public void Can_Be_Crated_With_Ranged_Value_And_Fixes_Order()
        {
            const uint min = 10;
            const uint max = 20;
            PopulationSize psize = PopulationSize.Create(max, min);
            Assert.Equal(min, psize.Min);
            Assert.Equal(max, psize.Max);
        }
    }
}
