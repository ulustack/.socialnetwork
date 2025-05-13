using System;
using System.Collections.Generic;

namespace Queues
{
    public class PostKuyrugu<T>
    {
        private Queue<T> gonderiler;

        public PostKuyrugu()
        {
            gonderiler = new Queue<T>();
        }

        public void Ekle(T gonderi)
        {
            gonderiler.Enqueue(gonderi);
        }

        public List<T> AkisiGetir()
        {
            return new List<T>(gonderiler);
        }

        public int Sayisi()
        {
            return gonderiler.Count;
        }
    }

    public class GonderiAkisi<T>
    {
        private Dictionary<T, PostKuyrugu<T>> kuyruklar;

        public GonderiAkisi()
        {
            kuyruklar = new Dictionary<T, PostKuyrugu<T>>();
        }

        public void KullaniciEkle(T id)
        {
            if (!kuyruklar.ContainsKey(id))
            {
                kuyruklar[id] = new PostKuyrugu<T>();
            }
        }

        public void PostEkle(T kullaniciId, T gonderi)
        {
            if (!kuyruklar.ContainsKey(kullaniciId))
            {
                KullaniciEkle(kullaniciId);
            }
            kuyruklar[kullaniciId].Ekle(gonderi);
        }

        public void AkisiGoster(T kullaniciId)
        {
            if (!kuyruklar.ContainsKey(kullaniciId))
            {
                Console.WriteLine($"{kullaniciId} için gönderi bulunamadı.");
                return;
            }

            var postlar = kuyruklar[kullaniciId].AkisiGetir();

            Console.WriteLine($"\"{kullaniciId}\" kullanıcısının gönderi akışı:");
            if (postlar.Count == 0)
            {
                Console.WriteLine("Hiç gönderi yok.");
            }
            else
            {
                foreach (var post in postlar)
                {
                    Console.WriteLine($"- {post}");
                }
            }
        }
    }
    /*
    class Program
    {
        static void Main(string[] args)
        {
            var akis = new Queues.GonderiAkisi<string>();

            akis.KullaniciEkle("A");
            akis.KullaniciEkle("B");

            akis.PostEkle("A", "Selam!");
            akis.PostEkle("A", "Bugün hava çok güzel.");
            akis.PostEkle("B", "Yeni gönderim!");

            akis.AkisiGoster("A");
            akis.AkisiGoster("B");
            akis.AkisiGoster("C"); // kullanıcı yoksa
        }
    }
    */
}
