using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;

namespace UDPServer
{
    public class UdpServer
    {
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001); // Serverov IPEndPoint, IP i port na kom ce server soket primati poruke
            serverSocket.Bind(serverEP); // Povezujemo serverov soket sa njegovim EP
            Console.WriteLine($"Server je pokrenut i ceka poruku na: {serverEP}");

            EndPoint posiljaocEP = new IPEndPoint(IPAddress.Any, 0); // Serverov IPEndPoint, IP i port na kom ce server soket primati poruke
            UdpClient udpServer = new UdpClient(6000); // UDP server na portu 6000
            Console.WriteLine("UDP Server je pokrenut i osluškuje na portu 6000...");

            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] prijemniBafer = new byte[1024];
            while (true)
            {
                // Prijem poruke
                byte[] receivedBytes = udpServer.Receive(ref clientEndPoint);
                string message = Encoding.UTF8.GetString(receivedBytes);

                Console.WriteLine($"Poruka primljena od TCP Servera: {message}");

                int brBajta = serverSocket.ReceiveFrom(prijemniBafer, ref posiljaocEP); // Primamo poruku i podatke o posiljaocu
                string poruka = Encoding.UTF8.GetString(prijemniBafer, 0, brBajta);

                Console.WriteLine("\n----------------------------------------------------------------------------------------\n");
                Console.WriteLine($"Stiglo je {brBajta} bajta od {posiljaocEP}, poruka:\n{poruka}");

                byte[] binarnaPoruka = Encoding.UTF8.GetBytes(message);
                 brBajta = serverSocket.SendTo(binarnaPoruka, 0, binarnaPoruka.Length, SocketFlags.None, posiljaocEP); // 3.
                Console.WriteLine($"Poslata je poruka duzine {brBajta} ka {posiljaocEP}");

                brBajta = serverSocket.ReceiveFrom(prijemniBafer, ref posiljaocEP); // Primamo poruku i podatke o posiljaocu
                poruka = Encoding.UTF8.GetString(prijemniBafer, 0, brBajta);
                Console.WriteLine($"Poruka primljena od UDP KLIJENTA: {poruka}");
                
                break;
            }
        Console.WriteLine("Server zavrsava sa radom");
            serverSocket.Close(); // Zatvaramo soket na kraju rada
            Console.ReadKey();
        }
    }
}