﻿using System;
using System.Collections.Generic;
using System.Linq;

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
                new Uredjaj("Svetlo",60001,new Dictionary<string, string>{{ "intezitet", "70" },{ "boja plava", "220" },{ "boja crvena", "110" }}),
                new Uredjaj("Klima",60002,new Dictionary<string, string>{{ "stanje", "iskljuceno" },{ "temperatura", "15" }})
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
        /* public void AzurirajFunkciju1(string ime, string funkcija, string vrednost)
         {
             foreach (var s in SviUredjaji().ToList())
             {
                 if (s.Ime == ime)
                 {
                     if (s.Funkcije.ContainsKey(funkcija))
                     {
                         s.Funkcije[funkcija] = vrednost;
                     }
                     else
                     {
                         s.Funkcije.Add(funkcija, vrednost);
                     }
                 }
             }


             // Ažuriraj vremensku oznaku
             PoslednjaPromena = DateTime.Now;

             // Evidentiraj promenu
             EvidencijaKomandi.Add($"[{PoslednjaPromena}] {Ime}: {funkcija} promenjena na {vrednost}");

         }*/
        // Metoda za prikaz trenutnog stanja uređaja
        /*public string PrikaziStanje()
        {
            string stanje = $"Uređaj: {Ime}, Port: {Port}, Stanje funkcija:";
            foreach (var funkcija in Funkcije)
            {
                stanje += $"({funkcija.Key}: {funkcija.Value});";
            }
            stanje.Substring(0, stanje.Length - 1);
            return stanje;
        }*/

        // Metoda za prikaz evidencije komandi
        /*public string PrikaziEvidenciju()
        {
            return string.Join("\n", EvidencijaKomandi);
        }*/

        public List<Uredjaj> SviUredjaji()
        {
            return uredjaji;
        }
        public void AzurirajListu(List<Uredjaj> noviUredjaji)
        {
            uredjaji = noviUredjaji;
        }

        public string IspisiSveUredjajeUTabeli(List<Uredjaj> lista)
        {
            // Zaglavlje tabele
            string tabela = string.Format("{0,-15} | {1,-10} | {2,-50}\n", "Ime Uređaja", "Port", "Funkcije");
            tabela += new string('-', 80) + "\n";

            // Ispis uređaja
            foreach (var uredjaj in lista)
            {
                // Pretvaranje funkcija u format ključ: vrednost
                string funkcije = string.Join(", ", uredjaj.Funkcije.Select(f => $"{f.Key}: {f.Value}"));

                // Dodavanje uređaja u tabelu
                tabela += string.Format("{0,-15} | {1,-10} | {2,-50}\n", uredjaj.Ime, uredjaj.Port, funkcije);
            }

            return tabela;
        }
        public string IspisiSveFunkcijeUredjaja()
        {
            // Zaglavlje tabele
            string tabela = string.Format("{0,-15} | {1,-10} | {2,-50}\n", "Ime Uređaja", "Port", "Funkcije");
            tabela += new string('-', 80) + "\n";

            // Pretvaranje funkcija u format ključ: vrednost
            string funkcije = string.Join(", ", this.Funkcije.Select(f => $"{f.Key}: {f.Value}"));

            // Dodavanje uređaja u tabelu
            tabela += string.Format("{0,-15} | {1,-10} | {2,-50}\n", this.Ime, this.Port, funkcije);


            return tabela;
        }

    }
}
