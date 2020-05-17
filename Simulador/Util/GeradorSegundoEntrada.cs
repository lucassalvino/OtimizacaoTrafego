using System;
using System.Collections.Generic;

namespace Simulador.Util
{
    class GeradorSegundoEntrada
    {
        public static List<int> GeraSegundoEntradaVeiculo(int veiculosPorHora, int totSecSimulacao)
        {
            List<int> listSegundos = new List<int>();
            Random rnd = new Random();

            // Não irá gerar entradas pra intersecção!
            if (veiculosPorHora == -1)
            {
                listSegundos.Add(-1);
                return listSegundos;
            }
            double lambda = veiculosPorHora / 3600.00;
            int lastSec = 0;
            double difftime = 0.0;
            double sumtime = 0.0;
            int i = 0;
            do
            {

                difftime = -(Math.Log(1 - rnd.NextDouble()) / lambda); // Gera o tempo de diferença entre a incidencia dos veículos!
                sumtime += difftime; // A diferença entre o proximo veículo é somada com o tempo da incidência do último veículo!
                if (((int)sumtime == lastSec) && lastSec != 0) // Verifica se os veículos incidem no mesmo instante e se o último segundo de incidência é diferente de zero!
                    sumtime += 1; // Acrescenta 1 segundo de diferença!
                listSegundos.Add((int)sumtime);
                //Console.WriteLine("Seg entrada do veiculo {0}: {1}", (i + 1).ToString().PadLeft(2, '0'), (int)sumtime); //Debug
                lastSec = (int)sumtime;
                i++;
            } while ((int)sumtime < totSecSimulacao);

            return listSegundos;
        }
    }
}
