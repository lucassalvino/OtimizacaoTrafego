using Newtonsoft.Json;
using Simulador.AuxLogs;
using Simulador.Entidades.Enuns;
using System;
using System.Collections.Generic;

namespace Simulador.Entidades
{
    public class Semaforo : BaseEntidade
    {
        #region Propriedades
        public int TempoAberto { get; set; }
        public int TempoFechado { get; set; }
        public int TempoAmarelo { get; set; }
        public int TempoAtual { get; set; } = 0;
        public int ProximoTempoAberto { get; set; }
        public int ProximoTempoFechado { get; set; }
        public int CicloTempo { get; set; }
        [JsonConverter(typeof(EstadosSemaforo))]
        public EstadosSemaforo EstadoSemaforo { get; set; }
        public List<int> RuasOrigem { get; set; } = new List<int>();
        public List<int> RuasDestino { get; set; } = new List<int>();
        public List<LogSemaforos> LogSemaforos { get; set; } = new List<LogSemaforos>();
        #endregion Propriedades
        #region Contrutor
        public Semaforo()
        {
            Id = -1;
            TempoAberto = -1;
            TempoFechado = -1;
            TempoAmarelo = -1;
            TempoAtual = 0;
            EstadoSemaforo = EstadosSemaforo.FECHADO;
            LogSemaforos = new List<LogSemaforos>();
        }
        #endregion Contrutor
        #region Metodos
        public void InicializaSemaforo(int id, int tempoAberto, int tempoFechado, int tempoAmarelo, EstadosSemaforo Estado)
        {
            if (id < 0) throw new Exception("O Id do semáforo deve ser maior ou igual a zero");
            if (tempoAberto <= 0 || tempoAmarelo <= 0 || tempoFechado <= 0)
                throw new Exception("Os valores de tempo devem ser maior do que zero");
            Id = id;
            ProximoTempoAberto = TempoAberto = tempoAberto;
            ProximoTempoFechado =  TempoFechado = tempoFechado;
            TempoAmarelo = tempoAmarelo;
            EstadoSemaforo = Estado;
            RuasOrigem.Clear();
            RuasDestino.Clear();
            TempoAtual = 0;
        }
        public void AtualizaStatusSemaforo(int tempoDecorrido, int instanteTempo)
        {
            TempoAtual += tempoDecorrido;
            switch (EstadoSemaforo)
            {
                case EstadosSemaforo.ABERTO:
                    if (TempoAtual >= TempoAberto)
                    {
                        TempoAtual = 0;
                        EstadoSemaforo = EstadosSemaforo.AMARELO;
                    }
                    break;
                case EstadosSemaforo.FECHADO:
                    if (TempoAtual >= TempoFechado)
                    {
                        TempoAberto = ProximoTempoAberto;
                        TempoAtual = 0;
                        EstadoSemaforo = EstadosSemaforo.ABERTO;
                        
                        LogSemaforos.Add(new LogSemaforos { 
                            InstanteTempo = instanteTempo,
                            TempoAberto = TempoAberto,
                            TempoFechado = TempoFechado
                        });
                    }
                    break;
                case EstadosSemaforo.AMARELO:
                    if (TempoAtual >= TempoAmarelo)
                    {
                        TempoFechado = ProximoTempoFechado;
                        TempoAtual = 0;
                        EstadoSemaforo = EstadosSemaforo.FECHADO;

                        LogSemaforos.Add(new LogSemaforos
                        {
                            InstanteTempo = instanteTempo,
                            TempoAberto = TempoAberto,
                            TempoFechado = TempoFechado
                        });
                    }
                    break;
            }
        }
        #endregion Metodos
    }
}
