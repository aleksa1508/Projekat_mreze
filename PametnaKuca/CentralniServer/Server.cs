using KucniUredjaji;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
//using UDPServer;
namespace TCPServer
{
    public class Server
    {
        //        private static UdpServer udpServer;

        static void Main(string[] args)
        {

            Random random = new Random();
            Uredjaj u = new Uredjaj();
            Dictionary<string, string> korisnici = new Dictionary<string, string>
            {
                { "user1", "a" },
                { "user2", "b" }
            };
            //inicijalizacija servera
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001);

            serverSocket.Bind(serverEP);

            serverSocket.Listen(5);


            Console.WriteLine($"Server je stavljen u stanje osluskivanja i ocekuje komunikaciju na {serverEP}");

            Socket acceptedSocket = serverSocket.Accept();
            //acceptedSocket.Blocking = false;
            IPEndPoint clientEP = acceptedSocket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Povezao se novi klijent! Njegova adresa je {clientEP}");
            //----------------------------------------------------------------------------------------


            byte[] buffer = new byte[4096];
            BinaryFormatter formatter = new BinaryFormatter();
            bool validacija = false;
            do
            {

                int brBajta = acceptedSocket.Receive(buffer);
                if (brBajta == 0)
                {
                    Console.WriteLine("Klijent je zavrsio sa radom");
                    return;
                }
                string poruka = Encoding.UTF8.GetString(buffer, 0, brBajta);
                Console.WriteLine("Poruka klijenta: " + poruka + " " + brBajta);
                string[] djelovi = poruka.Split(':');
                Console.WriteLine(djelovi.Length + " " + djelovi[0] + " " + djelovi[1]);
                if (djelovi.Length == 2 && korisnici.ContainsKey(djelovi[0]) && korisnici[djelovi[0]] == djelovi[1])
                {
                    Console.WriteLine("Prijavljivanje je uspesno\n");
                    string odgovor = "USPESNO";
                    brBajta = acceptedSocket.Send(Encoding.UTF8.GetBytes(odgovor));
                    
                    validacija = true;
                }
                else
                {
                    Console.WriteLine("Prijavljivanje nije uspesno\n");
                    string odgovor = "NEUSPESNO";
                    brBajta = acceptedSocket.Send(Encoding.UTF8.GetBytes(odgovor));
                    
                }
            } while (!validacija);
            //Thread.Sleep(2000);
            int udpPort = random.Next(50002, 60000);
            int brojBajta = acceptedSocket.Send(Encoding.UTF8.GetBytes(udpPort.ToString()));
            //kreiranje udp uticnice
            Socket controlSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint deviceEndpoint = new IPEndPoint(IPAddress.Any, 0);

            IPEndPoint udpServerEP = new IPEndPoint(IPAddress.Loopback, udpPort);
            controlSocket.Bind(udpServerEP);
            int receivedBytes = controlSocket.ReceiveFrom(buffer, ref deviceEndpoint);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            Console.WriteLine($"Korisnik preko UDP je poslao poruku-> {receivedMessage} {((IPEndPoint)deviceEndpoint).Port}");

            //PROOVJERA OD KOGA JE DOSLA PORUKA ALI NIJE DOBAR KOD
            /* if (((IPEndPoint)deviceEndpoint).Port == udpPort)
             {
                 Console.WriteLine("Poruka od korisnika.");
                 // Obrada poruke od uređaja
                 Console.WriteLine($"Korisnik preko UDP je poslao poruku-> {receivedMessage}");
             }*/
            try
            {
                while (true)
                {
                    List<Uredjaj> uredjaji = u.SviUredjaji();
                    foreach (var v in uredjaji)
                    {
                        foreach (var s in v.Funkcije)
                        {
                            Console.WriteLine(s.Key + " " + s.Value);
                        }
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {

                        formatter.Serialize(ms, uredjaji);
                        byte[] data = ms.ToArray();

                        acceptedSocket.Send(data);
                    }

                    receivedBytes = controlSocket.ReceiveFrom(buffer, ref deviceEndpoint);
                    /* receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                     Console.WriteLine($"Korisnik preko UDP je poslao poruku-> {receivedMessage}");*/

                    int udpPortUrejdjaja = 0;
                    string funkcija = "";
                    string vrednost = "";
                    string ime = "";
                    using (MemoryStream ms = new MemoryStream(buffer, 0, receivedBytes))
                    {
                        Uredjaj u1 = (Uredjaj)formatter.Deserialize(ms);
                        funkcija = (string)formatter.Deserialize(ms);
                        vrednost = (string)formatter.Deserialize(ms);
                        udpPortUrejdjaja = u1.Port;
                        ime = u1.Ime;
                    }

                    /*if (((IPEndPoint)deviceEndpoint).Port == udpPort)
                    {
                        Console.WriteLine("Poruka od korisnika.");
                        // Obrada poruke od uređaja
                        Console.WriteLine($"Korisnik preko UDP je poslao poruku-> {receivedMessage}");
                    }*/
                    //Console.WriteLine($"Klijent je preko udp poslao poruku-> {receivedMessage}");

                    //povezivanje uredjaja i servera
                    IPEndPoint uredjajEP = new IPEndPoint(IPAddress.Loopback, udpPortUrejdjaja);
                    //controlSocket.Bind(uredjajEP);
                    byte[] initialData = Encoding.UTF8.GetBytes(ime + ":" + funkcija + ":" + vrednost);
                    controlSocket.SendTo(initialData, uredjajEP);


                    string odgovor = "Da li zelite da izvrsite jos neku komandu";
                    initialData = Encoding.UTF8.GetBytes(odgovor);
                    controlSocket.SendTo(initialData, deviceEndpoint);
                    Console.WriteLine("KORISNIKOV END POINT->"+(deviceEndpoint));

                    receivedBytes = controlSocket.ReceiveFrom(buffer, ref deviceEndpoint);
                    receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine($"Korisnik preko UDP je poslao poruku-> {receivedMessage}");

                    

                    if (receivedMessage == "ne")
                    {
                        break;
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
            
            Console.WriteLine("Server zavrsava sa radom");
            Console.ReadKey();
            acceptedSocket.Close();
            controlSocket.Close();
        }

    }
}
