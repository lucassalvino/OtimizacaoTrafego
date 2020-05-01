using GeneticAlgorithm;
using System;
using System.Linq;
using System.Collections.Generic;
using TesteConsoleAlgoritmoGenetico.CaxeiroViajante;
using TesteConsoleAlgoritmoGenetico.Grafo;

namespace TesteConsoleAlgoritmoGenetico
{
    class Program
    {
        static void Main(string[] args)
        {
            int numeroGeracoes = 200;
            int numeroIndividuos = 100;
            int numeroGenesIndividos = 5;
            GerenteGrafo grafo = new GerenteGrafo();
            CalculaAvaliacao calcula = new CalculaAvaliacao();
            #region CriaGrafo
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if(i == j)
                        grafo.AdicionaAresta(i, j, 0);
                    else
                        grafo.AdicionaAresta(i, j, 1);

            Console.WriteLine("Grafo:");
            foreach (var aresta in grafo.Arestas)
            {
                Console.WriteLine($"\t{aresta.Origem.NumeroVertice} -> {aresta.Destino.NumeroVertice} : {aresta.Peso}");
            }
            #endregion CriaGrafo
            
            calcula.Grafo = grafo;
            GeneticAlgorithm<int> ag = new GeneticAlgorithm<int>(numeroGeracoes, calcula);
            #region PopulacaoInicial
            List<Chromosome<int>> populacaoinicial = new List<Chromosome<int>>();
            Random rand = new Random();
            for(int i =0; i<numeroIndividuos; i++)
            {
                var novo = new Chromosome<int>(numeroGenesIndividos);
                for(int j = 0; j < numeroGenesIndividos; j++)
                {
                    int gene = rand.Next() % numeroGenesIndividos;
                    if(novo.Genes != null)
                        while(novo.Genes.Contains(gene))
                            gene = rand.Next() % numeroGenesIndividos;
                    novo.AddGene(gene);
                }
                populacaoinicial.Add(novo);
            }
            #endregion PopulacaoInicial
            ag.DefineInitialPopulation(numeroIndividuos, numeroGenesIndividos, populacaoinicial);
            ag.Run();
            var melhor = ag.GetBestChromosome();
            Console.WriteLine(melhor.GetJson());
        }
    }
}
