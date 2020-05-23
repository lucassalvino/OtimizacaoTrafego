using GeneticAlgorithm;
using Simulador.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulador.Util
{
    public class FuncoesAuxiliares
    {
        public static List<string> initializeElement(int elementSize)
        {
            Random rand = new Random();
            var novo = new List<string>(elementSize);
            StringBuilder strcromossomo = new StringBuilder();
            string cromossomo = "";
            while (cromossomo.Count() < elementSize)
            {
                if (rand.NextDouble() < 0.5)
                    cromossomo += "1";
                else
                    cromossomo += "0";
            }
            
            strcromossomo.Append(cromossomo);
            
            var str = strcromossomo.ToString();
            for (int i = 0; i < str.Length; i++)
                novo.Add(str[i].ToString());
            return novo;
        }
        public static void penalty(ref List<string> value, decimal saturacao_1, decimal saturacao_2, ref decimal p1, ref decimal p2, double cicloTempo, int sizeCromossomo)
        {
            List<string> tmpCromossome;

            var x1 = ConvertBooleanToValue(0, 6, value);
            var x2 = ConvertBooleanToValue(7, 13, value);
            
            if(x1 == 0 || x2 == 0)
            {
                initializeElement(14);
            }

            if ((saturacao_1 > saturacao_2) && (p2 > p1))
            {
                while (true)
                {
                    tmpCromossome = initializeElement(sizeCromossomo);
                    x1 = ConvertBooleanToValue(0, 6, tmpCromossome);
                    x2 = ConvertBooleanToValue(7, 13, tmpCromossome);

                    p1 = (decimal)(x1 / cicloTempo);
                    p2 = (decimal)(x2 / cicloTempo);

                    if ((p1 > p2) && ((p1 + p2) == 1))
                    {
                        value = tmpCromossome;
                        break;
                    }

                }
            }
            else if (saturacao_2 > saturacao_1)
            {
                while (true)
                {
                    tmpCromossome = initializeElement(sizeCromossomo);
                    x1 = ConvertBooleanToValue(0, 6, tmpCromossome);
                    x2 = ConvertBooleanToValue(7, 13, tmpCromossome);

                    p1 = (decimal)(x1 / cicloTempo);
                    p2 = (decimal)(x2 / cicloTempo);

                    if ((p2 > p1) && ((p1 + p2) == 1))
                    {
                        value = tmpCromossome;
                        break;
                    }
                }
            }
            else if ((p1 + p2) >= 1)
            {
                while (true)
                {
                    tmpCromossome = initializeElement(sizeCromossomo);
                    x1 = ConvertBooleanToValue(0, 6, tmpCromossome);
                    x2 = ConvertBooleanToValue(7, 13, tmpCromossome);

                    p1 = (decimal)(x1 / cicloTempo);
                    p2 = (decimal)(x2 / cicloTempo);

                    if ((p1 + p2) == 1 && (x1 > 15 && x2 > 15))
                    {
                        value = tmpCromossome;
                        break;
                    }
                }
            }
        }

        public static int ConvertBooleanToValue(int inicio, int fim, List<string> value)
        {
            int i;
            int aux = 0;
            var s = value;
            
            for (i = inicio; i <= fim; ++i)
            {
                aux *= 2;
                if (s[i] == "1")
                {
                    aux += 1;
                }
            }
            return (aux);
        }

        public static decimal FuncaoObjetivo(ref List<string> chromossome, List<Semaforo> semaforos, List<Rua> ruas)
        {

            double x1, x2;
            decimal S1, S2;

            var rua1 = ruas.Where(x => semaforos[0].RuasOrigem.Contains(x.IdAresta)).FirstOrDefault();
            var rua2 = ruas.Where(x => semaforos[1].RuasOrigem.Contains(x.IdAresta)).FirstOrDefault();

            x1 = FuncoesAuxiliares.ConvertBooleanToValue(0, 6, chromossome);
            x2 = FuncoesAuxiliares.ConvertBooleanToValue(7, 13, chromossome);


            decimal capMax1 = rua1.CapacidadeMaxima;
            decimal actualFlux1 = rua1.VeiculosPorHora;
            double tempoCiclo = semaforos[0].CicloTempo;

            decimal capMax2 = rua2.CapacidadeMaxima;
            decimal actualFlux2 = rua2.VeiculosPorHora;


            decimal yi1 = actualFlux1 / capMax1;
            decimal yi2 = actualFlux2 / capMax2;

            // Calculando como x representando um tempo de verde!
            decimal p1 = (decimal)(x1 / tempoCiclo);
            decimal p2 = (decimal)(x2 / tempoCiclo);

            
            var tmp = chromossome;

            penalty(ref tmp, yi1, yi2, ref p1, ref p2, tempoCiclo, chromossome.Count);

            if (!tmp.Equals(chromossome))
            {
                chromossome = tmp;
            }
            
            S1 = ((decimal)tempoCiclo * (decimal)Math.Pow((1 - (double)p1), 2) / (2 * (1 - (decimal)(p1 * yi1))));
            S2 = ((decimal)tempoCiclo * (decimal)Math.Pow((1 - (double)p2), 2) / (2 * (1 - (decimal)(p2 * yi2))));

            return  (1 / (S1 + S2));
        }
    }
}
