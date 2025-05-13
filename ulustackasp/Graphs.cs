using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

//dürüst olmak gerekirse güzel iş.

namespace Graphs
{
public class GraphNode<T>
{
    public T Id { get; }
    private HashSet<GraphNode<T>> komsular;

    public GraphNode(T id)
    {
        Id = id;
        komsular = new HashSet<GraphNode<T>>();
    }

    public void Ekle(GraphNode<T> node)
    {
        if (node != this)
        {
            komsular.Add(node);
            node.komsular.Add(this);
        }
    }

    public void Sil(GraphNode<T> node)
    {
        if (node != this)
        {
            komsular.Remove(node);
            node.komsular.Remove(this);
        }
    }

    public HashSet<GraphNode<T>> Getir()
    {
        return komsular;
    }
}

    public class Graph<T>
    {
        private Dictionary<T, GraphNode<T>> dugumler;

        public Graph()
        {
            dugumler = new Dictionary<T, GraphNode<T>>();
        }

        public void KullaniciEkle(T id)
        {
            if (!dugumler.ContainsKey(id))
            {
                dugumler[id] = new GraphNode<T>(id);
            }
        }

        public void ArkadasEkle(T id1, T id2)
        {
            if (dugumler.ContainsKey(id1) && dugumler.ContainsKey(id2))
            {
                var node1 = dugumler[id1];
                var node2 = dugumler[id2];
                node1.Ekle(node2);
            }
        }

        public GraphNode<T> Getir(T id)
        {
            if (dugumler.ContainsKey(id))
            {
                return dugumler[id];
            }
            return null;
        }

        public void Recommend(T id)
        {
            if (!dugumler.ContainsKey(id))
            {
                return;
            }

            var hedef = dugumler[id];
            var visited = new HashSet<GraphNode<T>> { hedef };
            var scores = new Dictionary<T, int>();
            var queue = new Queue<GraphNode<T>>();

            foreach (var komsu in hedef.Getir())
            {
                visited.Add(komsu);
                queue.Enqueue(komsu);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var aday in current.Getir())
                {
                    if (visited.Contains(aday))
                    {
                        continue;
                    }

                    if (!scores.ContainsKey(aday.Id))
                    {
                        scores[aday.Id] = 0;
                    }

                    scores[aday.Id]++;
                }
            }

            var result = scores
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            Console.WriteLine($"\"{id}\" için arkadaş önerileri:");
            if (result.Count == 0)
            {
                Console.WriteLine("Hiç Öneri Yok.");
            }
            else
            {
                Console.WriteLine(string.Join(", ", result));
            }
        }
    }

    // Örnek Kullanım
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var graph = new Graph<string>();
            graph.KullaniciEkle("A");
            graph.KullaniciEkle("B");
            graph.KullaniciEkle("C");
            graph.KullaniciEkle("D");
            graph.KullaniciEkle("E");
            graph.KullaniciEkle("F");
            graph.KullaniciEkle("G");
            graph.KullaniciEkle("H");

            graph.ArkadasEkle("A", "B");
            graph.ArkadasEkle("A", "C");
            graph.ArkadasEkle("A", "D");

            graph.ArkadasEkle("B", "A");
            graph.ArkadasEkle("B", "C");
            graph.ArkadasEkle("B", "E");

            graph.ArkadasEkle("C", "D");
            graph.ArkadasEkle("C", "F");
            graph.ArkadasEkle("B", "B");

            graph.Recommend("A");
        }
    }
    
}


//yazmadan edemeyeceğim. müthiş kod yav.
// hayatımı inanılmaz kolaylaştırdı.

