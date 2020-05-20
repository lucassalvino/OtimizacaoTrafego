using System.Collections.Generic;

namespace Simulador.Entidades
{
    public class Cruzamento
    {
        #region Propriedades
        public int RuaOrigem { get; set; }
        public int RuaDestino { get; set; }
        public int carrosIniEstagio { get; set; }
        public int segundoIniEstagio { get; set; }
        #endregion Propriedades
        public void contagemEstagioInicial(int instanteSimulacao, int tempoSemaforo)
        {
            // Verifica se a já se passou um minuto na simulação
            if(instanteSimulacao%60 != 0)
            {
                
            }
        }
        #region Metodos

        #endregion Metodos
    }
}
