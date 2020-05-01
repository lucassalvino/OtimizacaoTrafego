using System.Collections.Generic;
using System.Linq;

namespace TesteConsoleAlgoritmoGenetico.Grafo
{
    public class GerenteGrafo
    {
        public List<Vertice> Vertices { get; set; } = new List<Vertice>();
        public List<Aresta> Arestas { get; set; } = new List<Aresta>();
        public bool ExisteVertice(int vertice)
        {
            foreach (var x in Vertices)
                if (x.NumeroVertice == vertice)
                    return true;
            return false;
        }
        public bool ExisteAresta(int origem, int destino)
        {
            foreach (var x in Arestas)
                if (x.Origem.NumeroVertice == origem && x.Destino.NumeroVertice == destino)
                    return true;
            return false;
        }
        public void AdicionaAresta(int origem, int destino, float peso)
        {
            if (!ExisteVertice(origem))
                Vertices.Add(new Vertice(origem));
            if (!ExisteVertice(destino))
                Vertices.Add(new Vertice(destino));
            if (ExisteAresta(origem, destino))
                Arestas.Remove(Arestas.Where(x => x.Origem.NumeroVertice == origem && x.Destino.NumeroVertice == destino).First());
            Arestas.Add(new Aresta {
                Origem = Vertices.Where(x=>x.NumeroVertice == origem).First(),
                Destino = Vertices.Where(x=>x.NumeroVertice == destino).First(),
                Peso = peso
            });
        }
        public Aresta ObtenhaAresta(int origem, int destino)
        {
            return Arestas.Where(x => x.Origem.NumeroVertice == origem && x.Destino.NumeroVertice == destino).FirstOrDefault();
        }
    }
}
