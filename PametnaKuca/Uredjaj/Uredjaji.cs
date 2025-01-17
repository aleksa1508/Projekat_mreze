using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KucniUredjaji;

namespace UredjajKomunikacija

{
    public class Uredjaji
    {
        static void Main(string[] args)
        {
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint destinationEP = new IPEndPoint(IPAddress.Any, 60000);
            udpSocket.Bind(destinationEP);
            EndPoint posiljaocEP = new IPEndPoint(IPAddress.Any, 0);
            Uredjaj u =new  Uredjaj();
            List<Uredjaj> uredjaji = u.SviUredjaji();
            while (true) // 1.
            {
                byte[] prijemniBafer = new byte[1024];
                try
                {
                   
                    int brBajta = udpSocket.ReceiveFrom(prijemniBafer, ref posiljaocEP);

                    string receivedMessage = Encoding.UTF8.GetString(prijemniBafer, 0, brBajta);

                    Console.WriteLine($"Stigao je odgovor od {posiljaocEP}, duzine {brBajta}->:\n{receivedMessage}"); // 4
                    
                    string[] parts = receivedMessage.Split(':');
                    Console.WriteLine(parts.Length + " " + parts[0] + " " + parts[1] + " " + parts[2]);
                    foreach(var s in uredjaji)
                    {
                        if (s.Ime == parts[0])
                        {
                            s.AzurirajFunkciju(parts[1], parts[2]);
                            break;
                        }
                    }
                   
                    break;

                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Doslo je do greske tokom slanja poruke: \n{ex}");
                }
            }

            Console.WriteLine("Klijen zavrsava sa radom");
            udpSocket.Close(); // Zatvaramo soket na kraju rada
            Console.ReadKey();
        }
    }
}
