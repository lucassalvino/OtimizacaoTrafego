using GeneticAlgorithm;
using Simulador.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteSimulacao.AuxAG
{
    public class AvaliacaoCromossomoAtrasoMedio : BaseEvaluation<string>
    {
        public List<Semaforo> Semaforos { get; set; } = null;
        public List<Rua> Ruas { get; set; } = null;
        public List<semaforoTemporario> ConverteCromossomoParaSemaforo(List<string> Cromossomo)
        {
            var retorno = new List<semaforoTemporario>();
            int qtdGenesCromossomo = Cromossomo.Count / Semaforos.Count; // numeros de genes por cromossomo
            int pat = 0; // posicao atual
            for (int i = 0; i < Semaforos.Count; i++)
            {
                StringBuilder sm = new StringBuilder();
                for (int j = 0; j < qtdGenesCromossomo; j++)
                {
                    sm.Append(Cromossomo[pat + j]);
                }
                pat += qtdGenesCromossomo;
                var semaforo = new semaforoTemporario
                {
                    aberto = Convert.ToInt32(sm.ToString().Substring(0, qtdGenesCromossomo / 2), 2),
                    fechado = Convert.ToInt32(sm.ToString().Substring(qtdGenesCromossomo / 2), 2)
                };
                retorno.Add(semaforo);
            }
            return retorno;
        }
        public override float CalculatesEvaluation(Chromosome<string> chromosome)
        {
            if (Param1 == null || Param2 == null)
                return base.CalculatesEvaluation(chromosome);
            Semaforos = (List<Semaforo>)Param1;
            Ruas = (List<Rua>)Param2;
            if (Semaforos == null || Ruas == null)
                throw new NullReferenceException("Null parameters");
            float avaliacao = 0;
            var semaforosCromossomo = ConverteCromossomoParaSemaforo(chromosome.Genes);
            for (int i = 0; i < Semaforos.Count() - 1; i++)
            {
               // avaliacao += FatorCromossomo(i, semaforosCromossomo[i], semaforosCromossomo[i + 1]);
            }
            return avaliacao;
        }
    }
}
