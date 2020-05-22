using System;
using System.Collections.Generic;
using System.Text;

namespace Simulador.Util
{
    class FuncoesAuxiliares
    {
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
        public static void penalty(ref string value, decimal saturacao_1, decimal saturacao_2, ref decimal p1, ref decimal p2, double cicloTempo, int sizeCromossomo)
        {
            string tmpCromossome;

            var x1 = ConvertBooleanToValue(0, 7, value);
            var x2 = ConvertBooleanToValue(8, 15, value);

            if ((saturacao_1 > saturacao_2) && (p2 > p1))
            {
                while (true)
                {
                    tmpCromossome = initializeElement(sizeCromossomo);
                    x1 = ConvertBooleanToValue(0, 7, tmpCromossome);
                    x2 = ConvertBooleanToValue(8, 15, tmpCromossome);

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
                    x1 = ConvertBooleanToValue(0, 7, tmpCromossome);
                    x2 = ConvertBooleanToValue(8, 15, tmpCromossome);

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
                    x1 = ConvertBooleanToValue(0, 7, tmpCromossome);
                    x2 = ConvertBooleanToValue(8, 15, tmpCromossome);

                    p1 = (decimal)(x1 / cicloTempo);
                    p2 = (decimal)(x2 / cicloTempo);

                    if ((p1 + p2) < 1 && (x1 > 15 && x2 > 15))
                    {
                        value = tmpCromossome;
                        break;
                    }
                }
            }
        }
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
    }
}
