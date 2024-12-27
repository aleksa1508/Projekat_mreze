using KucniUredjaji;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

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
            string odgovor = "";
            int brojBajta = 0;
            do
            {
                Console.WriteLine("Unesite korisnicko ime:");
                string korisnickoIme = Console.ReadLine();
                Console.WriteLine("Unesite lozinku:");
                string lozinka = Console.ReadLine();

                string format = $"{korisnickoIme}:{lozinka}";
                brojBajta = clientSocket.Send(Encoding.UTF8.GetBytes(format));
                brojBajta = clientSocket.Receive(buffer);
                 odgovor = Encoding.UTF8.GetString(buffer, 0, brojBajta);

                Console.WriteLine("Prijava->" + odgovor);
            } while (odgovor != "USPESNO");

            brojBajta = clientSocket.Receive(buffer);
            odgovor = Encoding.UTF8.GetString(buffer, 0, brojBajta);
            Console.WriteLine("UDP Port->" + odgovor);

            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint serverResponseEP = new IPEndPoint(IPAddress.Any, 0); // Endpoint za prijem odgovora

            int assignedPort;
            IPEndPoint destinationEP = new IPEndPoint(IPAddress.Loopback, 0);
            if (int.TryParse(odgovor, out assignedPort))
            {
                destinationEP = new IPEndPoint(IPAddress.Loopback, assignedPort);

            }
            string initialMessage =$"Klijent se povezao na udp port:{assignedPort}";
            byte[] initialData = Encoding.UTF8.GetBytes(initialMessage);
            udpSocket.SendTo(initialData, destinationEP);

            Console.WriteLine($"Sent to server: {initialMessage}");
            // Odredišna adresa servera
            //IPEndPoint destinationEP = new IPEndPoint(IPAddress.Loopback, 50002);
            while (true)
            {
                try
                {
                    brojBajta = clientSocket.Receive(buffer);

                    List<Uredjaj>uredjaji= new List<Uredjaj>();
                    using (MemoryStream ms = new MemoryStream(buffer, 0, brojBajta))
                    {
                        List<Uredjaj> lista = (List<Uredjaj>)formatter.Deserialize(ms);
                        uredjaji = lista;
                    }
                    Console.WriteLine("Lista dostupnih uredjaja:");
                    for (int i = 0; i < uredjaji.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {uredjaji[i].Ime} (Port: {uredjaji[i].Port = assignedPort})");
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
                    Console.WriteLine("[Ime funkcije , Vrednost]");
                    Console.WriteLine("--------------------------");
                    foreach (var v in izabraniUredjaj.Funkcije)
                    {
                        Console.WriteLine("-" + v.ToString());
                    }
                    // biranje funkcije uređaja
                    bool provera = false;
                    string vrednost = "";
                    string funkcija = "";
                    do
                    {
                        Console.WriteLine("Unesite ime funkcije koju zelite da promenite:");
                        funkcija = Console.ReadLine();
                        foreach (var f in izabraniUredjaj.Funkcije)
                        {
                            if (f.Key == funkcija)
                            {
                                provera = true;
                                break;
                            }
                        }
                        if (provera)
                        {
                            Console.WriteLine("Unesite novu vrijednost:");
                             vrednost = Console.ReadLine();
                        }
                       // izabraniUredjaj.AzurirajFunkciju(funkcija, vrednost);
                    } while (!provera);


                    initialData = Encoding.UTF8.GetBytes($"{izabraniUredjaj.Ime}:{funkcija}:{vrednost}");
                    udpSocket.SendTo(initialData, destinationEP);

                        brojBajta = clientSocket.Receive(buffer);
                        odgovor = Encoding.UTF8.GetString(buffer, 0, brojBajta);
                    string dodatak = "";
                    do
                    {
                        Console.WriteLine(odgovor + "(da/ne)");
                        dodatak = Console.ReadLine();
                    } while (dodatak != "da" && dodatak != "ne");
                    if (dodatak.ToLower() == "ne")
                    {
                        brojBajta = clientSocket.Send(Encoding.UTF8.GetBytes(dodatak));
                        break;
                    }
                    brojBajta = clientSocket.Send(Encoding.UTF8.GetBytes(dodatak));


                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Klijent zavrsava sa radom");
            Console.ReadKey();
            clientSocket.Close();
        }
    }
}
