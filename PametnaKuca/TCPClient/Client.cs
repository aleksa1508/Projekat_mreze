using KucniUredjaji;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TCPClient
{
    public class Client
    {
        static void Main(string[] args)
        {
            //Povezivanje
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 50001);
            byte[] buffer = new byte[4096];


            clientSocket.Connect(serverEP);
            Console.WriteLine("Klijent je uspesno povezan sa serverom!");
            BinaryFormatter formatter = new BinaryFormatter();

            while (true)
            {
                try
                {

                    Console.WriteLine("Unesite korisnicko ime:");
                    string korisnickoIme = Console.ReadLine();
                    Console.WriteLine("Unesite lozinku:");
                    string lozinka = Console.ReadLine();

                    string format = $"{korisnickoIme}:{lozinka}";
                    int brojBajta = clientSocket.Send(Encoding.UTF8.GetBytes(format));
                    if (brojBajta == 1)
                    {
                        break;
                    }
                    //odgovor servera
                    brojBajta = clientSocket.Receive(buffer);
                    string odgovor = Encoding.UTF8.GetString(buffer, 0, brojBajta);

                    Console.WriteLine("Prijava->" + odgovor);
                    if (odgovor == "USPESNO")
                    {
                        List<Uredjaj> uredjaji = new List<Uredjaj>();
                        brojBajta = clientSocket.Receive(buffer);
                        odgovor = Encoding.UTF8.GetString(buffer, 0, brojBajta);
                        int udpPort = 0;
                        using (MemoryStream ms = new MemoryStream(buffer, 0, brojBajta))
                        {
                            udpPort = (int)formatter.Deserialize(ms);
                            List<Uredjaj> lista = (List<Uredjaj>)formatter.Deserialize(ms);
                            uredjaji = lista;
                            //Console.WriteLine($"{udpPort}");
                        }
                        Console.WriteLine("Lista dostupnih uredjaja:");
                        for (int i = 0; i < uredjaji.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {uredjaji[i].Ime} (Port: {uredjaji[i].Port = udpPort})");
                        }

                        // Biranje uređaja
                        int izbor;
                        do
                        {
                            Console.WriteLine("Unesite redni broj uredjaja koji zelite da podesite:");
                            izbor = Int32.Parse(Console.ReadLine()) - 1;
                        } while (izbor < 0 || izbor >= uredjaji.Count);

                        Uredjaj izabraniUredjaj = uredjaji[izbor];
                        Console.WriteLine($"Izabrali ste uredjaj: {izabraniUredjaj.Ime}");
                        foreach (var v in izabraniUredjaj.Funkcije)
                        {
                            Console.WriteLine(v.ToString());
                        }
                        // Menjanje funkcija uređaja
                        Console.WriteLine("Unesite ime funkcije koju zelite da promenite:");
                        string funkcija = Console.ReadLine();
                        using (MemoryStream ms = new MemoryStream())
                        {

                            formatter.Serialize(ms, funkcija);
                            formatter.Serialize(ms, izabraniUredjaj.Ime.ToString());

                            //byte[] data = ms.ToArray();
                            clientSocket.Send(ms.ToArray());
                        }

                        //brojBajta = clientSocket.Send(Encoding.UTF8.GetBytes(funkcija));

                        /*
                        Console.WriteLine("Da li zelite jos neku komandu da izvrsite:");
                        if (odgovor == "DA")
                        {
                            break;
                        }*/
                    }
                    else
                    {
                        Console.WriteLine("Prijavljivanje nije uspelo,pokusajte ponovo!");
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            Console.WriteLine("Klijent zavrsava sa radom");
            Console.ReadKey();
            clientSocket.Close();
        }
    }
}
