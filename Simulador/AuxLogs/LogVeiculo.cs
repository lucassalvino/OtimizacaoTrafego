using System.Collections.Generic;

namespace Simulador.AuxLogs
{
    public class LogVeiculo
    {
        public int IdVeiculo { get; set; }
        public int InstanteCriacao { get; set; }
        public int VerticeOrigem { get; set; }
        public int VerticeDestino { get; set; }
        public List<LogVelocidadeVeiculo> VelocidadesTempo { get; set; } = new List<LogVelocidadeVeiculo>();
    }
}
