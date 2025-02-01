using KucniUredjaji;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
//using UDPServer;
namespace TCPServer
{
    public class Server
    {
        // private static UdpServer udpServer;

        static void PokreniKlijente(int brojKlijenata)
        {
            for (int i = 0; i < brojKlijenata; i++)
            {
                // Putanja do izvršnog fajla klijenta (potrebno je kompajlirati ga)
                string clientPath = @"C:\Users\Dell 3520\Desktop\AA\MREZE\github\PametnaKuca\UredjajiKomunikacija\bin\Debug\UredjajiKomunikacija.exe";
                Process klijentProces = new Process(); // Stvaranje novog procesa
                klijentProces.StartInfo.FileName = clientPath; //Zadavanje putanje za pokretanje
                klijentProces.StartInfo.Arguments = $"{i + 60001}"; // Argument - broj klijenta
                klijentProces.Start(); // Pokretanje klijenta
                Console.WriteLine($"Pokrenut klijent #{i + 1}");
            }
        }
        static void Main(string[] args)
        {

            Random random = new Random();
            Dictionary<string, string> korisnici = new Dictionary<string, string>
            {
                { "user1", "a" },
                { "user2", "b" }
            };
            Uredjaj u = new Uredjaj();
            //inicijalizacija servera
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001);

            serverSocket.Bind(serverEP);
            serverSocket.Blocking = false;
            serverSocket.Listen(5);

            BinaryFormatter formatter = new BinaryFormatter();

            Console.WriteLine($"Server je stavljen u stanje osluskivanja i ocekuje komunikaciju na {serverEP}");

            List<Socket> klijenti = new List<Socket>(); // Pravimo posebnu listu za klijentske sokete kako nam je ne bi obrisala Select funkcija
            List<Socket> udpSockets = new List<Socket>();
            int udpPort1 = 0;


