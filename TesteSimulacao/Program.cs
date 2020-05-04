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
                int numeroGeracoes = 50;
                Manager Simulacao = new Manager();
                Simulacao.SetOtimizacaoIAAG = new GeneticAlgorithm<string>(numeroGeracoes, new AvaliacaoCromossomo());
                Simulacao.CarregaMapaSimulacao("C:/entrada/Mapa.json");

                Simulacao.PastaLogEstradas = "C:/entrada/LogsEstradas";
                Simulacao.PastaLogsSemaforos = "C:/entrada/LogsSemaforos";
                Simulacao.PastaLogVeiculos = "C:/entrada/LogsVeiculos";
                Simulacao.PastaLogsVertices = "C:/entrada/LogsVertices";
                Simulacao.PastaLogsGerais = "C:/entrada/Gerais";

                Simulacao.QtdIteracoes = 1200;
                Simulacao.ImprimirLogTela = true;
                Simulacao.ImprimeLogOtimizacao = true;
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
