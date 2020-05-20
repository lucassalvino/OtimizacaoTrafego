using System;
using System.Collections.Generic;
using System.Text;

namespace Simulador.Util
{
    class FuncoesAuxiliares
    {
        public static decimal penalty(decimal S1, decimal S2, double p1, double p2, int newCycle)
        {
            double multiplifier = 1.5;
            if ((p1 + p2) > 1.0)
                multiplifier *= 2;
            else
            {
                if (newCycle > 140)
                    multiplifier *= 3;
                else
                {
                    if (S1 == S2 && ((S1 < 0.75M) && (S2 < 0.75M)))
                        multiplifier *= 5;
                }
            }
            return (decimal)multiplifier;
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

		public static decimal FuncaoObjetivo(double _x1, double _x2, double _capMax1, double _actualFlux1, double _capMax2, double _actualFlux2, double _tempoPerdido)
		{
			double x1, x2;
			decimal S1, S2;
			decimal result = 0.0M;

			//Parâmetros: Semáforos {Capacidade Máxima, Fluxo da Via}
			//x1 = (Factor * this.convertBooleanToValue(0, 7)) + 0.7;
			//x2 = (Factor * this.convertBooleanToValue(8, 15)) + 0.7;
			x1 = _x1;
			x2 = _x2;

			double capMax1 = _capMax1;
			double actualFlux1 = _actualFlux1;
			double tempoPerdido = _tempoPerdido;

			double yi1 = actualFlux1 / capMax1;
			double pi1 = yi1 / (double)x1;
			double newCycleTime = tempoPerdido / (1 - pi1);

			double verde1 = newCycleTime * pi1;
			double fracaoVerde1 = verde1 / newCycleTime;
			double newYi1 = yi1 * (1 / fracaoVerde1);


			double capMax2 = _capMax2;
			double actualFlux2 = _actualFlux2;
			double yi2 = actualFlux2 / capMax2;
			double pi2 = yi2 / (double)x2;

			double verde2 = newCycleTime * pi2;
			double fracaoVerde2 = verde2 / newCycleTime;
			double newYi2 = yi2 * (1 / fracaoVerde2);

			S1 = (decimal)newCycleTime * (decimal)Math.Pow((1 - (double)fracaoVerde1), 2) / (2 * (1 - (decimal)(fracaoVerde1 * newYi1)));
			S2 = (decimal)newCycleTime * (decimal)Math.Pow((1 - (double)fracaoVerde2), 2) / (2 * (1 - (decimal)(fracaoVerde2 * newYi2)));

			if (newCycleTime > 120)
			{
				result = ((S1 * 7 + S2 * 7) * penalty(S1, S2, pi1, pi2, (int)newCycleTime));
				return result;
			}
			else
			{
				if ((S1 < 0 || S2 < 0) || (S1 == S2))
				{
					result = ((S1 * 5 + S2 * 5) * penalty(S1, S2, pi1, pi2, (int)newCycleTime));
					return result;
				}
				else
				{
					if (S1 > S2 && pi2 > pi1)
					{
						result = ((S1 * 3 + S2) * penalty(S1, S2, pi1, pi2, (int)newCycleTime));
						return result;
					}
					else
					{
						if (S2 > S1 && pi1 > pi2)
						{
							result = ((S1 + S2 * 3) * penalty(S1, S2, pi1, pi2, (int)newCycleTime));
							return result;
						}
					}

				}
			}

			result = ((S1 * 2 + S2 * 2) * penalty(S1, S2, pi1, pi2, (int)newCycleTime));

			return result;
		}
	}
}
