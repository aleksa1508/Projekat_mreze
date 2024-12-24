using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

            while (true)
            {
                // Prijem poruke
                byte[] receivedBytes = udpServer.Receive(ref clientEndPoint);
                string message = Encoding.UTF8.GetString(receivedBytes);
                Console.WriteLine($"Poruka primljena od TCP Servera: {message}");

                byte[] binarnaPoruka = Encoding.UTF8.GetBytes("Server eho: " + message);
                int brBajta = serverSocket.SendTo(binarnaPoruka, 0, binarnaPoruka.Length, SocketFlags.None, posiljaocEP); // 3.
                Console.WriteLine($"Poslata je poruka duzine {brBajta} ka {posiljaocEP}");

            }
        }
    }
}
