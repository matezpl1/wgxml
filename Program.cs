using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            Configs config = new Configs();
            config.clientip.Clear();
            config.clientname.Clear();
            config.clientname2.Clear();
            int arethereconfigs = 0;
            string filename;
            string configfile = "wg_clients.log";
            if (args.Length > 0)
            {
                filename = args[0];

            }
            else
            {
                filename = "tunele2025.txt";
            }
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("nie odnaleziono pliku o nazwie " + filename);
            }
            if(args.Length > 1)
            {
                configfile = args[1];
                arethereconfigs = 1;
            }
            //odczytanie nazw z configów
            if(arethereconfigs == 1)
            {
                if (!File.Exists(configfile))
                {
                    throw new FileNotFoundException("nie odnaleziono pliku o nazwie " + configfile);
                }
                StreamReader reader3 = new StreamReader(configfile);
                while (reader3.EndOfStream == false)
                {
                    string line3 = reader3.ReadLine();
                    if (line3.Contains("client"))
                    {
                        Regex regex = new Regex(@"client(\d+)_(.+)\.conf");
                        Match match = regex.Match(line3);
                        string name2 = line3.Replace(".conf", "");
                        if (match.Success)
                        {
                            config.clientname2.Add(name2);
                            config.clientip.Add(match.Groups[1].Value);
                            config.clientname.Add(match.Groups[2].Value);
                        }
                    }
                    else if(line3.Contains("cl"))
                    {
                        Regex regex = new Regex(@"cl(\d+)_(.+)\.conf");
                        Match match = regex.Match(line3);
                        string name2 = line3.Replace(".conf", "");
                        if (match.Success)
                        {
                            config.clientname2.Add(name2);
                            config.clientip.Add(match.Groups[1].Value);
                            config.clientname.Add(match.Groups[2].Value);
                        }
                    }
                }
                reader3.Close();
            }
            //dodaj odczytywanie z pliku i zapisanie danych do klasy
            Peery peery = new Peery();
            peery.peer.Clear();
            peery.ip.Clear();
            peery.endpoint.Clear();
            peery.test.czas.Clear();
            peery.test.transfer.Clear();
            peery.test.lastconnect.Clear();
            peery.test.peer_index.Clear();
            peery.test.czaspeer.Clear();
            peery.test.transferpeer.Clear();
            peery.test.lastconnectpeer.Clear();

            int z = 0;
            //odczytywanie danych z xml'a
            if (File.Exists("wg_show.xml"))
            {
                StreamReader reader2 = new StreamReader("wg_show.xml");
                while (reader2.EndOfStream == false)
                {
                    string line2 = reader2.ReadLine();
                    if (line2.Contains("peer:"))
                    {
                        if (z == 1)
                        {
                            peery.test.lastconnect.Add("brak danych o last connect");
                            peery.test.lastconnectpeer.Add(peery.test.peer_index.Last());
                            peery.test.transfer.Add("brak danych o transfer");
                            peery.test.transferpeer.Add(peery.test.peer_index.Last());
                        }

                        if (!peery.peer.Contains(line2))
                        {
                            peery.peer.Add(line2);
                            peery.test.peer_index.Add(line2);
                        }
                        else
                        {
                            peery.test.czas.Add(DateTime.Now + "");
                            peery.test.peer_index.Add(line2);
                            peery.test.czaspeer.Add(peery.test.peer_index.Last());
                        }
                    }
                    if (line2.Contains("endpoint:"))
                    {
                        if (!peery.endpoint.Contains(line2))
                        {
                            peery.endpoint.Add(line2);
                        }
                    }
                    if (line2.Contains("allowed ips:") && z == 1)
                    {
                        if (!peery.ip.Contains(line2))
                        {
                            peery.ip.Add(line2);
                            peery.test.lastconnect.Add("brak danych o last connect");
                            peery.test.lastconnectpeer.Add(peery.test.peer_index.Last());
                            peery.test.transfer.Add("brak danych o transfer");
                            peery.test.transferpeer.Add(peery.test.peer_index.Last());
                        }
                    }
                    if (line2.Contains("allowed ips:"))
                    {
                        if (!peery.ip.Contains(line2))
                        {
                            peery.ip.Add(line2);
                            z = 1;
                        }
                    }
                    if (line2.Contains("<czas>"))
                    {
                        line2 = reader2.ReadLine();
                        peery.test.czas.Add(line2);
                        peery.test.czaspeer.Add(peery.test.peer_index.Last());
                    }
                    if (line2.Contains("latest handshake:"))
                    {
                        z = 0;
                        peery.test.lastconnect.Add(line2);
                        peery.test.lastconnectpeer.Add(peery.test.peer_index.Last());
                    }
                    if (line2.Contains("transfer:"))
                    {
                        z = 0;
                        peery.test.transfer.Add(line2);
                        peery.test.transferpeer.Add(peery.test.peer_index.Last());
                    }
                    if (line2.Contains("brak danych o last connect"))
                    {
                        z = 0;
                        peery.test.lastconnect.Add(line2);
                        peery.test.lastconnectpeer.Add(peery.test.peer_index.Last());
                    }
                    if (line2.Contains("brak danych o transfer"))
                    {
                        z = 0;
                        peery.test.transfer.Add(line2);
                        peery.test.transferpeer.Add(peery.test.peer_index.Last());
                    }

                }
                reader2.Close();
            }
            //odczytywanie danych z pliku txt z tunelingu
            StreamReader reader = new StreamReader(filename);
            StreamWriter writer = new StreamWriter("wg_show.xml");

            while (reader.EndOfStream == false)
            {

                string line = reader.ReadLine();
                if (line.Contains("peer:"))
                {
                    if (z == 1)
                    {
                        peery.test.lastconnect.Add("brak danych o last connect");
                        peery.test.lastconnectpeer.Add(peery.test.peer_index.Last());
                        peery.test.transfer.Add("brak danych o transfer");
                        peery.test.transferpeer.Add(peery.test.peer_index.Last());
                    }

                    if (!peery.peer.Contains(line))
                    {
                        peery.peer.Add(line);
                        peery.test.peer_index.Add(line);
                        peery.test.czas.Add(DateTime.Now + "");
                        peery.test.czaspeer.Add(peery.test.peer_index.Last());
                    }
                    else
                    {
                        peery.test.czas.Add(DateTime.Now + "");
                        peery.test.peer_index.Add(line);
                        peery.test.czaspeer.Add(peery.test.peer_index.Last());
                    }
                }
                if (line.Contains("endpoint:"))
                {
                    if (!peery.endpoint.Contains(line))
                    {
                        peery.endpoint.Add(line);
                    }
                }
                if (line.Contains("allowed ips:"))
                {
                    z = 1;
                    if (!peery.ip.Contains(line))
                    {
                        peery.ip.Add(line);
                    }
                }
                if (line.Contains("latest handshake:"))
                {
                    z = 0;
                    peery.test.lastconnect.Add(line);
                    peery.test.lastconnectpeer.Add(peery.test.peer_index.Last());
                }
                if (line.Contains("transfer:"))
                {
                    z = 0;
                    peery.test.transfer.Add(line);
                    peery.test.transferpeer.Add(peery.test.peer_index.Last());
                }

            }
            reader.Close();
            peery.test.lastconnect.Add("brak danych o last connect");
            peery.test.lastconnectpeer.Add(peery.test.peer_index.Last());
            peery.test.transfer.Add("brak danych o transfer");
            peery.test.transferpeer.Add(peery.test.peer_index.Last());

            //zapisywanie do pliku
            writer.Write("<peers>\n");
            int j = peery.peer.Count;
            for (int i = 0; i < j; i++)
            {
                writer.Write("<peer>\n");
                if (arethereconfigs == 1)
                {
                    Regex regex = new Regex(@"\b\d+\.\d+\.\d+\.(\d+)/\d+\b");
                    Match match = regex.Match(peery.ip.ElementAt(i));
                    string ip = match.Groups[1].Value;
                    int index =config.clientip.IndexOf(ip);
                    if (index != -1)
                    {
                        writer.Write("<clientname>\n" + config.clientname2.ElementAt(index) + "\n</clientname>");
                    }
                }
                writer.Write("<peerkey>\n" + peery.peer.ElementAt(i) + "\n</peerkey>\n");
                if (peery.endpoint.Count > i)
                {
                    writer.Write("<endpoint>\n" + peery.endpoint.ElementAt(i) + "\n</endpoint>\n");
                }
                if (peery.ip.Count > i)
                {
                    writer.Write("<ip>\n" + peery.ip.ElementAt(i) + "\n</ip>\n");
                }
                int k = peery.test.czaspeer.Count;
                int a = 1;
                for (int l = 0; l < k; l++)
                {
                    if (peery.peer.ElementAt(i) == peery.test.czaspeer.ElementAt(l))
                    {
                        writer.Write("<clienthistory_" + a + ">\n");

                        writer.Write("<czas>\n" + peery.test.czas.ElementAt(l) + "\n</czas>\n");

                        if (peery.test.lastconnect.Count > l)
                        {
                            writer.Write("<lastconnect>\n" + peery.test.lastconnect.ElementAt(l) + "\n</lastconnect>\n");
                        }
                        if (peery.test.transfer.Count > l)
                        {
                            writer.Write("<transfer>\n" + peery.test.transfer.ElementAt(l) + "\n</transfer>\n");
                        }
                        writer.Write("</clienthistory_" + a + ">\n");
                        a++;
                    }

                }
                writer.Write("</peer>\n");
            }
            writer.Write("</peers>");
            writer.Close();
            //Console.WriteLine(peery.test.czaspeer.Count);
            //Console.WriteLine(peery.test.transferpeer.Count);
        }
    }
    class Peery
    {
        public List<string> peer = new List<string> { "0" };
        public List<string> endpoint = new List<string> { "0" };
        public List<string> ip = new List<string> { "0" };
        public Test test;

        public Peery()
        {
            test = new Test();
        }
    }
    class Test
    {
        public List<string> czas = new List<string> { "0" };
        public List<string> lastconnect = new List<string> { "0" };
        public List<string> transfer = new List<string> { "0" };
        public List<string> peer_index = new List<string> { "0" };
        public List<string> czaspeer = new List<string> { "0" };
        public List<string> lastconnectpeer = new List<string> { "0" };
        public List<string> transferpeer = new List<string> { "0" };

        public Test()
        {
        }
    }
    class Configs
    {
        public List<string> clientip = new List<string> { "0" };
        public List<string> clientname = new List<string> { "0" };
        public List<string> clientname2 = new List<string> { "0" };
        public Configs()
        {

        }
    }
}
