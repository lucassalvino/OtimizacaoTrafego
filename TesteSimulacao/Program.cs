using GeneticAlgorithm;
using Simulador;
using System;
using TesteSimulacao.AuxAG;

namespace TesteSimulacao
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                int numeroGeracoes = 200;
                Manager Simulacao = new Manager();
                Simulacao.QtdIteracoes = 3600;
                Simulacao.ImprimirLogTela = true;
                Simulacao.ImprimeLogOtimizacao = true;
                Simulacao.PastaLogEstradas = "D:/entrada/LogsEstradas";
                Simulacao.PastaLogsSemaforos = "D:/entrada/LogsSemaforos";
                Simulacao.PastaLogVeiculos = "D:/entrada/LogsVeiculos";
                Simulacao.PastaLogsVertices = "D:/entrada/LogsVertices";
                Simulacao.PastaLogsGerais = "D:/entrada/Gerais";

                //Simulacao.SetOtimizacaoIAAG = new GeneticAlgorithm<string>(numeroGeracoes, new AvaliacaoCromossomo());
                Simulacao.CarregaMapaSimulacao("D:/entrada/Mapa.json");
                
                Simulacao.IniciaSimulacao();

                Simulacao.SalvaLogs();
            }
            catch(Exception erro)
            {
                Console.WriteLine("Erro: \n");
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\t" + erro.Message);
                Console.ResetColor();
                Console.WriteLine($"\n\nStack trace: \n\t{erro.StackTrace}");
            }
        }
    }
}
