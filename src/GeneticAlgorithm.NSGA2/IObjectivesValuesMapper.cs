using System.Collections.Generic;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public interface IObjectivesValuesMapper<TObjectivesValues, TObjective>
    {
        IComparer<TObjectivesValues> Comparer { get; }
        IEnumerable<TObjective> Objectives { get; }

        double GetObjectiveValue(TObjective objective, TObjectivesValues objectivesValues);
    }
}
