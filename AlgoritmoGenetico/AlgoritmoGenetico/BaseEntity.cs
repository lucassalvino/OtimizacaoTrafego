namespace GeneticAlgorithm
{
    public class BaseEntity
    {
        public string GetJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
