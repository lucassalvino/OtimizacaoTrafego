using System;
using System.Collections.Generic;
using System.Text;

namespace Simulador.Util
{
    class FuncoesAuxiliares
    {
        private static float ConvertBooleanToValue(int inicio, int fim, string value)
        {
            int i;
            float aux = 0;
            string s = value;
            for (i = inicio; i <= fim; ++i)
            {
                aux *= 2;
                if (s.Substring(i, 1).Equals("1"))
                {
                    aux += 1;
                }
            }
            return (aux);
        }
        public static void changeCromossome(int sizeChromossome, int posIni, int posEnd, ref string value)
        {
            string aux = "";
            for (int i = 0; i < sizeChromossome; i++)
            {
                if (new Random().NextDouble() < 0.5)
                    aux += "1";
                else
                    aux += "0";
            }

            if (posIni == 0 && posEnd == -1)
            {
                string aux2 = value.Substring(posIni, sizeChromossome);
                aux2 += aux;
                value = aux2;
            }
            else if (posIni == -1 && posEnd != -1)
            {
                aux += value.Substring(posEnd);
                value = aux;
            }
        }
        public static string initializeElement(int elementSize)
        {
            int i;
            string value = "";
            for (i = 0; i < elementSize; i++)
            {
                if (new Random().NextDouble() < 0.5)
                {
                    value = value + "0";
                }
                else
                {
                    value = value + "1";
                }
            }

            return value;
        }
        public static void penalty2(ref string value, double saturacao_1, double saturacao_2, ref double p1, ref double p2, double cicloTempo, int sizeCromossomo)
        {
            string tmpCromossome;
            if ((saturacao_1 > saturacao_2) && (p2 > p1))
            {
                while (true)
                {
                    tmpCromossome = initializeElement(sizeCromossomo);
                    var x1 = ConvertBooleanToValue(0, 3, tmpCromossome);
                    var x2 = ConvertBooleanToValue(4, 7, tmpCromossome);

                    p1 = x1 / cicloTempo;
                    p2 = x2 / cicloTempo;

                    if ((p1 > p2) && ((p1 + p2) < 1))
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
                    var x1 = ConvertBooleanToValue(0, 3, tmpCromossome);
                    var x2 = ConvertBooleanToValue(4, 7, tmpCromossome);

                    p1 = x1 / cicloTempo;
                    p2 = x2 / cicloTempo;

                    if ((p2 > p1) && ((p1 + p2) < 1))
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
                    var x1 = ConvertBooleanToValue(0, 3, tmpCromossome);
                    var x2 = ConvertBooleanToValue(4, 7, tmpCromossome);

                    p1 = x1 / cicloTempo;
                    p2 = x2 / cicloTempo;

                    if ((p1 + p2) < 1 && (x1 > 15 && x2 > 15))
                    {
                        value = tmpCromossome;
                        break;
                    }
                }
            }
        }

        public List<int> getDataSemaphore(double newCycle, double p, int amarelo)
        {
            List<int> ret = new List<int>();
            int tempoVerde = (int)(newCycle * p);
            int tempoVermelho = (int)(newCycle - (tempoVerde + amarelo));
            ret.Add(tempoVerde);
            ret.Add(tempoVermelho);

            return ret;
        }

		
	}
}
