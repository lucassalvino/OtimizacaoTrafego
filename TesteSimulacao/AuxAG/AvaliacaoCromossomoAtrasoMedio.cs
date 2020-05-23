using GeneticAlgorithm;
using Simulador.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simulador.Util;

namespace TesteSimulacao.AuxAG
{
    public class semaforosCruzamento
    {
        public int verdeSemaforo1;
        public int verdeSemaforo2;
    }
    public class AvaliacaoCromossomoAtrasoMedio : BaseEvaluation<string>
    {
        public List<Semaforo> Semaforos { get; set; } = null;
        public List<Rua> Ruas { get; set; } = null;
        public List<semaforosCruzamento> ConverteCromossomoParaSemaforo(List<string> Cromossomo)
        {
            var retorno = new List<semaforosCruzamento>();
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
                var semaforo = new semaforosCruzamento
                {
                    verdeSemaforo1 = Convert.ToInt32(sm.ToString().Substring(0, qtdGenesCromossomo / 2), 2),
                    verdeSemaforo2 = Convert.ToInt32(sm.ToString().Substring(qtdGenesCromossomo / 2), 2)
                };
                retorno.Add(semaforo);
            }
            return retorno;
        }

        public float FatorCromossomo(int index, semaforoTemporario configuracao, semaforoTemporario proximaConfiguracao)
        {
            var ruasEntrada = Semaforos[index].RuasOrigem;
            var ruasDestino = Semaforos[index].RuasDestino;
            float somatorioLivre = Ruas.Where(x => ruasEntrada.Contains(x.Id)).Select(x => (x.Comprimento * x.EspacoOcupado.Count) - x.EspacoOcupado.Sum()).Sum();
            float somatorioOcupado = Ruas.Where(x => ruasEntrada.Contains(x.Id)).Select(x => x.EspacoOcupado.Sum()).Sum();
            float espacoLivreProximo = Ruas.Where(x => ruasDestino.Contains(x.Id)).Select(x => (x.Comprimento * x.EspacoOcupado.Count) - x.EspacoOcupado.Sum()).Sum();

            float fator1 = (somatorioLivre + somatorioOcupado) / (somatorioOcupado + configuracao.aberto);
            float fator2 = (espacoLivreProximo - (somatorioOcupado / configuracao.aberto));
            if (fator2 < 0)
                fator2 *= -1;
            fator2 = fator2 - proximaConfiguracao.aberto;
            return fator1 - fator2;

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
            var tmp = chromosome.Genes;
            avaliacao = (float)FuncoesAuxiliares.FuncaoObjetivo(ref tmp, Semaforos, Ruas);
            if(!chromosome.Genes.Equals(tmp))
            {
                chromosome.Genes.Clear();
                for (int i = 0; i < tmp.Count; i++)
                    chromosome.Genes.Add(tmp[i]);
            }
            return avaliacao;
        }
    }
}
