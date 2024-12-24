using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using KucniUredjaji;
using System.IO;

namespace TCPServer
{
    public class Server
    {
        static void Main(string[] args)
        {
            Random random = new Random();   
            Uredjaj svijetlo = new Uredjaj("SVETLO", 52354);
            svijetlo.AzurirajFunkciju("Boja Crvena", "100");
            svijetlo.AzurirajFunkciju("Intenzitet", "70%");
            List<Uredjaj> uredjaji = new List<Uredjaj>();
            uredjaji.Add(svijetlo);
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

            //komunikacija servera i klijenta
            byte[] buffer = new byte[4096];
            BinaryFormatter formatter = new BinaryFormatter();


            while (true)
            {
                try
                {
                    int brBajta = acceptedSocket.Receive(buffer);
                    if (brBajta == 1)
                    {
                        Console.WriteLine("Klijent je zavrsio sa radom");
                        break;
                    }
                    string poruka = Encoding.UTF8.GetString(buffer, 0, brBajta);
                    Console.WriteLine("Poruka klijenta: " + poruka+" "+brBajta);

                    string[] djelovi = poruka.Split(':');
                    Console.WriteLine(djelovi.Length + " " + djelovi[0] + " " + djelovi[1]);
                    if (djelovi.Length == 2 && korisnici.ContainsKey(djelovi[0]) && korisnici[djelovi[0]] == djelovi[1])
                    {
                        Console.WriteLine("Prijavljivanje je uspesno\n");
                        string odgovor = "USPESNO";
                        brBajta = acceptedSocket.Send(Encoding.UTF8.GetBytes(odgovor));

                        int udpPort = random.Next(50000,60000);
                        using (MemoryStream ms = new MemoryStream())
                        {

                            formatter.Serialize(ms, udpPort);
                            formatter.Serialize(ms, uredjaji);

                            //byte[] data = ms.ToArray();
                            acceptedSocket.Send(ms.ToArray());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Prijavljivanje nije uspesno\n");
                        string odgovor = "NEUSPESNO";
                        brBajta = acceptedSocket.Send(Encoding.UTF8.GetBytes(odgovor));
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
            serverSocket.Close();
        }
    }
}
