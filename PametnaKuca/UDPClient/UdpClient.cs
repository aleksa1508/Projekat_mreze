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
            while (true) 
            {
                try
                {

                    byte[] binarnaPoruka = Encoding.UTF8.GetBytes("UDP Klijent se povezao");
                    int brBajta = clientSocket.SendTo(binarnaPoruka, 0, binarnaPoruka.Length, SocketFlags.None, destinationEP);
                    Console.WriteLine($"Uspesno poslato {brBajta} ka {destinationEP}");


                        brBajta = clientSocket.ReceiveFrom(prijemniBafer, ref posiljaocEP);

                        string poruka = Encoding.UTF8.GetString(prijemniBafer, 0, brBajta);

                        Console.WriteLine($"Stigao je odgovor od {posiljaocEP}, duzine {brBajta}, Funkcija je :{poruka}");

                        string[] delovi = poruka.Split(':');
                        string funkcija = delovi[0];
                        string imeUredjaj = delovi[1];
                        Console.WriteLine(funkcija + " " + imeUredjaj);
                        foreach(var u in uredjaj.SviUredjaji())
                        {
                       
                        if (u.Ime.ToString() == imeUredjaj)
                            {
                          
                            if (u.Ime.ToString() == "Svetlo")
                                {
                                    foreach(var f in u.Funkcije)
                                    {
                                    Console.WriteLine(f.Key.ToString());
                                        if(f.Key.ToString() == funkcija)
                                        {
                                            if (f.Key.ToString() == "intezitet")
                                            {
                                                Console.WriteLine($"Unesi novu vrijednost za {funkcija}(0%-100%):");
                                                string novaVrijednost=Console.ReadLine(); 
                                                u.AzurirajFunkciju(funkcija, novaVrijednost);
                                                Console.WriteLine($"Funkcija {funkcija} je azurirana na vrijednost->{novaVrijednost} ");
                                                binarnaPoruka = Encoding.UTF8.GetBytes(novaVrijednost+"%");
                                                brBajta = clientSocket.SendTo(binarnaPoruka, 0, binarnaPoruka.Length, SocketFlags.None, destinationEP);
                                                Console.WriteLine($"Uspesno poslato {brBajta} ka {destinationEP}");
                                                break;
                                            }
                                            else 
                                            {
                                                Console.WriteLine($"Unesi novu vrijednost za {funkcija}(0-255):");
                                                string novaVrijednost = Console.ReadLine();
                                                u.AzurirajFunkciju(funkcija, novaVrijednost);
                                                Console.WriteLine($"Funkcija {funkcija} je azurirana na vrijednost->{novaVrijednost} ");
                                                binarnaPoruka = Encoding.UTF8.GetBytes(novaVrijednost);
                                                brBajta = clientSocket.SendTo(binarnaPoruka, 0, binarnaPoruka.Length, SocketFlags.None, destinationEP);
                                                Console.WriteLine($"Uspesno poslato {brBajta} ka {destinationEP}");
                                            break;
                                            }
                                        }
                                    }
                                }else if (u.Ime == "TV")
                                {

                                }
                            }else{
                                Console.WriteLine("Nije pronadjen uredjaj");
                            }
                        }
                    Console.WriteLine("AZURIRANJE VRIJEDNOSTI\n");

                        foreach(var a in uredjaj.SviUredjaji())
                        {
                            if (a.Ime == "Svetlo")
                            {
                                foreach(var f in a.Funkcije)
                                {
                                    Console.WriteLine(f.Key +  " "+ f.Value);
                                }
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
            clientSocket.Close(); // Zatvaramo soket na kraju rada
            Console.ReadKey();

        }
    }
}