            PokreniKlijente(2);
            
            
            byte[] buffer = new byte[4096];
            try
            {
                while (true)
                {
                    List<Socket> checkRead = new List<Socket>();
                    List<Socket> checkError = new List<Socket>();

                    if (klijenti.Count < 5)
                    {
                        checkRead.Add(serverSocket);

                    }
                    checkError.Add(serverSocket);

                    foreach (Socket s in klijenti)
                    {
                        checkRead.Add(s);
                        checkError.Add(s);
                    }

                    foreach (Socket s in udpSockets)
                    {
                        checkRead.Add(s);
                        checkError.Add(s);
                    }
                    Socket.Select(checkRead, null, checkError, 1000*1000);


                    if (checkRead.Count > 0)
                    {
                        Console.WriteLine($"Broj dogadjaja je: {checkRead.Count}");
                        foreach (Socket s in checkRead)
                        {
                            if (s == serverSocket)
                            {

                                Socket client = serverSocket.Accept();
                                client.Blocking = false;
                                klijenti.Add(client);
                                Console.WriteLine($"Klijent se povezao sa {client.RemoteEndPoint}");
                                
                            }
                            /*
                             
                             */
                            else if (udpSockets.Contains(s))
                            {
                                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                                int receivedBytes = s.ReceiveFrom(buffer, ref clientEP);
                                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                               
                                Console.WriteLine($"Poruka od UDP klijenta ({clientEP}): {receivedMessage}");
                                while (true)
                                {

                                    List<Uredjaj> uredjaji = u.SviUredjaji();
                                    foreach (var v in uredjaji)
                                    {
                                        foreach (var s2 in v.Funkcije)
                                        {
                                            Console.WriteLine(s2.Key + " " + s2.Value);
                                        }
                                    }
                                    Console.WriteLine(u.IspisiSveUredjajeUTabeli(uredjaji));

                                    using (MemoryStream ms = new MemoryStream())
                                    {

                                        formatter.Serialize(ms, uredjaji);
                                        byte[] data = ms.ToArray();

                                        s.SendTo(data, clientEP);
                                    }


                                    receivedBytes = s.ReceiveFrom(buffer, ref clientEP);

                                    int udpPortUrejdjaja = 0;
                                    string funkcija = "";
                                    string vrednost = "";
                                    string ime = "";
                                    Uredjaj u1 = new Uredjaj();
                                    using (MemoryStream ms = new MemoryStream(buffer, 0, receivedBytes))
                                    {
                                        u1 = (Uredjaj)formatter.Deserialize(ms);
                                        funkcija = (string)formatter.Deserialize(ms);
                                        vrednost = (string)formatter.Deserialize(ms);
                                        udpPortUrejdjaja = u1.Port;
                                        ime = u1.Ime;


                                    }
                                    foreach (var s1 in uredjaji)
                                    {
                                        if (s1.Ime == u1.Ime)
                                        {
                                            s1.AzurirajFunkciju(funkcija, vrednost);
                                            break;
                                        }
                                    }
                                    Console.WriteLine("\n" + funkcija + " " + vrednost);
                                    //povezivanje uredjaja i servera
                                    IPEndPoint uredjajEP = new IPEndPoint(IPAddress.Loopback, udpPortUrejdjaja);
                                    // udpSocket.Bind(uredjajEP); ovo ja mislim ne treba!!!!!
                                    byte[] initialData = Encoding.UTF8.GetBytes(ime + ":" + funkcija + ":" + vrednost);
                                    s.SendTo(initialData, uredjajEP);

                                    string odgovor = "Da li zelite da izvrsite jos neku komandu";
                                    initialData = Encoding.UTF8.GetBytes(odgovor);
                                    s.SendTo(initialData, clientEP);

                                    receivedBytes = s.ReceiveFrom(buffer, ref clientEP);
                                    receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                                    Console.WriteLine($"Korisnik preko UDP je poslao poruku-> {receivedMessage}");


                                    foreach (var v in u.uredjaji)
                                    {
                                        foreach (var s4 in v.Funkcije)
                                        {
                                            Console.WriteLine(s4.Key + " " + s4.Value);
                                        }
                                    }

                                    if (receivedMessage == "ne")
                                    {
                                        break;
                                    }
                                }

                            }
                            else 
                            {
                                int receivedBytes1 = s.Receive(buffer);
                                if (receivedBytes1 > 0)
                                {
                                    string poruka = Encoding.UTF8.GetString(buffer, 0, receivedBytes1);
                                    Console.WriteLine($"Poruka od klijenta: {poruka}");

                                    string[] djelovi = poruka.Split(':');
                                    if (djelovi.Length == 2 && korisnici.ContainsKey(djelovi[0]) && korisnici[djelovi[0]] == djelovi[1])
                                    {
                                        // Клијент валидан, шаљемо одговор
                                        string odgovor = "USPESNO";
                                        s.Send(Encoding.UTF8.GetBytes(odgovor));

                                        // Креирамо UDP сокет за комуникацију
                                         udpPort1 = random.Next(50002, 60000);
                                        s.Send(Encoding.UTF8.GetBytes(udpPort1.ToString()));

                                        Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                                        IPEndPoint udpServerEP = new IPEndPoint(IPAddress.Loopback, udpPort1);
                                        udpSocket.Bind(udpServerEP);
                                        udpSockets.Add(udpSocket);

                                        Console.WriteLine($"UDP soket kreiran na portu {udpPort1}");
                                        //for petlja i

                                    }
                                    else
                                    {
                                        string odgovor = "NEUSPESNO";
                                        s.Send(Encoding.UTF8.GetBytes(odgovor));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Klijent je prekinuo vezu.");
                                    checkRead.Remove(s);
                                    s.Close();
                                    continue;
                                }
                            }

                            
                        }
                        
                    }
                    checkRead.Clear();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Doslo je do greske {ex}");
            }

            foreach (Socket s in klijenti)
            {
               // s.Send(Encoding.UTF8.GetBytes("Server je zavrsio sa radom"));
                s.Close();
            }
            foreach (var udpSocket in udpSockets)
            {
                udpSocket.Close();
            }
            

            Console.WriteLine("Server zavrsava sa radom");
            Console.ReadKey();
            //acceptedSocket.Close();
            serverSocket.Close();
        }

    }
}
