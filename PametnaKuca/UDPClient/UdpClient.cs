using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UDPClient
{
    public class UdpClient
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint destinationEP = new IPEndPoint(IPAddress.Any, 50001); // Odredisni IPEndPoint, IP i port ka kome saljemo. U slucaju 8. tacke je potrebno uneti IP adresu server racunara
            EndPoint posiljaocEP = new IPEndPoint(IPAddress.Any, 0);

                    byte[] prijemniBafer = new byte[1024];
            while (true) // 1.
            {
                try 
                {
                    int brBajta = clientSocket.ReceiveFrom(prijemniBafer, ref posiljaocEP);

                    string ehoPoruka = Encoding.UTF8.GetString(prijemniBafer, 0, brBajta);

                    Console.WriteLine($"Stigao je odgovor od {posiljaocEP}, duzine {brBajta}, eho glasi:\n{ehoPoruka}"); // 4

                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Doslo je do greske tokom slanja poruke: \n{ex}");
                }
            }

            Console.WriteLine("Klijen zavrsava sa radom");
            clientSocket.Close(); // Zatvaramo soket na kraju rada
            Console.ReadKey();
        
        }
    }
}
