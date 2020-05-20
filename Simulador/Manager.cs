using BaseGrafo;
using GeneticAlgorithm;
using Newtonsoft.Json;
using Simulador.AuxLogs;
using Simulador.Entidades;
using Simulador.Entidades.Leitura;
using Simulador.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Simulador
{
    public class Manager
    {
        public string PastaLogVeiculos { get; set; }
        public string PastaLogEstradas { get; set; }
        public string PastaLogsSemaforos { get; set; }
        public string PastaLogsVertices { get; set; }
        public string PastaLogsGerais { get; set; }
        public string PastaCruzamento { get; set; }
        public GeneticAlgorithm<string> SetOtimizacaoIAAG { get; set; }

        #region Metodos
        public void CarregaMapaSimulacao(string CaminhoArquivoSimulacao)
        {
            Entrada DadosEntrada = new Entrada(); ;
            using (StreamReader file = new StreamReader(CaminhoArquivoSimulacao))
            {
                if (file == null)
                    throw new Exception($"Arquivo {CaminhoArquivoSimulacao} não foi encontrado!");
                string conteudoArquivo = file.ReadToEnd();
                DadosEntrada = JsonConvert.DeserializeObject<Entrada>(conteudoArquivo);
                file.Close();
            }
            if (DadosEntrada == null)
                throw new Exception("Não foi possível realizar a serialização do arquivo de entrada de dados");

            #region ProcessaEntrada
            int auxId = 0;

            /// grafo e ruas
            foreach (var item in DadosEntrada.Ruas)
            {
                grafo.AdicionaAresta(item.VerticeOrigem, item.VerticeDestino, item.Distancia, auxId);
                Aresta aresta = grafo.ObtenhaAresta(item.VerticeOrigem, item.VerticeDestino);
                Rua ruaAdicionar = new Rua()
                {
                    Comprimento = aresta.Peso,
                    NumeroFaixas = item.NumeroVias,
                    IdAresta = aresta.Id,
                    VelocidadeMaxima = item.VelocidadeMaxima,
                    Id = auxId++,
                    Descricao = $"Rua sentido {aresta.Origem} até {aresta.Destino}",
                };
                ruaAdicionar.PreparaRua();
                RuasSimulacao.Add(ruaAdicionar);
            }

            /// comprimento veiculos
            foreach (var item in DadosEntrada.ComprimentosVeiculos)
            {
                if (item <= 0)
                    throw new Exception("Um comprimento não pode ser menor ou igual a zero");
                geradorVeiculos.AdicionarComprimentoPossivel(item);
            }

            /// velocidade inicial
            foreach (var item in DadosEntrada.VelocidadeInicial)
            {
                if (item <= 0)
                    throw new Exception("A velocidade inicial não pode ser menor ou igual a zero");
                geradorVeiculos.AdicionarVelocidadePossivel(item);
            }

            /// Semaforos
            auxId = 0;
            foreach (var item in DadosEntrada.Semaforos)
            {
                Semaforo auxSema = new Semaforo()
                {
                    Id = auxId++,
                    TempoAberto = item.TempoAberto,
                    TempoAmarelo = item.TempoAmarelo,
                    TempoFechado = item.TempoFechado,
                    EstadoSemaforo = Entidades.Enuns.EstadosSemaforo.ABERTO,
                    ProximoTempoAberto = item.TempoAberto,
                    ProximoTempoFechado = item.TempoFechado,
                    TempoAtual = 0
                };
                auxSema.LogSemaforos.Add(new LogSemaforos { 
                    InstanteTempo = 0,
                    TempoAberto = item.TempoAberto,
                    TempoFechado = item.TempoFechado
                });

                Rua RuaOrigem = GetRua(item.VerticeOrigemOrigem, item.VerticeDestinoOrigem);
                Rua RuaDestino = GetRua(item.VerticeOrigemDestino, item.VerticeDestinoDestino);
                if (item.VerticeOrigemDestino == item.VerticeDestinoDestino && RuaDestino == null)//Ultimo semáforo e sem rua depois do semáforo
                    throw new Exception("Não existe uma rua após o último semáforo.");
                if (RuaOrigem == null || RuaDestino == null)
                    throw new Exception("Rua de Origem/Destino não foi encontrada.");
                auxSema.RuasOrigem.Add(RuaOrigem.Id);
                auxSema.RuasDestino.Add(RuaDestino.Id);
                Semaforos.Add(auxSema);
            }
            // taxa de geracao veiculos
            TaxaGeracao.AddRange(DadosEntrada.TaxasGeracao.OrderBy((x) => x.Vertice).Select((x) => x.Taxa));
            if (TaxaGeracao.Count != grafo.NumeroVertices)
                throw new Exception("Quantidade de taxas de geração está incompleta!");

            // Inicializar a as listas de geração aqui!
            // Parametros: Taxa de Geração de veículos, e QtdDeIterações (Duração da Simulação)
            foreach (var veiculosPorHora in TaxaGeracao)
            {
                ListaDeTempoEntradaVeiculosVertices.Add(GeradorSegundoEntrada.GeraSegundoEntradaVeiculo(veiculosPorHora, QtdIteracoes));
            }
            #endregion ProcessaEntrada
        }
        public void IniciaSimulacao()
        {
            SegundoSimulacao = 0;
            IdVeiculo = 0;
            if (!VerificaCarregamentoDados())
                throw new Exception("Carregue os dados da simulacao");
            inicializaFilaEsperaVerice();
            while (SegundoSimulacao < QtdIteracoes)
            {
                GeradoraVeiculos();
                ProcessaVeiculoSimulacao();
                ProcessaSemaforos();
                SegundoSimulacao++;
            }
        }
        private void SalvaLog(List<string> Log, string nomeArquivo)
        {
            using (StreamWriter file = new StreamWriter(nomeArquivo))
            {
                file.Write(string.Join("\n", Log));
                file.Close();
            }
        }
        private void verificaPasta(string pasta)
        {
            if (!Directory.Exists(pasta))
            {
                var di = Directory.CreateDirectory(pasta);
                if (!di.Exists)
                    throw new Exception($"Não é possível escrever em [{pasta}]");
            }
        }
        public void SalvaLogs()
        {
            // salva log veiculos
            if (!string.IsNullOrEmpty(PastaLogVeiculos))
            {
                verificaPasta(PastaLogVeiculos);
                Console.WriteLine("Salvando Logs veículos");
                foreach (var veiculo in VeiculosSimulacao)
                {
                    List<string> logs = new List<string>();
                    logs.Add($"Instante tempo;Velocidade veículo");
                    foreach (var item in veiculo.LogVeiculo.VelocidadesTempo)
                    {
                        logs.Add($"{item.InstanteTempo};{item.Velociadade}");
                    }
                    SalvaLog(logs, $"{PastaLogVeiculos}/{veiculo.Id}.csv");
                }
                Console.WriteLine("Log veículos salvos");
            }
            // salva log gerais
            if (!string.IsNullOrEmpty(PastaLogsGerais))
            {
                verificaPasta(PastaLogsGerais);
                Console.WriteLine("Salvando logs gerais");
                List<string> logs = new List<string>();
                //Origem veiculos por vertice
                int n = grafo.NumeroVertices;
                int[] origem = new int[n];
                int[] destinos = new int[n];
                for(int i = 0; i<n; i++)
                {
                    origem[i] = 0;
                    destinos[i] = 0;
                }
                VeiculosSimulacao.ForEach(x =>
                {
                    if(x.PercursoVeiculo.Count >= 0)
                    {
                        origem[x.PercursoVeiculo.First()] += 1;
                        destinos[x.PercursoVeiculo.Last()] += 1;
                    }
                });
                logs.Add("vertice; quantidade origem; quantidade destino");
                for(int i = 0; i < n; i++)
                {
                    logs.Add($"{i};{origem[i]};{destinos[i]}");
                }
                SalvaLog(logs, $"{PastaLogsGerais}/OrigemDestinos.csv");

                #region VelocidadeMediaVeiculos
                Console.WriteLine("Gerando logs de velocidade média");
                logs = new List<string>();
                logs.Add("instante tempo; media velocidade");
                for (int i = 0; i < QtdIteracoes; i++)
                {
                    var veiculosComVelocidadeSegundo = VeiculosSimulacao.Where(x => x.LogVeiculo.VelocidadesTempo.Where(y => y.InstanteTempo == i).Count()>0 && x.VerticeAtual != x.PercursoVeiculo.LastOrDefault()).ToList();
                    var somatorio = veiculosComVelocidadeSegundo.Select(x => x.LogVeiculo.VelocidadesTempo.Where(y => y.InstanteTempo == i).ToList()).Sum(x => x.Sum(y => y.Velociadade));
                    var media = (float)somatorio / veiculosComVelocidadeSegundo.Count();
                    logs.Add($"{i};{media}");
                }
                SalvaLog(logs, $"{PastaLogsGerais}/MeidaVelocidadePorTempo.csv");
                #endregion VelocidadeMediaVeiculos

                #region VelocidadeMediaPorRua
                Console.WriteLine("Gerando logs de velocidade média por rua por tempo");
                verificaPasta($"{PastaLogsGerais}/MeidaVelocidadeRuas");
                foreach(var rua in RuasSimulacao)
                {
                    List<string> log = new List<string>();
                    log.Add("Instante Tempo;Velocidade Média");
                    foreach (var logInstante in rua.MediaVelocidadesPorInstante)
                    {
                        log.Add($"{logInstante.InstanteTempo}; {logInstante.VelocidadeMedia}");
                    }
                    SalvaLog(log, $"{PastaLogsGerais}/MeidaVelocidadeRuas/{rua.Id}.csv");
                }
                #endregion VelocidadeMediaPorRua
                Console.WriteLine("Logs Gerais salvos");
            }

            // salva logs semaforos
            if (!string.IsNullOrEmpty(PastaLogsSemaforos))
            {
                verificaPasta(PastaLogsSemaforos);
                Console.WriteLine("Salvando logs semáforos");
                foreach(var semaforo in Semaforos)
                {
                    var logs = new List<string>();
                    logs.Add("Instante de tempo; tempo aberto (s); tempo fechado(2)");
                    foreach (var item in semaforo.LogSemaforos)
                    {
                        logs.Add($"{item.InstanteTempo};{item.TempoAberto};{item.TempoFechado}");
                    }
                    SalvaLog(logs, $"{PastaLogsSemaforos}/{semaforo.Id}.csv");
                }
                Console.WriteLine("Logs semáforos salvos");
            }
            // salva logs LogsEstradas
            if (!string.IsNullOrEmpty(PastaLogEstradas))
            {
                verificaPasta(PastaLogEstradas);
                Console.WriteLine("Iniciando geração de log de estradas");
                for (int i = 0; i < grafo.NumeroArestas; i++)
                {
                    List<string> LogSalvar = new List<string>();
                    List<LogOcupacaoVias> salvarAresta = LogOcupacaoVias.Where((x => x.IdAresta == i)).ToList();
                    LogSalvar.Add("Instante de tempo; Espaco Ocupado; Quantidade de veículos");
                    foreach (var item in salvarAresta)
                    {
                        LogSalvar.Add($"{item.InstanteTempo};{item.EspacoOcupado};{item.QuantidadadeVeiculos}");
                    }
                    SalvaLog(LogSalvar, $"{PastaLogEstradas}/{i}.csv");
                }
                Console.WriteLine("Logs de estradas salvos");
            }
            if (!string.IsNullOrEmpty(PastaLogsVertices))
            {
                verificaPasta(PastaLogsVertices);
                Console.WriteLine("Iniciando geração de logs dos vertices");
                for(int i = 0; i<grafo.NumeroVertices; i++)
                {
                    List<string> logs = new List<string>();
                    logs.Add("Instante tempo;Quantidade de veiculos;Espaco ocupado");
                    LogQtdVeiculosEsperaTempo.Where(x => x.Vertice == i).ToList().ForEach(x =>
                    {
                        logs.Add($"{x.InstanteTempo};{x.QtdVeiculos};{x.EspacoOcupado}");
                    });
                    SalvaLog(logs,$"{PastaLogsVertices}/{i}.csv");
                }
                Console.WriteLine("Logs de vertices salvos");
            }
        }
        public Grafo GetGrafoSimulacao()
        {
            return grafo;
        }
        public Rua GetRua(int origem, int destino)
        {
            return RuasSimulacao.Where((x) => x.IdAresta == grafo.ObtenhaAresta(origem, destino)?.Id).FirstOrDefault();
        }
        public bool ImprimirLogTela { get; set; }
        public bool ImprimeLogOtimizacao { get; set; }
        
        #endregion Metodos

        private List<Chromosome<string>> CriaPopulacaoInicial(int numeroIndividuos)
        {
            int ValorMinimo = 30;
            int valorMaximo = 120;
            int numeroGenesCromossomo = 14;// cada tempo tem o valor máximo de 127 (7 bits)
            List<Chromosome<string>> populacaoinicial = new List<Chromosome<string>>();
            Random rand = new Random();
            for (int i = 0; i < numeroIndividuos; i++)
            {
                var novo = new Chromosome<string>(numeroGenesCromossomo*Semaforos.Count);
                StringBuilder strcromossomo = new StringBuilder();
                for (int j = 0; j<Semaforos.Count(); j++)
                {
                    int tempoaberto = 0;
                    int tempofechado = 0;
                    while (tempoaberto < ValorMinimo)
                        tempoaberto = rand.Next() % valorMaximo;
                    while (tempofechado < ValorMinimo)
                        tempofechado = rand.Next() % valorMaximo;
                    string cromossomo = $"{Convert.ToString(tempoaberto, 2)}{Convert.ToString(tempofechado, 2)}";
                    while (cromossomo.Count() < numeroGenesCromossomo)
                    {
                        if(rand.Next()%2 == 0)
                            cromossomo = cromossomo.Insert(0, "0");
                        else
                            cromossomo = cromossomo.Insert(1, "1");
                    }
                    strcromossomo.Append(cromossomo);
                }
                var str = strcromossomo.ToString();
                for (int j = 0; j < str.Length; j++)
                    novo.AddGene(str[j].ToString());
                populacaoinicial.Add(novo);
            }
            return populacaoinicial;
        }

        #region MetodosPrivados
        private void GeradoraVeiculos()
        {
            if (ImprimirLogTela)
                Console.WriteLine("Iniciando rotina de geração de veículos");
            int n = grafo.NumeroVertices;
            int idx;
            for (int i = 0; i < n; i++)
            {
                idx = -1;
                var segundoVertice = ListaDeTempoEntradaVeiculosVertices[i];
                idx = segundoVertice.LastIndexOf(SegundoSimulacao);
                if(idx != -1)
                {
                    // gera veiculo e inicializa log de veiculos
                    Veiculo veiculoAdicionar = geradorVeiculos.GeraVeiculoAleatorio(IdVeiculo, grafo, i);

                    veiculoAdicionar.LogVeiculo = new LogVeiculo()
                    {
                        IdVeiculo = veiculoAdicionar.Id,
                        InstanteCriacao = SegundoSimulacao,
                        VerticeOrigem = i,
                        VerticeDestino = veiculoAdicionar.PercursoVeiculo.Last()
                    };
                    // veiculo no vertice aguardando para começar a trafegar
                    VeiculosEsperaVertice[i].Enqueue(veiculoAdicionar);

                    #region TratativaLogs
                    if (ImprimirLogTela)
                        Console.WriteLine($"Realizado inserção de veículo no vértice {i}.");
                    LogGeracaoVeiculos.Add(new LogGeracaoVeiculo()
                    {
                        VerticeIncersao = i,
                        IdVeiculo = IdVeiculo,
                        SegundoSimulacao = SegundoSimulacao
                    });
                    LogTrajetos.Add(new LogTrajetosVeiculos
                    {
                        IdVeiculo = IdVeiculo,
                        PercursoVeiculo = veiculoAdicionar.PercursoVeiculo
                    });
                    #endregion TratativaLogs

                    //adiciona na coleção de veículos
                    VeiculosSimulacao.Add(veiculoAdicionar);
                    IdVeiculo++;
                }
            }

            #region TratativaLogs

            for (int i = 0; i < n; i++)
            {
                LogQtdVeiculosEsperaTempo.Add(
                    new LogQtdVeiculosEsperaVertice()
                    {
                        InstanteTempo = SegundoSimulacao,
                        QtdVeiculos = VeiculosEsperaVertice[i].Count,
                        Vertice = i,
                        EspacoOcupado = VeiculosEsperaVertice[i].Sum(x=>x.Comprimento)
                    });
            }
            #endregion TrativaLogs
        }
        private void ProcessaVeiculoSimulacao()
        {
            if (ImprimirLogTela)
                Console.WriteLine("Iniciando rotina de Processamento de veículos");
            foreach (var rua in RuasSimulacao)
            {
                Aresta ArestaCorrespondente = grafo.GetAresta(rua.IdAresta);
                Vertice VerticeOrigem = grafo.GetVertice(ArestaCorrespondente.Origem);
                if (VeiculosEsperaVertice[ArestaCorrespondente.Origem].Count > 0)
                {
                    // verifica se a rua suporta adicionar mais um veículo
                    if (rua.AdicionaVeiculo(VeiculosEsperaVertice[ArestaCorrespondente.Origem].Peek(), SegundoSimulacao))
                    {
                        if (ImprimirLogTela)
                            Console.WriteLine($"O veículo {VeiculosEsperaVertice[ArestaCorrespondente.Origem].Peek().Id} entrou na rua {rua.Id}");
                        VeiculosEsperaVertice[ArestaCorrespondente.Origem].Dequeue();
                    }
                }
                #region TrativaLogs
                LogOcupacaoVias.Add(new LogOcupacaoVias
                {
                    IdAresta = rua.IdAresta,
                    EspacoOcupado = (int)rua.MediaOcupacaoVias(),
                    InstanteTempo = SegundoSimulacao,
                    QuantidadadeVeiculos = rua.NumeroVeiculosNaVia
                });
                #endregion TrativaLogs
                // trata veiculos já na via
            }
            ProcessaVeiculosVias();
            TrocaVeiculosRua();
        }
        private void ProcessaVeiculosVias()
        {
            foreach (Rua rua in RuasSimulacao)
            {
                rua.PocessaFilaVeiculos(SegundoSimulacao, Semaforos, MargemErroViaLotada);
            }
        }
        private void ProcessaSemaforos()
        {
            if(SetOtimizacaoIAAG != null)
            {
                var populacaoInicial = CriaPopulacaoInicial(500);
                SetOtimizacaoIAAG.DefineInitialPopulation(500, 14 * Semaforos.Count, populacaoInicial);
                if (ImprimeLogOtimizacao)
                    Console.WriteLine("Iniciando otimização");
                SetOtimizacaoIAAG.Run(Semaforos, RuasSimulacao);
                var melhorSolucao = SetOtimizacaoIAAG.GetBestChromosome();
                if (ImprimeLogOtimizacao)
                    Console.WriteLine(JsonConvert.SerializeObject(melhorSolucao));
            }
            foreach (var at in Semaforos)
            {
                at.AtualizaStatusSemaforo(1, SegundoSimulacao);
            }
        }

        /// <summary>
        /// remove os veículos da rua quando já estão no final da rua.
        /// se for final do trajeto finaliza percurso
        /// se tiver mais percurso coloca na proxima rua
        /// </summary>
        private void TrocaVeiculosRua()
        {
            foreach (var rua in RuasSimulacao)
            {
                var sema = Semaforos.Where(x => x.RuasOrigem.Contains(rua.Id)).FirstOrDefault();
                bool temSem = sema != null;
                for (int i = 0; i < rua.NumeroFaixas; i++)
                {
                    var veiculos = rua.VeiculosNaRua[i].ToList();
                    foreach (var veiculo in veiculos)
                    {
                        if ((rua.EspacoOcupado[i] + MargemErroViaLotada) >= rua.Comprimento)
                        {
                            bool removeVeiculo = true;
                            if (temSem && sema.EstadoSemaforo != Entidades.Enuns.EstadosSemaforo.ABERTO)
                            {
                                removeVeiculo = false;
                            }

                            if (removeVeiculo)
                            {
                                var arestaRuaAt = grafo.ObtenhaAresta(rua.IdAresta);
                                int verticeOrigemProximaRua = arestaRuaAt.Destino;
                                veiculo.VerticeAtual = veiculo.ProximoDestinoVeiculo();
                                int verticeDestinoProximaRua = veiculo.ProximoDestinoVeiculo();
                                var procimaAresta = grafo.ObtenhaAresta(verticeOrigemProximaRua, verticeDestinoProximaRua);
                                if (procimaAresta is object)
                                {
                                    var prua = RuasSimulacao.Where(x => x.IdAresta == procimaAresta.Id).FirstOrDefault();
                                    if (prua is object)
                                    {
                                        if (prua.AdicionaVeiculo(veiculo, SegundoSimulacao))
                                        {
                                            rua.RemoveVeiculo();
                                        }
                                        // o veículo precisa trocar de rua mas a proxima rua não tem espaço suficiente
                                        else
                                        {
                                            //veiculo permanece onde está
                                        }
                                    }
                                }// fim percurso do veículo
                                else{
                                    rua.RemoveVeiculo(); // veiculo removido da simulação, chegou ao destino
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool VerificaCarregamentoDados()
        {
            if (grafo == null || grafo.NumeroVertices == 0 || grafo.NumeroArestas == 0) return false;
            if (RuasSimulacao == null || RuasSimulacao.Count == 0) return false;
            return true;
        }
        private Veiculo GetVeiculo(int id)
        {
            return VeiculosSimulacao.Where((x) => x.Id == id).FirstOrDefault();
        }
        public void inicializaFilaEsperaVerice()
        {
            if (grafo.NumeroVertices > 0)
            {
                VeiculosEsperaVertice.Clear();
                for (int i = 0; i < grafo.NumeroVertices; i++)
                    VeiculosEsperaVertice.Add(new Queue<Veiculo>());
            }
        }
        #endregion MetodosPrivados

        #region Propriedades
        private List<Veiculo> VeiculosSimulacao { get; set; } = new List<Veiculo>();
        private List<Rua> RuasSimulacao { get; set; } = new List<Rua>();
        private List<Semaforo> Semaforos { get; set; } = new List<Semaforo>();
        private List<Queue<Veiculo>> VeiculosEsperaVertice { get; set; } = new List<Queue<Veiculo>>();
        private Grafo grafo = new Grafo();
        private List<int> TaxaGeracao = new List<int>();
        public List<List<int>> ListaDeTempoEntradaVeiculosVertices = new List<List<int>>();
        private GeradorVeiculos geradorVeiculos = new GeradorVeiculos();
        private int SegundoSimulacao = 0;
        private int IdVeiculo = 0;
        public int TempoDelayRotinas { get; set; }
        public int MargemErroViaLotada { get; set; } = 2;
        public int QtdIteracoes { get; set; }
        #endregion Propriedades

        #region PropriedadesLogs
        public List<LogGeracaoVeiculo> LogGeracaoVeiculos { get; set; } = new List<LogGeracaoVeiculo>();
        public List<LogQtdVeiculosEsperaVertice> LogQtdVeiculosEsperaTempo { get; set; } = new List<LogQtdVeiculosEsperaVertice>();
        public List<LogOcupacaoVias> LogOcupacaoVias { get; set; } = new List<LogOcupacaoVias>();
        public List<LogTrajetosVeiculos> LogTrajetos { get; set; } = new List<LogTrajetosVeiculos>();
        #endregion PropriedadesLogs

        #region SalvarLogs
        public void SalvarLogsGeracaoVeiculos(string caminhoVeiculosVertice, string CaminhoVeiculoPorTempo)
        {
            List<string> LogSalvar = new List<string>();
            for (int i = 0; i < grafo.NumeroVertices; i++)
            {
                LogSalvar.Add($"{i};{LogGeracaoVeiculos.Where((x) => x.VerticeIncersao == i).Sum((x) => 1)}");
            }
            using (StreamWriter file = new StreamWriter(caminhoVeiculosVertice))
            {
                if (file == null)
                    throw new Exception("arquivo não encontrao");
                file.Write(string.Join("\n", LogSalvar));
                file.Close();
            }
            LogSalvar.Clear();
            foreach (var item in LogGeracaoVeiculos)
            {
                LogSalvar.Add($"{item.SegundoSimulacao};{LogGeracaoVeiculos.Where((x) => x.SegundoSimulacao == item.SegundoSimulacao).Sum((x) => 1)}");
            }
            using (StreamWriter file = new StreamWriter(CaminhoVeiculoPorTempo))
            {
                if (file == null)
                    throw new Exception("arquivo não encontrao");
                file.Write(string.Join("\n", LogSalvar));
                file.Close();
            }
        }
        public void SalvarLogVeiculosEspera(string caminhoVeiculosEsperaTempo)
        {
            List<string> LogSalvar = new List<string>();
            foreach (var item in LogQtdVeiculosEsperaTempo)
            {
                LogSalvar.Add($"{item.InstanteTempo};{LogQtdVeiculosEsperaTempo.Where((x) => x.InstanteTempo == item.InstanteTempo).Sum((x) => 1)}");
            }
            using (StreamWriter file = new StreamWriter(caminhoVeiculosEsperaTempo))
            {
                if (file == null)
                    throw new Exception("arquivo não encontrao");
                file.Write(string.Join("\n", LogSalvar));
                file.Close();
            }
        }
        #endregion SalvarLogs
    }
}
