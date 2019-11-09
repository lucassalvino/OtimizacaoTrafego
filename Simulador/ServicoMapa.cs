using System;
using System.IO;

namespace Simulador
{
    public class ServicoMapa
    {
        public static string GetMapaSimulacaoAtual(string caminho = "")
        {
            if (string.IsNullOrEmpty(caminho))
            {
                caminho = "Config/Mapa.json";
            }
            using (StreamReader file = new StreamReader(caminho))
            {
                if (file == null)
                    throw new Exception($"Arquivo {caminho} não foi encontrado!");
                string conteudoArquivo = file.ReadToEnd();
                file.Close();
                return conteudoArquivo;
            }
        }
    }
}
