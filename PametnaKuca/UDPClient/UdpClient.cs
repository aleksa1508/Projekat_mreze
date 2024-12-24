using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KucniUredjaji;

namespace UDPClient
{
    public class UdpClient
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint destinationEP = new IPEndPoint(IPAddress.Loopback, 50001); // Odredisni IPEndPoint, IP i port ka kome saljemo. U slucaju 8. tacke je potrebno uneti IP adresu server racunara
            EndPoint posiljaocEP = new IPEndPoint(IPAddress.Any, 0);
            Uredjaj uredjaj = new Uredjaj();
            byte[] prijemniBafer = new byte[1024];
            while (true) // 1.
            {
                try
                {

                    byte[] binarnaPoruka = Encoding.UTF8.GetBytes("UDP Klijent se povezao");
                    int brBajta = clientSocket.SendTo(binarnaPoruka, 0, binarnaPoruka.Length, SocketFlags.None, destinationEP);
                    Console.WriteLine($"Uspesno poslato {brBajta} ka {destinationEP}");

                    brBajta = clientSocket.ReceiveFrom(prijemniBafer, ref posiljaocEP);

                    string poruka = Encoding.UTF8.GetString(prijemniBafer, 0, brBajta);

                    Console.WriteLine($"Stigao je odgovor od {posiljaocEP}, duzine {brBajta}, Funkcija je :{poruka}");

                 


                    break;

           

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