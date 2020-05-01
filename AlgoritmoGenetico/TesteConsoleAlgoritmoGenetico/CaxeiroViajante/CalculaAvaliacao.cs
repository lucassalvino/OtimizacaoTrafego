using GeneticAlgorithm;
using TesteConsoleAlgoritmoGenetico.Grafo;

namespace TesteConsoleAlgoritmoGenetico.CaxeiroViajante
{
    public class CalculaAvaliacao: BaseEvaluation<int>
    {
        public GerenteGrafo Grafo { get; set; }
        public override float CalculatesEvaluation(Chromosome<int> chromosome)
        {
            int pontoCorte = 100;
            float avaliacao = 0;
            int n = chromosome.Genes.Count;
            for (int i = 0; i < (n-1); i++)
            {
                var aresta = Grafo.ObtenhaAresta(
                    chromosome.Genes[i], chromosome.Genes[i + 1]);
                if(aresta != null)
                {
                    avaliacao += aresta.Peso;
                }
                else // caminho do cromossomo é inválido
                {
                    avaliacao = (-1 * pontoCorte);
                    break;
                }
            }
            return avaliacao / pontoCorte;
        }
    }
}
