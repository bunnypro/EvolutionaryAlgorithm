using System;
using System.Collections.Generic;

namespace EvolutionaryAlgorithm.Primitive
{
    public struct PopulationSize
    {
        public static PopulationSize Create(uint size)
        {
            return new PopulationSize
            {
                Min = size,
                Max = size
            };
        }

        public static PopulationSize Create(uint min, uint max)
        {
            if (min > max)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            return new PopulationSize
            {
                Min = min,
                Max = max
            };
        }

        public uint Min { get; private set; }
        public uint Max { get; private set; }
    }
}
