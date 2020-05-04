namespace GeneticAlgorithm
{
    public class BaseEvaluation<GeneType> : BaseEntity
    {
        public object Param1 { get; set; }
        public object Param2 { get; set; }
        public object Param3 { get; set; }
        public object Param4 { get; set; }
        public object Param5 { get; set; }
        public virtual float CalculatesEvaluation(Chromosome<GeneType> chromosome)
        {
            return 0;
        }
    }
}
