using KucniUredjaji;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UredjajKomunikacija

{
    public class Uredjaji
    {
        static void Main(string[] args)
        {
            int broj = 0;
            foreach (var arg in args)
            {
                Console.WriteLine($"Primljen argument: {arg}");
                broj = Int32.Parse(arg);
            }
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint destinationEP = new IPEndPoint(IPAddress.Any, broj);
            udpSocket.Bind(destinationEP);
            EndPoint posiljaocEP = new IPEndPoint(IPAddress.Any, 0);
            Uredjaj u = new Uredjaj();
            List<Uredjaj> uredjaji = u.SviUredjaji();
            bool kraj = false;
            while (!kraj) // 1.
            {
                byte[] prijemniBafer = new byte[1024];
                try
                {

                    int brBajta = udpSocket.ReceiveFrom(prijemniBafer, ref posiljaocEP);

                    string receivedMessage = Encoding.UTF8.GetString(prijemniBafer, 0, brBajta);
                    Console.WriteLine("Stigla je poruka " + receivedMessage);
                    if (receivedMessage == "Server je zavrsio sa radom")
                    {
                        kraj = true;
                        break;
                    }

                    Console.WriteLine($"Stigao je odgovor od {posiljaocEP}, duzine {brBajta}->:{receivedMessage}"); // 4

                    string[] parts = receivedMessage.Split(':');
                    Console.WriteLine(parts.Length + " " + parts[0] + " " + parts[1] + " " + parts[2]);
                    foreach (var s in uredjaji)
                    {
                        if (s.Ime == parts[0])
                        {
                            s.AzurirajFunkciju(parts[1], parts[2]);
                            Console.WriteLine(s.IspisiSveFunkcijeUredjaja());
                            break;
                        }
                    }


                    foreach (var s in uredjaji)
                    {

                        foreach (var e in s.EvidencijaKomandi)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }



                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Doslo je do greske tokom slanja poruke: \n{ex}");
                }
            }
            Console.WriteLine("Uredjaj zavrsava sa radom");
            udpSocket.Close(); // Zatvaramo soket na kraju rada
            Console.ReadKey();
        }
    }
}