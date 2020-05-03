using Simulador.AuxLogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulador.Entidades
{
    public class Rua : BaseEntidade
    {
        #region Propriedades
        public int Comprimento { get; set; }
        public int NumeroFaixas { get; set; }
        public int IdAresta { get; set; }
        public int VelocidadeMaxima { get; set; }
        public List<int> EspacoOcupado { get; set; } = new List<int>();
        public List<Queue<Veiculo>> VeiculosNaRua { get; set; } = new List<Queue<Veiculo>>();
        public List<LogMediaVelocidade> MediaVelocidadesPorInstante { get; set; } = new List<LogMediaVelocidade>();

        public int NumeroVeiculosNaVia 
        {
            get
            {
                var retorno = 0;
                VeiculosNaRua.ForEach(x => retorno += x.Count);
                return retorno;
            }
        }
        #endregion Propriedades

        #region Construtores
        public Rua(int Id = -1)
        {
            NumeroFaixas = -1;
            Comprimento = -1;
            VelocidadeMaxima = -1;
            this.Id = Id;
        }
        public Rua(int id, int numeroFaixas, int comprimento, int velocidadeMaxima, int idAresta)
        {
            Id = id;
            IdAresta = idAresta;
            NumeroFaixas = numeroFaixas;
            Comprimento = comprimento;
            VelocidadeMaxima = velocidadeMaxima;
            PreparaRua();
        }

        #endregion Construtores
        public bool AdicionaVeiculo(Veiculo novoVeiculo, int instanteTempo)
        {
            VerificaInicializacao();
            if (novoVeiculo == null)
                throw new Exception("Veiculo não foi setado");
            for (int i = 0; i < NumeroFaixas; i++)
            {
                if (Comprimento >= (EspacoOcupado[i] + novoVeiculo.Comprimento))
                {
                    novoVeiculo.LogVeiculo.VelocidadesTempo.Add(new LogVelocidadeVeiculo()
                    {
                        Velociadade = 0,
                        InstanteTempo = instanteTempo
                    });
                    VeiculosNaRua[i].Enqueue(novoVeiculo);
                    EspacoOcupado[i] += novoVeiculo.Comprimento;
                    return true;
                }
            }
            return false;
        }
        public Veiculo RemoveVeiculo()
        {
            VerificaInicializacao();
            for (int i = 0; i < NumeroFaixas; i++)
            {
                if (VeiculosNaRua[i].Count > 0)
                {
                    EspacoOcupado[i] -= VeiculosNaRua[i].Peek().Comprimento;
                    return VeiculosNaRua[i].Dequeue();
                }
            }
            return null;
        }
        #region Metodos
        public void PocessaFilaVeiculos(int SegundoSimalcao, List<Semaforo> Semaforos, int margemErroViaLotada = 2)
        {
            var semAt = Semaforos.Where(x => x.RuasOrigem.Contains(Id)).FirstOrDefault();
            bool Existesema = semAt != null;
            float somatorioMeida = 0;
            int qtdVeiculos = 0;
            for (int i = 0; i < NumeroFaixas; i++)
            {
                Queue<Veiculo> novaFila = new Queue<Veiculo>();
                List<Veiculo> veiculos = VeiculosNaRua[i].ToList();

                foreach (var veiculo in veiculos)
                {
                    qtdVeiculos++;
                    veiculo.PosicaoAtualNaVia += veiculo.Velocidade;
                    if (veiculo.PosicaoAtualNaVia <= Comprimento)
                    {
                        if (Existesema && semAt.EstadoSemaforo == Enuns.EstadosSemaforo.ABERTO)
                        {
                            veiculo.Velocidade += 1;
                        }
                        else
                        {
                            if (!Existesema)
                                veiculo.Velocidade += 1;
                        }

                        if (Existesema && semAt.EstadoSemaforo != Enuns.EstadosSemaforo.ABERTO)
                            veiculo.Velocidade -= 1;
                    }
                    else
                    {
                        veiculo.Velocidade -= 1;
                    }
                    #region tratalimitesVelocidade
                    if (veiculo.Velocidade < 0) veiculo.Velocidade = 0; // evita velocidade negativa (para quando o veiculo já se encontra parado
                    if (veiculo.Velocidade > VelocidadeMaxima) veiculo.Velocidade = VelocidadeMaxima; // se a velocidade final do veículo for superior a máxima da via, o veículo recebe a velocidade máxima da via
                    #endregion tratalimitesVelocidade
                    somatorioMeida += veiculo.Velocidade;

                    veiculo.LogVeiculo.VelocidadesTempo.Add(new LogVelocidadeVeiculo()
                    {
                        InstanteTempo = SegundoSimalcao,
                        Velociadade = veiculo.Velocidade
                    });

                    if ((EspacoOcupado[i] + margemErroViaLotada) >= Comprimento)
                    {
                        if (veiculo.VerticeAtual == veiculo.PercursoVeiculo.Last())
                        {
                            Veiculo car = RemoveVeiculo();
                        }
                        else
                        {
                            novaFila.Enqueue(veiculo);
                        }
                    }
                    else
                    {
                        novaFila.Enqueue(veiculo);
                    }
                }
                VeiculosNaRua[i] = novaFila;
            }
            MediaVelocidadesPorInstante.Add(new LogMediaVelocidade
            {
                InstanteTempo = SegundoSimalcao,
                VelocidadeMedia = qtdVeiculos == 0? 0: (somatorioMeida / qtdVeiculos)
            });
        }
        public float MediaOcupacaoVias()
        {
            return EspacoOcupado.Sum((x) => x) / NumeroFaixas;
        }
        #endregion Metodos

        #region MetodosPrivados
        public void PreparaRua()
        {
            if (Comprimento <= 0)
                throw new Exception("Defina o comprimento da rua");
            if (VelocidadeMaxima <= 0)
                throw new Exception("Defina a velocidade máxima do tráfego");
            if (NumeroFaixas <= 0)
                throw new Exception("Defina a quantidade de faixas");
            if (VeiculosNaRua.Count == 0)
            {
                for (int i = 0; i < NumeroFaixas; i++)
                {
                    VeiculosNaRua.Add(new Queue<Veiculo>());
                    EspacoOcupado.Add(0);
                }
            }
        }
        private void VerificaInicializacao()
        {
            if (VeiculosNaRua.Count != NumeroFaixas)
                throw new Exception("Rua ainda não foi inicializada");
        }
        #endregion MetodosPrivados
    }
}
