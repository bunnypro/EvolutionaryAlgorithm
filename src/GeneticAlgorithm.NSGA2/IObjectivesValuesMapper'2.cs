namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public interface IObjectivesValueMapper<TObjective, TObjectivesValue>
    {
        double GetValue(TObjective objective, TObjectivesValue objectivesValue);
    }
}
