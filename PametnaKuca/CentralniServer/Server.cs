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
            List<Uredjaj> uredjaji = u.SviUredjaji();
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
            Thread.Sleep(2000);
            int udpPort = random.Next(50000, 60000);
            int brojBajta = acceptedSocket.Send(Encoding.UTF8.GetBytes(udpPort.ToString()));
            //kreiranje udp uticnice
            Socket controlSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint deviceEndpoint = new IPEndPoint(IPAddress.Any, 0);
            IPEndPoint udpServerEP = new IPEndPoint(IPAddress.Any, udpPort);
            controlSocket.Bind(udpServerEP);
            int receivedBytes = controlSocket.ReceiveFrom(buffer, ref deviceEndpoint);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            Console.WriteLine($"UDP klijent je poslao poruku-> {receivedMessage}");

            while (true)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {

                        formatter.Serialize(ms, uredjaji);
                        byte[] data = ms.ToArray();

                        acceptedSocket.Send(data);
                    }

                    receivedBytes = controlSocket.ReceiveFrom(buffer, ref deviceEndpoint);
                    receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine($"UDP klijent je poslao poruku-> {receivedMessage}");

                    string[] parts = receivedMessage.Split(':');
                    Console.WriteLine(parts.Length + " " + parts[0] + " " + parts[1] + " " + parts[2]);
                    foreach (var s in uredjaji)
                    {
                        if (s.Ime == parts[0])
                        {
                            s.AzurirajFunkciju(parts[1], parts[2]);
                            break;
                        }
                    }
                    //Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaa");
                    string odgovor = "Da li zelite da izvrsite jos neku komandu";
                    brojBajta = acceptedSocket.Send(Encoding.UTF8.GetBytes(odgovor));

                    brojBajta = acceptedSocket.Receive(buffer);
                    odgovor = Encoding.UTF8.GetString(buffer, 0, brojBajta);

                    if (odgovor == "ne")
                    {
                        break;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Server zavrsava sa radom");
            Console.ReadKey();
            acceptedSocket.Close();
            controlSocket.Close();
        }

    }
}
