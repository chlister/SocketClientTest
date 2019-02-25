using SocketClientTest.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpelSocketClient sl = new SimpelSocketClient(new TcpClient(), 8891, "192.168.1.2");
            sl.StartClient();
            
            Console.WriteLine("Program has ended....");
        }
    }

}
