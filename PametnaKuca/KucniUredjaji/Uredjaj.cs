using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KucniUredjaji
{
    [Serializable]
    public class Uredjaj
    {
        public string Ime { get; set; }                // Ime uređaja
        public int Port { get; set; }                  // Port na kojem uređaj komunicira
        public Dictionary<string, string> Funkcije { get; set; } // Funkcije uređaja i njihove vrednosti
        public List<string> EvidencijaKomandi { get; private set; } // Evidencija komandi sa vremenskim oznakama
        public DateTime PoslednjaPromena { get; private set; }      // Vremenska oznaka poslednje promene

        public List<Uredjaj> uredjaji { get; set; }
        // Konstruktor
        public Uredjaj(string ime, int port)
        {
            Ime = ime;
            Port = port;
            Funkcije = new Dictionary<string, string>();
            EvidencijaKomandi = new List<string>();
            PoslednjaPromena = DateTime.Now;
        }
        public Uredjaj(string ime, int port, Dictionary<string, string> funkcije)
        {
            Ime = ime;
            Port = port;
            Funkcije = funkcije;
            EvidencijaKomandi = new List<string>();
            PoslednjaPromena = DateTime.Now;
        }

        public Uredjaj()
        {
            uredjaji = new List<Uredjaj> {
                new Uredjaj("Svetlo",60000,new Dictionary<string, string>{{ "intezitet", "70%" },{ "STANJE", "ISKLJUCENO" }})
            };
        }

        // Dodavanje ili ažuriranje funkcije uređaja
        public void AzurirajFunkciju(string funkcija, string vrednost)
        {
            if (Funkcije.ContainsKey(funkcija))
            {
                Funkcije[funkcija] = vrednost;
            }
            else
            {
                Funkcije.Add(funkcija, vrednost);
            }
            
            // Ažuriraj vremensku oznaku
            PoslednjaPromena = DateTime.Now;

            // Evidentiraj promenu
            EvidencijaKomandi.Add($"[{PoslednjaPromena}] {Ime}: {funkcija} promenjena na {vrednost}");
        }

        // Metoda za prikaz trenutnog stanja uređaja
        public string PrikaziStanje()
        {
            string stanje = $"Uređaj: {Ime}, Port: {Port}, Stanje funkcija:";
            foreach (var funkcija in Funkcije)
            {
                stanje += $"({funkcija.Key}: {funkcija.Value});";
            }
            stanje.Substring(0, stanje.Length - 1);
            return stanje;
        }

        // Metoda za prikaz evidencije komandi
        public string PrikaziEvidenciju()
        {
            return string.Join("\n", EvidencijaKomandi);
        }

        public List<Uredjaj> SviUredjaji()
        {
            return uredjaji;
        }
        public void AzurirajListu(List<Uredjaj>noviUredjaji)
        {
            uredjaji=noviUredjaji;
        }




    }
}
