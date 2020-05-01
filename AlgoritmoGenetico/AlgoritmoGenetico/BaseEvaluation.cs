namespace GeneticAlgorithm
{
    public class BaseEvaluation<GeneType> : BaseEntity
    {
        public virtual float CalculatesEvaluation(Chromosome<GeneType> chromosome)
        {
            return 0;
        }
    }
}
